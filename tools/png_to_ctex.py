"""把 PNG 转成 Godot 的 .ctex（CompressedTexture2D），供 mod pck 直接加载。

为什么需要：游戏是导出后的 Godot 工程，运行时不带 PNG 导入器，
`res://xxx.png` 会报 "No loader found for resource"，mod 里必须放已导入的 .ctex。

格式来自实际 mod pck 反推（figure_Saya / RegentFX / Koishi 的贴图），
56 字节头 + 无损 WebP 位流：

    GST2                     magic
    1                        版本
    w, h
    0x0d000000               flags
    0xffffffff               mipmap limit
    0, 0, 0                  保留
    2                        数据格式 = WebP
    (h << 16) | w            尺寸紧凑表示
    0
    5                        Image::FORMAT_RGBA8
    len(webp)                第 0 级数据长度
    <webp 字节>              RIFF....WEBPVP8L（无损）

用法：
    python tools/png_to_ctex.py <输入目录或文件> [...]
不传参数时处理 mods/Viridis/art_source/characters/viridis/ → assets/characters/viridis/
"""

from __future__ import annotations

import io
import struct
import sys
from pathlib import Path

from PIL import Image

ROOT = Path(__file__).resolve().parent.parent
SOURCE = ROOT / "mods" / "Viridis" / "art_source" / "characters" / "viridis"
TARGET = ROOT / "mods" / "Viridis" / "assets" / "characters" / "viridis"

WEBP_DATA_FORMAT = 2
FLAGS = 0x0D000000
FORMAT_RGBA8 = 5


def encode_webp_lossless(img: Image.Image) -> bytes:
    buf = io.BytesIO()
    img.save(buf, format="WEBP", lossless=True, quality=100, method=6, exact=True)
    return buf.getvalue()


def png_to_ctex(src: Path, dst: Path) -> tuple[int, int, int]:
    with Image.open(src) as im:
        rgba = im.convert("RGBA")
        w, h = rgba.size
        webp = encode_webp_lossless(rgba)

    header = struct.pack(
        "<4sI I I I I I I I I I I I I",
        b"GST2",
        1,  # 版本
        w,
        h,
        FLAGS,
        0xFFFFFFFF,  # mipmap limit
        0,
        0,
        0,
        WEBP_DATA_FORMAT,
        (h << 16) | w,
        0,
        FORMAT_RGBA8,
        len(webp),
    )
    assert len(header) == 56, len(header)

    dst.parent.mkdir(parents=True, exist_ok=True)
    dst.write_bytes(header + webp)
    return w, h, len(header) + len(webp)


def main(argv: list[str]) -> int:
    if argv:
        jobs: list[tuple[Path, Path]] = []
        for arg in argv:
            p = Path(arg)
            if p.is_dir():
                for f in sorted(p.rglob("*.png")):
                    jobs.append((f, TARGET / (f.stem + ".ctex")))
            else:
                jobs.append((p, TARGET / (p.stem + ".ctex")))
    else:
        if not SOURCE.is_dir():
            print(f"找不到源目录: {SOURCE}")
            return 1
        jobs = [(f, TARGET / (f.stem + ".ctex")) for f in sorted(SOURCE.glob("*.png"))]

    if not jobs:
        print("没有 PNG 需要转换")
        return 1

    for src, dst in jobs:
        w, h, size = png_to_ctex(src, dst)
        print(f"{src.name:34s} -> {dst.name:34s} {w}x{h}  {size} bytes")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))