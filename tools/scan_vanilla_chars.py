import mmap, re

p = r"D:\all\steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"
f = open(p, 'rb')
m = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)

def ctx(off, before=260, after=520):
    seg = bytes(m[max(0, off - before):off + after])
    return ''.join(chr(b) if 32 <= b < 127 or b == 10 else '.' for b in seg)

out = []
w = out.append

def find_all(kw, cap=20000):
    hits, start = [], 0
    while len(hits) < cap:
        i = m.find(kw, start)
        if i < 0:
            break
        hits.append(i); start = i + 1
    return hits

hits = find_all(b'animations/characters')
w(f"### animations/characters hits: {len(hits)}")
uniq = []
for off in hits[:400]:
    seg = bytes(m[off:off + 120])
    s = ''.join(chr(b) if 32 <= b < 127 else '.' for b in seg)
    s = s.split('.')[0]
    if s and s not in uniq:
        uniq.append(s)
for s in uniq[:40]:
    w("  " + s)

for kw in (b'preview_animation', b'default_animation'):
    h = find_all(kw, 20)
    w(f"\n### {kw!r} hits: {len(h)}")
    for off in h[:4]:
        w(f"--- @{off} ---")
        w(ctx(off))

w("\n### character tscn candidates")
h = find_all(b'/characters/', 500)
w(f"hits {len(h)}")
for off in h[:6]:
    w(f"--- @{off} ---")
    w(ctx(off, 120, 360))

open(r"D:\study\ai\viridis\refs\vanilla_character_paths.txt", "w", encoding="utf-8").write("\n".join(out))
print("done")
m.close(); f.close()