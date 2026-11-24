from sts2_paths import game_pack

import struct, sys, os
p = game_pack()
f = open(p, "rb")
h = f.read(112)
magic, ver, a, b, c, flags = struct.unpack_from("<IIIIII", h, 0)
file_base, dir_off = struct.unpack_from("<QQ", h, 24)
f.seek(dir_off)
(count,) = struct.unpack("<I", f.read(4))
f.seek(dir_off + 4)
ents = []
for _ in range(count):
    (plen,) = struct.unpack("<I", f.read(4))
    raw = f.read(plen)
    name = raw.split(b"\x00")[0].decode("utf-8", "replace")
    ofs, size = struct.unpack("<QQ", f.read(16))
    f.read(20)
    ents.append((name, ofs, size))
print("magic ok:", magic == 0x43504447, "files:", count, "dir@", dir_off, "size", os.path.getsize(p))
loc = [e for e in ents if "localization" in e[0].lower()]
print("localization entries:", len(loc))
from collections import Counter
top = Counter(e[0].split("/")[0] for e in loc)
print("top dirs:", top.most_common(8))
for e in sorted(loc)[:40]:
    print(f"  {e[2]:>8}  {e[0]}")
