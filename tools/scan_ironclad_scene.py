from sts2_paths import game_pack, repo_path

import mmap, re

p = game_pack()
f = open(p, 'rb')
m = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)

def text(off, length):
    seg = bytes(m[off:off + length])
    return ''.join(chr(b) if 32 <= b < 127 or b == 10 else '.' for b in seg)

out = []
w = out.append

start = 462198000
region = text(start, 12000)
w("===== ironclad character scene region =====")
w(region)

names = re.findall(r'"([A-Za-z0-9_\-]{3,28})"', region)
w("\n===== quoted strings (candidates: animation names) =====")
w(", ".join(sorted(set(names))))

open(repo_path("refs/vanilla_ironclad_scene.txt"), "w", encoding="utf-8").write("\n".join(out))
print("done")
m.close(); f.close()