import struct, sys, re, hashlib

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

print("files:", count)


def head(name, n):
    ofs, size = ents[name]
    f.seek(ofs)
    return f.read(n)


ctex = [n for n in ents if n.endswith(".ctex")]
print("ctex files:", len(ctex))

# 找 non-VRAM 的 import（metadata 里 vram_texture false 或没有 .s3tc 后缀）
imports = [n for n in ents if n.endswith(".png.import")]
print("png.import:", len(imports))
for imp in imports[:6]:
    print("\nIMPORT", imp)
    print(head(imp, 400).decode("utf-8", "replace"))

print("\n--- ctex headers ---")
for name in ctex[:8]:
    ofs, size = ents[name]
    d = head(name, 40)
    if d[:4] != b"GST2":
        print(name, "not GST2", d[:8].hex())
        continue
    ver, w, hh, flags, mip, fmt = struct.unpack_from("<6I", d, 4)
    print(f"{name}\n   {w}x{hh} ver={ver} flags=0x{flags:x} mip=0x{mip:08x} fmt={fmt} size={size}")
    print("   payload[28:40]:", d[28:40])