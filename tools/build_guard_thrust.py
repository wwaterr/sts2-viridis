"""Register the approved fast thrust to the source guard sprite and author entry/exit meshes."""
import json
import numpy as np
from PIL import Image
from scipy import ndimage
import build_keyed_thrust as keyed

ROOT=keyed.ROOT
OUT=keyed.BASE/'guard-thrust'
OLD=keyed.OUT
NAME='viridis-guard-thrust'
SOURCE=keyed.BASE.parent/'shovel-strike/frames/guard.png'
CUT=keyed.BASE/'knee-on-shin'
SCALE=.86
TOE=np.array([131.,502.])
ORIGIN=np.array([235.,504.])
REGISTER=np.array([156.,586.])
ENTER,CORE,EXIT=12,46,22
DURATION=ENTER+CORE+EXIT
GUARD_POINTS={
 'pelvis':(235,282),'chest':(225,194),'head':(230,143),
 'arm':(200,163),'elbow':(181,219),'wrist':(159,250),'grip':(135,278),'tip':(69,223),
 'far_arm':(258,167),'far_elbow':(287,220),'far_wrist':(313,250),'far_hand':(350,282),
 'rear_thigh':(186,319),'rear_knee':(166,363),'rear_ankle':(126,414),'rear_toe':(131,502),
 'lead_thigh':(282,321),'lead_knee':(311,366),'lead_ankle':(345,419),'lead_toe':(388,502),
 'hip_in':(264,328),'hip_out':(301,311),'knee_in':(293,373),'knee_out':(320,357),
 'calf_in':(311,391),'calf_out':(339,384),'sock_in':(330,422),'sock_out':(350,414)}
GUARD_POINTS.update(rear_hem=GUARD_POINTS['rear_thigh'],lead_hem=GUARD_POINTS['lead_thigh'])
for n,target in keyed.segment_ends.items():GUARD_POINTS[n+'_end']=GUARD_POINTS[target]

def source_layers(guard):
    rig=json.loads((CUT/'viridis-fullbody.json').read_text())
    origins={}
    for b in rig['bones']:
        origins[b['name']]=origins.get(b.get('parent'),ORIGIN)+[b['x'],-b['y']]
    part={}
    for s in rig['slots']:
        n=s['name'];a=rig['skins'][0]['attachments'][n][n]
        x,y=origins[s['bone']]+[a['x']-a['width']/2,-a['y']-a['height']/2]
        canvas=Image.new('RGBA',guard.size)
        canvas.paste(Image.open(CUT/'parts'/(n+'.png')),(round(x),round(y)))
        part[n]=canvas
    def stack(*images):
        canvas=Image.new('RGBA',guard.size)
        for p in images:canvas.alpha_composite(p)
        return canvas
    legs={};shorts=[]
    for side in ['rear','lead']:
        thigh=np.array(part[side+'_thigh']);y=np.indices(thigh.shape[:2])[0]
        skin=(thigh[:,:,0]>164)&(thigh[:,:,1]>129)&(thigh[:,:,3]>60)
        # The dark ink below the hem belongs to the exposed thigh. Packing it
        # with the shorts leaves an ink spur when the leg starts to bend.
        cloth=(~ndimage.binary_dilation(skin,iterations=3))&(y<344)
        fabric=thigh.copy();fabric[:,:,3]*=cloth
        thigh[:,:,3]*=~cloth;shorts.append(Image.fromarray(fabric))
        legs[side]=stack(Image.fromarray(thigh),part[side+'_shin'])
    layers=dict(far=stack(*(part[n] for n in ['open_upper','open_fore','open_hand'])),
        rear=legs['rear'],lead=legs['lead'],
        body=stack(part['pelvis'],*shorts,part['torso'],part['head']),
        rear_boot=part['rear_boot'],lead_boot=part['lead_boot'],
        arm=stack(part['upper_arm'],part['forearm']),tool=part['shovel'],hand=part['hand'])
    # Keep the exact source ink on the frontmost painted surface. The cutout
    # underpaint is retained where another layer covers the texture.
    src=np.array(guard);covered=np.zeros(src.shape[:2],bool)
    for n in reversed(keyed.SLOTS):
        a=np.array(layers[n]);visible=(a[:,:,3]>28)&(~covered)
        take=visible&(src[:,:,3]>8)
        a[take,:3]=src[take,:3]
        a[visible&(src[:,:,3]<8),3]=0
        covered|=a[:,:,3]>200
        layers[n]=Image.fromarray(a)
    return layers

def geometry(i,u):
    # u=0 is guard; u=1 is one of the existing painted key poses.
    g={n:REGISTER+(np.array(p)-TOE)/SCALE for n,p in GUARD_POINTS.items()}
    pos={n:v+(keyed.P[n][i]-v)*u for n,v in g.items()}
    head_arc=np.sin(np.pi*u)**2
    pos['head']+=np.array([0,2.5])*head_arc
    for n in ['lead_ankle','lead_toe','sock_in','sock_out']:pos[n][1]-=6*head_arc
    # Rotate the tucked wrist and trowel around their joints. Polar interpolation
    # preserves their length while the blade travels up through the front.
    for n in ['arm','elbow','wrist','grip','far_arm','far_elbow','far_wrist']:
        target=keyed.aim[n];v=g[target]-g[n];end=keyed.P[target][i]-keyed.P[n][i]
        ang=np.degrees(np.arctan2(v[1],v[0]))
        angle=ang+u*keyed.wrap(np.degrees(np.arctan2(end[1],end[0]))-ang)
        length=(1-u)*np.linalg.norm(v)+u*np.linalg.norm(end)
        pos[target]=pos[n]+keyed.rot(angle)@np.array([length,0])
    angles={}
    for n in keyed.P:
        if n in keyed.aim:
            d=pos[keyed.aim[n]]-pos[n];angles[n]=np.degrees(np.arctan2(d[1],d[0]))
        else:angles[n]=keyed.angles[n][i]*u
    for n,target in keyed.segment_ends.items():
        pos[n+'_end']=pos[target];angles[n+'_end']=angles[n]
    return pos,angles

def local_pose(bones,i,u):
    pos,angles=geometry(i,u);out={}
    for b in bones[1:]:
        n=b['name'];up=b['parent']
        p,pa=(REGISTER,0) if up=='root' else (pos[up],angles[up])
        loc,ang=keyed.local_matrix(pos[n],angles[n],p,pa)
        out[n]=[loc[0]-b['x'],-loc[1]-b['y'],keyed.wrap(-ang-b['rotation'])]
    return out

def opacity(k,lo,hi):
    return float(keyed.smoothstep((k-lo)/(hi-lo)))

def color_key(t,v):
    return dict(time=round(t/60,7),color='ffffff'+format(round(np.clip(v,0,1)*255),'02x'))

def slot_attachment(t,n):
    return {'time':round(t/60,7),**({'name':n} if n else {})}

def build():
    (OUT/'parts').mkdir(parents=True,exist_ok=True)
    rig=json.loads((OLD/'viridis-keyed-thrust-quick.json').read_text())
    bones=rig['bones'];old_slots=rig['slots'];skin=rig['skins'][0]['attachments']
    core=rig['animations']['attack'];guard=Image.open(SOURCE).convert('RGBA')
    tex={n:Image.open(OLD/'parts'/(n+'.png')).convert('RGBA') for slot in skin.values() for n in slot}
    gpos,gang=geometry(0,0)
    ids={b['name']:j for j,b in enumerate(bones)}
    # Build the source attachments in the old attack coordinate system so each
    # of the two sets of paintings deforms on the same articulated chain.
    origp={n:v[0].copy() for n,v in keyed.P.items()};origa={n:v[0] for n,v in keyed.angles.items()}
    for n in origp:keyed.P[n][0]=gpos[n];keyed.angles[n][0]=gang[n]
    for n,plate in source_layers(guard).items():
        tag='rest_'+n
        l,t,r,b=plate.getchannel('A').getbbox();rect=(l-2,t-2,r+2,b+2)
        part=plate.crop(rect);tex[tag]=part
        if n=='hand':
            # The glove follows the wrist-to-fist axis independently of the blade.
            attach=keyed.weighted_mesh(tag,n,0,rect,ids,1/SCALE,REGISTER-TOE/SCALE)
            vs=attach['vertices']
            for j in range(0,len(vs),5):
                relative=keyed.rot(gang['grip'])@np.array([vs[j+2],-vs[j+3]])+gpos['grip']-gpos['wrist']
                xy=keyed.rot(-gang['wrist'])@relative
                vs[j+1:j+4]=[ids['wrist'],round(xy[0],5),round(-xy[1],5)]
        else:attach=keyed.weighted_mesh(tag,n,0,rect,ids,1/SCALE,REGISTER-TOE/SCALE)
        skin[tag]={tag:attach}
    for n in origp:keyed.P[n][0]=origp[n];keyed.angles[n][0]=origa[n]
    # Exact, unscaled source plate at the gameplay origin for setup and idle.
    tex['guard_reference']=guard
    skin['guard_reference']={'guard_reference':dict(width=guard.width,height=guard.height,
        x=guard.width/2-ORIGIN[0],y=ORIGIN[1]-guard.height/2)}
    base_slots=[]
    for s in old_slots:
        n=s['name'];base_slots.extend([{'name':n,'bone':'root'},{'name':'rest_'+n,'bone':'root'}])
    base_slots.append(dict(name='guard_reference',bone='root',attachment='guard_reference'))
    rig['slots']=base_slots
    entry=[local_pose(bones,0,float(keyed.smoothstep(f/ENTER))) for f in range(ENTER+1)]
    exit_poses=[local_pose(bones,3,1-float(keyed.smoothstep(f/EXIT))) for f in range(EXIT+1)]
    tracks={}
    for n,bt in core['bones'].items():
        tframes=[];rframes=[]
        def key(t,v):
            tframes.append(dict(time=round(t,7),x=round(v[0],5),y=round(v[1],5)))
            angle=v[2];last=rframes[-1]['value'] if rframes else angle
            rframes.append(dict(time=round(t,7),value=round(last+keyed.wrap(angle-last),5)))
        for f,pose in enumerate(entry[:-1]):key(f/60,pose[n])
        for mv,rv in zip(bt['translate'],bt['rotate']):
            if mv['time']>CORE/60+1e-6:break
            key(ENTER/60+mv['time'],[mv['x'],mv['y'],rv['value']])
        for f,pose in enumerate(exit_poses[1:],1):key((ENTER+CORE+f)/60,pose[n])
        tracks[n]=dict(translate=tframes,rotate=rframes)
    # Spine requires parents first: insert registration after root and remap weights.
    for b in bones[1:]:
        if b['parent']=='root':b['parent']='attack_registration'
    bones.insert(1,dict(name='attack_registration',parent='root',x=float(TOE[0]-ORIGIN[0]),
                        y=float(ORIGIN[1]-TOE[1]),scaleX=SCALE,scaleY=SCALE))
    for table in skin.values():
        for a in table.values():
            if a.get('type')!='mesh':continue
            v=a['vertices'];j=0
            while j<len(v):
                count=v[j];j+=1
                for _ in range(count):
                    if v[j]>0:v[j]+=1
                    j+=4
    s_tracks={};idle={}
    for s in old_slots:
        n=s['name'];tag='rest_'+n
        swaps=[slot_attachment(0,None),slot_attachment(3,s['attachment'])]
        for k in core['slots'].get(n,{}).get('attachment',[]):
            if 0<k['time']<=CORE/60:swaps.append(slot_attachment(ENTER+k['time']*60,k['name']))
        swaps.append(slot_attachment(DURATION-2,None))
        # Stagger the redraw at the end of preparation by anatomy. At the actual
        # idle boundary only the reference plate is visible.
        lo,hi={'tool':(6,9),'hand':(7,10),'arm':(7,11),'body':(6,11)}.get(n,(6,10))
        fight_alpha=[];rest_alpha=[]
        for f in range(DURATION+1):
            if f<=ENTER:q=opacity(f,lo,hi)
            elif f<ENTER+CORE:q=1.
            else:q=opacity((DURATION-f)*ENTER/EXIT,lo,hi)
            fight_alpha.append(color_key(f,q))
            rest_alpha.append(color_key(f,(1-q)))
        s_tracks[n]={'attachment':swaps,'rgba':fight_alpha}
        s_tracks[tag]={'attachment':[slot_attachment(0,None),slot_attachment(1,tag),slot_attachment(ENTER,None),
                                     slot_attachment(ENTER+CORE,tag),slot_attachment(DURATION,None)],'rgba':rest_alpha}
        for slot in [n,tag]:idle[slot]={'attachment':[slot_attachment(0,None)]}
    s_tracks['guard_reference']={'attachment':[slot_attachment(0,'guard_reference'),slot_attachment(3,None),
       slot_attachment(DURATION-2,'guard_reference')],'rgba':[color_key(0,1),color_key(1,.88),color_key(2,.35),
           color_key(3,0),color_key(DURATION-3,0),color_key(DURATION-2,.35),color_key(DURATION-1,.88),color_key(DURATION,1)]}
    idle['guard_reference']={'attachment':[slot_attachment(0,'guard_reference'),slot_attachment(60,'guard_reference')],
                             'rgba':[color_key(0,1)]}
    rig['animations']={'idle_loop':{'slots':idle},'attack':{'bones':tracks,'slots':s_tracks}}
    (OUT/(NAME+'.json')).write_text(json.dumps(rig,separators=(',',':'))+'\n')
    for n,im in tex.items():im.save(OUT/'parts'/(n+'.png'))
    page=Image.new('RGBA',(2048,2048));atlas=[NAME+'.png','size:2048,2048','filter:Linear,Linear','pma:false']
    x=y=3;row=0
    for n,im in sorted(tex.items(),key=lambda kv:-kv[1].height):
        w,h=im.size
        if x+w+3>2048:x=3;y+=row+5;row=0
        assert y+h+3<=2048
        page.paste(im,(x,y));atlas.extend([n,f'bounds:{x},{y},{w},{h}']);x+=w+5;row=max(row,h)
    page.save(OUT/(NAME+'.png'));(OUT/(NAME+'.atlas')).write_text('\n'.join(atlas)+'\n')
if __name__=='__main__':
    from finish_guard_thrust import main
    main()
