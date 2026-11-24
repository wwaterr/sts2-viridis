"""把 SlayTheSpire2.pck 里的美术资源按分类抽成 PNG，存到工作区根目录。

用法:  python extract_sts2_art.py [pck路径] [输出根目录]
省略 pck 路径时，从 STS2_GAME_PCK 读取。

原理: Godot 4.5 打包，目录表在文件尾部（不是标准布局）。
      纹理是 .ctex 容器，其中 2311 个内部嵌了无损 WebP(RIFF/WEBP)，
      直接抠出来即可解码；另有 1244 个是 VRAM 压缩(.s3tc/.bptc)，
      本脚本只登记不解码（需要 texconv / gdsdecomp）。
"""
import io
import json
import os
import re
import struct
import sys

from PIL import Image

from tools.sts2_paths import game_pack

DEFAULT_OUT = os.path.dirname(os.path.abspath(__file__))

# 分类规则：按源路径前缀归到根目录下的文件夹（长前缀优先）
CATEGORIES = [
    ("images/packed/card_portraits/", "cards"),
    ("images/timeline/epoch_portraits/", "epoch"),
    ("images/relics/", "relics"),
    ("images/potions/", "potions"),
    ("images/powers/", "powers"),
    ("images/events/", "events"),
    ("images/rooms/", "rooms"),
    ("images/ancients/", "ancients"),
    ("images/monsters/", "monsters"),
    ("images/vfx/", "vfx"),
    ("images/ui/", "ui"),
    ("images/map/", "map"),
    ("images/atlases/", "atlases"),
    ("images/packed/", "packed_misc"),
    ("images/", "misc"),
    ("animations/", "spine_sheets"),
]


def load_pck_index(fh):
    fh.seek(24)
    base = struct.unpack("<Q", fh.read(8))[0]
    fh.seek(32)
    dir_off = struct.unpack("<Q", fh.read(8))[0]
    fh.seek(dir_off)
    (count,) = struct.unpack("<I", fh.read(4))
    idx = {}
    for _ in range(count):
        (plen,) = struct.unpack("<I", fh.read(4))
        path = fh.read(plen).rstrip(b"\x00").decode("utf-8", "replace")
        off, size = struct.unpack("<QQ", fh.read(16))
        fh.read(16)  # md5
        fh.read(4)   # flags
        idx[path] = (off, size)
    return base, idx


def categorize(src_path):
    for prefix, cat in CATEGORIES:
        if src_path.startswith(prefix):
            return cat, src_path[len(prefix):]
    return "other", src_path


def main():
    pck = sys.argv[1] if len(sys.argv) > 1 else game_pack()
    out_root = sys.argv[2] if len(sys.argv) > 2 else DEFAULT_OUT

    fh = open(pck, "rb")
    base, idx = load_pck_index(fh)

    def read(key):
        off, size = idx[key]
        fh.seek(base + off)
        return fh.read(size)

    # .import 存根里带有 "path=" 指向真正的 .ctex，用它还原原始资源路径
    src_of = {}
    for key in idx:
        if not key.endswith(".import"):
            continue
        try:
            txt = read(key).decode("utf-8", "replace")
        except Exception:
            continue
        m = re.search(r'path="res://([^"]+\.ctex)"', txt)
        if m and m.group(1) in idx:
            src_of[m.group(1)] = key[: -len(".import")]

    stats = {}
    skipped = []
    for ctex, src in sorted(src_of.items()):
        if not ctex.endswith(".ctex"):
            continue
        cat, rel = categorize(src)
        data = read(ctex)
        pos = data.find(b"RIFF")
        if pos < 0 or data[pos + 8:pos + 12] != b"WEBP":
            skipped.append((cat, src))
            continue
        (riff_len,) = struct.unpack("<I", data[pos + 4:pos + 8])
        try:
            im = Image.open(io.BytesIO(data[pos:pos + 8 + riff_len]))
            im.load()
        except Exception:
            skipped.append((cat, src))
            continue
        # 源扩展名可能是 .svg/.tres，统一输出 .png
        dest = os.path.join(out_root, cat, os.path.splitext(rel)[0] + ".png")
        os.makedirs(os.path.dirname(dest), exist_ok=True)
        im.save(dest)
        st = stats.setdefault(cat, {"files": 0, "bytes": 0})
        st["files"] += 1
        st["bytes"] += os.path.getsize(dest)

    # Spine 图集（分件清单）：纯文本 atlas + 索引
    atlas_dir = os.path.join(out_root, "spine_atlas")
    rows = []
    for key in sorted(idx):
        if not key.endswith(".spatlas"):
            continue
        try:
            j = json.loads(read(key).decode("utf-8"))
        except Exception:
            continue
        src = j.get("source_path", "").replace("res://", "")
        if not src:
            continue
        dest = os.path.join(atlas_dir, src[: -len(".atlas")] + ".atlas")
        os.makedirs(os.path.dirname(dest), exist_ok=True)
        with open(dest, "w", encoding="utf-8") as o:
            o.write(j.get("atlas_data", ""))
        head = j.get("atlas_data", "").split("\n")
        size = next((l[5:] for l in head if l.startswith("size:")), "?")
        regions = sum(1 for l in head if l.startswith("bounds:"))
        rows.append((src, size, regions))
    if rows:
        os.makedirs(atlas_dir, exist_ok=True)
        with open(os.path.join(atlas_dir, "_index.tsv"), "w", encoding="utf-8") as o:
            o.write("source_path\tsheet_size\tregions\n")
            for r in rows:
                o.write(f"{r[0]}\t{r[1]}\t{r[2]}\n")

    print("== extracted ==")
    total = 0
    for cat in sorted(stats):
        s = stats[cat]
        total += s["files"]
        print(f"  {cat:14s} {s['files']:5d} files  {s['bytes'] / 1048576:7.1f} MB")
    print(f"  {'TOTAL':14s} {total:5d} files  {sum(v['bytes'] for v in stats.values()) / 1048576:7.1f} MB")
    print(f"\n== skipped (VRAM-compressed, needs texconv): {len(skipped)} ==")
    per = {}
    for cat, _ in skipped:
        per[cat] = per.get(cat, 0) + 1
    for cat, n in sorted(per.items(), key=lambda kv: -kv[1]):
        print(f"  {cat:14s} {n:5d}")
    print(f"\n== spine atlases: {len(rows)} -> {atlas_dir}")


if __name__ == "__main__":
    main()