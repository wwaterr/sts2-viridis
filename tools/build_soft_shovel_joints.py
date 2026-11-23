"""Re-cut Viridis's limbs in setup pose and skin both sides of each cut in common coordinates."""
import json
import math
from pathlib import Path
from zipfile import ZipFile, ZIP_DEFLATED

import numpy as np
from PIL import Image, ImageDraw
from scipy import ndimage

ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / "mods/Viridis/art_source/characters/viridis"
BASE = ART / "shovel-fullbody/knee-on-shin"
OUT = BASE.parent / "soft-joints"
NAME = "viridis-flat-thrust"
rig = json.loads((BASE / "viridis-silent-thrust.json").read_text())
bones = rig["bones"]
tracks = rig["animations"]["attack"]["bones"]
attachments = rig["skins"][0]["attachments"]
origin = {}
for bone in bones:
    # All setup bones in the source are untranslated in angle and scale.
    origin[bone["name"]] = origin.get(bone.get("parent"), np.array([235., 504.])) + [bone["x"], -bone["y"]]

guard = Image.open(ART / "shovel-strike/frames/guard.png").convert("RGBA")
source = np.array(guard)
yy, xx = np.indices(source.shape[:2]) + .5
surfaces = {}
for slot in rig["slots"]:
    name = slot["name"]
    a = attachments[name][name]
    px, py = origin[slot["bone"]]
    pos = round(px + a["x"] - a["width"]/2), round(py - a["y"] - a["height"]/2)
    canvas = Image.new("RGBA", guard.size)
    canvas.paste(Image.open(BASE / "parts" / (name+".png")), pos)
    surfaces[name] = canvas


def signed_along(x, y, bone, axis):
    direction = np.array(axis, dtype=float)
    direction /= np.linalg.norm(direction)
    cx, cy = origin[bone]
    return (x-cx)*direction[0] + (y-cy)*direction[1]


def smooth(x):
    x = np.clip(x, 0, 1)
    return x*x*(3-2*x)


def restore_joint(upper, lower, axis, window, split=0):
    # Sample the uncut painting on both sides of the seam. Keep two pixels of
    # overlap for filtering and use the same lattice and skin weights on either side.
    s = signed_along(xx, yy, lower, axis)
    for name, keep in [(upper,s < split+1.25), (lower,s >= split-1.25)]:
        dst = np.array(surfaces[name])
        dst[window] = 0
        mask = window & keep
        dst[mask] = source[mask]
        surfaces[name] = Image.fromarray(dst)


arm_l=np.interp(yy,[205,210,218,226,238],[197,195,194,189,180])
arm_r=np.interp(yy,[205,214,222,232,240],[277,270,275,279,286])
# A shared redraw window spans the complete skin width, including the transparent
# outline pixels. The torso/tie borders are just outside the inner arm contours.
restore_joint("upper_arm","forearm",(-.49,.87),
              (yy>=211)&(yy<237)&(xx>150)&(xx<arm_l))
restore_joint("open_upper","open_fore",(.59,.81),
              (yy>=215)&(yy<237)&(xx>arm_r)&(xx<326))
for side, axis, side_mask, split in [
    ("rear",(-.50,.86),xx<229,0),("lead",(.49,.87),xx>232,-14)
]:
    restore_joint(side+"_thigh",side+"_shin",axis,(yy>=341)&(yy<398)&side_mask,split)

# Peel the painted plaster off the skin before bending it. Preserve the original
# cross, outline and antialiasing as a rigid detail at the shin, above the skin.
patch_rect=(294,350,324,380)
patch_rgb=np.asarray(guard.crop(patch_rect))[:,:,:3].astype(float)
stencil=Image.new("L",(30,30))
ImageDraw.Draw(stencil).polygon([(1,15),(3,6),(12,9),(15,2),(24,4),(22,13),
    (28,17),(25,25),(16,23),(13,29),(5,27),(7,19)],fill=255)
core=(np.asarray(stencil)>0)&(patch_rgb[:,:,0]<220)&(patch_rgb[:,:,1]-patch_rgb[:,:,2]>33)
py_grid,px_grid=np.indices(core.shape)
plane=np.stack((px_grid,py_grid,np.ones_like(px_grid)),axis=-1)
healthy=(patch_rgb[:,:,0]>218)&(patch_rgb[:,:,0]-patch_rgb[:,:,1]>14)&(~ndimage.binary_dilation(core))
skin_color=plane@np.linalg.lstsq(plane[healthy],patch_rgb[healthy],rcond=None)[0]
# Unmix the skin at the antialiased border so the decal does not carry a pale rim.
halo=ndimage.binary_dilation(core)
alpha=np.clip((skin_color[:,:,2]-patch_rgb[:,:,2])/68,0,1)*halo
alpha[ndimage.binary_erosion(core)]=1
pigment=(patch_rgb-(1-alpha[:,:,None])*skin_color)/np.maximum(alpha[:,:,None],.001)
patch=Image.fromarray(np.dstack([np.uint8(np.clip(pigment,0,255)),np.uint8(np.rint(255*alpha))]))
for name in ["lead_thigh","lead_shin"]:
    im=surfaces[name]
    crop=np.array(im.crop(patch_rect))
    edge_distance=ndimage.distance_transform_edt(source[:,:,3]>150)[350:380,294:324]
    area=ndimage.binary_dilation(core,iterations=4)&(edge_distance>2.7)
    crop[area,:3]=np.uint8(np.clip(skin_color[area],0,255))
    im.paste(Image.fromarray(crop),patch_rect[:2])
patch_name="lead_plaster"
rig["slots"].insert(next(i for i,s in enumerate(rig["slots"]) if s["name"]=="lead_shin")+1,
                    {"name":patch_name,"bone":"plaster_anchor","attachment":patch_name})
attachments[patch_name]={patch_name:{"width":30,"height":30,"x":0,"y":0}}

# Bisectors maintain the width of the painted contour at sharp bends. Their only
# keys are derived from the existing joint rotation; attack timing stays identical.
for child in ["forearm","open_fore","rear_shin","lead_shin","upper_arm","open_upper"]:
    helper=child+"_fold"
    bones.append({"name":helper,"parent":child,"x":0,"y":0})
    origin[helper]=origin[child]
    tracks[helper]={"rotate":[{"time":k["time"],"value":round(-k["value"]*.5,5)}
                              for k in tracks[child]["rotate"]]}
    if child in ("rear_shin","lead_shin","forearm"):
        # The concave side reaches the intersection of the two limb contours.
        inner=child+"_inside"
        bones.append({"name":inner,"parent":helper,"x":0,"y":0})
        origin[inner]=origin[child]
        scales=[]
        for k in tracks[child]["rotate"]:
            gain=round(min(1.60,1/max(.3,math.cos(math.radians(k["value"]*.5)))),5)
            scales.append({"time":k["time"],"x":gain,"y":gain})
        tracks[inner]={"scale":scales}
# Pin the center to the kneecap surface, retaining the lower leg's orientation.
bones.append({"name":"plaster_anchor","parent":"lead_shin_fold","x":-3,"y":2})
tracks["plaster_anchor"]={"rotate":[{"time":k["time"],"value":round(k["value"]*.5,5)}
                                   for k in tracks["lead_shin"]["rotate"]]}
indices={b["name"]:i for i,b in enumerate(bones)}


def mix(a,b,f):
    return {k: a.get(k,0)*(1-f)+b.get(k,0)*f for k in a.keys()|b.keys()}


def bend(x,y,upper,lower,axis,radius):
    d=signed_along(x,y,lower,axis)/radius
    center={lower+"_fold":1}
    if lower in ("rear_shin","lead_shin","forearm"):
        lateral=signed_along(x,y,lower,(-axis[1],axis[0]))
        if lower=="forearm": lateral=-lateral
        center=mix(center,{lower+"_inside":1},smooth(lateral/9))
    if d<0:
        return mix({upper:1},center,smooth(d+1))
    return mix(center,{lower:1},smooth(d))


def skin(name,x,y):
    if name in ("torso","pelvis"):
        if name=="pelvis": return {"hips":1}
        weights=mix({"ribcage":1},{"hips":1},smooth((y-236)/60))
        for shoulder in ["upper_arm","open_upper"]:
            dx,dy=np.array([x,y])-origin[shoulder]
            weights=mix(weights,{shoulder+"_fold":1},.42*math.exp(-((dx/18)**2+(dy/22)**2)))
        return weights
    if name.startswith(("rear_","lead_")):
        side=name.split("_")[0]
        thigh,shin,boot=[side+"_"+s for s in ["thigh","shin","boot"]]
        if name==boot: return {boot:1}
        axis=(-.50,.86) if side=="rear" else (.49,.87)
        weights=bend(x,y,thigh,shin,axis,21 if side=="rear" else 18)
        weights=mix({"hips":1},weights,smooth((y-266)/54))
        return mix(weights,{boot:1},smooth((y-origin[boot][1]+26)/23))
    upper,fore,hand,axis=(("open_upper","open_fore","open_hand",(.59,.81))
                             if name.startswith("open") else ("upper_arm","forearm","hand",(-.49,.87)))
    if name==hand: return {hand:1}
    weights=bend(x,y,upper,fore,axis,23)
    socket=mix({"ribcage":1},{upper+"_fold":1},.68)
    weights=mix(socket,weights,smooth((y-origin[upper][1]+2)/34))
    return mix(weights,{hand:1},smooth((signed_along(x,y,hand,axis)+13)/16))


def mesh(name,rect):
    l,t,r,b=rect
    w,h=r-l,b-t
    # Global six-pixel grid keeps the two independently textured meshes coincident.
    columns=list(range(l,r+1,6))
    rows=list(range(t,b+1,6))
    nx,ny=len(columns),len(rows)
    border=([(i,0) for i in range(nx)]+[(nx-1,j) for j in range(1,ny)]
            +[(i,ny-1) for i in reversed(range(nx-1))]+[(0,j) for j in reversed(range(1,ny-1))])
    points=border+[(i,j) for j in range(1,ny-1) for i in range(1,nx-1)]
    ids={p:i for i,p in enumerate(points)}
    vertices,uvs,tris=[],[],[]
    for i,j in points:
        x,y=columns[i],rows[j]
        uvs.extend([round((x-l)/w,7),round((y-t)/h,7)])
        weights={k:v for k,v in skin(name,x,y).items() if v>1e-6}
        total=sum(weights.values())
        vertices.append(len(weights))
        for bone,weight in sorted(weights.items(), key=lambda kv: indices[kv[0]]):
            px,py=origin[bone]
            vertices.extend([indices[bone],round(x-px,4),round(py-y,4),round(weight/total,7)])
    for j in range(ny-1):
        for i in range(nx-1):
            a,bb,c,d=[ids[p] for p in [(i,j),(i+1,j),(i+1,j+1),(i,j+1)]]
            tris.extend([a,bb,c,a,c,d])
    return dict(type="mesh",width=w,height=h,uvs=uvs,triangles=tris,vertices=vertices,hull=len(border))


def build():
    parts=OUT/"parts"
    parts.mkdir(exist_ok=True,parents=True)
    regions={}
    for slot in rig["slots"]:
        name=slot["name"]
        if name in ("upper_arm","forearm","open_upper","open_fore","rear_thigh","rear_shin","lead_thigh","lead_shin","torso"):
            l,t,r,b=surfaces[name].getchannel("A").getbbox()
            rect=((l-2)//6*6,(t-2)//6*6,math.ceil((r+2)/6)*6,math.ceil((b+2)/6)*6)
            regions[name]=surfaces[name].crop(rect)
            attachments[name][name]=mesh(name,rect)
        elif name==patch_name:
            regions[name]=patch
        else:
            regions[name]=Image.open(BASE/"parts"/(name+".png")).convert("RGBA")
        regions[name].save(parts/(name+".png"))
    page=Image.new("RGBA",(512,512))
    atlas=[NAME+".png","size:512,512","filter:Linear,Linear","pma:false"]
    x,y,row=2,2,0
    for name,im in sorted(regions.items(), key=lambda item:-item[1].height):
        w,h=im.size
        if x+w+2>512: x,y,row=2,y+row+4,0
        if y+h+2>512: raise ValueError("Atlas overflow")
        page.paste(im,(x,y))
        atlas.extend([name,f"bounds:{x},{y},{w},{h}"])
        x+=w+4;row=max(row,h)
    page.save(OUT/(NAME+".png"))
    (OUT/(NAME+".atlas")).write_text("\n".join(atlas)+"\n")
    (OUT/(NAME+".json")).write_text(json.dumps(rig,separators=(",",":"))+"\n")
    exports=ROOT/"outputs/viridis-animation"
    exports.mkdir(parents=True,exist_ok=True)
    with ZipFile(exports/"Viridis-soft-joints.zip","w",ZIP_DEFLATED) as z:
        for f in [OUT/(NAME+ext) for ext in (".json",".atlas",".png")]+sorted(parts.glob("*.png")):
            z.write(f,f.relative_to(OUT))

if __name__=="__main__":
    build()
