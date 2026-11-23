"""Render both exported Spine skins at identical track times using their weighted triangle vertices."""
import json
from pathlib import Path
from subprocess import run
from tempfile import TemporaryDirectory

import numpy as np
from PIL import Image, ImageDraw, ImageFont
from numba import njit

ROOT=Path(__file__).resolve().parents[1]
ART=ROOT/"mods/Viridis/art_source/characters/viridis/shovel-fullbody"
OUT=ROOT/"outputs/viridis-animation"
WIDTH,HEIGHT=520,520
ORIGIN=np.array([197,493])
BG=(40,49,54)


@njit
def raster(canvas,texture,points,uv,triangles):
    height,width=canvas.shape[:2]
    th,tw=texture.shape[:2]
    for a,b,c in triangles:
        ax,ay=points[a]; bx,by=points[b]; cx,cy=points[c]
        dx,dy=bx-ax,by-ay; ex,ey=cx-ax,cy-ay
        det=dx*ey-dy*ex
        if abs(det)<.0001: continue
        for y in range(max(0,int(np.floor(min(ay,by,cy)))),min(height,int(np.ceil(max(ay,by,cy))))):
            for x in range(max(0,int(np.floor(min(ax,bx,cx)))),min(width,int(np.ceil(max(ax,bx,cx))))):
                px=x+.5-ax; py=y+.5-ay
                s=(px*ey-py*ex)/det; t=(dx*py-dy*px)/det
                if min(s,t,1-s-t)<-1e-7: continue
                u=(uv[a,0]*(1-s-t)+uv[b,0]*s+uv[c,0]*t)*tw-.5
                v=(uv[a,1]*(1-s-t)+uv[b,1]*s+uv[c,1]*t)*th-.5
                xx=int(np.floor(u)); yy=int(np.floor(v)); fx=u-xx; fy=v-yy
                col=np.zeros(4)
                for j in range(2):
                    for i in range(2):
                        p=texture[min(max(yy+j,0),th-1),min(max(xx+i,0),tw-1)]
                        w=(fx if i else 1-fx)*(fy if j else 1-fy)*p[3]/255
                        col[:3]+=p[:3]*w;col[3]+=w
                for k in range(3):
                    canvas[y,x,k]=round(col[k]+canvas[y,x,k]*(1-col[3]))


class Actor:
    def __init__(self,path):
        js=json.loads(path.read_text())
        self.bones=js["bones"]
        self.idx={b["name"]:i for i,b in enumerate(self.bones)}
        self.tracks=js["animations"]["attack"]["bones"]
        self.draw=[]
        for slot in js["slots"]:
            n=slot["name"]
            a=js["skins"][0]["attachments"][n][n]
            texture=np.array(Image.open(path.parent/"parts"/(n+".png")).convert("RGBA"))
            if a.get("type")=="mesh":
                uvs=np.array(a["uvs"]).reshape(-1,2)
                triangles=np.array(a["triangles"]).reshape(-1,3)
                verts=a["vertices"]; at=0; entries=[]
                for i in range(len(uvs)):
                    count=verts[at]; at+=1
                    for _ in range(count):
                        bone,x,y,weight=verts[at:at+4];at+=4
                        entries.append((i,bone,x,y,weight))
                assert at==len(verts)
            else:
                uvs=np.array([(0,0),(1,0),(1,1),(0,1)],float)
                triangles=np.array([(0,1,2),(0,2,3)])
                entries=[(i,self.idx[slot["bone"]],a["x"]+(u-.5)*a["width"],a["y"]+(.5-v)*a["height"],1)
                         for i,(u,v) in enumerate(uvs)]
            vertex,bone,x,y,weight=np.array(entries).T
            self.draw.append((texture,uvs,triangles,vertex.astype(int),bone.astype(int),np.stack([x,y,np.ones_like(x)],axis=1),weight))

    def pose(self,frame):
        world=[]
        for b in self.bones:
            tracks=self.tracks.get(b["name"],{})
            def key(channel):return tracks[channel][frame*10] if channel in tracks else {}
            rot=np.deg2rad(key("rotate").get("value",0));c,s=np.cos(rot),np.sin(rot)
            tr=key("translate");sc=key("scale");sx=sc.get("x",1);sy=sc.get("y",1)
            local=np.array([[c*sx,-s*sy,b["x"]+tr.get("x",0)],[s*sx,c*sy,b["y"]+tr.get("y",0)],[0,0,1]])
            world.append(world[self.idx[b["parent"]]]@local if "parent" in b else local)
        world=np.array(world)
        dst=np.full((HEIGHT*2,WIDTH*2,3),BG,dtype=np.uint8)
        for tex,uvs,tris,vertices,bones,local,weight in self.draw:
            coords=np.einsum("ijk,ik->ij",world[bones,:2],local)*weight[:,None]
            points=np.zeros((len(uvs),2))
            np.add.at(points,vertices,coords)
            points=(ORIGIN+points*[1,-1])*2
            raster(dst,tex,points,uvs,tris)
        joints={b["name"]:ORIGIN+world[i,:2,2]*[1,-1] for i,b in enumerate(self.bones)}
        return Image.fromarray(dst).resize((WIDTH,HEIGHT),Image.Resampling.LANCZOS),joints


def main():
    before=Actor(ART/"knee-on-shin/viridis-silent-thrust.json")
    after=Actor(ART/"soft-joints/viridis-flat-thrust.json")
    font=ImageFont.truetype("C:/Windows/Fonts/msyh.ttc",19)
    sheet=Image.new("RGB",(3*286,2*226+52),BG)
    poses=[(0,"rear_shin","蓄力 · 后膝"),(11,"forearm","收铲 · 持铲肘"),(4,"lead_shin","出铲 · 前膝")]
    with TemporaryDirectory(dir=OUT) as tmp:
        for f in range(28):
            pair=Image.new("RGB",(WIDTH*2,HEIGHT+52),BG);d=ImageDraw.Draw(pair)
            for i,(actor,title) in enumerate([(before,"上一版"),(after,"关节修订版")]):
                pic,joints=actor.pose(f)
                pair.paste(pic,(i*WIDTH,52))
                d.text(((i+.5)*WIDTH,28),title,font=font,anchor="mm",fill="#e9e6dd")
                for col,(frame,joint,caption) in enumerate(poses):
                    if f!=frame:continue
                    x,y=map(round,joints[joint]);crop=pic.crop((x-57,y-46,x+57,y+45)).resize((286,226),Image.Resampling.LANCZOS)
                    sheet.paste(crop,(col*286,52+i*226))
                    dd=ImageDraw.Draw(sheet)
                    dd.text(((col+.5)*286,25),caption,font=font,anchor="mm",fill="#e9e6dd")
                    dd.text((col*286+8,52+i*226+6),title,font=font,fill="#f2edde",stroke_width=1,stroke_fill="#283136")
            d.line((WIDTH,65,WIDTH,HEIGHT+36),fill="#4c565a")
            pair.save(Path(tmp)/f"{f:03}.png")
        for fps,suffix in [(30,""),(10,"-slow")]:
            run(["ffmpeg","-y","-v","error","-framerate",str(fps),"-i",str(Path(tmp)/"%03d.png"),"-c:v","libwebp_anim","-quality","95","-loop","0",str(OUT/("viridis-soft-joints"+suffix+".webp"))],check=True)
    sheet.save(OUT/"viridis-soft-joints-detail.png")

if __name__=="__main__":main()
