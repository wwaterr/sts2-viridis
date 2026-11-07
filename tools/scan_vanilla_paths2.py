import mmap, re

p = r"D:\all\steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"
f = open(p, 'rb')
m = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)

out = []
w = out.append

def find_all(kw, cap=100000):
    hits, start = [], 0
    while len(hits) < cap:
        i = m.find(kw, start)
        if i < 0:
            break
        hits.append(i); start = i + 1
    return hits

for kw in (b'scenes/characters/', b'/spine/', b'animated_character'):
    hits = find_all(kw, 2000)
    w(f"\n### {kw!r}: {len(hits)}")
    seen = []
    for off in hits[:3000]:
        seg = bytes(m[off:off + 110])
        s = ''.join(chr(b) if 32 <= b < 127 else '|' for b in seg).split('|')[0]
        if s and s not in seen:
            seen.append(s)
    for s in seen[:45]:
        w("  " + s)

open(r"D:\study\ai\viridis\refs\vanilla_scene_paths.txt", "w", encoding="utf-8").write("\n".join(out))
print("done")
m.close(); f.close()