"""Preview setup -> idle_loop -> attack -> idle_loop using the delivered Spine tracks."""
import json
from subprocess import run
from bisect import bisect_right
import numpy as np
from PIL import Image,ImageDraw,ImageFont
from build_guard_thrust import ROOT,OUT,NAME,ORIGIN,DURATION,SOURCE
from thrust_raster import raster

BG=(40,49,54)
EXPORT=ROOT/'outputs/viridis-animation'
class Playback:
    def __init__(self, rig_path=OUT/(NAME+'.json')):
        self.rig=json.loads(rig_path.read_text())
        self.art={}
        for slot,attachments in self.rig['skins'][0]['attachments'].items():
            for n,a in attachments.items():
                tex=np.array(Image.open(rig_path.parent/'parts'/(a.get('path',n)+'.png')))
                if a.get('type')!='mesh':
                    self.art[n]=(tex,a);continue
                local=[];owner=[];bone=[];w=[];v=iter(a['vertices'])
                for j in range(len(a['uvs'])//2):
                    for _ in range(next(v)):
                        idx,x,y,wt=[next(v) for _ in range(4)]
                        bone.append(idx);local.append((x,y,1));owner.append(j);w.append(wt)
                self.art[n]=(tex,np.array(a['uvs']).reshape(-1,2),np.array(a['triangles']).reshape(-1,3),np.array(owner),np.array(bone),np.array(local),np.array(w))
    @staticmethod
    def span(keys,t):
        times=[k['time'] for k in keys];i=max(0,bisect_right(times,t+1e-7)-1);j=min(i+1,len(keys)-1)
        f=np.clip((t-times[i])/(times[j]-times[i]),0,1) if i!=j else 0
        return keys[i],keys[j],f
    def frame(self,clip,t,painting=None,background=BG,group=None):
        anim=self.rig['animations'][clip];world=[];bones={}
        for b in self.rig['bones']:
            x=b.get('x',0);y=b.get('y',0);angle=b.get('rotation',0)
            tr=anim.get('bones',{}).get(b['name'])
            if tr:
                a,c,f=self.span(tr['translate'],t);x+=a['x']+f*(c['x']-a['x']);y+=a['y']+f*(c['y']-a['y'])
                a,c,f=self.span(tr['rotate'],t);angle+=a['value']+f*(c['value']-a['value'])
            co,si=np.cos(np.radians(angle)),np.sin(np.radians(angle));sx=b.get('scaleX',1);sy=b.get('scaleY',1)
            m=np.array([[co*sx,-si*sy,x],[si*sx,co*sy,y],[0,0,1]])
            m=bones[b['parent']]@m if 'parent' in b else m;bones[b['name']]=m;world.append(m)
        world=np.array(world);dst=np.full((520,608,3),background,dtype=np.uint8)
        for slot in self.rig['slots']:
            if group and slot['name'] not in [group,'rest_'+group]:continue
            a=slot.get('attachment');tracks=anim.get('slots',{}).get(slot['name'],{})
            for k in tracks.get('attachment',[]):
                if k['time']<=t+1e-7:a=k.get('name')
            if not a:continue
            if painting and (slot['name']=='guard_reference' or slot['name'].startswith('rest_')!=(painting=='guard')):continue
            alpha=1
            if 'rgba' in tracks:
                l,r,f=self.span(tracks['rgba'],t);al=int(l['color'][-2:],16);ar=int(r['color'][-2:],16);alpha=(al+f*(ar-al))/255
            if painting:alpha=1
            if alpha<=0:continue
            meta=self.art[a];tex=meta[0]
            if len(meta)==2:
                image=Image.fromarray(dst);layer=Image.fromarray(tex)
                info=meta[1];l=round(ORIGIN[0]+info.get('x',0)-layer.width/2)
                top=round(ORIGIN[1]-info.get('y',0)-layer.height/2)
                image.paste(layer,(l,top),mask=layer.getchannel('A').point(lambda v:round(v*alpha)))
                dst=np.array(image);continue
            if alpha<1:
                tex=tex.copy();tex[:,:,3]=np.rint(tex[:,:,3]*alpha)
            _,uv,tris,owner,b,local,w=meta
            coords=np.einsum('ijk,ik->ij',world[b,:2],local)*w[:,None]
            points=np.zeros((len(uv),2));np.add.at(points,owner,coords)
            points=ORIGIN+points*[1,-1]
            raster(dst,tex,points,uv,tris)
        return Image.fromarray(dst)

def main():
    p=Playback();frames=[p.frame('attack',k/60) for k in range(DURATION+1)]
    idle=p.frame('idle_loop',0)
    guard=Image.open(SOURCE).convert('RGBA');ref=Image.new('RGB',guard.size,BG);ref.paste(guard,mask=guard.getchannel('A'))
    for f in [idle,frames[0],frames[-1]]:assert np.array_equal(np.array(f),np.array(ref))
    raw=b''.join(f.tobytes() for f in [idle]*18+frames+[idle]*17)
    for ext,codec in [('webp',['-c:v','libwebp_anim','-quality','95','-loop','0']),('mp4',['-c:v','libx264','-crf','16','-pix_fmt','yuv420p','-movflags','+faststart'])]:
        run(['ffmpeg','-v','error','-y','-f','rawvideo','-pix_fmt','rgb24','-s','608x520','-framerate','60','-i','pipe:0',*codec,str(EXPORT/('viridis-guard-thrust.'+ext))],input=raw,check=True)
    preview=Image.new('RGB',(4*285,3*244),BG)
    for i,k in enumerate([0,3,6,8,10,12,19,36,58,65,72,80]):
        preview.paste(frames[k].resize((285,244),Image.Resampling.LANCZOS),(i%4*285,i//4*244))
    (ROOT/'build/guard-thrust').mkdir(parents=True,exist_ok=True);preview.save(ROOT/'build/guard-thrust/check.png')

if __name__=='__main__':main()
