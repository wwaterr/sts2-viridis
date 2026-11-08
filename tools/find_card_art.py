import struct, sys, re
from collections import Counter

P = r"D:\all\steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"

f = open(P, "rb")
h = f.read(112)
fb, do = struct.unpack_from("<QQ", h, 24)
f.seek(do)
(n,) = struct.unpack("<I", f.read(4))
f.seek(do + 4)
e = {}
for _ in range(n):
    (pl,) = struct.unpack("<I", f.read(4))
    raw = f.read(pl)
    name = raw.split(b"\x00")[0].decode("utf-8", "replace")
    ofs, size = struct.unpack("<QQ", f.read(16))
    f.read(20)
    e[name] = (fb + ofs, size)


def dims(name):
    ofs, size = e[name]
    f.seek(ofs)
    d = f.read(60)
    if d[:4] != b"GST2":
        return None
    fl = struct.unpack_from("<14I", d, 4)
    return (fl[1], fl[2], fl[11], size)


pat = sys.argv[1] if len(sys.argv) > 1 else r"cards/"
rx = re.compile(pat, re.I)
hits = [nm for nm in e if rx.search(nm) and nm.endswith(".ctex")]
print(f"ctex hits: {len(hits)}")

counter = Counter()
examples = {}
for nm in hits:
    d = dims(nm)
    if not d:
        continue
    key = (d[0], d[1], d[2])
    counter[key] += 1
    examples.setdefault(key, nm)

print("\n尺寸分布（宽x高 fmt 数量）:")
for (w, hh, fmt), c in counter.most_common(15):
    print(f"  {w:5d} x {hh:5d}  fmt={fmt:2d}  x{c:4d}   e.g. {examples[(w, hh, fmt)]}")

# 源 png 路径样例
print("\n源 png 样例:")
for nm in hits[:1]:
    pass
imgs = [nm for nm in e if nm.endswith(".png.import") and rx.search(nm)]
for nm in imgs[:6]:
    ofs, size = e[nm]
    f.seek(ofs)
    txt = f.read(size).decode("utf-8", "replace")
    m = re.search(r'path="res://([^"]+)"', txt)
    vram = "vram_texture=true" in txt
    ctex = m.group(1).replace("res://", "") if m else None
    d = dims(ctex) if ctex in e else None
    print(f"  {nm[:-7]:62s} {d[0]}x{d[1]} fmt={d[2]} vram={vram}")
