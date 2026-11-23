"""Register the two painted views locally on overlap frames, then pack them as Spine replacement drawings."""
import json
from zipfile import ZipFile,ZIP_DEFLATED
from concurrent.futures import ThreadPoolExecutor
import numpy as np
from PIL import Image
from scipy import ndimage as nd
from build_guard_thrust import OUT,ROOT,NAME,ORIGIN,DURATION,ENTER,CORE,EXIT,build,slot_attachment,opacity
from preview_guard_thrust import Playback

def surface(player,f,mode,group):
    rgb=np.array(player.frame('attack',f/60,mode,(0,0,0),group),dtype=np.float32)/255
    white=np.array(player.frame('attack',f/60,mode,(255,255,255),group),dtype=np.float32)/255
    rgba=np.dstack((rgb,np.clip(1-np.median(white-rgb,axis=2),0,1)))
    yy,xx=np.where(rgba[:,:,3]>.035)
    l,r=xx.min()-2,xx.max()+3;t,b=yy.min()-2,yy.max()+3
    return rgba[t:b,l:r].copy(),np.array([l,t,r,b],float)

def contours(im):
    mask=im[:,:,3]>.18
    rows=np.where(mask.any(axis=1))[0]
    left=np.argmax(mask[rows],axis=1)
    right=im.shape[1]-np.argmax(mask[rows,::-1],axis=1)
    return [nd.gaussian_filter1d(np.interp(np.arange(len(mask)),rows,edge),3.5) for edge in [left,right]]

def align_piece(a,b,u,ink):
    # Register each material separately. Matching the sleeve to the sleeve keeps
    # a moving hand or trowel from dragging the vest paint underneath it.
    (im0,box0),(im1,box1)=a,b
    l,t,r,bot=np.rint((1-u)*box0+u*box1).astype(int)
    yy,xx=np.indices((bot-t,r-l),dtype=np.float32);yn=(yy[:,0]+.5)/(bot-t)
    sides=[];source_y=[]
    for im in [im0,im1]:
        sy=yn*im.shape[0]-.5
        source_y.append(sy)
        sides.append([np.interp(sy,np.arange(im.shape[0]),line) for line in contours(im)])
    left=(1-u)*sides[0][0]+u*sides[1][0]
    right=(1-u)*sides[0][1]+u*sides[1][1]
    at=(xx-left[:,None])/np.maximum((right-left)[:,None],1)
    def project(im,j):
        lo,hi=sides[j];x=lo[:,None]+at*(hi-lo)[:,None]
        y=np.broadcast_to(source_y[j][:,None],x.shape)
        return np.stack([nd.map_coordinates(ch,[y,x],order=1,prefilter=False,mode='constant') for ch in im.transpose(2,0,1)],2)
    # Hold each painting through most of the movement; switch ink only once
    # the silhouettes coincide. The mesh still moves continuously on each frame.
    rgba=(1-ink)*project(im0,0)+ink*project(im1,1)
    alpha=rgba[:,:,3];rgb=rgba[:,:,:3]/np.maximum(.001,alpha[:,:,None])
    return l,t,Image.fromarray(np.uint8(np.rint(255*np.clip(np.dstack((rgb,alpha)),0,1))))

def transfer(player,f):
    u=opacity(f if f<ENTER else (DURATION-f)*ENTER/EXIT,5,11.4)
    result=Image.new('RGBA',(608,520))
    switches={'far':.40,'rear':.59,'lead':.45,'body':.55,'rear_boot':.65,'lead_boot':.62,'arm':.45,'tool':.63,'hand':.50}
    for group,cross in switches.items():
        a=surface(player,f,'guard',group);b=surface(player,f,'attack',group)
        ink=opacity(u,cross-.09,cross+.09)
        l,t,piece=align_piece(a,b,u,ink);result.alpha_composite(piece,(l,t))
    l,t,r,b=result.getbbox();l-=2;t-=2
    return f,l,t,result.crop((l,t,r+2,b+2))

def main():
    build()
    p=Playback();rig=p.rig;slots=rig['slots'];skin=rig['skins'][0]['attachments']
    tag='paint_transition';slots.append(dict(name=tag,bone='root'));skin[tag]={}
    motion=rig['animations']['attack']['slots'];idle=rig['animations']['idle_loop']['slots']
    idle[tag]={'attachment':[slot_attachment(0,None)]}
    spans=[list(range(5,12)),list(range(60,72))]
    keys=[slot_attachment(0,None)]
    for span in spans:
        lo,hi=span[0],span[-1]+1
        for n,tr in motion.items():
            if n=='guard_reference':continue
            events=tr['attachment']
            last=next((k.get('name') for k in reversed(events) if k['time']<=hi/60+1e-7),None)
            events[:]=[k for k in events if not lo/60-1e-7<=k['time']<=hi/60+1e-7]
            events.extend([slot_attachment(lo,None),slot_attachment(hi,last)])
            events.sort(key=lambda k:k['time'])
    # Use a clean playback object for rendering the two registered source paintings.
    src=Playback()
    with ThreadPoolExecutor(max_workers=3) as pool:
        for f,l,t,tex in pool.map(lambda f:transfer(src,f),sum(spans,[])):
            n='transfer_%02d'%f;tex.save(OUT/'parts'/(n+'.png'));w,h=tex.size
            skin[tag][n]=dict(width=w,height=h,x=float(l+w/2-ORIGIN[0]),y=float(ORIGIN[1]-t-h/2))
            keys.append(slot_attachment(f,n))
    for span in spans:keys.append(slot_attachment(span[-1]+1,None))
    motion[tag]={'attachment':sorted(keys,key=lambda k:k['time'])}
    (OUT/(NAME+'.json')).write_text(json.dumps(rig,separators=(',',':'))+'\n')
    files=[OUT/'parts'/(n+'.png') for table in skin.values() for n in table]
    textures=[(f.stem,Image.open(f).convert('RGBA')) for f in files]
    atlas=[NAME+'.png','size:4096,2048','filter:Linear,Linear','pma:false']
    page=Image.new('RGBA',(4096,2048));x=y=3;row=0
    for n,im in sorted(textures,key=lambda v:-v[1].height):
        w,h=im.size
        if x+w+3>4096:x=3;y+=row+5;row=0
        assert y+h+3<=2048
        page.paste(im,(x,y));atlas.extend([n,f'bounds:{x},{y},{w},{h}']);x+=w+5;row=max(row,h)
    page.save(OUT/(NAME+'.png'));(OUT/(NAME+'.atlas')).write_text('\n'.join(atlas)+'\n')
    with ZipFile(ROOT/'outputs/viridis-animation/Viridis-guard-thrust.zip','w',ZIP_DEFLATED) as z:
        for f in [OUT/(NAME+s) for s in ('.atlas','.png','.json')]+files:z.write(f,f.relative_to(OUT))

if __name__=='__main__':main()
