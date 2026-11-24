"""Retime the approved keyed thrust. Landmarks are 60 fps frame indices."""
from copy import deepcopy
import json
from subprocess import run
from zipfile import ZipFile, ZIP_DEFLATED

import numpy as np
from PIL import Image, ImageDraw, ImageFont
from preview_keyed_thrust import OUT, EXPORT, BG, Player

BASE = "viridis-keyed-thrust"
NEW = BASE + "-quick"
OLD_FRAMES = [0, 6, 13, 19, 27, 39, 49, 60]
NEW_FRAMES = [0, 3, 7, 11, 25, 46, 51, 60]


def main():
    rig = json.loads((OUT / (BASE + ".json")).read_text())
    result = deepcopy(rig)
    attack = result["animations"]["attack"]
    for group in ("bones", "slots"):
        for tracks in attack[group].values():
            for keys in tracks.values():
                for key in keys:
                    key["time"] = round(float(np.interp(key["time"] * 60, OLD_FRAMES, NEW_FRAMES)) / 60, 7)
    target = OUT / (NEW + ".json")
    target.write_text(json.dumps(result, separators=(",", ":")) + "\n")
    with ZipFile(EXPORT / (NEW + ".zip"), "w", ZIP_DEFLATED) as z:
        z.write(target, BASE + ".json")
        for f in [OUT / (BASE + ".atlas"), OUT / (BASE + ".png"), *sorted((OUT / "parts").glob("*.png"))]:
            z.write(f, f.relative_to(OUT))
    old, new = Player(), Player(target)
    # Corresponding key poses must still match with their active skins.
    for a, b in zip(OLD_FRAMES, NEW_FRAMES):
        delta = abs(np.asarray(old.pose(a), dtype=float) - np.asarray(new.pose(b)))
        assert delta.mean() < .01, (a, b, delta.mean())
    title = ImageFont.truetype("msyh.ttc", 20)
    frames = []
    for frame in range(60):
        pair = Image.new("RGB", (1120, 540), BG)
        draw = ImageDraw.Draw(pair)
        for i, (player, label) in enumerate([(old, "上一版"), (new, "快出慢收")]):
            draw.text((280 + 560*i, 23), label, fill="#eee9dd", font=title, anchor="mm")
            pair.paste(player.pose(frame).resize((560, 492), Image.Resampling.LANCZOS), (560*i, 45))
        draw.line((560, 57, 560, 526), fill="#515b5f")
        frames.append(pair.tobytes())
    pixels = b"".join(frames)
    for ext, codec in [("webp", ["-c:v", "libwebp_anim", "-quality", "93", "-loop", "0"]),
                       ("mp4", ["-c:v", "libx264", "-crf", "16", "-pix_fmt", "yuv420p", "-movflags", "+faststart"])]:
        run(["ffmpeg", "-v", "error", "-y", "-f", "rawvideo", "-pixel_format", "rgb24", "-video_size", "1120x540",
             "-framerate", "60", "-i", "pipe:0", *codec, str(EXPORT / ("viridis-thrust-timing." + ext))], input=pixels, check=True)


if __name__ == "__main__":
    main()
