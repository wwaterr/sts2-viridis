from sts2_paths import game_pack, repo_path

import mmap, re, sys

p = game_pack()
f = open(p, 'rb')
m = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)
print("size", m.size())

def ctx(off, before=200, after=420):
    seg = bytes(m[max(0, off - before):off + after])
    return ''.join(chr(b) if 32 <= b < 127 or b == 10 else '.' for b in seg)

out = []
w = out.append

for kw in (b'.skel', b'.atlas', b'spine_godot_extension'):
    hits = []
    start = 0
    while True:
        i = m.find(kw, start)
        if i < 0 or len(hits) >= 200000:
            break
        hits.append(i)
        start = i + 1
        if len(hits) >= 50000:
            break
    w(f"\n### {kw!r}: {len(hits)} hits (capped), first 10: {hits[:10]}")
    for off in hits[:3]:
        w(f"--- @{off} ---")
        w(ctx(off))

# version-ish strings near atlas hits
w("\n### version candidates near first .atlas hits")
for off in [x for x in re.finditer(rb'\d\.\d\.\d+', bytes(m[:2000000]))][:0]:
    pass

open(repo_path("refs/vanilla_spine_scan.txt"), "w", encoding="utf-8").write("\n".join(out))
print("written", len(out), "lines")
m.close(); f.close()