// Build the weighted cutout rig and editor textures from the approved neutral pose.
// Run from any directory with Node and ffmpeg on PATH.
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { execFileSync } from 'node:child_process';

const repo = fileURLToPath(new URL('../', import.meta.url));
const src = path.join(repo,'mods/Viridis/art_source/characters/viridis/spine');
const dest = path.join(repo,'mods/Viridis/assets/characters/viridis/combat');
const parts = path.join(src,'parts_v1');
for (const dir of [dest,parts]) fs.mkdirSync(dir,{recursive:true});
const art = path.join(src,'viridis_neutral_rig_reference_v1.png');
const resource = 'res://Viridis/characters/viridis/combat';
const scale = 0.16; // 237 game units from soles to cap.
const bones = [], pivots = new Map();
function bone(name,parent,x,y) {
  const base = pivots.get(parent) ?? [512,1505];
  bones.push({name,...(parent && {parent}),x:(x-base[0])*scale,y:(base[1]-y)*scale});
  pivots.set(name,[x,y]);
}
bone('root',null,512,1505);
bone('hips','root',512,785);
bone('chest','hips',512,555);
bone('head','chest',512,306);
bone('hem','hips',512,555);
for (const [side,sign] of [['r',-1],['l',1]]) {
  bone('arm_'+side,'chest',512+sign*93,355);
  bone('hand_'+side,'arm_'+side,512+sign*214,650);
  bone('boot_'+side,'root',512+sign*88,1350);
  bone('eye_'+side,'head',512+sign*39,218);
}
const clamp = x=>Math.max(0,Math.min(1,x));
const ramp = (v,a,b)=>clamp((v-a)/(b-a));
function weights(x,y) {
  const w = {};
  function blend(name,t) {
    for (const key in w) w[key]*=1-t;
    w[name]=(w[name]??0)+t;
  }
  const side = x<512?'r':'l', dx=Math.abs(x-512);
  blend('hips',1);
  blend('boot_'+side,ramp(y,870,1230));
  blend('hem',ramp(y,500,620)*(1-ramp(y,680,802))*(1-ramp(dx,105,155)));
  blend('chest',1-ramp(y,440,670));
  const shoulder = 82+Math.max(0,y-370)*0.24;
  const arm = ramp(dx,shoulder-20,shoulder+20)*ramp(y,310,370)*(1-ramp(y,807,831));
  blend('arm_'+side,arm);
  blend('hand_'+side,arm*ramp(y,606,686));
  blend('head',1-ramp(y,284,335));
  const eye = (1-ramp(Math.abs(dx-39),16,34))*(1-ramp(Math.abs(y-218),12,27));
  blend('eye_'+side,eye);
  return Object.entries(w).filter(([,v])=>v>0);
}
// Bands share the exact same vertices and weights on both sides of every seam.
const xs=[184,236,285,324,365,400,433,448,460,474,489,500,512,524,535,550,564,576,591,624,659,700,739,788,840];
const ys=[8,95,160,188,197,205,212,218,225,232,240,252,278,300,322,350,390,440,475,516,556,605,650,696,748,790,818,880,965,1040,1120,1195,1260,1355,1450,1520];
function section(x,y) {
  if(y<300)return 'head';
  if(y<818 && (x<365 || x>659))return x<512?'outer_arm_r':'outer_arm_l';
  if(y<556)return 'upper_body';
  if(y<818)return 'apron_shorts';
  return (y<1195?'leg_':'boot_')+(x<512?'r':'l');
}
const groups = new Map();
for(let j=0;j<ys.length-1;j++)for(let i=0;i<xs.length-1;i++){
  const key=section((xs[i]+xs[i+1])/2,(ys[j]+ys[j+1])/2);
  if(!groups.has(key))groups.set(key,[]);
  groups.get(key).push([i,j]);
}
const attachments={},slots=[];
const atlas=['viridis.png','size:1024,1536','filter:Linear,Linear','pma:false'];
for(const [name,cells] of groups){
  const left=Math.min(...cells.map(([i])=>i)),right=Math.max(...cells.map(([i])=>i))+1;
  const top=Math.min(...cells.map(([,j])=>j)),bottom=Math.max(...cells.map(([,j])=>j))+1;
  const x=xs[left],y=ys[top],width=xs[right]-x,height=ys[bottom]-y;
  const points=[],index=new Map(),vertices=[],uvs=[],triangles=[];
  function add(i,j){
    const key=i+','+j;
    if(index.has(key))return index.get(key);
    const id=points.length; points.push([i,j]);index.set(key,id);
    const px=xs[i],py=ys[j],w=weights(px,py);
    vertices.push(w.length);
    for(const [b,weight] of w){
      const [bx,by]=pivots.get(b);
      vertices.push(bones.findIndex(v=>v.name===b),(px-bx)*scale,(by-py)*scale,weight);
    }
    uvs.push((px-x)/width,(py-y)/height);
    return id;
  }
  for(let i=left;i<=right;i++)add(i,top);
  for(let j=top+1;j<=bottom;j++)add(right,j);
  for(let i=right-1;i>=left;i--)add(i,bottom);
  for(let j=bottom-1;j>top;j--)add(left,j);
  const hull=points.length;
  for(const [i,j] of cells){
    const a=add(i,j),b=add(i+1,j),c=add(i+1,j+1),d=add(i,j+1);
    triangles.push(a,b,c,a,c,d);
  }
  slots.push({name,bone:'root',attachment:name});
  attachments[name]={[name]:{type:'mesh',uvs,triangles,vertices,hull,width,height}};
  atlas.push(name,'bounds:'+ [x,y,width,height].join(','));
  execFileSync('ffmpeg',['-loglevel','error','-y','-i',art,'-vf',`crop=${width}:${height}:${x}:${y}`,path.join(parts,name+'.png')]);
}
function track(points){return points.map(([time,value])=>({time,value}));}
function motion(end,poses) {
  const result={};
  for(const [name,keys] of Object.entries(poses)){
    result[name]={rotate:track(keys)};
    if(keys.at(-1)[0]<end)result[name].rotate.push({time:end,value:0});
  }
  return {bones:result};
}
const cycle=(amp,delay=0)=>Array.from({length:13},(_,i)=>[i*0.2,amp*(Math.cos(delay)-Math.cos(i*Math.PI/6-delay))/2]);
const idle=motion(2.4,{chest:cycle(-1.7),head:cycle(2.3,.4),hem:cycle(2.0,.55),arm_l:cycle(3.1,.35),arm_r:cycle(-2.8,.35),hand_l:cycle(2,.8),hand_r:cycle(-2,.8)});
idle.bones.chest.translate=cycle(1.25).map(([time,y])=>({time,y}));
for(const name of ['eye_r','eye_l'])idle.bones[name]={scale:[[0,1],[1.38,1],[1.46,.075],[1.5,.075],[1.60,1],[2.4,1]].map(([time,y])=>({time,x:1,y}))};
function gesture(end,wind,hit,values){
  return motion(end,Object.fromEntries(Object.entries(values).map(([name,[a,b]])=>[name,[[0,0],[wind,a],[hit,b],[end,0]]])));
}
const animations={idle_loop:idle,
  attack:gesture(.70,.11,.20,{chest:[5,-11],head:[-3,5],arm_l:[-8,23],hand_l:[-3,12],hem:[-2,3]}),
  attack_heavy:gesture(1,.14,.22,{hips:[3,-5],chest:[7,-14],head:[-4,7],arm_l:[-14,35],hand_l:[-7,16],arm_r:[8,-12]}),
  hurt:gesture(.32,.035,.10,{chest:[-1,10],head:[-1,6],arm_r:[1,-9]}),
  die:motion(1.1,{root:[[0,0],[.16,5],[.65,68],[.87,84],[1.1,80]],head:[[0,0],[.36,12],[1.1,3]],arm_l:[[0,0],[.42,20],[1.1,8]]})
};
const rig={skeleton:{spine:'4.2.36',images:'./parts_v1/'},bones,slots,skins:[{name:'default',attachments}],animations};
function json(file,value){fs.writeFileSync(file,JSON.stringify(value,(_,v)=>typeof v==='number'?Number(v.toFixed(6)):v)+'\n');}
json(path.join(dest,'viridis.spjson'),rig);
json(path.join(dest,'viridis.spatlas'),{source_path:resource+'/viridis.atlas',atlas_data:atlas.join('\n')+'\n',normal_texture_prefix:'n',specular_texture_prefix:'s'});
// Editor import resolves image names relative to this copy.
fs.copyFileSync(path.join(dest,'viridis.spjson'),path.join(src,'viridis.json'));
