"""Assemble the painted cels into a registered PNG sequence and a Godot SpriteFrames clip."""
import json
from pathlib import Path
from shutil import copyfile
from subprocess import run
from zipfile import ZipFile, ZIP_DEFLATED

import numpy as np
from PIL import Image, ImageDraw, ImageFont
from scipy import ndimage

ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / "mods/Viridis/art_source/characters/viridis"
DEST = ART / "thrust-cels"
KEYS = ART / "shovel-fullbody/keyframe-study/keys"
OUT = ROOT / "outputs/viridis-animation"
SIZE = (608, 520)
FLOOR = (131, 502)
BG = (40, 49, 54)
FPS = 60
# Holds are counted in display frames. The hurt/impact pose begins on tick 19.
TIMING = [
    ("guard",2), ("shift",4), ("raise",5), ("chamber",3),
    ("drive",3), ("pass",2), ("reach",5), ("release",4),
    ("bend",5), ("withdraw",5), ("recenter",5), ("recover",6),
    ("lower",6), ("open",4), ("settle",4), ("guard",2),
]
# Rear boot contact in source pixels. Normalize each whole drawing to the
# shared canvas. The first two recovery drawings have wider source proportions.
LAYOUT = {
    "shift": ("inbetweens",0,(149,527),.905),
    "raise": ("inbetweens",1,(671,530),.91),
    "pass": ("inbetweens",2,(111,1003),1.01),
    "withdraw": ("inbetweens",3,(595,1002),1.01),
    "recenter": ("inbetweens",4,(145,1525),.94),
    "settle": ("inbetweens",5,(659,1525),.94),
    "release": ("release",0,(99,559),(.742,.84)),
    "bend": ("release",1,(740,558),(.772,.85)),
    "lower": ("release",2,(204,1186),.78),
    "open": ("release",3,(819,1189),.78),
}

def isolate_sheet(name,expected):
    a = np.array(Image.open(DEST / "source" / (name+".png")).convert("RGBA"))
    labels,_ = ndimage.label(a[:,:,3]>80)
    counts = np.bincount(labels.ravel())
    ids = np.where(counts[1:]>10000)[0]+1
    assert len(ids)==expected
    objects = ndimage.find_objects(labels)
    order = sorted(ids,key=lambda i:objects[i-1][0].start)
    figures = []
    for j in range(0,len(order),2):
        for i in sorted(order[j:j+2],key=lambda i:objects[i-1][1].start):
            mask = ndimage.binary_dilation(labels==i,iterations=4)
            rgba = a.copy();rgba[~mask]=0
            figures.append(Image.fromarray(rgba))
    return figures

def register(im,contact,scale):
    box=im.getchannel("A").getbbox()
    cropped=im.crop(box)
    w,h=cropped.size
    sx,sy=(scale,scale) if np.isscalar(scale) else scale
    sprite=cropped.resize((round(w*sx),round(h*sy)),Image.Resampling.LANCZOS)
    x=round(FLOOR[0]+(box[0]-contact[0])*sx)
    y=round(FLOOR[1]+(box[1]-contact[1])*sy)
    assert x>=0 and y>=0 and x+sprite.width<=SIZE[0] and y+sprite.height<=SIZE[1]
    frame=Image.new("RGBA",SIZE);frame.paste(sprite,(x,y))
    return frame

def main():
    dst=DEST/"frames";dst.mkdir(exist_ok=True,parents=True)
    guard=ART/"shovel-strike/frames/guard.png"
    names=list(dict.fromkeys(name for name,_ in TIMING))
    sheets={"inbetweens":isolate_sheet("inbetweens",6),"release":isolate_sheet("release",4)}
    paths={};pics={}
    for i,name in enumerate(names):
        f=dst/f"{i:02d}-{name}.png";paths[name]=f
        if name=="guard":copyfile(guard,f)
        elif name in LAYOUT:
            sheet,cell,anchor,scale=LAYOUT[name]
            register(sheets[sheet][cell],anchor,scale).save(f)
        else:
            with Image.open(KEYS/(name+".png")) as key:
                register(key.convert("RGBA"),(156,586),.86).save(f)
        pics[name]=Image.open(f).convert("RGBA")
    assert guard.read_bytes()==paths["guard"].read_bytes()
    resource=[f'[gd_resource type="SpriteFrames" load_steps={len(names)+1} format=3]','']
    for n,f in paths.items():
        resource.append(f'[ext_resource type="Texture2D" path="frames/{f.name}" id="{n}"]')
    def animation(name,timing,loop):
        frames=", ".join(f'{{"duration": {hold}.0, "texture": ExtResource("{n}")}}' for n,hold in timing)
        return f'{{"frames": [{frames}], "loop": {str(loop).lower()}, "name": &"{name}", "speed": {FPS}.0}}'
    resource+=['','[resource]','animations = ['+animation("idle_loop",[("guard",60)],True)+', '+animation("attack",TIMING,False)+']']
    (DEST/"viridis-thrust.tres").write_text("\n".join(resource)+"\n",encoding="utf-8")
    (DEST/"viridis-thrust.tscn").write_text(
        '[gd_scene load_steps=2 format=3]\n\n[ext_resource type="SpriteFrames" path="viridis-thrust.tres" id="frames"]\n\n'
        '[node name="Viridis" type="AnimatedSprite2D"]\nsprite_frames = ExtResource("frames")\nanimation = &"idle_loop"\n'
        'autoplay = "idle_loop"\ncentered = false\noffset = Vector2(-235, -504)\n',encoding="utf-8")
    tick=0;timeline=[]
    for n,hold in TIMING:
        timeline.append({"file":"frames/"+paths[n].name,"start_tick":tick,"hold_ticks":hold})
        tick+=hold
    (DEST/"timing.json").write_text(json.dumps({"fps":FPS,"size":SIZE,"pivot":[235,504],"attack":timeline},indent=2)+"\n")
    OUT.mkdir(parents=True,exist_ok=True)
    with ZipFile(OUT/"Viridis-thrust-cels.zip","w",ZIP_DEFLATED) as z:
        for f in [*paths.values(),DEST/"timing.json",DEST/"viridis-thrust.tres",DEST/"viridis-thrust.tscn"]:
            z.write(f,f.relative_to(DEST))
    def composite(n):
        img=Image.new("RGB",SIZE,BG);img.paste(pics[n],mask=pics[n].getchannel("A"));return img
    images={n:composite(n) for n in names}
    raw=b"".join(images[n].tobytes()*hold for n,hold in [("guard",14),*TIMING,("guard",14)])
    for ext,codec in [("webp",["-c:v","libwebp_anim","-quality","95","-loop","0"]),
                      ("mp4",["-c:v","libx264","-pix_fmt","yuv420p","-crf","16","-movflags","+faststart"])]:
        run(["ffmpeg","-v","error","-y","-f","rawvideo","-pix_fmt","rgb24","-s","608x520","-framerate","60",
             "-i","pipe:0",*codec,str(OUT/("viridis-thrust-cels."+ext))],input=raw,check=True)
    sheet=Image.new("RGB",(4*300,4*279),BG);d=ImageDraw.Draw(sheet)
    font=ImageFont.truetype("C:/Windows/Fonts/msyh.ttc",15)
    labels=["待机","压低重心","抬铲","蓄力","蹬地送肩","伸臂","平刺到位","卸力","屈肘","收铲","回重心","收势","放低铲尖","打开肩膀","落回站姿","待机"]
    for j,((n,h),label) in enumerate(zip(TIMING,labels)):
        x=j%4*300;y=j//4*279
        sheet.paste(images[n].resize((300,257),Image.Resampling.LANCZOS),(x,y))
        d.text((x+150,y+266),label,font=font,anchor="mm",fill="#e4e4d8")
    sheet.save(OUT/"viridis-thrust-cels-sheet.png")
    print(len(names),"unique cels,",tick,"attack ticks")

if __name__=="__main__":
    main()
