"""生成小松绿的占位美术（角色选人肖像 / 图标 / 地图标记 / 背景）。

跑法：
    python tools/gen_character_art.py

输出到 mods/Viridis/assets/characters/viridis/，替换同名文件即可换图。
"""

import math
import os

from PIL import Image, ImageDraw, ImageFilter

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "mods", "Viridis", "assets", "characters", "viridis")

GREEN_DARK = (18, 42, 32)
GREEN = (46, 110, 82)
GREEN_LIGHT = (122, 200, 154)
CREAM = (232, 240, 226)


def leaf(draw, cx, cy, length, width, angle, color):
    """画一片叶子：两段圆弧拼成柳叶形。"""
    rad = math.radians(angle)
    dx, dy = math.cos(rad), math.sin(rad)
    px, py = -dy, dx
    pts = []
    steps = 24
    for i in range(steps + 1):
        t = i / steps
        x = cx + dx * length * (t - 0.5) + px * width * math.sin(math.pi * t) * 0.5
        y = cy + dy * length * (t - 0.5) + py * width * math.sin(math.pi * t) * 0.5
        pts.append((x, y))
    for i in range(steps, -1, -1):
        t = i / steps
        x = cx + dx * length * (t - 0.5) - px * width * math.sin(math.pi * t) * 0.5
        y = cy + dy * length * (t - 0.5) - py * width * math.sin(math.pi * t) * 0.5
        pts.append((x, y))
    draw.polygon(pts, fill=color)


def silhouette(size, color, scale=1.0, glow=False):
    """一个抽象的"松绿"剪影：头部圆 + 肩线 + 头顶枝叶。"""
    w, h = size
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    cx = w / 2
    head_r = h * 0.105 * scale
    head_y = h * 0.30
    d.ellipse([cx - head_r, head_y - head_r, cx + head_r, head_y + head_r], fill=color)

    # 肩 / 身体
    body_top = head_y + head_r * 0.9
    body_w = w * 0.34 * scale
    d.polygon(
        [
            (cx - body_w, h * 1.0),
            (cx - body_w * 0.72, body_top + h * 0.06),
            (cx - head_r * 0.55, body_top),
            (cx + head_r * 0.55, body_top),
            (cx + body_w * 0.72, body_top + h * 0.06),
            (cx + body_w, h * 1.0),
        ],
        fill=color,
    )

    # 头顶枝叶
    for ang, ln in ((-42, 0.20), (-12, 0.24), (18, 0.22), (48, 0.18)):
        leaf(
            d,
            cx,
            head_y - head_r * 0.55,
            h * ln * scale,
            h * 0.055 * scale,
            ang - 90,
            color,
        )

    if glow:
        img = img.filter(ImageFilter.GaussianBlur(radius=h * 0.012))
        base = Image.new("RGBA", (w, h), (0, 0, 0, 0))
        base.alpha_composite(img)
        sharp = Image.new("RGBA", (w, h), (0, 0, 0, 0))
        d2 = ImageDraw.Draw(sharp)
        silhouette_into(d2, size, color, scale)
        base.alpha_composite(sharp)
        img = base

    return img


def silhouette_into(d, size, color, scale=1.0):
    w, h = size
    cx = w / 2
    head_r = h * 0.105 * scale
    head_y = h * 0.30
    d.ellipse([cx - head_r, head_y - head_r, cx + head_r, head_y + head_r], fill=color)
    body_top = head_y + head_r * 0.9
    body_w = w * 0.34 * scale
    d.polygon(
        [
            (cx - body_w, h * 1.0),
            (cx - body_w * 0.72, body_top + h * 0.06),
            (cx - head_r * 0.55, body_top),
            (cx + head_r * 0.55, body_top),
            (cx + body_w * 0.72, body_top + h * 0.06),
            (cx + body_w, h * 1.0),
        ],
        fill=color,
    )
    for ang, ln in ((-42, 0.20), (-12, 0.24), (18, 0.22), (48, 0.18)):
        leaf(d, cx, head_y - head_r * 0.55, h * ln * scale, h * 0.055 * scale, ang - 90, color)


def vgrad(size, top, bottom):
    w, h = size
    img = Image.new("RGB", (w, h))
    d = ImageDraw.Draw(img)
    for y in range(h):
        t = y / max(1, h - 1)
        c = tuple(int(top[i] + (bottom[i] - top[i]) * t) for i in range(3))
        d.line([(0, y), (w, y)], fill=c)
    return img


def make_select_icon(path, locked=False):
    w, h = 512, 768
    bg = vgrad((w, h), (26, 56, 44), (10, 24, 18)).convert("RGBA")
    # 背光
    halo = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    hd = ImageDraw.Draw(halo)
    hd.ellipse([w * 0.12, h * 0.10, w * 0.88, h * 0.72], fill=(*GREEN_LIGHT, 60))
    bg.alpha_composite(halo.filter(ImageFilter.GaussianBlur(40)))

    body_color = (70, 78, 74, 255) if locked else (*GREEN_LIGHT, 255)
    bg.alpha_composite(silhouette((w, h), body_color, 1.0))

    # 底部渐隐
    fade = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    fd = ImageDraw.Draw(fade)
    for y in range(int(h * 0.62), h):
        t = (y - h * 0.62) / (h * 0.38)
        fd.line([(0, y), (w, y)], fill=(8, 20, 15, int(220 * t)))
    bg.alpha_composite(fade)

    if locked:
        bg = bg.convert("L").convert("RGBA")
    bg.convert("RGB").save(path)


def make_bg(path):
    w, h = 1920, 1080
    img = vgrad((w, h), (16, 44, 34), (6, 14, 11)).convert("RGBA")
    d = ImageDraw.Draw(img)

    # 远景树影
    for i, x in enumerate(range(-100, w + 200, 190)):
        top = h * (0.28 + 0.05 * ((i * 37) % 5) / 5)
        d.polygon(
            [(x, h), (x + 120, h), (x + 60, top), (x + 20, top + 60)],
            fill=(22, 58, 44, 200),
        )

    # 光斑
    glow = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    gd = ImageDraw.Draw(glow)
    for i in range(26):
        cx = (i * 631) % w
        cy = (i * 271) % h
        r = 12 + (i * 7) % 26
        gd.ellipse([cx - r, cy - r, cx + r, cy + r], fill=(*GREEN_LIGHT, 40))
    img.alpha_composite(glow.filter(ImageFilter.GaussianBlur(9)))
    img.convert("RGB").save(path)


def make_icon(path, size=128, outline=False):
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    r = size * 0.46
    if outline:
        d.ellipse([size / 2 - r, size / 2 - r, size / 2 + r, size / 2 + r], outline=GREEN_LIGHT, width=max(2, size // 26))
        d.rounded_rectangle(
            [size * 0.3, size * 0.14, size * 0.7, size * 0.86],
            radius=size * 0.1,
            outline=GREEN_LIGHT,
            width=max(2, size // 26),
        )
    else:
        d.ellipse([size / 2 - r, size / 2 - r, size / 2 + r, size / 2 + r], fill=GREEN_DARK)
        silhouette_into(d, (size, size), (*GREEN_LIGHT, 255), 1.25)
    img.save(path)


def make_marker(path):
    size = 96
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.polygon(
        [(size * 0.5, size * 0.02), (size * 0.98, size * 0.5), (size * 0.5, size * 0.98), (size * 0.02, size * 0.5)],
        fill=GREEN,
        outline=CREAM,
    )
    silhouette_into(d, (size, size), (*CREAM, 255), 1.5)
    img.save(path)


def main():
    os.makedirs(OUT, exist_ok=True)
    make_select_icon(os.path.join(OUT, "character_select_icon.png"))
    make_select_icon(os.path.join(OUT, "character_select_icon_locked.png"), locked=True)
    make_bg(os.path.join(OUT, "character_select_bg.png"))
    make_icon(os.path.join(OUT, "character_icon.png"))
    make_icon(os.path.join(OUT, "character_icon_outline.png"), outline=True)
    make_marker(os.path.join(OUT, "map_marker.png"))

    for name in sorted(os.listdir(OUT)):
        p = os.path.join(OUT, name)
        with Image.open(p) as im:
            print(f"{name:34s} {im.size[0]}x{im.size[1]}  {os.path.getsize(p)} bytes")


if __name__ == "__main__":
    main()