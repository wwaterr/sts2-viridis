"""Add an anatomically weighted breathing mesh to the guard-matched 1.5x attack rig."""
import json
from math import cos, pi
from subprocess import Popen
from zipfile import ZIP_DEFLATED, ZipFile

import numpy as np
from PIL import Image
from scipy import ndimage

from build_guard_thrust import NAME, ORIGIN, OUT, SOURCE, source_layers
from preview_guard_thrust import EXPORT, Playback

PERIOD = 3.6
FPS = 30
BASE = OUT / (NAME + "-1p5x.json")
TARGET = OUT / (NAME + "-1p5x-breath.json")
# Pivot in the original painting, local translation (Spine Y up), rotation and inhale peak.
MOTION = {
    "chest_left": ((221, 244), (-1.15, 3.6), -.20, 1.34),
    "chest_right": ((241, 244), (1.10, 3.6), .20, 1.34),
    "neck": ((230, 149), (.38, 3.05), -.46, 1.48),
    "near_shoulder": ((200, 163), (-.72, 3.10), -.34, 1.40),
    "far_shoulder": ((258, 167), (.86, 3.28), .38, 1.40),
    "grip": ((135, 278), (.18, 1.32), -.58, 1.56),
    "open_glove": ((316, 255), (.50, 1.45), .42, 1.53),
}

def smooth(t):
    t = np.clip(t, 0, 1)
    return t*t*(3-2*t)

def envelope(t, peak):
    u = t/peak if t < peak else (PERIOD-t)/(PERIOD-peak)
    return (1-cos(pi*u))*.5

def skin_weights(guard):
    # Visible painted surfaces distinguish the trowel/forearm from the apron at
    # the same height. The lower legs, boots and both planted soles belong to root.
    y,x = np.indices((guard.height,guard.width))
    names = ["root",*MOTION]
    w = np.zeros((len(names),guard.height,guard.width))
    w[0] = 1
    layers = source_layers(guard)
    for name in ("far","rear","lead","body","rear_boot","lead_boot","arm","tool","hand"):
        a = np.asarray(layers[name].getchannel("A"))/255
        local = {"root":1.}
        if name == "body":
            torso = 1-smooth((y-178)/111)
            head = 1-smooth((y-142)/16)
            right = smooth((x-210)/41)
            local = {"root":(1-head)*(1-torso),"neck":head,
                     "chest_right":(1-head)*torso*right,"chest_left":(1-head)*torso*(1-right)}
        elif name in ("arm","far"):
            distal = smooth((y-177)/69)
            local = {"near_shoulder" if name=="arm" else "far_shoulder":1-distal,
                     "grip" if name=="arm" else "open_glove":distal}
        elif name in ("hand","tool"):
            local={"grip":1.}
        w *= 1-a
        for bone,weight in local.items():
            w[names.index(bone)] += a*weight
    # Continue edge weights into transparent mesh vertices so outlines follow
    # their painted limb, then soften the weight transition across nearby joints.
    inside = np.array(guard.getchannel("A"))>160
    iy,ix = ndimage.distance_transform_edt(~inside,return_distances=False,return_indices=True)
    w = ndimage.gaussian_filter(w[:,iy,ix],sigma=(0,3.6,3.6))
    w[:,y>=320]=0
    w[0,y>=320]=1
    return names,w

def add_mesh(rig, guard):
    names,fields=skin_weights(guard)
    bones=rig["bones"]
    pivots={"root":ORIGIN}
    for name,(pivot,*_) in MOTION.items():
        px,py=pivot;pivots[name]=pivot
        bones.append({"name":"breath_"+name,"parent":"root","x":px-ORIGIN[0],"y":ORIGIN[1]-py})
    indices={b["name"]:i for i,b in enumerate(bones)}
    xs=list(range(52,421,10));ys=list(range(23,516,10))
    nx,ny=len(xs),len(ys)
    hull=([(i,0) for i in range(nx)]+[(nx-1,j) for j in range(1,ny)]+
          [(i,ny-1) for i in reversed(range(nx-1))]+[(0,j) for j in reversed(range(1,ny-1))])
    points=hull+[(i,j) for j in range(1,ny-1) for i in range(1,nx-1)]
    ids={pt:i for i,pt in enumerate(points)}
    vertices=[];uv=[];triangles=[]
    for i,j in points:
        x,y=xs[i],ys[j];weights=fields[:,y,x]
        keep=np.flatnonzero(weights>.0001);total=weights[keep].sum()
        vertices.append(len(keep));uv.extend([x/guard.width,y/guard.height])
        for k in keep:
            name=names[k];px,py=pivots[name]
            bone=indices["root" if name=="root" else "breath_"+name]
            vertices.extend([bone,float(x-px),float(py-y),round(weights[k]/total,7)])
    for j in range(ny-1):
        for i in range(nx-1):
            a,b,c,d=(ids[p] for p in [(i,j),(i+1,j),(i+1,j+1),(i,j+1)])
            triangles.extend([a,b,c,a,c,d])
    mesh={"type":"mesh","path":"guard_reference","width":guard.width,"height":guard.height,
          "hull":len(hull),"uvs":uv,"triangles":triangles,"vertices":vertices}
    rig["skins"][0]["attachments"]["guard_reference"]["breathing_guard"]=mesh

def build():
    rig=json.loads(BASE.read_text());guard=Image.open(SOURCE).convert("RGBA")
    add_mesh(rig,guard)
    tracks={}
    for name,(_,travel,angle,peak) in MOTION.items():
        pos=[];rot=[]
        for i in range(55):
            t=PERIOD*i/54;a=envelope(t,peak)
            pos.append(dict(time=round(t,7),x=round(a*travel[0],6),y=round(a*travel[1],6)))
            rot.append(dict(time=round(t,7),value=round(a*angle,6)))
        tracks["breath_"+name]={"translate":pos,"rotate":rot}
    loop=rig["animations"]["idle_loop"]
    loop["bones"]=tracks
    loop["slots"]["guard_reference"]["attachment"]=[dict(time=0,name="breathing_guard")]
    TARGET.write_text(json.dumps(rig,separators=(",",":"))+"\n")
    with ZipFile(EXPORT/"Viridis-breath-guard-1p5x.zip","w",ZIP_DEFLATED) as z:
        z.write(TARGET,NAME+".json")
        for f in [OUT/(NAME+".atlas"),OUT/(NAME+".png"),*sorted((OUT/"parts").glob("*.png"))]:
            z.write(f,f.relative_to(OUT))
    assert rig["animations"]["attack"]==json.loads(BASE.read_text())["animations"]["attack"]

def encode(path,frames,webp):
    codec=(["-c:v","libwebp_anim","-quality","97","-loop","0"] if webp else
           ["-c:v","libx264","-crf","16","-pix_fmt","yuv420p","-movflags","+faststart"])
    with Popen(["ffmpeg","-y","-v","error","-f","rawvideo","-pix_fmt","rgb24","-s","608x520",
                "-framerate",str(FPS),"-i","pipe:0",*codec,str(path)],stdin=-1) as p:
        for frame in frames:p.stdin.write(frame.tobytes())
        p.stdin.close()
        assert p.wait()==0

def preview():
    player=Playback(TARGET);period_frames=round(PERIOD*FPS)
    idle=[player.frame("idle_loop",i/FPS) for i in range(period_frames)]
    start=np.array(idle[0]);end=np.array(player.frame("idle_loop",PERIOD))
    assert np.array_equal(start,end)
    peak=np.array(idle[42]);assert np.array_equal(start[324:],peak[324:])
    reference=np.array(player.frame("attack",0))
    delta=np.abs(start.astype(float)-reference)
    assert delta.mean()<.08
    attack=[player.frame("attack",i/FPS) for i in range(27)]
    encode(EXPORT/"viridis-guard-breath.webp",idle,True)
    encode(EXPORT/"viridis-breath-to-attack.mp4",[*idle,*idle,*attack,*idle[:36]],False)
    print("Idle loop pixel error:",np.max(np.abs(start.astype(float)-end)),"neutral mean error:",delta.mean())

if __name__=="__main__":
    build();preview()
