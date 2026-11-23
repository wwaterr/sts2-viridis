// Derive the full-body cutout from the first shovel rig. Coordinates are in guard.png pixels.
const fs = require('node:fs');
const path = require('node:path');
const {createCanvas,loadImage,Path2D} = require('@napi-rs/canvas');
const root = path.resolve(__dirname,'../mods/Viridis/art_source/characters/viridis');
const silentThrust=process.argv.includes('--silent-thrust');
const kneeOnShin=silentThrust||process.argv.includes('--knee-on-shin');
const prior = path.join(root,'shovel-cutout'), fullbody = path.join(root,'shovel-fullbody');
const out=kneeOnShin?path.join(fullbody,'knee-on-shin'):fullbody;
const parts = path.join(out,'parts');
fs.mkdirSync(parts,{recursive:true});
const previous = JSON.parse(fs.readFileSync(path.join(prior,'viridis-shovel.json')));
const surface = () => createCanvas(608,520);
const shape = (ctx,d,fill,edge) => {
  const p = new Path2D(d); ctx.fillStyle=fill;ctx.fill(p);
  if(edge){ctx.strokeStyle=edge;ctx.lineWidth=1.05;ctx.stroke(p);}
};
function inside(ctx,d,paint){ctx.save();ctx.clip(new Path2D(d));paint();ctx.restore();}
const r = n=>+n.toFixed(5);
const deg = 180/Math.PI;
const turn = ([x,y],a) => [x*Math.cos(a/deg)-y*Math.sin(a/deg), x*Math.sin(a/deg)+y*Math.cos(a/deg)];
const add = (a,b)=>[a[0]+b[0],a[1]+b[1]];
const sub = (a,b)=>[a[0]-b[0],a[1]-b[1]];
const heading = ([x,y])=>Math.atan2(y,x)*deg;
const delta = (x)=>(x+540)%360-180;
const length = v=>Math.hypot(...v);
const piv = {
  root:[235,504],hips:[235,283],chest:[234,240],ribcage:[234,240],head:[231,150],
  upper_arm:[200,163],forearm:[181,218.8],hand:[158.7,249.7],shovel:[135,278],
  open_upper:[258,167],open_fore:[287.5,220.5],open_hand:[313,250.5],
  rear_thigh:[205,286],rear_shin:[166,363],rear_boot:[126,426],
  lead_thigh:[265,286],lead_shin:[312,367],lead_boot:[346,427],
  rear_contact:[131,502],lead_contact:[388,502],rear_heel:[97,501]
};
const parent={root:null,hips:'root',chest:'hips',ribcage:'chest',head:'chest',
  upper_arm:'chest',forearm:'upper_arm',hand:'forearm',shovel:'hand',
  open_upper:'chest',open_fore:'open_upper',open_hand:'open_fore',
  rear_thigh:'hips',rear_shin:'rear_thigh',rear_boot:'rear_shin',
  lead_thigh:'hips',lead_shin:'lead_thigh',lead_boot:'lead_shin',
  rear_contact:'rear_boot',lead_contact:'lead_boot',rear_heel:'rear_boot'};

// Cut along the vest armhole, apron hem, fork of the shorts, boot cuffs and glove cuff.
const sleeve='M 256 151 Q 248 158 248 172 Q 244 188 251 198 Q 255 205 260 215 L 268 226 L 294 248 L 301 259 L 300 278 L 310 304 L 378 313 L 391 142 Z';
const head='M 138 20 H 309 V 150 L 278 149 L 268 153 L 261 151 L 259 155 L 250 152 L 245 151 L 243 145 Q 236 147 230 144 L 224 141 L 226 151 L 220 148 L 217 153 L 212 150 L 205 153 L 196 152 L 184 157 L 150 158 Z';
const hem='M 72 299 L 167 287 L 177 270 L 205.7 265 Q 230 282.7 258.5 294.2 Q 268 274 272.6 253 L 292.7 277 L 297.5 272.6 L 299 283 L 315 309 L 408 309 V 520 H 68 Z';
const rear='M 60 222 H 232 L 237.8 276 L 234.6 302 L 230 322 L 225 342 L 194 519 H 60 Z';
const lead='M 232 222 H 410 V 519 H 194 L 225 342 L 230 322 L 234.6 302 L 237.8 276 Z';
const cuts={
  rear:{ above:'M 56 210 H 261 V 404 L 178 377 L 151 361 L 61 342 Z',
    boot:'M 60 398 L 124 404.5 L 126.4 409.2 Q 134 413.5 145.9 415.6 L 148.4 411 L 158 410 L 187 429 L 186 519 H 60 Z'},
  lead:{ above:'M 215 201 H 408 V 323 L 331 361 Q 336 373 324 381 Q 311 390 301 378 L 257 395 L 215 396 Z',
    boot:'M 276 448 L 326 420 L 330.5 416 L 332.8 419.7 Q 343 418.3 350.7 413.7 L 350 409 L 364 407 L 392 402 L 415 510 H 275 Z'}
};
const below = d => 'M 0 0 H 608 V 520 H 0 Z '+d;
function inverseClip(ctx,d,paint){
  ctx.save();ctx.clip(new Path2D(below(d)),'evenodd');paint();ctx.restore();
}
const dims={};
async function extract(){
  const compact=await loadImage(path.join(prior,'parts/body.png'));
  const plate=surface();plate.getContext('2d').drawImage(compact,84,33);
  const orig=g=>g.drawImage(plate,0,0);
  function part(name,paint){
    const c=surface(),g=c.getContext('2d');paint(g);
    const a=g.getImageData(0,0,608,520).data;
    let l=608,t=520,R=0,B=0;
    for(let y=0;y<520;y++)for(let x=0;x<608;x++)if(a[(608*y+x)*4+3]>6){
      l=Math.min(l,x);t=Math.min(t,y);R=Math.max(R,x);B=Math.max(B,y);
    }
    l-=2;t-=2; const w=R+3-l,h=B+3-t;
    const crop=createCanvas(w,h);crop.getContext('2d').drawImage(c,-l,-t);
    fs.writeFileSync(path.join(parts,name+'.png'),crop.toBuffer('image/png'));
    dims[name]={w,h,l,t};
  }
  const skin='#e8c6aa',edge='#77604c';
  part('head',g=>inside(g,head,()=>orig(g)));
  part('torso',g=>{
    shape(g,'M 212 163 Q 213 148 223 139 Q 236 132 248 147 L 251 162 L 237 177 L 218 169 Z',skin,edge);
    shape(g,'M 242 160 Q 256 149 264 161 Q 272 175 264 194 L 260 217 L 247 214 Z','#86805c','#574e38');
    inverseClip(g,head,()=>inverseClip(g,sleeve,()=>inverseClip(g,hem,()=>orig(g))));
  });
  part('pelvis',g=>{
    shape(g,'M 183 277 Q 204 256 242 254 Q 271 252 282 271 L 289 290 Q 263 298 243 321 L 230 325 Q 220 311 205 304 L 177 298 Z','#6c5042','#443b34');
  });
  const upperCut='M 211 125 H 346 V 194 L 301 215 L 273 232 L 230 254 Z';
  const cuff='M 282 268 L 301 258 Q 306 251 317 244 L 321 240 L 354 227 L 386 259 V 316 H 286 Z';
  part('open_upper',g=>{
    shape(g,'M 247 185 Q 242 168 250 159 Q 259 151 265 163 L 273 185 Z','#dfcbb0',edge);
    shape(g,'M 274 207 L 286 203 Q 300 215 297 223 Q 291 233 281 229 Z',skin,edge);
    inside(g,sleeve,()=>inside(g,upperCut,()=>orig(g)));
  });
  part('open_fore',g=>{
    shape(g,'M 278 223 Q 272 214 282 211 Q 291 209 296 221 L 304 232 L 292 238 Z',skin,edge);
    shape(g,'M 300 241 L 310 235 L 321 250 Q 325 260 316 261 Q 310 261 302 251 Z',skin,edge);
    inside(g,sleeve,()=>inverseClip(g,upperCut,()=>inverseClip(g,cuff,()=>orig(g))));
  });
  part('open_hand',g=>inside(g,sleeve,()=>inside(g,cuff,()=>orig(g))));
  for(const side of ['rear','lead']){
    const back=side==='rear',zone=back?rear:lead, k=cuts[side];
    const moveKnee=kneeOnShin&&!back;
    // Follow a convex kneecap above the plaster. The lower attachment gets the
    // original painted pixels; the thigh receives a concealed rounded skin overlap.
    const split=moveKnee?'M 215 201 H 408 V 312 L 323 345 L 317 354 Q 309 344 299 350 Q 291 355 294 364 L 285 371 L 215 394 Z':k.above;
    const pixels=g=>inside(g,zone,()=>inside(g,hem,()=>inverseClip(g,sleeve,()=>orig(g))));
    part(side+'_thigh',g=>{
      const fabric=g.createLinearGradient(0,248,0,314);
      fabric.addColorStop(0,'#614736');fabric.addColorStop(1,'#926f58');
      shape(g,back?'M 161 301 Q 164 279 183 263 Q 205 246 224 259 Q 244 268 236 287 L 223 316 Z'
        :'M 234 298 Q 226 275 246 261 Q 270 246 283 267 Q 293 282 300 306 L 243 325 Z',fabric,'#544235');
      shape(g,back?'M 163 352 L 180 357 L 174 371 Q 168 381 158 374 Q 149 369 155 361 Z'
        :moveKnee?'M 290 351 L 310 345 Q 321 357 322 365 Q 321 375 312 376 Q 305 377 299 368 Z'
        :'M 296 352 L 316 350 L 328 368 Q 333 378 320 384 Q 310 388 304 376 Z',skin,edge);
      inside(g,split,()=>pixels(g));
    });
    part(side+'_shin',g=>{
      if(!moveKnee)shape(g,back?'M 148 379 Q 150 368 155 358 Q 163 349 173 356 Q 180 362 172 374 L 164 388 Z'
        :'M 308 382 Q 302 373 302 365 Q 303 354 314 354 Q 325 354 331 372 L 337 386 L 318 397 Z',skin,edge);
      shape(g,back?'M 132 398 Q 138 395 149 402 L 140 420 Q 137 428 130 426 Q 122 423 125 416 Z'
        :'M 327 403 Q 338 399 346 399 L 355 418 Q 358 426 350 429 Q 342 432 339 423 Z',skin,edge);
      inverseClip(g,split,()=>inverseClip(g,k.boot,()=>pixels(g)));
    });
    part(side+'_boot',g=>inside(g,k.boot,()=>pixels(g)));
  }
  // Preserve the already approved hand, tool and striking arm exactly.
  for(const name of ['upper_arm','forearm','hand','shovel']){
    fs.copyFileSync(path.join(prior,'parts',name+'.png'),path.join(parts,name+'.png'));
    const a=previous.skins[0].attachments[name][name];
    dims[name]={w:a.width,h:a.height,l:piv[name][0]+a.x-a.width/2,t:piv[name][1]-a.y-a.height/2};
  }
}

// Hip travel, pelvic roll, chest rotation and projected shoulder separation are staggered.
// The rear toe and lead sole are constrained in world coordinates during the whole swing.
const phases=[
  // time   hip dx/dy  roll chest  heel  yaw   free shoulder/elbow/wrist absolute offsets
  [0,       0,0,       0,0,        0,  0,      0,0,0],
  [.05,   -11,10,      3,-5,       0, -4,    -10,-22,-12],
  [.10,   -15,21,      5,-10,      4, -7,    -25,-47,-30],
  [.14,     1,14,     -2,-6,      17, -3,    -18,-35,-25],
  [.17,    17,6,      -5,4,       29,  7,     19,10,3],
  [.20,    30,5,      -6,13,      35, 15,     79,93,85],
  [.24,    33,9,      -5,15,      37, 16,     90,108,99],
  [.32,    28,17,     -2,12,      32, 12,     84,110,108],
  [.45,    13,15,      1,5,       18,  5,     48,70,59],
  [.60,    -4,6,       2,-2,       4, -1,     -3,10,16],
  [.73,    -2,2,       0,-1,       0,  0,     -5,-5,-2],
  [.86,     0,0,       0,0,        0,  0,      0,0,0]
];
function sample(keys,time,field){
  const j=keys.findIndex(k=>k[0]>time);
  if(j<0)return keys.at(-1)[field];
  if(j===0)return keys[0][field];
  const a=keys[j-1],b=keys[j],s=(time-a[0])/(b[0]-a[0]);
  // Cubic Hermite interpolation with slope limited at extrema to prevent a foot recoil.
  const slope=(i)=>{
    if(i===0||i===keys.length-1)return 0;
    const p=keys[i-1],q=keys[i],n=keys[i+1];
    const left=(q[field]-p[field])/(q[0]-p[0]),right=(n[field]-q[field])/(n[0]-q[0]);
    return left*right<=0?0:2*left*right/(left+right);
  };
  const dt=b[0]-a[0];
  return (2*s**3-3*s*s+1)*a[field]+(s**3-2*s*s+s)*dt*slope(j-1)
    +(-2*s**3+3*s*s)*b[field]+(s**3-s*s)*dt*slope(j);
}
const strikes=[[0,0,0,0],[.05,35,58,20],[.10,74,108,51],[.14,131,149,78],
  [.17,209,190,114],[.20,270,233,159],[.24,277,243,166],[.32,292,263,193],
  [.45,306,291,241],[.60,335,333,314],[.73,363,364,366],[.86,360,360,360]];
// Front raise: the elbow leads across the apron, then bends to bring the grip
// up in front of the face. Keep the blade in the same grip on the downward thrust.
const frontBody=[
  [0,0,0,0,0,0,0,0,0,0],
  [.08,-2,5,1,0,0,2,-3,-6,-5],
  [.17,-5,9,2,-3,0,5,-8,-15,-12],
  [.25,-4,6,2,-5,3,7,-12,-19,-15],
  [.29,-2,6,1,-4,6,8,-12,-20,-15],
  [.33,12,7,-3,4,20,12,2,-5,-7],
  [.38,29,15,-6,15,34,16,20,2,-4],
  [.43,30,19,-5,17,34,15,23,12,6],
  [.52,22,16,-3,12,27,11,19,17,11],
  [.66,5,9,1,3,12,3,10,18,12],
  [.80,-2,3,1,-1,2,-1,-3,-4,-2],
  [.94,0,0,0,0,0,0,0,0,0]
];
// Absolute offsets from the source sprite, in screen-space degrees.
const frontArm=[
  [0,0,0,0],[.06,-10,-29,-25],[.13,-36,-90,-70],
  [.19,-86,-151,-137],[.25,-136,-189,-162],[.29,-139,-190,-163],
  [.33,-110,-155,-164],[.38,-60,-101,-164],[.43,-54,-95,-157],
  [.52,-65,-122,-171],[.66,-40,-94,-116],[.80,-10,-30,-38],[.94,0,0,0]
];
// Keep the grip on the near side during the lift. The rear heel releases before
// the pelvis advances, leaving elbow extension for the last part of the thrust.
const longBody=[
  [0,0,0,0,0,0,0,0,0,0],
  [.08,-4,7,1,-1,0,1,-3,-6,-5],
  [.17,-11,14,3,-5,0,3,-7,-16,-11],
  [.25,-11,12,3,-7,2,5,-12,-20,-15],
  [.29,-8,11,2,-6,6,6,-12,-20,-15],
  [.33,15,12,-4,5,28,12,3,-3,-6],
  [.38,51,22,-8,22,48,22,14,-6,-9],
  [.43,53,24,-7,23,48,22,20,8,5],
  [.52,33,20,-4,15,36,14,18,14,10],
  [.66,8,10,1,3,15,4,11,20,14],
  [.80,-2,3,1,-1,2,-1,-3,-4,-2],
  [.94,0,0,0,0,0,0,0,0,0]
];
const longArm=[
  [0,0,0,0],[.06,-10,-28,-24],[.13,-32,-87,-72],
  [.19,-65,-158,-147],[.25,-103,-220,-196],[.29,-106,-222,-197],
  [.33,-109,-192,-197],[.38,-95,-119,-195],[.43,-89,-116,-192],
  [.52,-82,-149,-189],[.66,-42,-94,-116],[.80,-10,-30,-38],[.94,0,0,0]
];
// Match the reference's loaded opening pose, early extension and longer recovery.
// Times here are source frame numbers on a 30 fps timeline.
const seconds=rows=>rows.map(([frame,...values])=>[frame/30,...values]);
const silentBody=seconds([
  [0,-32,47,5,-12,0,-9,-16,-86,-80],
  [.40,-5,40,1,-7,22,-4,-9,-73,-74],
  [1,38,29,-6,18,43,18,35,26,17],
  [2,58,30,-8,27,49,24,64,82,77],
  [4,60,32,-7,28,49,24,74,100,96],
  [8,55,31,-6,25,46,22,71,98,96],
  [10,45,27,-4,19,40,19,58,89,86],
  [12,16,17,0,7,23,8,18,53,47],
  [14,-2,11,2,-3,8,-1,-9,-8,-3],
  [16,-5,6,2,-3,2,-3,-7,-12,-8],
  [20,0,0,0,0,0,0,0,0,0],
  [28,0,0,0,0,0,0,0,0,0]
]);
// Last column is the trowel's absolute offset. Its pivot stays in the fist
// as the hand turns from a tucked grip into line with the extending elbow.
const silentArm=seconds([
  [0,-68,-218,-125,-201],[.40,-77,-215,-126,-202],[1,-94,-145,-131,-207],
  [2,-97,-124,-138,-214],[4,-96,-120,-138,-214],[8,-94,-124,-137,-213],
  [10,-86,-151,-128,-204],[12,-59,-173,-120,-182],[14,-34,-116,-94,-121],
  [16,-11,-41,-44,-44],[18,2,3,5,5],[20,0,0,0,0],[28,0,0,0,0]
]);
const leadStride=seconds([[0,-10,-3],[.55,6,-9],[2,33,0],[10,33,0],
  [12,18,-7],[14,4,-5],[16,0,0],[28,0,0]]);
function solveLeg(side,hip,tilt,roll,step){
  const thigh=piv[side+'_thigh'],knee=piv[side+'_shin'],ankle=piv[side+'_boot'];
  const toe=add(piv[side+'_contact'],step);
  const A=add(hip,turn(sub(thigh,piv.hips),roll)),B=add(toe,turn(sub(ankle,toe),tilt));
  const u=sub(knee,thigh),v=sub(ankle,knee),base=sub(B,A),d=length(base),l1=length(u),l2=length(v);
  if(d>l1+l2+.001||d<Math.abs(l1-l2))throw Error(side+' leg cannot reach '+d+' / '+(l1+l2));
  const along=(l1*l1-l2*l2+d*d)/(2*d);
  const sign=Math.sign(sub(ankle,thigh)[0]*u[1]-sub(ankle,thigh)[1]*u[0]);
  const across=sign*Math.sqrt(Math.max(0,l1*l1-along*along));
  const K=add(A,[(base[0]*along-base[1]*across)/d,(base[1]*along+base[0]*across)/d]);
  const first=delta(heading(sub(K,A))-heading(u)),second=delta(heading(sub(B,K))-heading(v));
  return [first-roll,second-first,tilt-second];
}
function makeRig(){
  const bones=Object.entries(parent).map(([name,up])=>{
    const [x,y]=sub(piv[name],up?piv[up]:piv[name]);return {name,...(up?{parent:up}:{}),x:r(x),y:r(-y)};
  });
  const order=['pelvis','rear_shin','rear_thigh','rear_boot','lead_shin','lead_thigh','lead_boot',
    'open_fore','open_upper','open_hand','torso','head','forearm','upper_arm','shovel','hand'];
  const slots=order.map(name=>({name,bone:({torso:'ribcage',pelvis:'hips'})[name]??name,attachment:name}));
  const attachments={};
  for(const s of slots){
    const {w,h,l,t}=dims[s.name],p=piv[s.bone];
    attachments[s.name]={[s.name]:{width:w,height:h,x:r(l+w/2-p[0]),y:r(p[1]-t-h/2)}};
  }
  return {skeleton:{spine:'4.2.36',images:'./parts/'},bones,slots,skins:[{name:'default',attachments}],animations:{attack:{bones:makeTimelines(phases,strikes)}}};
}
function makeTimelines(bodyKeys,armKeys,stepKeys){
  const timelines={};
  function key(b,type,time,data){((timelines[b]??={})[type]??=[]).push({time,...data});}
  const end=bodyKeys.at(-1)[0],rate=stepKeys?300:100;
  for(let i=0;i<=Math.round(end*rate);i++){
    const t=i/rate,[x,y,roll,chest,heel,yaw,ou,of,oh]=Array.from({length:9},(_,k)=>sample(bodyKeys,t,k+1));
    const [u,f,h]=[1,2,3].map(k=>sample(armKeys,t,k));
    key('hips','translate',t,{x:r(x),y:r(-y)});
    key('upper_arm','translate',t,{x:r(yaw),y:r(yaw*.10)});
    key('open_upper','translate',t,{x:r(-yaw*.68),y:r(-yaw*.11)});
    key('ribcage','scale',t,{x:r(1-yaw*.009),y:1});
    const rot={hips:roll,chest:chest-roll,head:-chest*.60,
      upper_arm:u-chest,forearm:f-u,hand:h-f,open_upper:ou-chest,open_fore:of-ou,open_hand:oh-of};
    if(armKeys[0].length===5)rot.shovel=sample(armKeys,t,4)-h;
    for(const side of ['rear','lead']){
      const step=side==='lead'&&stepKeys?[sample(stepKeys,t,1),sample(stepKeys,t,2)]:[0,0];
      const angles=solveLeg(side,add(piv.hips,[x,y]),side==='rear'?heel:0,roll,step);
      ['thigh','shin','boot'].forEach((part,j)=>rot[side+'_'+part]=angles[j]);
    }
    for(const [name,angle] of Object.entries(rot))key(name,'rotate',t,{value:r(-angle)});
  }
  return timelines;
}
async function main(){
  if(silentThrust){
    const rig=JSON.parse(fs.readFileSync(path.join(out,'viridis-fullbody.json')));
    rig.animations={attack:{bones:makeTimelines(silentBody,silentArm,leadStride)}};
    fs.writeFileSync(path.join(out,'viridis-silent-thrust.json'),JSON.stringify(rig)+'\n');
    return;
  }
  const both=process.argv.includes('--both-forearms-front');
  if(both||process.argv.includes('--forearm-front')){
    const base=both?'viridis-forearm-front':'viridis-long-stab';
    const rig=JSON.parse(fs.readFileSync(path.join(out,base+'.json')));
    const fore=rig.slots.findIndex(s=>s.name===(both?'open_fore':'forearm'));
    if(fore<0||rig.slots[fore+1]?.name!==(both?'open_upper':'upper_arm'))throw Error('Unexpected arm slot order');
    [rig.slots[fore],rig.slots[fore+1]]=[rig.slots[fore+1],rig.slots[fore]];
    fs.writeFileSync(path.join(out,both?'viridis-both-forearms-front.json':'viridis-forearm-front.json'),JSON.stringify(rig)+'\n');
    return;
  }
  const long=process.argv.includes('--long-stab');
  if(long||process.argv.includes('--front-stab')){
    const rig=JSON.parse(fs.readFileSync(path.join(out,'viridis-fullbody.json')));
    rig.animations={attack:{bones:long?makeTimelines(longBody,longArm):makeTimelines(frontBody,frontArm)}};
    fs.writeFileSync(path.join(out,long?'viridis-long-stab.json':'viridis-front-stab.json'),JSON.stringify(rig)+'\n');
    return;
  }
  await extract();
  const rig=makeRig();
  if(kneeOnShin){
    const base=JSON.parse(fs.readFileSync(path.join(fullbody,'viridis-both-forearms-front.json')));
    rig.animations=base.animations;
    rig.slots=base.slots;
    const index=rig.slots.findIndex(slot=>slot.name==='lead_shin');
    if(index<0||rig.slots[index+1]?.name!=='lead_thigh')throw Error('Unexpected lead leg slot order');
    [rig.slots[index],rig.slots[index+1]]=[rig.slots[index+1],rig.slots[index]];
  }
  fs.writeFileSync(path.join(out,'viridis-fullbody.json'),JSON.stringify(rig)+'\n');
  const page=createCanvas(512,512),pg=page.getContext('2d');
  const atlas=['viridis-fullbody.png','size:512,512','filter:Linear,Linear','pma:false'];
  let x=2,y=2,row=0;
  for(const [name,{w,h}] of Object.entries(dims).sort((a,b)=>b[1].h-a[1].h)){
    if(x+w+2>512){x=2;y+=row+4;row=0;}
    if(y+h+2>512)throw Error('Atlas overflow at '+name);
    pg.drawImage(await loadImage(path.join(parts,name+'.png')),x,y);
    atlas.push(name,'bounds:'+ [x,y,w,h].join(','));x+=w+4;row=Math.max(row,h);
  }
  fs.writeFileSync(path.join(out,'viridis-fullbody.png'),page.toBuffer('image/png'));
  fs.writeFileSync(path.join(out,'viridis-fullbody.atlas'),atlas.join('\n')+'\n');
}
main().catch(err=>{console.error(err);process.exitCode=1;});
