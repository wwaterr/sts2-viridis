import mmap, re

p = r"D:\all\steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"
f = open(p, 'rb')
m = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)

out = []
w = out.append

def ctx(off, before=300, after=500):
    seg = bytes(m[max(0, off - before):off + after])
    return ''.join(chr(b) if 32 <= b < 127 or b == 10 else '.' for b in seg)

for kw in (b'"rarity"', b'"Rarity"', b'rarity:', b'"pool_id"'):
    hits, start = [], 0
    while len(hits) < 5000:
        i = m.find(kw, start)
        if i < 0:
            break
        hits.append(i); start = i + 1
    w(f"\n### {kw!r}: {len(hits)} hits")
    for off in hits[:3]:
        w(f"--- @{off} ---")
        w(ctx(off))

open(r"D:\study\ai\viridis\refs\raw\card_data_probe.txt", "w", encoding="utf-8").write("\n".join(out))
print("done, hits written")
m.close(); f.close()