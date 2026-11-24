from sts2_paths import game_pack

import struct, sys, os, re

P = game_pack()
PATTERN = sys.argv[1] if len(sys.argv) > 1 else r"character"

f = open(P, "rb")
h = f.read(112)
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

rx = re.compile(PATTERN, re.I)
hits = [e for e in ents if rx.search(e[0])]
print(f"match {len(hits)} / {count}")
for e in sorted(hits)[:120]:
    print(f"  {e[2]:>9}  {e[0]}")

# 取第一个命中项的头部字节，判断格式
if hits and len(sys.argv) > 2:
    name, ofs, size = sorted(hits)[0]
    f.seek(file_base + ofs)
    data = f.read(min(size, 64))
    print("\nfirst bytes of", name, ":", data[:16].hex())