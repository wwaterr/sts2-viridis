// Draw only the attachments and transforms present in the exported Spine rig.
const fs=require('node:fs'),path=require('node:path');
const {execFileSync}=require('node:child_process');
const {createCanvas,loadImage}=require('@napi-rs/canvas');
const repo=path.resolve(__dirname,'..');
const silent=process.argv.includes('--silent-thrust');
const knee=silent||process.argv.includes('--knee-on-shin');
const baseDir=path.join(repo,'mods/Viridis/art_source/characters/viridis/shovel-fullbody');
const dir=knee?path.join(baseDir,'knee-on-shin'):baseDir;
const out=path.join(repo,'outputs/viridis-animation');
const both=knee||process.argv.includes('--both-forearms-front');
const forearmFront=both||process.argv.includes('--forearm-front');
const long=forearmFront||process.argv.includes('--long-stab');
const front=long||process.argv.includes('--front-stab');
const prefix=knee?'viridis-knee-on-shin':both?'viridis-both-forearms-front':forearmFront?'viridis-forearm-front':long?'viridis-long-stab':front?'viridis-front-stab':'viridis-fullbody';
const rig=JSON.parse(fs.readFileSync(path.join(dir,silent?'viridis-silent-thrust.json':knee?'viridis-fullbody.json':prefix+'.json')));
const clip=rig.animations.attack.bones;
const duration=clip.hips.translate.at(-1).time;
function value(keys,t,field,def){
  if(!keys)return def;
  const j=keys.findIndex(k=>k.time>t);
  if(j<0)return keys.at(-1)[field]??def;
  if(j===0)return keys[0][field]??def;
  const a=keys[j-1],b=keys[j],s=(t-a.time)/(b.time-a.time);
  return (1-s)*(a[field]??def)+s*(b[field]??def);
}
function matrices(t){
  const m={};
  for(const b of rig.bones){
    const tracks=clip[b.name]??{}, angle=value(tracks.rotate,t,'value',0)*Math.PI/180;
    const sx=value(tracks.scale,t,'x',1),sy=value(tracks.scale,t,'y',1),c=Math.cos(angle),s=Math.sin(angle);
    const x=b.x+value(tracks.translate,t,'x',0),y=b.y+value(tracks.translate,t,'y',0);
    const [A,B,C,D,E,F]=m[b.parent]??[1,0,0,1,0,0];
    m[b.name]=[(A*c+C*s)*sx,(B*c+D*s)*sx,(C*c-A*s)*sy,(D*c-B*s)*sy,A*x+C*y+E,B*x+D*y+F];
  }
  return m;
}
const W=silent?480:416,H=silent?384:412,zoom=silent?.55:.74,ox=silent?209:167,oy=silent?333:389;
const screen=([x,y])=>[ox+x*zoom,oy-y*zoom];
function label(g,title,x,y,size=16){g.fillStyle='#efebe2';g.font=size+'px "Microsoft YaHei",sans-serif';g.fillText(title,x,y);}
async function main(){
  fs.mkdirSync(out,{recursive:true});
  const images={};
  for(const slot of rig.slots)images[slot.name]=await loadImage(path.join(dir,'parts',slot.name+'.png'));
  function render(t,slots=rig.slots,attachments=rig.skins[0].attachments,textures=images){
    const c=createCanvas(W,H),g=c.getContext('2d'),m=matrices(t);
    g.fillStyle='#283136';g.fillRect(0,0,W,H);
    if(!silent){g.strokeStyle='#576168';g.beginPath();g.moveTo(22,oy+.5);g.lineTo(W-22,oy+.5);g.stroke();}
    for(const slot of slots){
      const att=attachments[slot.name][slot.attachment];
      const [a,b,c,d,e,f]=m[slot.bone],[x,y]=screen([e,f]);
      g.save();g.setTransform(a*zoom,-b*zoom,-c*zoom,d*zoom,x,y);
      g.drawImage(textures[slot.name],att.x-att.width/2,-att.y-att.height/2);g.restore();
    }
    return c;
  }
  if(silent){
    const source=fs.readFileSync(path.join(repo,'build/silent-motion/attack.rgba'));
    const bytes=W*H*4,frameCount=source.length/bytes;
    if(frameCount!==28||Math.abs(duration-frameCount/30)>1e-6)throw Error('Reference timing mismatch');
    const panel=createCanvas(W,H),pg=panel.getContext('2d'),pixels=pg.createImageData(W,H);
    const pair=createCanvas(W*2,H+44),g=pair.getContext('2d'),frames=[];
    g.fillStyle='#283136';g.fillRect(0,0,pair.width,pair.height);
    g.textAlign='center';
    label(g,'静默猎手 · 原版普攻',W/2,32,21);
    label(g,'小松绿 · 平刺',W*1.5,32,21);
    for(let i=0;i<frameCount;i++){
      pixels.data.set(source.subarray(i*bytes,(i+1)*bytes));
      pg.putImageData(pixels,0,0);g.drawImage(panel,0,44);g.drawImage(render(i/30),W,44);
      g.fillStyle='#4a5459';g.fillRect(W,64,1,H-42);
      frames.push(Buffer.from(g.getImageData(0,0,pair.width,pair.height).data));
    }
    const input=Buffer.concat(frames);
    for(const [suffix,fps,codec] of [
      ['.webp',30,['-c:v','libwebp_anim','-quality','94','-loop','0']],
      ['-slow.webp',10,['-c:v','libwebp_anim','-quality','94','-loop','0']],
      ['.mp4',30,['-c:v','libx264','-crf','16','-pix_fmt','yuv420p','-movflags','+faststart']]
    ]){
      execFileSync('ffmpeg',['-loglevel','error','-y','-f','rawvideo','-pix_fmt','rgba',
        '-s',pair.width+'x'+pair.height,'-framerate',String(fps),'-i','pipe:0',...codec,
        path.join(out,'viridis-vs-silent-attack'+suffix)],{input});
    }
    return;
  }
  const frames=[];
  const times=[...Array(20).fill(0),...Array.from({length:Math.round(duration*50)+1},(_,i)=>i*.02),...Array(14).fill(duration)];
  for(const t of times)frames.push(Buffer.from(render(t).getContext('2d').getImageData(0,0,W,H).data));
  const encode=(name,fps)=>execFileSync('ffmpeg',['-loglevel','error','-y','-f','rawvideo','-pix_fmt','rgba','-s',W+'x'+H,
    '-framerate',String(fps),'-i','pipe:0','-loop','0','-quality','94',path.join(out,name+'.webp')],{input:Buffer.concat(frames)});
  encode(prefix+(front?'':'-swing'),50);encode(prefix+'-slow',20);
  const strip=createCanvas(1450,321),g=strip.getContext('2d');g.fillStyle='#283136';g.fillRect(0,0,1450,321);
  const poses=front?[[.155,'胸前屈肘'],[.26,'举铲'],[.33,'送肩'],[.38,long?'前刺':'下刺'],[.66,'收铲']]:
    [[.10,'蓄力'],[.15,'蹬地转肩'],[.20,'出铲'],[.32,'随挥'],[.60,'收势']];
  for(const [i,[t,title]] of poses.entries()){
    g.drawImage(render(t),i*290+3,0,284,281);label(g,title,i*290+110,301,17);
  }
  fs.writeFileSync(path.join(out,prefix+'-poses.png'),strip.toBuffer('image/png'));
  if(knee){
    const before=JSON.parse(fs.readFileSync(path.join(baseDir,'viridis-both-forearms-front.json')));
    const oldImages={...images};
    for(const key of ['lead_thigh','lead_shin'])oldImages[key]=await loadImage(path.join(baseDir,'parts',key+'.png'));
    const t=.43,[x,y]=screen(matrices(t).lead_shin.slice(4));
    const sheet=createCanvas(580,250),ctx=sheet.getContext('2d');ctx.fillStyle='#283136';ctx.fillRect(0,0,580,250);
    [render(t,before.slots,before.skins[0].attachments,oldImages),render(t)].forEach((pose,i)=>{
      ctx.drawImage(pose,Math.round(x)-45,Math.round(y)-35,90,68,i*290+10,4,270,204);
      label(ctx,i?'膝盖随小腿':'原版 · 膝盖随大腿',i*290+54,237,18);
    });
    fs.writeFileSync(path.join(out,prefix+'-comparison.png'),sheet.toBuffer('image/png'));
  }else if(forearmFront){
    const before=JSON.parse(fs.readFileSync(path.join(dir,both?'viridis-forearm-front.json':'viridis-long-stab.json'))).slots;
    const sheet=createCanvas(680,310),ctx=sheet.getContext('2d');
    ctx.fillStyle='#283136';ctx.fillRect(0,0,680,310);
    [before,rig.slots].forEach((slots,i)=>{
      const time=.26, elbow=matrices(time)[both?'open_fore':'forearm'],[x,y]=screen(elbow.slice(4));
      ctx.drawImage(render(time,slots),Math.round(x)-52,Math.round(y)-54,104,86,i*340+10,6,312,258);
      label(ctx,i?'小臂在前':'原版 · 大臂在前',i*340+80,292,20);
    });
    fs.writeFileSync(path.join(out,prefix+'-comparison.png'),sheet.toBuffer('image/png'));
  }
  if(!front){
  const sheet=createCanvas(1050,550),ctx=sheet.getContext('2d');
  ctx.fillStyle='#283136';ctx.fillRect(0,0,1050,550);
  const groups=[
    ['头、胸与骨盆',['head','torso','pelvis']],
    ['持铲臂',['upper_arm','forearm','hand','shovel']],
    ['空手臂',['open_upper','open_fore','open_hand']],
    ['后腿',['rear_thigh','rear_shin','rear_boot']],
    ['前腿',['lead_thigh','lead_shin','lead_boot']]
  ];
  groups.forEach(([title,items],i)=>{
    const cx=(i+.5)*210;label(ctx,title,210*i+30,38,18);
    const scale=Math.min(1.28,425/items.reduce((sum,n)=>sum+images[n].height,0));
    let y=61;
    for(const n of items){
      const im=images[n],w=im.width*scale,h=im.height*scale;
      ctx.drawImage(im,cx-w/2,y,w,h);y+=h+13;
    }
  });
  fs.writeFileSync(path.join(out,'viridis-fullbody-parts.png'),sheet.toBuffer('image/png'));
  }
  const mov=(m,b,x,y)=>{const [a,b1,c,d,e,f]=m[b];return [a*x+c*y+e,b1*x+d*y+f];};
  const max={rear:0,lead:0};
  const first=matrices(0); let heelRise=0;
  for(let i=0;i<=Math.round(duration*1000);i++){
    const m=matrices(i/1000);
    for(const [side,local] of [['rear',[5,-76]],['lead',[42,-75]]]){
      const a=mov(m,side+'_boot',...local),b=mov(first,side+'_boot',...local);
      max[side]=Math.max(max[side],Math.hypot(a[0]-b[0],a[1]-b[1]));
    }
    heelRise=Math.max(heelRise,mov(m,'rear_boot',-29,-75)[1]-mov(first,'rear_boot',-29,-75)[1]);
  }
  if(max.rear>.25||max.lead>.25||heelRise<12)throw Error('Foot plant check: '+JSON.stringify({max,heelRise}));
  console.log('Foot drift and rear heel rise (art pixels)',max,heelRise);
  if(front){
    const pos=(t,name,local=[0,0])=>mov(matrices(t),name,...local);
    const grip=t=>pos(t,'shovel'),tip=t=>pos(t,'shovel',[-65,55]);
    const peak=.27,hit=.38;
    let backtrack=0;
    for(let i=1;i<=380;i++){
      const t=i/1000;
      backtrack=Math.max(backtrack,grip(0)[0]-grip(t)[0]);
      if(t>=.19 && grip(t)[0]<=pos(t,'upper_arm')[0])throw Error('Grip passed behind shoulder');
    }
    const drop=tip(peak)[1]-tip(hit)[1],reach=tip(hit)[0]-tip(peak)[0];
    const valid=long?(reach>125&&drop>10&&Math.atan2(drop,reach)<Math.PI/4):drop>90&&reach>20;
    if(!valid||backtrack>2)throw Error('Invalid front thrust '+JSON.stringify({drop,reach,backtrack}));
    console.log('Tip travel (forward, down); grip rearward from guard:',reach,drop,backtrack);
  }
}
main().catch(err=>{console.error(err);process.exitCode=1;});
