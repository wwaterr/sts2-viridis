// Render the exported region attachments and weighted body mesh directly from Spine JSON.
const fs = require('node:fs');
const path = require('node:path');
const { execFileSync } = require('node:child_process');
const { createCanvas, loadImage } = require('@napi-rs/canvas');
const repo=path.resolve(__dirname,'..');
const src=path.join(repo,'mods/Viridis/art_source/characters/viridis/shovel-cutout');
const out=path.join(repo,'outputs/viridis-animation');
const rig=JSON.parse(fs.readFileSync(path.join(src,'viridis-shovel.json')));
const indices=new Map(rig.bones.map((b,i)=>[b.name,i]));
const clip=rig.animations.attack;
function rotation(keys,t){
  if(!keys)return 0;
  const j=keys.findIndex(k=>k.time>t);
  if(j<0)return keys.at(-1).value;
  if(j===0)return keys[0].value;
  const a=keys[j-1],b=keys[j];return a.value+(b.value-a.value)*(t-a.time)/(b.time-a.time);
}
function matrices(t){
  const world=[];
  for(const b of rig.bones){
    const a=rotation(clip.bones[b.name]?.rotate,t)*Math.PI/180,co=Math.cos(a),si=Math.sin(a);
    const [A,B,C,D,X,Y]=world[indices.get(b.parent)]??[1,0,0,1,0,0];
    world.push([A*co+C*si,B*co+D*si,C*co-A*si,D*co-B*si,A*b.x+C*b.y+X,B*b.x+D*b.y+Y]);
  }
  return world;
}
const zoom=.73, origin=[175,382], width=405,height=410;
function screen(x,y){return [origin[0]+x*zoom,origin[1]-y*zoom];}
function meshVertices(m,world){
  const result=[],v=m.vertices;
  for(let i=0;i<v.length;){
    const n=v[i++];let x=0,y=0;
    for(let j=0;j<n;j++){
      const [a,b,c,d,e,f]=world[v[i++]],u=v[i++],z=v[i++],w=v[i++];
      x+=w*(a*u+c*z+e);y+=w*(b*u+d*z+f);
    }
    result.push(screen(x,y));
  }
  return result;
}
function drawMesh(g,m,world,tex){
  const pts=meshVertices(m,world),target=g.getImageData(0,0,width,height),dst=target.data;
  for(let i=0;i<m.triangles.length;i+=3){
    const ids=m.triangles.slice(i,i+3),[a,b,c]=ids.map(id=>pts[id]);
    const [ua,ub,uc]=ids.map(id=>m.uvs.slice(2*id,2*id+2));
    const dx=b[0]-a[0],dy=b[1]-a[1],ex=c[0]-a[0],ey=c[1]-a[1],det=dx*ey-ex*dy;
    for(let y=Math.max(0,Math.floor(Math.min(a[1],b[1],c[1])));y<Math.min(height,Math.ceil(Math.max(a[1],b[1],c[1])));y++)
      for(let x=Math.max(0,Math.floor(Math.min(a[0],b[0],c[0])));x<Math.min(width,Math.ceil(Math.max(a[0],b[0],c[0])));x++){
        const px=x+.5-a[0],py=y+.5-a[1],s=(px*ey-py*ex)/det,t=(dx*py-dy*px)/det;
        if(s<0||t<0||s+t>1)continue;
        const u=(ua[0]+s*(ub[0]-ua[0])+t*(uc[0]-ua[0]))*tex.width-.5;
        const v=(ua[1]+s*(ub[1]-ua[1])+t*(uc[1]-ua[1]))*tex.height-.5;
        const base=(y*width+x)*4, rgb=[0,0,0];let alpha=0;
        for(const oy of [0,1])for(const ox of [0,1]){
          const xx=Math.max(0,Math.min(tex.width-1,Math.floor(u)+ox)),yy=Math.max(0,Math.min(tex.height-1,Math.floor(v)+oy));
          const src=(xx+yy*tex.width)*4, blend=(1-Math.abs(u-(Math.floor(u)+ox)))*(1-Math.abs(v-(Math.floor(v)+oy)))*tex.data[src+3]/255;
          alpha+=blend;for(let k=0;k<3;k++)rgb[k]+=blend*tex.data[src+k];
        }
        for(let k=0;k<3;k++)dst[base+k]=rgb[k]+dst[base+k]*(1-alpha);
      }
  }
  g.putImageData(target,0,0);
}
function line(g,points,color,size){
  g.strokeStyle=color;g.lineWidth=size;g.beginPath();
  points.forEach(([x,y],i)=>i?g.lineTo(x,y):g.moveTo(x,y));g.stroke();
}
async function main(){
  const images={};
  for(const s of rig.slots)images[s.name]=await loadImage(path.join(src,'parts',s.name+'.png'));
  const body=images.body, tmp=createCanvas(body.width,body.height),bg=tmp.getContext('2d');
  bg.drawImage(body,0,0);const tex=bg.getImageData(0,0,body.width,body.height);
  function pose(time,bones=false){
    const c=createCanvas(width,height),g=c.getContext('2d');g.fillStyle='#283136';g.fillRect(0,0,width,height);
    const world=matrices(time);
    for(const slot of rig.slots){
      const m=rig.skins[0].attachments[slot.name][slot.attachment],img=images[slot.name];
      if(m.type==='mesh'){drawMesh(g,m,world,tex);continue;}
      const [a,b,cc,d,e,f]=world[indices.get(slot.bone)],[x,y]=screen(e,f);
      g.save();g.setTransform(a*zoom,-b*zoom,-cc*zoom,d*zoom,x,y);
      g.drawImage(img,m.x-m.width/2,-m.y-m.height/2);g.restore();
    }
    if(bones){
      const joints=['upper_arm','forearm','hand','shovel'].map(n=>{const m=world[indices.get(n)];return screen(m[4],m[5]);});
      line(g,joints,'#7af9de',2);
      for(const [x,y] of joints){g.beginPath();g.arc(x,y,3.5,0,2*Math.PI);g.fillStyle='#ffcb79';g.fill();}
    }
    return c;
  }
  fs.mkdirSync(out,{recursive:true});
  const frames=[];
  const times=[...Array(22).fill(0),...Array.from({length:41},(_,i)=>i/50),...Array(12).fill(.8)];
  for(const t of times)frames.push(Buffer.from(pose(t).getContext('2d').getImageData(0,0,width,height).data));
  execFileSync('ffmpeg',['-loglevel','error','-y','-f','rawvideo','-pix_fmt','rgba','-s',width+'x'+height,
    '-framerate','50','-i','pipe:0','-loop','0','-quality','95',path.join(out,'viridis-cutout-swing.webp')],{input:Buffer.concat(frames)});
  const sheet=createCanvas(1010,520),g=sheet.getContext('2d');g.fillStyle='#283136';g.fillRect(0,0,1010,520);
  const labels=(s,x,y,size=16)=>{g.fillStyle='#e6ded0';g.font=size+'px "Microsoft YaHei", sans-serif';g.fillText(s,x,y);};
  labels('去掉持铲臂的身体',23,34);g.drawImage(body,20,57,body.width*.84,body.height*.84);
  labels('独立附件',312,34);
  for(const [name,label,x,y,k] of [['upper_arm','上臂与袖口',313,64,1.6],['forearm','前臂',431,92,1.5],['hand','握铲手',313,248,1.65],['shovel','整把铲',428,243,1.5]]){
    const im=images[name];g.drawImage(im,x,y,im.width*k,im.height*k);labels(label,x,y+im.height*k+22,14);
  }
  labels('命中姿势 · 肩 / 肘 / 腕 / 握点',604,34);
  g.drawImage(pose(.2,true),601,48);
  fs.writeFileSync(path.join(out,'viridis-cutout-breakdown.png'),sheet.toBuffer('image/png'));
  const contact=createCanvas(width*3,height),ctx=contact.getContext('2d');
  for(const [i,t] of [0,.12,.2].entries())ctx.drawImage(pose(t),i*width,0);
  fs.writeFileSync(path.join(out,'viridis-cutout-keyposes.png'),contact.toBuffer('image/png'));
}
main().catch(e=>{console.error(e);process.exitCode=1;});
