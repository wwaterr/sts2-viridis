// Sample the shipped Spine timelines and rasterize their weighted triangles for art review.
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { execFileSync } from 'node:child_process';
const repo=fileURLToPath(new URL('../',import.meta.url));
const src=path.join(repo,'mods/Viridis/art_source/characters/viridis/spine');
const rig=JSON.parse(fs.readFileSync(path.join(src,'viridis.json')));
const out=path.join(repo,'outputs/viridis-animation');
fs.mkdirSync(out,{recursive:true});
const indices=new Map(rig.bones.map((b,i)=>[b.name,i]));
const meshes=rig.slots.map(slot=>{
  const mesh=rig.skins[0].attachments[slot.name][slot.name];
  const data=execFileSync('ffmpeg',['-loglevel','error','-i',path.join(src,'parts_v1',slot.name+'.png'),'-f','rawvideo','-pix_fmt','rgba','pipe:1'],{maxBuffer:16e6});
  return {...mesh,data};
});
function sample(keys,time,field,fallback){
  if(!keys?.length)return fallback;
  let i=0;
  while(i+1<keys.length && time>=keys[i+1].time)i++;
  const a=keys[i],b=keys[i+1];
  const start=a[field]??fallback;
  if(!b)return start;
  return start+((b[field]??fallback)-start)*Math.max(0,(time-a.time)/(b.time-a.time));
}
const width=320,height=420;
function frame(clip,time,zoom=1.64,origin=width/2){
  const pixels=Buffer.alloc(width*height*3);
  for(let i=0;i<pixels.length;i+=3){pixels[i]=40;pixels[i+1]=46;pixels[i+2]=49;}
  const world=[];
  for(const bone of rig.bones){
    const t=clip.bones[bone.name]??{};
    const angle=sample(t.rotate,time,'value',0)*Math.PI/180;
    const sx=sample(t.scale,time,'x',1),sy=sample(t.scale,time,'y',1);
    const [a,b,c,d,e,f]=world[indices.get(bone.parent)]??[1,0,0,1,0,0];
    const cos=Math.cos(angle),sin=Math.sin(angle);
    const x=bone.x+sample(t.translate,time,'x',0),y=bone.y+sample(t.translate,time,'y',0);
    world.push([(a*cos+c*sin)*sx,(b*cos+d*sin)*sx,(c*cos-a*sin)*sy,(d*cos-b*sin)*sy,a*x+c*y+e,b*x+d*y+f]);
  }
  for(const mesh of meshes){
    const v=mesh.vertices, points=[];
    for(let k=0;k<v.length;){
      let x=0,y=0;
      const n=v[k++];
      for(let j=0;j<n;j++){
        const [a,b,c,d,e,f]=world[v[k++]];
        const u=v[k++],z=v[k++],w=v[k++];
        x+=w*(a*u+c*z+e);y+=w*(b*u+d*z+f);
      }
      const idx=points.length*2;
      points.push([origin+x*zoom,height-14-y*zoom,mesh.uvs[idx]*mesh.width,mesh.uvs[idx+1]*mesh.height]);
    }
    for(let i=0;i<mesh.triangles.length;i+=3){
      const [a,b,c]=mesh.triangles.slice(i,i+3).map(j=>points[j]);
      const dx=b[0]-a[0],dy=b[1]-a[1],ex=c[0]-a[0],ey=c[1]-a[1],det=dx*ey-ex*dy;
      if(Math.abs(det)<1e-7)continue;
      for(let y=Math.max(0,Math.floor(Math.min(a[1],b[1],c[1])));y<Math.min(height,Math.ceil(Math.max(a[1],b[1],c[1])));y++){
        for(let x=Math.max(0,Math.floor(Math.min(a[0],b[0],c[0])));x<Math.min(width,Math.ceil(Math.max(a[0],b[0],c[0])));x++){
          const px=x+.5-a[0],py=y+.5-a[1],s=(px*ey-py*ex)/det,t=(dx*py-dy*px)/det;
          if(s<0 || t<0 || s+t>1)continue;
          const u=Math.min(mesh.width-1,Math.max(0,Math.floor(a[2]+s*(b[2]-a[2])+t*(c[2]-a[2]))));
          const v=Math.min(mesh.height-1,Math.max(0,Math.floor(a[3]+s*(b[3]-a[3])+t*(c[3]-a[3]))));
          const at=(v*mesh.width+u)*4,alpha=mesh.data[at+3]/255,offset=(y*width+x)*3;
          for(let ch=0;ch<3;ch++)pixels[offset+ch]=mesh.data[at+ch]*alpha+pixels[offset+ch]*(1-alpha);
        }
      }
    }
  }
  return pixels;
}
const idle=rig.animations.idle_loop;
if(!frame(idle,0).equals(frame(idle,2.4)))throw Error('idle loop seam');
const frames=Array.from({length:48},(_,i)=>frame(idle,i/20));
if(frames[0].equals(frames[24]))throw Error('stationary idle');
function encode(name,frames){
  execFileSync('ffmpeg',['-loglevel','error','-y','-f','rawvideo','-pixel_format','rgb24','-video_size',width+'x'+height,'-framerate','20','-i','pipe:0','-loop','0','-quality','90',path.join(out,name+'.webp')],{input:Buffer.concat(frames)});
}
encode('viridis-idle',frames);
const reel=[];
for(const [name,seconds] of [['idle_loop',2.4],['attack',.7],['attack_heavy',1],['hurt',.32],['die',1.1]]){
  for(let i=0;i<Math.ceil(seconds*20);i++)reel.push(frame(rig.animations[name],i/20,1.08,260));
}
for(let j=0;j<9;j++)reel.push(reel.at(-1));
encode('viridis-combat',reel);
