import struct
p = r"D:\all\steam\steamapps\workshop\content\2868840\3749627968\RestSiteFondleCursorMod.pck"
d = open(p, "rb").read()
dir_off = struct.unpack_from("<Q", d, 32)[0]
print("dir_off", dir_off, "size", len(d))
print("bytes at dir:", d[dir_off:dir_off+120].hex(' '))
count = struct.unpack_from("<I", d, dir_off)[0]
print("count?", count)

def parse(pad):
    off = dir_off + 4
    out = []
    for _ in range(count):
        (plen,) = struct.unpack_from("<I", d, off); off += 4
        path = d[off:off+plen].decode("utf-8", "replace"); off += plen
        if pad: off += (-plen) % 4
        ofs, size = struct.unpack_from("<QQ", d, off); off += 16
        md5 = d[off:off+16]; off += 16
        (flags,) = struct.unpack_from("<I", d, off); off += 4
        out.append((path, ofs, size, flags))
    return out, off

for pad in (True, False):
    try:
        ents, end = parse(pad)
        ok = all(0 <= e[1] <= len(d) for e in ents)
        print(f"pad={pad} parsed={len(ents)} end={end} file_size={len(d)} exact={end==len(d)} offsets_ok={ok}")
        for e in ents[:6]: print("   ", e)
    except Exception as ex:
        print("pad", pad, "failed", ex)
