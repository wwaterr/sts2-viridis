from sts2_paths import workshop_root

import mmap, struct, os, sys

p = workshop_root() / "3815352089/ThirtySevenCharacterMod.pck"
key = b"res://ThirtySevenCharacterModCode/Cards/Acceptance.cs"

f = open(p, 'rb')
m = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)
print("file size:", len(m))
print("header:", bytes(m[:8]).hex())

i = m.find(key)
print("found at", i, "path_len", struct.unpack('<I', m[i-4:i])[0])

after = i + len(key)
# candidate layout A: offset(8) size(8) md5(16) flags(4)
for name, skip in (("A: off,size,md5", 0), ("B: md5,off,size", 16)):
    base = after + skip
    off, size = struct.unpack('<QQ', m[base:base+16])
    ok = 0 < size < len(m) and 0 < off < len(m)
    sample = bytes(m[off:off+40]) if ok else b''
    print(f"layout {name}: offset={off} size={size} plausible={ok} sample={sample[:40]!r}")

# scan for the actual card text anywhere (maybe stored uncompressed)
j = m.find(b"namespace ThirtySeven")
print("namespace hit:", j)
if j > 0:
    print(bytes(m[j-200:j+1500]).decode('utf-8', 'replace'))
m.close(); f.close()