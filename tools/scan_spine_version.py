from sts2_paths import game_pack, repo_path

import mmap, re

p = game_pack()
f = open(p, 'rb')
m = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)

def ctx(off, before=180, after=220):
    seg = bytes(m[max(0, off - before):off + after])
    return ''.join(chr(b) if 32 <= b < 127 or b == 10 else '.' for b in seg)

out = []
w = out.append

for kw in (b'4.2.36', b'spine_version', b'SpineSkeletonFileResource'):
    hits, start = [], 0
    while len(hits) < 3000:
        i = m.find(kw, start)
        if i < 0:
            break
        hits.append(i); start = i + 1
    w(f"\n### {kw!r}: {len(hits)}")
    for off in hits[:3]:
        w(f"--- @{off} ---")
        w(ctx(off))

open(repo_path("refs/spine_version.txt"), "w", encoding="utf-8").write("\n".join(out))
print("done")
m.close(); f.close()