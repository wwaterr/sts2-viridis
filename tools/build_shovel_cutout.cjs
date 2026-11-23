// Extract the original painted surfaces, complete the overlaps, then author the Spine rig.
// Requires @napi-rs/canvas; NODE_PATH may point at the bundled Node dependencies.
const { createCanvas, loadImage, Path2D } = require('@napi-rs/canvas');
const fs = require('node:fs');
const path = require('node:path');
const dir = path.resolve(__dirname, '../mods/Viridis/art_source/characters/viridis/shovel-cutout');
const W = 608, H = 520;
const parts = path.join(dir, 'parts');
fs.mkdirSync(parts,{recursive:true});
const canvas = () => createCanvas(W,H);
function clip(ctx, outline, paint) {
  ctx.save(); ctx.clip(new Path2D(outline)); paint(); ctx.restore();
}
function shape(ctx, outline, color, stroke) {
  const p = new Path2D(outline);
  ctx.fillStyle=color; ctx.fill(p);
  if(stroke){ctx.strokeStyle=stroke;ctx.lineWidth=1.1;ctx.stroke(p);}
}
// Follow the shirt/vest and tie contours, then travel through negative space back around the tool.
const limb = 'M 199.7 152.8 Q 196 162 197.6 171 Q 198.6 177 196.1 184 Q 193.6 190.5 198.6 199.1 L 196.1 205.6 L 195.8 215 L 195.9 220 L 191 231 L 179 246 L 173 253 L 172 263 L 157 277 L 155.7 298 L 152 302 L 54 281 L 52 211 L 152 211 L 161 168 Z';
const proximal = 'M 50 70 H 248 V 262 L 191 226 L 171 213 L 50 155 Z';
const distal = 'M 50 155 L 171 213 L 191 226 L 248 262 V 310 H 50 Z';
const aboveCuff = 'M 51 80 H 242 V 295 L 166 254.5 L 155.5 245 L 150 241.5 L 146 239 L 105 203 L 51 192 Z';
const fist = 'M 146 239.1 Q 152 238.8 155.6 245 L 166.1 253.5 L 170.2 259.6 L 161 266 L 152.5 282.6 L 148.3 285.2 Q 144.5 282.1 142.6 285.4 Q 141.8 288.4 139 292.7 L 132 291.1 L 117.2 280.8 L 116.8 276.5 L 120.1 272.4 L 125.4 265.1 L 127 261.9 L 139 256 Z';
const weapon = 'M 55 214 L 123 214 L 121 254 L 129 264 L 123.6 273.5 L 117 275 L 101 270 L 58 270 Z M 139.5 292.6 Q 140.8 289.7 142.6 285.4 Q 144.4 282.6 148.4 285 L 155.4 288.4 L 156 300 L 141 300 Z';
function cut(ctx, mask, image) {clip(ctx,mask,()=>ctx.drawImage(image,0,0));}
function crop(name,surface) {
  const g=surface.getContext('2d'), data=g.getImageData(0,0,W,H).data;
  let l=W,t=H,r=0,b=0;
  for(let y=0;y<H;y++)for(let x=0;x<W;x++)if(data[(y*W+x)*4+3]>3){
    l=Math.min(l,x);t=Math.min(t,y);r=Math.max(r,x);b=Math.max(b,y);
  }
  l-=2;t-=2;r+=3;b+=3;
  const c=createCanvas(r-l,b-t);c.getContext('2d').drawImage(surface,-l,-t);
  fs.writeFileSync(path.join(parts,name+'.png'),c.toBuffer('image/png'));
  return {name,l,t,w:c.width,h:c.height};
}
async function main(){
  const original=await loadImage(path.join(dir,'../shovel-strike/frames/guard.png'));
  const plate=await loadImage(path.join(dir,'shoulder-plate.png'));
  const layers={};
  function layer(name,paint){
    const c=canvas();paint(c.getContext('2d'));layers[name]=crop(name,c);
  }
  layer('body',g=>{
    g.drawImage(original,0,0);
    g.save();g.globalCompositeOperation='destination-out';shape(g,limb,'white');g.restore();
    // Use only the generated armhole patch in the hole; the rest is the original sprite.
    g.save();g.globalCompositeOperation='destination-over';
    clip(g,'M 192 150 L 208 150 L 214 222 L 188 221 Z',()=>g.drawImage(plate,0,0,W,H));g.restore();
  });
  layer('upper_arm',g=>{
    shape(g,'M 190 174 Q 190 163 196.5 158 Q 201 154.8 204.3 161.4 Q 206 166 202 173 L 197 179 Z','#dfcbb4','#6c5846');
    shape(g,'M 181.9 207 Q 178.8 215 173.4 219.9 Q 169.8 227.2 177.7 229.4 Q 183.5 231.1 188.8 222.9 L 191.7 215 Z','#e9c5ac','#78604d');
    clip(g,limb,()=>cut(g,proximal,original));
  });
  layer('forearm',g=>{
    // Rounded skin extends inside the sleeve-side elbow and underneath the flared glove cuff.
    const skin=g.createLinearGradient(167,212,183,225);
    skin.addColorStop(0,'#f1d1b7');skin.addColorStop(1,'#dfb99e');
    shape(g,'M 169.7 227 Q 169.2 219 174.4 214.1 Q 181.6 207.8 187 214.1 Q 193 219.5 187.7 226.2 L 180 234 Z',skin,'#806553');
    shape(g,'M 157.5 239.7 Q 153.2 246 149.5 251.7 Q 147.3 257.3 153 260.7 Q 158.3 262.1 162.8 254.8 L 170.1 245.2 Z','#edcbb1','#846854');
    clip(g,'M 169 208 L 199 215 L 183 238 L 168.8 252.2 L 156 244 L 152.8 243 L 165 218 Z',()=>
      clip(g,limb,()=>clip(g,distal,()=>cut(g,aboveCuff,original))));
  });
  layer('shovel',g=>{
    // Continuous wooden shaft between the two exposed painted ends.
    const wood=g.createLinearGradient(132,273,125.5,281.8);
    wood.addColorStop(0,'#a27f58');wood.addColorStop(.55,'#956d4d');wood.addColorStop(1,'#624737');
    shape(g,'M 109.4 253.7 L 152.5 286.1 Q 157.2 289.5 152.1 295.5 Q 149.4 299 145.9 296.1 L 104.1 262.4 Z',wood,'#433b30');
    cut(g,weapon,original);
  });
  layer('hand',g=>{
    // Close the palm behind the two exposed handle ends.
    shape(g,'M 123.7 265.3 Q 126 260.2 132.4 260.1 Q 145 258 151.1 267.9 L 149.3 281.7 Q 147.5 287.7 139.4 292.4 Q 133.8 292 128.9 287.2 L 119.4 280.7 Q 116.4 276.9 121 271.8 Z','#807955','#514b39');
    cut(g,fist,original);
  });
  const pivots={root:[235,504],chest:[235,295],upper_arm:[200,163],forearm:[181,218.8],hand:[158.7,249.7],shovel:[135,278]};
  const hierarchy={root:null,chest:'root',upper_arm:'chest',forearm:'upper_arm',hand:'forearm',shovel:'hand'};
  const bones=Object.entries(hierarchy).map(([name,parent])=>{
    const p=pivots[name],q=parent?pivots[parent]:p;
    return {name,...(parent?{parent}:{}),x:p[0]-q[0],y:q[1]-p[1]};
  });
  const names=['body','forearm','upper_arm','shovel','hand'];
  const slots=names.map(name=>({name,bone:name==='body'?'root':name,attachment:name}));
  const attachments={};
  for(const name of names){
    const v=layers[name],p=pivots[name]??pivots.root;
    attachments[name]={[name]:{width:v.w,height:v.h,x:v.l+v.w/2-p[0],y:p[1]-v.t-v.h/2}};
  }
  // Body follows chest above the waist, legs remain pinned. The shoulder is 100% chest-weighted.
  const b=layers.body,rows=[b.t,152,222,247,265,281,300,318,336,353,b.t+b.h];
  const cols=[b.l,179,238,269,285,303,316,340,b.l+b.w];
  const pts=[],ids=new Map(),uvs=[],vertices=[],triangles=[];
  const add=(col,j)=>{
    const key=col+','+j;if(ids.has(key))return ids.get(key);
    const id=pts.length;pts.push(key);ids.set(key,id);
    const x=cols[col],y=rows[j],s=Math.max(0,Math.min(1,(332-y)/80));
    const outsideHip=Math.max(0,Math.min(1,(x-278)/31));
    const openHand=outsideHip*Math.max(0,Math.min(1,(331-y)/25));
    const chest=Math.max(s*s*(3-2*s),openHand);
    uvs.push((x-b.l)/b.w,(y-b.t)/b.h);
    const weights=[[0,1-chest],[1,chest]].filter(([,w])=>w>0);vertices.push(weights.length);
    for(const [idx,w] of weights){const p=pivots[bones[idx].name];vertices.push(idx,x-p[0],p[1]-y,w);}
    return id;
  };
  const last=cols.length-1,lastRow=rows.length-1;
  for(let i=0;i<=last;i++)add(i,0);
  for(let j=1;j<=lastRow;j++)add(last,j);
  for(let i=last-1;i>=0;i--)add(i,lastRow);
  for(let j=lastRow-1;j>0;j--)add(0,j);
  const hull=pts.length;
  for(let j=0;j<lastRow;j++)for(let i=0;i<last;i++)
    triangles.push(add(i,j),add(i+1,j),add(i+1,j+1),add(i,j),add(i+1,j+1),add(i,j+1));
  attachments.body.body={type:'mesh',width:b.w,height:b.h,uvs,vertices,triangles,hull};
  // Absolute screen-space limb directions, unwrapped along the overhead arc.
  // time, chest lean, upper-arm angle, forearm angle, wrist-to-fist angle
  const upper0=Math.atan2(55.8,-19)*180/Math.PI;
  const lower0=Math.atan2(30.9,-22.3)*180/Math.PI;
  const hand0=Math.atan2(28.3,-23.7)*180/Math.PI;
  const keys=[
    [0,0,upper0,lower0,hand0],[.045,-4,139,175,148],[.085,-8,176,232,177],
    [.12,-7,215,270,202],[.15,-2,267,304,224],[.177,7,328,341,251],
    [.20,16,380,354,291],[.232,16.4,384,361,296],[.29,13.5,397,388,317],
    [.37,8.5,405,403,336],[.48,3,420,422,389],[.60,-1.6,447,454,452],
    [.70,-.8,upper0+363,lower0+364,hand0+366],[.80,0,upper0+360,lower0+360,hand0+360]
  ];
  const anim=Object.fromEntries(['chest','upper_arm','forearm','hand'].map(name=>[name,{rotate:[]}]));
  for(const [time,c,u,l,h] of keys){
    const angles=[c,u-upper0-c,l-lower0-(u-upper0),h-hand0-(l-lower0)];
    Object.values(anim).forEach((track,i)=>track.rotate.push({time,value:+(-angles[i]).toFixed(4)}));
  }
  const rig={skeleton:{spine:'4.2.36',images:'./parts/'},bones,slots,skins:[{name:'default',attachments}],
    animations:{attack:{bones:anim}}};
  fs.writeFileSync(path.join(dir,'viridis-shovel.json'),JSON.stringify(rig,null,2)+'\n');
  const page=createCanvas(512,512),pg=page.getContext('2d');
  const atlas=['viridis-shovel.png','size:512,512','filter:Linear,Linear','pma:false'];
  let nextY=2;
  for(const name of names){
    const {w,h}=layers[name];
    const x=name==='body'?2:layers.body.w+6, y=name==='body'?2:nextY;
    if(x+w+2>512||y+h+2>512)throw Error('Atlas overflow: '+name);
    pg.drawImage(await loadImage(path.join(parts,name+'.png')),x,y);
    atlas.push(name,'bounds:'+ [x,y,w,h].join(','));
    if(name!=='body')nextY+=h+4;
  }
  fs.writeFileSync(path.join(dir,'viridis-shovel.png'),page.toBuffer('image/png'));
  fs.writeFileSync(path.join(dir,'viridis-shovel.atlas'),atlas.join('\n')+'\n');
}
main().catch(e=>{console.error(e);process.exitCode=1;});
