"""Author the four-key Viridis thrust using registered cutout skins and Spine weighted meshes."""
import json
import math
from pathlib import Path
from zipfile import ZipFile, ZIP_DEFLATED

import numpy as np
from PIL import Image, ImageDraw, ImageFilter
from scipy import ndimage
from scipy.interpolate import PchipInterpolator
from scipy.spatial import Delaunay

ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/"mods/Viridis/art_source/characters/viridis/shovel-fullbody"
STUDY=BASE/"keyframe-study"
OUT=BASE/"keyed-thrust"
PARTS=OUT/"parts"
NAMES=["chamber","drive","reach","recover"]
SIZE=(700,615)
reg=json.loads((STUDY/"registration.json").read_text())
frames=[Image.open(STUDY/"keys"/(s+".png")).convert("RGBA") for s in NAMES]

# World-space landmarks in the four registered drawings. Shoulder, elbow and wrist
# are independent of the sleeve hem; the garment can turn around the arm.
landmarks={
 "pelvis":[(238,351),(313,339),(379,335),(229,325)],
 "chest":[(264,249),(329,222),(408,231),(251,211)],
 "head":[(291,190),(338,160),(433,171),(277,149)],
 "arm":[(241,221),(316,196),(417,207),(233,185)],
 "elbow":[(168,257),(285,224),(475,218),(184,251)],
 "wrist":[(200,288),(341,232),(539,219),(230,260)],
 "grip":[(246,305),(374,235),(573,225),(272,266)],
 "tip":[(344,287),(456,209),(662,211),(365,231)],
 "far_arm":[(286,222),(296,202),(368,207),(272,190)],
 "far_elbow":[(306,308),(249,245),(322,233),(305,267)],
 "far_wrist":[(335,326),(215,267),(277,256),(344,295)],
 "far_hand":[(371,343),(179,299),(235,268),(381,310)],
 "rear_thigh":[(223,403),(261,391),(300,379),(209,373)],
 "rear_knee":[(236,453),(234,435),(253,430),(204,438)],
 "rear_ankle":[(158,491),(170,477),(179,482),(145,491)],
 "rear_toe":[(156,586)]*4,
 "lead_thigh":[(322,386),(381,368),(433,365),(321,374)],
 "lead_knee":[(364,424),(427,407),(505,392),(361,415)],
 "lead_ankle":[(350,481),(407,478),(480,475),(376,481)],
 "lead_toe":[(404,585),(459,584),(527,583),(455,585)]}
P={n:np.asarray(v,float) for n,v in landmarks.items()}
parent={"pelvis":"root","chest":"pelvis","head":"chest","arm":"chest","elbow":"arm","wrist":"elbow","grip":"wrist","tip":"grip",
        "far_arm":"chest","far_elbow":"far_arm","far_wrist":"far_elbow","far_hand":"far_wrist"}
for side in ["rear","lead"]:
    parent.update({side+"_thigh":"pelvis",side+"_knee":side+"_thigh",side+"_ankle":side+"_knee",side+"_toe":side+"_ankle"})
aim={"arm":"elbow","elbow":"wrist","wrist":"grip","grip":"tip","far_arm":"far_elbow","far_elbow":"far_wrist","far_wrist":"far_hand",
     "rear_thigh":"rear_knee","rear_knee":"rear_ankle","lead_thigh":"lead_knee","lead_knee":"lead_ankle"}
# Define bone angles in drawing coordinates (positive clockwise); export flips Y.
angles={}
for n in P:
    if n in aim:
        d=P[aim[n]]-P[n];angles[n]=np.degrees(np.arctan2(d[:,1],d[:,0]))
    else:angles[n]=np.zeros(4)
angles["rear_ankle"]=np.array([-23,-34,-37,-21]);angles["lead_ankle"]=np.array([0,1,1,-7])
angles["head"]=np.array([0,-2,3,-4]);angles["chest"]=np.array([0,-2,10,-4])

def poly(coords):
    m=Image.new("L",(SIZE[0]*3,SIZE[1]*3))
    xy=[(round(x*3),round(y*3)) for x,y in zip(coords[::2],coords[1::2])]
    ImageDraw.Draw(m).polygon(xy,fill=255)
    return m.resize(SIZE,Image.Resampling.LANCZOS)

def cut(im,m):
    a=im.copy();a.putalpha(Image.fromarray(np.uint8(np.asarray(im.getchannel("A"),float)*np.asarray(m)/255)))
    return a

# The previous cut tests are registered by matching interior painted pixels in the source.
def register_patch(key,part,approx):
    patch=Image.open(STUDY/"cut-test"/(key+"-"+part+".png")).convert("RGBA")
    src=np.asarray(frames[0 if key=="chamber" else 2],dtype=int)
    a=np.asarray(patch);ys,xs=np.where(a[:,:,3]>249);ys=ys[::13];xs=xs[::13]
    match=min((np.mean(abs(src[y+ys,x+xs,:3]-a[ys,xs,:3])),x,y)
              for x in range(approx[0]-10,approx[0]+11) for y in range(approx[1]-10,approx[1]+11))
    _,x,y=match
    canvas=Image.new("RGBA",SIZE);canvas.paste(patch,(x,y));return canvas

# Occluding edges are traced on the four source keys. Outside the other shapes
# the painter's original alpha defines the silhouette.
arm_masks=[None,
 [266,198,270,193,277,191,288,188,301,184,310,179,318,178,328,182,334,189,337,200,337,207,334,213,329,216,331,220,336,223,336,240,306,241,291,240,282,237,276,234,274,231,274,226,269,222,266,217],
 None,[182,196,208,174,228,165,246,166,254,179,253,202,243,218,220,239,213,240,208,233,205,242,232,243,234,274,175,273,169,255,180,218]]
far_masks=[
 [288,298,308,291,319,300,335,311,350,310,399,326,401,374,351,371,331,347,316,326,296,321,278,312],
 [281,209,306,217,295,237,259,267,232,280,209,306,174,329,148,312,151,275,209,253,236,232,259,223],
 [336,199,369,188,397,186,377,220,363,244,351,244,334,241,291,266,282,278,236,300,196,288,198,249,256,240,274,230,283,240,335,214],
 [282,244,301,245,324,261,343,278,352,276,413,293,413,333,366,334,339,311,319,300,319,294,296,282,278,270]]
hand_masks=[
 [193,279,208,272,224,286,260,286,271,304,273,313,258,322,234,326,212,322,190,311,187,304],
 [329,217,334,214,340,215,351,221,363,221,382,221,386,225,393,237,390,245,384,251,372,256,362,255,357,249,349,245,338,248,330,250,326,243,334,242,337,233,336,223],
 [535,212,542,200,560,206,565,210,595,211,599,226,595,239,572,245,559,240,546,237,535,235,532,226],
 [228,247,236,240,247,245,278,245,289,248,297,262,294,272,278,281,251,284,231,278,223,272]]
tool_mask=[269,284,278,277,276,271,298,270,351,278,350,301,313,317,279,320,279,309,265,312,264,299]
# Boot rims run over the flesh. The broad outside rectangle picks up the existing transparent outline.
sock_rims={"rear":[[73,531,136,490,146,480,161,481,171,494,184,505,177,531,170,608,63,608],
 [88,520,139,475,150,465,160,465,177,483,186,493,174,515,169,606,88,606],
 [88,535,147,486,163,468,175,467,182,480,201,491,190,515,169,608,88,608],
 [67,541,116,491,125,476,136,475,150,490,170,498,166,523,166,606,67,606]],
 "lead":[[301,600,306,501,324,481,337,477,362,478,374,482,378,509,425,559,426,600],
 [350,600,359,509,386,472,402,471,428,477,434,485,431,517,473,551,475,600],
 [422,600,432,501,450,472,465,470,491,473,503,477,507,508,543,552,544,600],
 [341,600,344,507,351,479,368,476,386,476,401,470,408,482,410,513,473,550,475,600]]}
rear_masks=[None,[237,391,256,403,276,408,270,423,248,446,175,497,156,485,151,473,184,438,215,425,229,405],
 None,[181,381,187,372,199,368,218,368,233,374,234,394,222,435,214,446,171,476,151,501,129,484,140,456,161,433,182,422]]
# Front thigh coordinates for removing each reference leg from the body plate.
lead_masks=[None,[350,385,367,359,382,349,397,347,424,367,456,391,451,423,425,484,392,485,393,438,405,412,406,407],
 None,[296,387,312,367,326,357,337,358,377,394,385,423,396,480,359,484,341,444,340,420,343,412]]
approx=[{"arm":(149,198),"rear-leg":(149,388),"lead-leg":(294,363)},
        {"arm":(388,188),"rear-leg":(165,368),"lead-leg":(403,334)}]

def backfill(im,a,b,length,at_start=False):
    # Grow a rounded cap behind an occluding cuff/hem, continuing the nearest
    # opaque skin pigment into it. Keep the visible stroke above the extension.
    vec=np.asarray(b)-a;vec=vec/np.linalg.norm(vec)
    if at_start:vec=-vec
    anchor=np.asarray(a if at_start else b)
    alpha=np.array(im.getchannel("A"));yy,xx=np.indices(alpha.shape)
    along=(xx-anchor[0])*vec[0]+(yy-anchor[1])*vec[1]
    side=(xx-anchor[0])*(-vec[1])+(yy-anchor[1])*vec[0]
    rgba=np.asarray(im,dtype=float)
    valid=(alpha>220)&(along>-22)&(along<4)&(abs(side)<25)&(rgba[:,:,0]>155)&(rgba[:,:,0]-rgba[:,:,1]>16)&(rgba[:,:,1]-rgba[:,:,2]>6)
    if not np.any(valid):return im
    width=min(max(np.percentile(abs(side[valid]),85),8),18)
    add=(along>=-6)&(along<length)&((side/(width+.01))**2+(np.maximum(along,0)/length)**2<1)
    indices=ndimage.distance_transform_edt(~valid,return_distances=False,return_indices=True)
    colors=np.array(im)[tuple(indices)].copy()
    colors[:,:,3]=add.astype('uint8')*255
    return Image.alpha_composite(Image.fromarray(colors),im)

def make_parts():
    under=Image.open(OUT/"source/hidden-torso.png").convert("RGBA").resize((1247,1261),Image.Resampling.LANCZOS)
    sleeve=cut(frames[2],poly([338,199,351,194,371,188,386,187,399,185,378,221,363,247,353,244,340,231,336,216]))
    variants={}
    for i,key in enumerate(NAMES):
        im=frames[i]
        if i in (0,2):
            test="chamber" if i==0 else "extension"
            arm,rear,lead=[register_patch(test,n,approx[i//2][n]) for n in ["arm","rear-leg","lead-leg"]]
        else:arm,rear,lead=[cut(im,poly(m[i])) for m in (arm_masks,rear_masks,lead_masks)]
        hand=cut(im,poly(hand_masks[i]));far=cut(im,poly(far_masks[i]))
        if i in (0,3):
            m=np.array(far)
            # The proximal end is hidden behind the other glove or blade.
            # Continue the far forearm from skin rather than baking in the near prop.
            x0=315 if i==0 else 313
            m[:,:x0]=0
            far=Image.fromarray(m)
            far=backfill(far,np.array([x0+6,315 if i==0 else 277]),P['far_wrist'][i],22,True)
        if i!=2:
            # The far shoulder in drive is completely hidden by the near sleeve.
            # Paint the tapered continuation and borrow the exposed cloth cap from
            # reach, registered to the shoulder/elbow axis. Both go under the key's paint.
            shoulder,elbow,wrist=[P[n][i] for n in ('far_arm','far_elbow','far_wrist')]
            backing=Image.new("RGBA",SIZE);pen=ImageDraw.Draw(backing)
            def section(a,b,ra,rb):
                direction=b-a;normal=np.array([-direction[1],direction[0]])/np.linalg.norm(direction)
                corners=[tuple(p) for p in [a+ra*normal,b+rb*normal,b-rb*normal,a-ra*normal]]
                pen.polygon(corners,fill='#e6bfa8',outline='#796353',width=2)
            section(shoulder,elbow,12,10);section(elbow,wrist,10,9)
            pen.ellipse((elbow[0]-9,elbow[1]-9,elbow[0]+9,elbow[1]+9),fill='#e6bfa8')
            turn=rot(angles['far_arm'][2]-angles['far_arm'][i])
            shift=P['far_arm'][2]-turn@shoulder
            affine=tuple(np.column_stack([turn,shift]).ravel())
            cap=sleeve.transform(SIZE,Image.Transform.AFFINE,affine,Image.Resampling.BICUBIC)
            backing.alpha_composite(cap)
            if i==1:
                clipped=np.array(far);clipped[:,275:]=0;far=Image.fromarray(clipped)
            far=Image.alpha_composite(backing,far)
        boots={s:cut(im,poly(sock_rims[s][i])) for s in ("rear","lead")}
        # The original wrist or thigh can peek above the rim; preserve the linen and
        # boot but remove enclosed skin pixels at the top using the registered leg.
        for s,skin in [('rear',rear),('lead',lead)]:
            boot=boots[s];alpha=np.array(boot.getchannel("A"))
            overlap=np.array(skin.getchannel("A"))
            alpha=np.minimum(alpha,255-overlap)
            boot.putalpha(Image.fromarray(alpha))
        box=reg[key]["source_box"];toe=reg[key]["source_rear_boot_anchor"]
        tmp=Image.new("RGBA",SIZE);tmp.alpha_composite(under.crop(box),dest=(156-toe[0]+box[0],586-toe[1]+box[1]))
        # Align the generated costume insert to the original on local torso paint.
        a=np.array(im);u=np.array(tmp);y,x=np.indices(a.shape[:2])
        sample=(a[:,:,3]>230)&(y>P["pelvis"][i,1]-42)&(y<P["pelvis"][i,1]+15)&(abs(x-P["pelvis"][i,0])<45)
        sy,sx=np.where(sample);sy=sy[::7];sx=sx[::7]
        _,dx,dy=min((np.mean(abs(a[sy,sx].astype(float)-u[sy-dy,sx-dx])),dx,dy)
                    for dx in range(-17,18) for dy in range(-13,14))
        insert=Image.new("RGBA",SIZE);insert.alpha_composite(tmp,dest=(dx,dy))
        a=np.array(im);u=np.array(insert)
        # Replace all pixels in the arm band with the clean vest when owned by
        # an arm/tool. Retain original face and all original exposed garment paint.
        tool_end=P["tip"][i];wrist=P["wrist"][i];grip=P["grip"][i]
        weapon_zone=poly([*(grip+[-11,-13]),*(tool_end+[-30,-24]),*(tool_end+[8,-5]),*(tool_end+[7,9]),*(grip+[15,25]),*(grip+[-12,14])])
        remove=np.maximum.reduce([np.asarray(q.getchannel("A")) for q in [arm,hand,far]])
        remove=np.maximum(remove,np.asarray(weapon_zone))
        remove=ndimage.maximum_filter(remove,9)>8
        # Use the underplate for the entire side at limb height, to avoid isolated
        # fragments where the old sleeve or tool crossed empty space.
        top=int(P['head'][i,1]+11)
        lo=int(max(P['far_hand'][i,1]+28,P['wrist'][i,1]+36))
        band=(y>top)&(y<lo)&((a[:,:,3]>0)|(u[:,:,3]>0))
        outside=(u[:,:,3]<22)&band
        a[remove|outside]=u[remove|outside]
        # Restore only the costume upper half from the underpainting; crop the
        # visible skin off at its short hems using a per-column painted contour.
        for limb in [rear,lead]:
            la=np.array(limb.getchannel("A"))
            valid=la>115
            xs=np.where(valid.any(axis=0))[0]
            topcurve=np.argmax(valid[:,xs],axis=0)
            # Per-column cut extends below the first skin pixel, leaving the
            # shorts above it, including lips on the outer sides.
            for col,row in zip(xs,topcurve):
                a[row:,col,3]=0
        a[y>max(P['rear_thigh'][i,1],P['lead_thigh'][i,1])+46,3]=0
        # A bent leg can occlude one lip of the shorts while the other lip is
        # still visible below the skin. Keep that original dark cloth contour.
        ref=np.asarray(im); rgb=ref[:,:,:3].astype(float)
        hemzone=(y>P['pelvis'][i,1]+7)&(y<[440,415,414,408][i])&(abs(x-P['pelvis'][i,0])<132)
        cloth=(rgb[:,:,0]<171)&(rgb[:,:,0]>47)&(rgb[:,:,0]-rgb[:,:,1]>12)&(rgb[:,:,1]-rgb[:,:,2]>5)&(ref[:,:,3]>35)
        restore=hemzone&cloth
        a[restore]=ref[restore]
        body=Image.fromarray(a)
        # Strip isolated remnants outside the connected head/clothing plate.
        mask=a[:,:,3]>25;lab,_=ndimage.label(mask)
        counts=np.bincount(lab.ravel());counts[0]=0
        keep=ndimage.binary_dilation(lab==counts.argmax(),iterations=2);a[~keep]=0
        body=Image.fromarray(a)
        arm=backfill(arm,P['elbow'][i],P['wrist'][i],14)
        rear=backfill(rear,P['rear_thigh'][i],P['rear_knee'][i],19,True)
        rear=backfill(rear,P['rear_knee'][i],P['rear_ankle'][i],17)
        if i==0:
            lead=backfill(lead,P['lead_thigh'][0],P['lead_knee'][0],20,True)
            variants['lead']={0:backfill(lead,P['lead_knee'][0],P['lead_ankle'][0],15)}
        if i==2:
            blade=cut(im,poly([594,216,599,215,600,198,604,193,648,193,668,205,668,219,626,237,601,241,599,229,591,230]))
            wood=Image.new("RGBA",SIZE);d=ImageDraw.Draw(wood)
            d.line([(552,231),(608,217)],fill="#51392b",width=12)
            d.line([(552,229),(607,216)],fill="#986d4c",width=7)
            variants['tool']={2:Image.alpha_composite(wood,blade)}
        for slot,part in [('body',body),('far',far),('arm',arm),('hand',hand),('rear',rear),('rear_boot',boots['rear']),('lead_boot',boots['lead'])]:
            variants.setdefault(slot,{})[i]=part
    return variants
for side in ["rear","lead"]:
    n=side+"_hem";P[n]=P[side+"_thigh"].copy();angles[n]=np.zeros(4);parent[n]="pelvis"

# Front-leg cage, following the short hem, knee contours and sock rim seen in
# each drawing. All vertices sample the same chamber texture throughout the clip.
front_cage={
 'hip_in':[(297,405),(352,386),(405,383),(294,390)],
 'hip_out':[(335,368),(399,348),(451,338),(334,356)],
 'knee_in':[(347,423),(408,405),(482,400),(344,413)],
 'knee_out':[(383,413),(447,395),(522,385),(380,402)],
 'calf_in':[(335,454),(396,444),(466,445),(347,449)],
 'calf_out':[(375,450),(438,442),(509,440),(386,449)],
 'sock_in':[(337,482),(394,477),(465,473),(359,481)],
 'sock_out':[(366,484),(421,483),(494,477),(391,478)]}
for n,pts in front_cage.items():
    P[n]=np.asarray(pts,float);angles[n]=np.zeros(4)
    parent[n]='lead_thigh' if n.startswith('hip') else 'lead_ankle' if n.startswith('sock') else 'lead_knee'
segment_ends={n:aim[n] for n in ('arm','elbow','far_arm','far_elbow','rear_thigh','rear_knee')}
for n,target in segment_ends.items():
    end=n+'_end';P[end]=P[target];angles[end]=angles[n];parent[end]=n

SLOTS=["far","rear","lead","body","rear_boot","lead_boot","arm","tool","hand"]
# Poses 0 through 3 are the drawings. Passing the drive silhouette on release
# keeps the long arm from jumping straight to the deeply folded recovery shape.
TICKS=np.array([0,6,13,19,27,39,49,60])/60
POSES=np.array([0,1,2,2,1,3,3,0])
CHANGE=[(0,0),(.063,1),(.165,2),(.389,1),(.56,3),(.923,0)]
slot_bias={"far":.014,"arm":0,"rear":-.009,"hand":.003,"body":-.003,"rear_boot":.008,"lead_boot":.008}
# Scale only the front limb to follow the different projected lengths across keys.
def smoothstep(x):
    x=np.clip(x,0,1);return x*x*(3-2*x)

def rot(angle):
    c=np.cos(np.radians(angle));s=np.sin(np.radians(angle));return np.array([[c,-s],[s,c]])

def wrap(a):return (a+180)%360-180

def world_poses():
    ts=np.arange(61)/60
    positions={}
    for n,data in P.items():
        keys=data[POSES].copy()
        if n not in ("rear_toe","lead_toe"):
            # Follow-through and braking carry the shoulder a little past contact;
            # keep the registered feet at rest on the held frame.
            if n in ("head","chest","arm","elbow","wrist","grip","tip","far_arm"):
                keys[3]+=[3,3];keys[4]+=[9,5]
            if n in ("pelvis","rear_hem","lead_hem"):keys[3]+=[2,3];keys[4]+=[6,4]
        pos=PchipInterpolator(TICKS,keys,axis=0)(ts)
        if n in ("lead_ankle","lead_toe","sock_in","sock_out","calf_in","calf_out"):
            for lo,hi,h in [(0,13,12),(19,39,8),(49,60,5)]:
                t=np.arange(lo,hi+1);pos[t,1]-=h*(.55 if n.startswith('calf') else 1)*np.sin(np.pi*(t-lo)/(hi-lo))**2
        positions[n]=pos
    # The off hand sweeps below the chest as the shoulders turn; interpolate the
    # direction of its chain instead of shortening it by moving through the ribs.
    for n,target in [("far_arm","far_elbow"),("far_elbow","far_wrist"),("far_wrist","far_hand")]:
        ang=np.unwrap(np.radians(angles[n][POSES]));a=PchipInterpolator(TICKS,ang)(ts)
        length=PchipInterpolator(TICKS,np.linalg.norm(P[target]-P[n],axis=1)[POSES])(ts)
        positions[target]=positions[n]+np.stack([np.cos(a),np.sin(a)],axis=1)*length[:,None]
    direction={}
    for n in P:
        if n in aim:
            diff=positions[aim[n]]-positions[n]
            direction[n]=np.degrees(np.unwrap(np.arctan2(diff[:,1],diff[:,0])))
        else:direction[n]=PchipInterpolator(TICKS,angles[n][POSES])(ts)
    for n,target in segment_ends.items():
        positions[n+'_end']=positions[target];direction[n+'_end']=direction[n]
    return positions,direction

def local_matrix(pos,angle,p,pa):
    return rot(-pa)@(pos-p),wrap(angle-pa)

def weights(slot,points,i):
    xy=np.array(points)
    if slot=='lead':
        names=list(front_cage)
        points=np.array([P[n][i] for n in names]);tri=Delaunay(points)
        sums={n:np.zeros(len(xy)) for n in names}
        for j,v in enumerate(xy):
            bary=np.einsum('ijk,ik->ij',tri.transform[:,:2],v-tri.transform[:,2])
            bary=np.column_stack([bary,1-bary.sum(axis=1)])
            nearest=np.argmax(bary.min(axis=1))
            w=np.clip(bary[nearest],0,1);w/=w.sum()
            for k,b in zip(tri.simplices[nearest],w):sums[names[k]][j]=b
        return sums
    if slot in ('arm','far','rear'):
        chain={'arm':['arm','elbow','wrist'],'far':['far_arm','far_elbow','far_wrist','far_hand'],
               'rear':['rear_thigh','rear_knee','rear_ankle'],'lead':['lead_thigh','lead_knee','lead_ankle']}[slot]
        sums={}
        for n,target in zip(chain,chain[1:]):
            a,b=P[n][i],P[target][i];d=b-a
            s=np.clip(np.sum((xy-a)*d,axis=1)/(d@d),.06,.94)
            dist=np.linalg.norm(xy-a-s[:,None]*d,axis=1)
            influence=1/((dist**2+64)**1.9)
            if n in segment_ends:
                f=np.clip(np.sum((xy-a)*d,axis=1)/(d@d),0,1)
                sums[n]=influence*(1-f);sums[n+'_end']=influence*f
            else:sums[n]=influence
        if slot=="far":
            d=np.linalg.norm(xy-P["far_hand"][i],axis=1)
            sums["far_wrist"]+=1/((d*d+64)**1.9)
    elif slot=='body':
        y=xy[:,1];neck=P['head'][i,1]+9;waist=P['pelvis'][i,1]-24
        h=1-smoothstep((y-neck+5)/26);low=smoothstep((y-waist+18)/41)
        sums={'head':h,'chest':(1-h)*(1-low),'pelvis':(1-h)*low}
        hem=smoothstep((y-P['pelvis'][i,1]-2)/45)*.88
        left=smoothstep((P['pelvis'][i,0]+15-xy[:,0])/33)
        sums={k:v*(1-hem) for k,v in sums.items()}
        sums.update(rear_hem=hem*left,lead_hem=hem*(1-left))
    else:
        n={'hand':'grip','tool':'grip','rear_boot':'rear_ankle','lead_boot':'lead_ankle'}[slot]
        sums={n:np.ones(len(xy))}
        if slot.endswith('_boot'):
            toe=slot.split('_')[0]+'_toe'
            end=smoothstep((xy[:,1]-P[n][i,1]-20)/66)
            sums={n:1-end,toe:end}
    total=sum(sums.values());return {n:w/total for n,w in sums.items()}

def weighted_mesh(name,slot,i,rect,bone_ids,source_scale=1,source_offset=(0,0)):
    l,t,r,b=rect;w,h=r-l,b-t
    spacing=11 if slot in ('arm','rear','lead','far') else 15
    nx=max(2,math.ceil(w/spacing));ny=max(2,math.ceil(h/spacing))
    border=([(x,0) for x in range(nx+1)]+[(nx,y) for y in range(1,ny+1)]+
            [(x,ny) for x in reversed(range(nx))]+[(0,y) for y in reversed(range(1,ny))])
    grid=border+[(x,y) for y in range(1,ny) for x in range(1,nx)]
    index={v:j for j,v in enumerate(grid)}
    uv=np.asarray(grid)/[nx,ny];points=(uv*[w,h]+[l,t])*source_scale+source_offset
    wg=weights(slot,points,i);v=[]
    local={}
    for n in wg:
        local[n]=(points-P[n][i])@rot(angles[n][i])
    for j in range(len(grid)):
        used=[n for n in wg if wg[n][j]>.00015];total=sum(wg[n][j] for n in used);v.append(len(used))
        for n in used:
            x,y=local[n][j];v.extend([bone_ids[n],round(x,4),round(-y,4),round(wg[n][j]/total,7)])
    triangles=[]
    for y in range(ny):
        for x in range(nx):
            a,bb,c,d=[index[p] for p in [(x,y),(x+1,y),(x+1,y+1),(x,y+1)]]
            triangles.extend([a,bb,c,a,c,d])
    return dict(type="mesh",width=w,height=h,uvs=uv.round(7).ravel().tolist(),vertices=v,triangles=triangles,hull=len(border))

def build():
    PARTS.mkdir(parents=True,exist_ok=True)
    positions,direction=world_poses()
    bones=[dict(name='root')];anim={};ids={'root':0}
    for n in P:
        up=parent[n];values=[]
        for j in range(61):
            p,pa=(np.array([156,586]),0) if up=='root' else (positions[up][j],direction[up][j])
            loc,angle=local_matrix(positions[n][j],direction[n][j],p,pa)
            values.append([loc[0],-loc[1],-angle])
        values=np.asarray(values);values[:,2]=np.degrees(np.unwrap(np.radians(values[:,2])))
        x,y,a=values[0];ids[n]=len(bones)
        bones.append(dict(name=n,parent=up,x=round(x,4),y=round(y,4),rotation=round(a,4)))
        move=values-values[0]
        anim[n]={"translate":[dict(time=round(j/60,7),x=round(x,4),y=round(y,4)) for j,(x,y,_) in enumerate(move)],
                 "rotate":[dict(time=round(j/60,7),value=round(a,4)) for j,(_,_,a) in enumerate(move)]}
    variants=make_parts();slots=[];attachments={};slot_tracks={};textures={}
    for n in SLOTS:
        imgs=variants[n];names={i:(n if len(imgs)==1 else n+'_'+NAMES[i]) for i in imgs}
        slots.append(dict(name=n,bone='root',attachment=names[min(imgs)]))
        attachments[n]={}
        if len(imgs)>1:
            slot_tracks[n]={"attachment":[dict(time=round(t+(slot_bias[n] if j else 0),5),name=names[k]) for j,(t,k) in enumerate(CHANGE)]}
        for i,im in imgs.items():
            a=im.getchannel("A").point(lambda v:255 if v>6 else 0);l,t,r,b=a.getbbox()
            rect=(l-2,t-2,r+2,b+2);nm=names[i];tex=im.crop(rect)
            tex.save(PARTS/(nm+'.png'));textures[nm]=tex
            attachments[n][nm]=weighted_mesh(nm,n,i,rect,ids)
    name='viridis-keyed-thrust'
    rig=dict(skeleton=dict(spine="4.2.36",images="./parts/"),bones=bones,slots=slots,
             skins=[dict(name="default",attachments=attachments)],animations={"attack":{"bones":anim,"slots":slot_tracks}})
    (OUT/(name+".json")).write_text(json.dumps(rig,separators=(',',':'))+'\n')
    page=Image.new("RGBA",(2048,1024));atlas=[name+'.png','size:2048,1024','filter:Linear,Linear','pma:false']
    x=y=3;row=0
    for n,im in sorted(textures.items(),key=lambda kv:-kv[1].height):
        w,h=im.size
        if x+w+3>2048:x=3;y+=row+5;row=0
        assert y+h+3<=1024
        page.paste(im,(x,y));atlas.extend([n,f'bounds:{x},{y},{w},{h}']);x+=w+5;row=max(row,h)
    page.save(OUT/(name+'.png'));(OUT/(name+'.atlas')).write_text('\n'.join(atlas)+'\n')
    with ZipFile(ROOT/'outputs/viridis-animation/Viridis-keyed-thrust.zip','w',ZIP_DEFLATED) as z:
        for p in [OUT/(name+ext) for ext in ['.json','.atlas','.png']]+list(sorted(PARTS.glob('*.png'))):z.write(p,p.relative_to(OUT))

if __name__=='__main__':build()
