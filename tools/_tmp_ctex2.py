import struct, sys

P = sys.argv[1]

f = open(P, "rb")
h = f.read(112)
file_base, dir_off = struct.unpack_from("<QQ", h, 24)
f.seek(dir_off)
(count,) = struct.unpack("<I", f.read(4))
f.seek(dir_off + 4)
ents = {}
for _ in range(count):
    (plen,) = struct.unpack("<I", f.read(4))
    raw = f.read(plen)
    name = raw.split(b"\x00")[0].decode("utf-8", "replace")
    ofs, size = struct.unpack("<QQ", f.read(16))
    f.read(20)
    ents[name] = (file_base + ofs, size)

ctex = [n for n in ents if n.endswith(".ctex")]
for name in ctex[:3]:
    ofs, size = ents[name]
    f.seek(ofs)
    d = f.read(min(size, 4096))
    riff = d.find(b"RIFF")
    print(name, "size=", size, "RIFF@", riff)
    print("  first 32 bytes hex:", d[:32].hex())
    magic = d[:4]
    fields = struct.unpack_from("<6I", d, 4)
    print("  magic", magic, "ver,w,h,flags,mip,fmt =", fields)
    if riff > 0:
        print("  bytes 24..riff:", d[24:riff].hex())
        print("  webp tag:", d[riff:riff+16])