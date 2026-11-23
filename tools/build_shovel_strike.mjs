// Register the six drawn poses on their planted boots and time the first shovel strike.
// Run with Node and FFmpeg. Source coordinates are measured on poses.png.
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { execFileSync } from 'node:child_process';

const repo = fileURLToPath(new URL('../', import.meta.url));
const source = path.join(repo, 'mods/Viridis/art_source/characters/viridis/shovel-strike');
const review = path.join(repo, 'outputs/viridis-animation');
const parts = path.join(source, 'frames');
for (const dir of [review, parts]) fs.mkdirSync(dir, { recursive: true });
const ff = (args, input) => execFileSync('ffmpeg', ['-loglevel', 'error', '-y', ...args],
  { input, maxBuffer: 128 * 1024 * 1024 });
const sheet = ff(['-i', path.join(source, 'poses.png'), '-f', 'rawvideo', '-pix_fmt', 'rgba', 'pipe:1']);
const W = 608, H = 520, ground = 504, front = 394, back = 132;
const poses = [
  // name, crop L/T/R/B, front toe, rear toe, ground line on the sheet
  ['guard',       80,   0, 470, 505, 436, 174, 490],
  ['load',       585,  32, 950, 505, 922, 667, 490],
  ['pass',      1020,   0,1430, 505,1401,1080, 490],
  ['contact',     25, 558, 536,1015, 432,  96, 998],
  ['follow',     537, 577,1000,1015, 935, 592, 998],
  ['recover',   1080, 513,1475,1018,1445,1152,1000],
];
const clamp = t => Math.max(0, Math.min(1, t));
const smooth = t => (t = clamp(t), t * t * (3 - 2 * t));
function registered([name, left, top, right, bottom, frontToe, rearToe, baseline]) {
  const pixels = Buffer.alloc(W * H * 4);
  for (let y = 0; y < H; y++) {
    const sy = y + baseline - ground;
    if (sy < top || sy >= bottom) continue;
    // Correct the rear foot only below the hips, fading the correction across the stance.
    const leg = smooth((y - (ground - 223)) / 165);
    const delta = rearToe - frontToe - (back - front);
    for (let x = 0; x < W; x++) {
      const influence = 1 - smooth((x - back - 18) / (front - back - 70));
      const sx = x + frontToe - front + leg * influence * delta;
      if (sx < left || sx >= right - 1) continue;
      const i = (sy * 1536 + Math.floor(sx)) * 4;
      const t = sx - Math.floor(sx), offset = (y * W + x) * 4;
      const a = (1-t)*sheet[i+3], b = t*sheet[i+7], alpha = a+b;
      // The generated sheet carries a near-invisible matte (alpha 0–3) between drawings.
      if (alpha <= 4) continue;
      for (let c = 0; c < 3; c++) pixels[offset+c] = (a*sheet[i+c]+b*sheet[i+4+c])/alpha;
      pixels[offset+3] = Math.round(alpha);
    }
  }
  ff(['-f','rawvideo','-pix_fmt','rgba','-s',W+'x'+H,'-i','pipe:0','-frames:v','1',
    path.join(parts,name+'.png')],pixels);
  return pixels;
}
const images = poses.map(registered);
const fps = 50;
// In game time, attack starts on tick zero. Each exposure is an integer 20 ms frame.
const exposure = [[0,0],[2,1],[7,2],[10,3],[15,4],[22,5],[32,0]];
const end = 38/fps, unit = 0.5;
const attachments = Object.fromEntries(poses.map(([name]) => [name,{
  width:W*unit,height:H*unit,x:(W/2-(front+back)/2)*unit,y:(ground-H/2)*unit
}]));
const idleScale = [[0,1],[.6,1.005],[1.2,1.013],[1.8,1.005],[2.4,1]];
const rig = {
  skeleton:{spine:'4.2.36',images:'./frames/'},
  bones:[{name:'root'},{name:'body',parent:'root'}],
  slots:[{name:'pose',bone:'body',attachment:'guard'}],
  skins:[{name:'default',attachments:{pose:attachments}}],
  animations:{
    idle_loop:{bones:{body:{scale:idleScale.map(([time,y])=>({time,x:1,y}))}}},
    attack:{slots:{pose:{attachment:[...exposure.map(([tick,i])=>({time:tick/fps,name:poses[i][0]})),
      {time:end,name:'guard'}]}}}
  }
};
fs.writeFileSync(path.join(source,'shovel-strike.json'),JSON.stringify(rig,null,2)+'\n');

function display(rgba,scaleY){
  // 304×260 review viewport. Box-filter 2×2 samples over a neutral combat value.
  const w=W/2,h=H/2,frame=Buffer.alloc(w*h*3),bg=[42,47,48];
  for(let y=0;y<h;y++)for(let x=0;x<w;x++){
    const out=(y*w+x)*3;
    const rgb=[0,0,0];
    for(const dy of [0,1])for(const dx of [0,1]){
      const sy=Math.round(ground+(2*y+dy-ground)/scaleY);
      const at=(sy*W+2*x+dx)*4;
      const alpha=sy<0||sy>=H?0:rgba[at+3]/255;
      for(let c=0;c<3;c++) rgb[c]+=alpha*(rgba[at+c]??0)+(1-alpha)*bg[c];
    }
    for(let c=0;c<3;c++)frame[out+c]=Math.round(rgb[c]/4);
  }
  return frame;
}
function breathing(t){
  const next=idleScale.findIndex(([time])=>time>t);
  if(next<1)return 1;
  const [a,ya]=idleScale[next-1],[b,yb]=idleScale[next];
  return ya+(yb-ya)*(t-a)/(b-a);
}
const frames=[];
for(let tick=0;tick<120;tick++)frames.push(display(images[0],breathing(tick/fps)));
const attack=images.map(im=>display(im,1));
for(let tick=0;tick<end*fps;tick++){
  const [,index]=exposure.findLast(([start])=>start<=tick);
  frames.push(attack[index]);
}
ff(['-f','rawvideo','-pix_fmt','rgb24','-s',(W/2)+'x'+(H/2),'-framerate',String(fps),
  '-i','pipe:0','-loop','0','-lossless','1',path.join(review,'viridis-shovel-strike.webp')],Buffer.concat(frames));
