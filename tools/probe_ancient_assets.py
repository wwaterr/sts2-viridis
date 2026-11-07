import mmap

p = r"D:\all\steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"
f = open(p, 'rb')
m = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)

out = []
for kw in (b'ancient', b'Ancient'):
    hits, start = [], 0
    while True:
        i = m.find(kw, start)
        if i < 0:
            break
        hits.append(i)
        start = i + 1
    out.append(f"### {kw!r}: {len(hits)} hits")
    seen = set()
    for off in hits:
        seg = bytes(m[max(0, off - 60):off + 60])
        txt = ''.join(chr(b) if 32 <= b < 127 else '.' for b in seg)
        if txt in seen:
            continue
        seen.add(txt)
        if len(seen) <= 40:
            out.append(txt)

open(r"D:\study\ai\viridis\refs\raw\ancient_asset_probe.txt", "w", encoding="utf-8").write("\n".join(out))
print("ok")
m.close(); f.close()