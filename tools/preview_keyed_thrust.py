"""Sample the exported Spine tracks and active attachments for a registered side-by-side preview."""
import json
from bisect import bisect_right
from pathlib import Path
from subprocess import run
import numpy as np
from PIL import Image, ImageDraw, ImageFont
from build_keyed_thrust import ROOT,OUT,STUDY,NAMES
from thrust_raster import raster

EXPORT=ROOT/'outputs/viridis-animation'
BG=(40,49,54)
class Player:
    def __init__(self,rig_path=OUT/'viridis-keyed-thrust.json'):
        self.rig=json.loads(rig_path.read_text())
        self.anim=self.rig['animations']['attack'];self.skin={}
        for slot,attachments in self.rig['skins'][0]['attachments'].items():
            for name,a in attachments.items():
                local=[];vertex=[];bones=[];weights=[]
                uv=np.asarray(a['uvs']).reshape(-1,2);data=iter(a['vertices'])
                for j in range(len(uv)):
                    for _ in range(next(data)):
                        b,x,y,w=[next(data) for _ in range(4)]
                        bones.append(b);local.append([x,y,1]);weights.append(w);vertex.append(j)
                assert list(data)==[]
                tex=np.array(Image.open(OUT/'parts'/(name+'.png')))
                self.skin[name]=(tex,uv,np.array(a['triangles']).reshape(-1,3),np.array(vertex),np.array(bones),np.array(local),np.array(weights))

        self.times={n:[k['time'] for k in tracks['translate']] for n,tracks in self.anim['bones'].items()}

    def pose(self,j):
        world=[];byname={};time=j/60
        for b in self.rig['bones']:
            tracks=self.anim['bones'].get(b['name']);x=b.get('x',0);y=b.get('y',0);a=b.get('rotation',0)
            if tracks:
                times=self.times[b['name']]
                left=max(0,bisect_right(times,time+1e-8)-1);right=min(left+1,len(times)-1)
                mix=np.clip((time-times[left])/(times[right]-times[left]),0,1) if right!=left else 0
                t0,t1=tracks['translate'][left],tracks['translate'][right]
                r0,r1=tracks['rotate'][left]['value'],tracks['rotate'][right]['value']
                x+=t0['x']+(t1['x']-t0['x'])*mix;y+=t0['y']+(t1['y']-t0['y'])*mix
                a+=r0+(r1-r0)*mix
            c=np.cos(np.radians(a));s=np.sin(np.radians(a))
            local=np.array([[c,-s,x],[s,c,y],[0,0,1]])
            m=byname[b['parent']]@local if 'parent' in b else local;world.append(m);byname[b['name']]=m
        world=np.array(world);dst=np.full((615,700,3),BG,dtype=np.uint8)
        for slot in self.rig['slots']:
            n=slot['name'];a=slot['attachment']
            for k in self.anim['slots'].get(n,{}).get('attachment',[]):
                if k['time']<=time:a=k['name']
            tex,uv,tris,idx,b,local,w=self.skin[a]
            coords=np.einsum('ijk,ik->ij',world[b,:2],local)*w[:,None]
            points=np.zeros((len(uv),2));np.add.at(points,idx,coords)
            points=points*[1,-1]+[156,586];raster(dst,tex,points,uv,tris)
        return Image.fromarray(dst)

def main():
    player=Player()
    labels=['压重心','蹬地送肩','平刺伸展','屈肘收势']
    refs=[Image.open(STUDY/'keys'/(n+'.png')).convert('RGBA') for n in NAMES]
    images=[];font=ImageFont.truetype("msyh.ttc",19)
    temp=ROOT/'build/key-thrust';temp.mkdir(exist_ok=True,parents=True)
    poses=Image.new('RGB',(4*310,2*272),BG)
    for j in range(60):
        q=0 if j<6 else 1 if j<13 else 2 if j<39 else 3
        ref=Image.new('RGB',(700,615),BG);ref.paste(refs[q],mask=refs[q].getchannel('A'))
        pic=player.pose(j)
        if j in (0,6,13,39):
            col=(0,6,13,39).index(j)
            for row,img in enumerate([ref,pic]):poses.paste(img.resize((310,272),Image.Resampling.LANCZOS),(col*310,row*272))
        pair=Image.new('RGB',(1120,540),BG);d=ImageDraw.Draw(pair)
        for k,(label,img) in enumerate([('关键帧  ·  '+labels[q],ref),('绑骨平刺',pic)]):
            d.text((280+560*k,24),label,fill='#eee9dd',font=font,anchor='mm')
            pair.paste(img.resize((560,492),Image.Resampling.LANCZOS),(560*k,45))
        d.line((560,57,560,526),fill='#515b5f')
        images.append(np.asarray(pair).tobytes())
        if j in (0,3,5,6,9,10,13,22,26,30,33,39,50,56):pic.resize((350,308)).save(temp/f'pose-{j:02d}.png')
    poses.save(temp/'pose-matches.png')
    source=b''.join(images)
    for name,fps,codec in [('viridis-keyed-compare.webp',60,['-c:v','libwebp_anim','-quality','92','-loop','0']),
                           ('viridis-keyed-compare-slow.webp',20,['-c:v','libwebp_anim','-quality','93','-loop','0']),
                           ('viridis-keyed-compare.mp4',60,['-c:v','libx264','-pix_fmt','yuv420p','-crf','16','-movflags','+faststart'])]:
        run(['ffmpeg','-loglevel','error','-y','-f','rawvideo','-pixel_format','rgb24','-video_size','1120x540','-framerate',str(fps),'-i','pipe:0',*codec,str(EXPORT/name)],input=source,check=True)

if __name__=='__main__':main()
