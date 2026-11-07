import struct, glob, os
base = r"D:\all\steam\steamapps\workshop\content\2868840"
for p in sorted(glob.glob(base + r"\*\*.pck")):
    d = open(p, "rb").read()
    dir_off = struct.unpack_from("<Q", d, 32)[0]
    count = struct.unpack_from("<I", d, dir_off)[0]
    h96, h100, h104 = struct.unpack_from("<III", d, 96)
    print(f"{os.path.basename(p):38s} size={len(d):>9} dir={dir_off:>9} count={count:>4} h96={h96:>6} h100={h100:>4} h104={h104:>6}")
