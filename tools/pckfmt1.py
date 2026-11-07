import struct
p = r"D:\all\steam\steamapps\workshop\content\2868840\3749627968\RestSiteFondleCursorMod.pck"
d = open(p, "rb").read()
print("size", len(d))
print("head64:", d[:64].hex(' '))
magic, ver, vmaj, vmin, vpat, flags = struct.unpack_from("<IIIIII", d, 0)
print("magic", hex(magic), "ver", ver, "engine", vmaj, vmin, vpat, "flags", hex(flags))
print("u64@24:", struct.unpack_from("<Q", d, 24)[0])
print("u32@96:", struct.unpack_from("<I", d, 96)[0])
print("u32@100:", struct.unpack_from("<I", d, 100)[0], "u64@104:", struct.unpack_from("<Q", d, 104)[0])
print("tail96:", d[-96:].hex(' '))
