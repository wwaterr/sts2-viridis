"""Hold the fully extended thrust and give both breathing wrists offset angular arcs."""
import json
import sys
from math import pi, sin
from subprocess import run
from zipfile import ZIP_DEFLATED, ZipFile

import numpy as np
from PIL import Image, ImageDraw, ImageFont

from build_guard_breath import EXPORT, FPS, NAME, ORIGIN, OUT, PERIOD, TARGET, encode, envelope
from preview_guard_thrust import Playback
from guard_breath_limbs import add_limb_motion

OUTPUT = OUT / (NAME + "-held-breath.json")
HOLD = 20/90
FULL_REACH = 23/90


def hold_pose(attack):
    tracks = attack["bones"]["grip"]["translate"]
    peak = min((k["time"] for k in tracks), key=lambda t:abs(t-FULL_REACH))
    end = tracks[-1]["time"]
    release = peak + HOLD
    for group in attack.values():
        for channels in group.values():
            for channel,keys in channels.items():
                before = [k.copy() for k in keys if k["time"] <= peak]
                after = [k.copy() for k in keys if k["time"] > peak]
                if channel != "attachment" and before and after:
                    left,right=before[-1],after[0]
                    if left["time"] == peak:
                        pose=left.copy()
                    else:
                        u=(peak-left["time"])/(right["time"]-left["time"])
                        if channel=="rgba":
                            c0=bytes.fromhex(left["color"]);c1=bytes.fromhex(right["color"])
                            pose={"color":bytes(round(a+(b-a)*u) for a,b in zip(c0,c1)).hex()}
                        else:
                            pose={k:left[k]+(right[k]-left[k])*u for k in left if k!="time"}
                        before.append(dict(pose,time=peak))
                    before.append(dict(pose,time=release))
                for key in after:
                    t=key["time"]
                    key["time"]=end if t==end else release+(t-peak)*(end-release)/(end-peak)
                keys[:]=before+after
    return peak,release,end


def set_wrist(rig,name,pivot,angle,lag,settle):
    # Move the pivot from the grip to the glove cuff, rebinding its mesh
    # coordinates so that the neutral drawing stays in exactly the same place.
    index=next(i for i,b in enumerate(rig["bones"]) if b["name"]==name)
    bone=rig["bones"][index]
    x,y=pivot[0]-ORIGIN[0],ORIGIN[1]-pivot[1]
    dx,dy=bone["x"]-x,bone["y"]-y
    bone.update(x=x,y=y)
    mesh=rig["skins"][0]["attachments"]["guard_reference"]["breathing_guard"]
    v=mesh["vertices"];offset=0
    while offset<len(v):
        count=v[offset];offset+=1
        for _ in range(count):
            if v[offset]==index:
                v[offset+1]+=dx;v[offset+2]+=dy
            offset+=4
    keys=rig["animations"]["idle_loop"]["bones"][name]["rotate"]
    for key in keys:
        t=key["time"]
        follow=sin(pi*np.clip((t-2.15)/(PERIOD-2.15),0,1))**2
        key["value"]=round(angle*envelope(t,lag)+settle*follow,6)


def package(target, archive):
    with ZipFile(EXPORT/archive,"w",ZIP_DEFLATED) as z:
        z.write(target,NAME+".json")
        for f in [OUT/(NAME+".atlas"),OUT/(NAME+".png"),*sorted((OUT/"parts").glob("*.png"))]:
            z.write(f,f.relative_to(OUT))


def deepen_breath(idle):
    # Let the rib cage lead; the head follows less and the wrists keep their lag.
    gains = {
        "breath_chest_left": (1.85, 1.5), "breath_chest_right": (1.85, 1.5),
        "breath_neck": (1.55, 1.25),
        "breath_near_shoulder": (1.75, 1.5), "breath_far_shoulder": (1.75, 1.5),
        "breath_grip": (1.55, 1.4), "breath_open_glove": (1.55, 1.4),
    }
    for bone, (travel, angle) in gains.items():
        tracks = idle["bones"][bone]
        for key in tracks["translate"]:
            key["x"] = round(key["x"] * travel, 6)
            key["y"] = round(key["y"] * travel, 6)
        for key in tracks["rotate"]:
            key["value"] = round(key["value"] * angle, 6)


def longer_extension():
    rig=json.loads(OUTPUT.read_text())
    deepen_breath(rig["animations"]["idle_loop"])
    add_limb_motion(rig)
    attack=rig["animations"]["attack"]
    authored=[k["time"] for k in attack["bones"]["grip"]["translate"]]
    launch=min(authored,key=lambda t:abs(t-12/90))
    peak=min(authored,key=lambda t:abs(t-FULL_REACH))
    release=peak+HOLD
    end=authored[-1]
    extra=7/90
    old_clock=[0,launch,peak,release,end]
    new_clock=[0,launch,peak+extra,release+extra,end]
    for group in attack.values():
        for tracks in group.values():
            for keys in tracks.values():
                for key in keys:
                    key["time"]=float(np.interp(key["time"],old_clock,new_clock))
    target=OUT/(NAME+"-long-extension.json")
    target.write_text(json.dumps(rig,separators=(",",":"))+"\n")
    package(target,"Viridis-long-extension-breath.zip")
    old,new=Playback(OUTPUT),Playback(target)
    # Compare rendered poses at corresponding phases, including the entire hold.
    for t in np.r_[np.linspace(0,end,19),peak,release,end]:
        a=np.asarray(old.frame("attack",t),float)
        b=np.asarray(new.frame("attack",np.interp(t,old_clock,new_clock)),float)
        assert np.abs(a-b).mean()<.005
    held=np.array(old.frame("attack",peak))
    for t in np.linspace(peak+extra,release+extra,15):
        assert np.array_equal(held,np.array(new.frame("attack",t)))
    idle=new.frame("attack",0)
    sheet=Image.new("RGB",(960,452),(40,49,54))
    draw=ImageDraw.Draw(sheet);font=ImageFont.truetype("msyh.ttc",18)
    for i,label in enumerate(["上一版","新版 · 出刺更舒展"]):
        draw.text((240+480*i,22),label,font=font,anchor="mm",fill="#eee9dd")
    pair=[];single=[]
    for j in range(-12,int(np.ceil(end*60))+13):
        t=j/60
        for i,player in enumerate([old,new]):
            frame=player.frame("attack",t) if 0<=t<end else idle
            sheet.paste(frame.resize((480,410),Image.Resampling.LANCZOS),(480*i,40))
            if i==1:single.append(frame.tobytes())
        draw.line((480,48,480,442),fill="#515b5f")
        pair.append(sheet.tobytes())
    for frames,size,file,codec in [
        (pair,"960x452","viridis-extension-compare.webp",["-c:v","libwebp_anim","-quality","95","-loop","0"]),
        (single,"608x520","viridis-long-extension.mp4",["-c:v","libx264","-crf","16","-pix_fmt","yuv420p","-movflags","+faststart"])]:
        run(["ffmpeg","-y","-v","error","-f","rawvideo","-pix_fmt","rgb24","-s",size,"-framerate","60",
             "-i","pipe:0",*codec,str(EXPORT/file)],input=b"".join(frames),check=True)
    print("Phase durations:",np.diff(new_clock))


def main():
    rig=json.loads(TARGET.read_text())
    peak,release,end=hold_pose(rig["animations"]["attack"])
    set_wrist(rig,"breath_grip",(159,250),-3.2,1.49,.95)
    set_wrist(rig,"breath_open_glove",(313,250),4.2,1.67,-1.35)
    OUTPUT.write_text(json.dumps(rig,separators=(",",":"))+"\n")
    package(OUTPUT,"Viridis-held-thrust-breath.zip")

    old=Playback(TARGET);player=Playback(OUTPUT)
    ref=np.array(old.frame("attack",peak))
    for t in np.linspace(peak,release,12):
        assert np.array_equal(ref,np.array(player.frame("attack",t)))
    for t in [0,.1,peak,end]:
        assert np.array_equal(np.array(old.frame("attack",t)),np.array(player.frame("attack",t)))
    for k in [8,19,30,42,58,65,75]:
        t=k/90
        warped=t if t<=peak else release+(t-peak)*(end-release)/(end-peak)
        err=abs(np.array(old.frame("attack",t),float)-np.array(player.frame("attack",warped),float))
        assert err.mean()<.005
    idle=[player.frame("idle_loop",i/FPS) for i in range(round(PERIOD*FPS))]
    rest=np.array(idle[0]);assert np.array_equal(rest,np.array(player.frame("idle_loop",PERIOD)))
    assert all(np.array_equal(rest[324:],np.array(f)[324:]) for f in idle)
    strike=[player.frame("attack",i/FPS) for i in range(int(np.ceil(end*FPS)))]
    encode(EXPORT/"viridis-held-thrust.webp",[*idle[:1]*8,*strike,*idle[:1]*8],True)
    encode(EXPORT/"viridis-breath-wrists.webp",idle,True)
    encode(EXPORT/"viridis-breath-held-thrust.mp4",[*idle,*strike,*idle],False)
    print(f"Peak {peak:.6f}, release {release:.6f}, end {end:.9f}, recovery {(end-release)/end:.1%}")

if __name__=="__main__":
    if "--longer-extension" in sys.argv:longer_extension()
    else:main()
