"""Export the 1.5x attack from the guard-matched Spine rig and render both clocks."""
import json
from math import ceil
from subprocess import run
from zipfile import ZipFile, ZIP_DEFLATED

import numpy as np
from PIL import Image, ImageDraw, ImageFont

from preview_guard_thrust import BG, EXPORT, NAME, OUT, SOURCE, Playback

SPEED = 1.5
FPS = 60


def main():
    original = Playback()
    fast = Playback()
    attack = fast.rig["animations"]["attack"]
    end = 0.
    for group in attack.values():
        for tracks in group.values():
            for keys in tracks.values():
                for key in keys:
                    assert "curve" not in key  # These authored tracks are linear or stepped.
                    key["time"] /= SPEED
                    end = max(end, key["time"])
    target = OUT / (NAME + "-1p5x.json")
    target.write_text(json.dumps(fast.rig, separators=(",", ":")) + "\n")
    fast.rig = json.loads(target.read_text())
    with ZipFile(EXPORT / "Viridis-guard-thrust-1p5x.zip", "w", ZIP_DEFLATED) as z:
        z.write(target, NAME + ".json")
        for f in [OUT/(NAME+".atlas"), OUT/(NAME+".png"), *sorted((OUT/"parts").glob("*.png"))]:
            z.write(f, f.relative_to(OUT))

    idle = fast.frame("idle_loop", 0)
    source = Image.open(SOURCE).convert("RGBA")
    reference = Image.new("RGB", source.size, BG)
    reference.paste(source, mask=source.getchannel("A"))
    for pose in [idle, fast.frame("attack", 0), fast.frame("attack", end)]:
        assert np.array_equal(np.array(reference), np.array(pose))

    for k in [8, 12, 19, 40, 58, 65, 79]:
        a = np.asarray(original.frame("attack", k/FPS), dtype=float)
        b = np.asarray(fast.frame("attack", k/(FPS*SPEED)), dtype=float)
        assert np.max(np.abs(a-b)) <= 1, k

    duration = end*SPEED
    side = (480, 410)
    pair = Image.new("RGB", (960,452), BG)
    pen = ImageDraw.Draw(pair)
    font = ImageFont.truetype("C:/Windows/Fonts/msyh.ttc", 18)
    for i,title in enumerate(["动骨原版 · 1×", "整体提速 · 1.5×"]):
        pen.text((240+480*i,22), title, fill="#eee9dd", font=font, anchor="mm")
    timed = []
    single = []
    for j in range(-15,ceil(duration*FPS)+16):
        t = j/FPS
        old = original.frame("attack", t) if 0<=t<duration else idle
        new = fast.frame("attack", t) if 0<=t<end else idle
        for i,img in enumerate([old, new]):
            pair.paste(img.resize(side, Image.Resampling.LANCZOS), (i*480,40))
        pen.line((480,49,480,442), fill="#515b5f")
        timed.append(pair.tobytes())
        if j<ceil(end*FPS)+16: single.append(new.tobytes())
    for frames,size,file,codec in [
        (timed, "960x452", "viridis-guard-1p5x-compare.webp", ["-c:v","libwebp_anim","-quality","94","-loop","0"]),
        (single, "608x520", "viridis-guard-1p5x.mp4", ["-c:v","libx264","-crf","16","-pix_fmt","yuv420p","-movflags","+faststart"])
    ]:
        run(["ffmpeg","-v","error","-y","-f","rawvideo","-pix_fmt","rgb24","-s",size,
             "-framerate",str(FPS),"-i","pipe:0",*codec,str(EXPORT/file)], input=b"".join(frames),check=True)
    print(f"attack {duration:.6f}s -> {end:.6f}s")


if __name__ == "__main__":
    main()
