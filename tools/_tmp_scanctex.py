import struct, sys, os, re

W = r"D:\all\steam\steamapps\workshop\content\2868840"


def entries(path):
    f = open(path, "rb")
    h = f.read(112)
    fb, do = struct.unpack_from("<QQ", h, 24)
    f.seek(do)
    (n,) = struct.unpack("<I", f.read(4))
    f.seek(do + 4)
    e = {}
    for _ in range(n):
        (pl,) = struct.unpack("<I", f.read(4))
        raw = f.read(pl)
        name = raw.split(b"\x00")[0].decode("utf-8", "replace")
        ofs, size = struct.unpack("<QQ", f.read(16))
        f.read(20)
        e[name] = (fb + ofs, size)
    return f, e


def read(f, e, name, n):
    ofs, size = e[name]
    f.seek(ofs)
    return f.read(min(n, size))


samples = 0
for mod in sorted(os.listdir(W)):
    d = os.path.join(W, mod)
    if not os.path.isdir(d):
        continue
    pcks = [x for x in os.listdir(d) if x.endswith(".pck")]
    if not pcks:
        continue
    pck = os.path.join(d, pcks[0])
    try:
        f, e = entries(pck)
    except Exception as ex:
        print("skip", mod, ex)
        continue

    for imp in [n for n in e if n.endswith(".png.import")][:12]:
        txt = read(f, e, imp, 400).decode("utf-8", "replace")
        m = re.search(r'path="(res://[^"]+)"', txt)
        if not m:
            continue
        ctex_path = m.group(1).replace("res://", "", 1)
        if ctex_path not in e:
            continue
        d0 = read(f, e, ctex_path, 60)
        if d0[:4] != b"GST2":
            continue
        fields = struct.unpack_from("<14I", d0, 4)
        # fields: ver,w,h,flags,mip,d6,d7,d8,d9,d10,d11,d12,d13
        payload = read(f, e, ctex_path, 56 + 8)[56:64]
        print(f"{mod:26s} {os.path.basename(ctex_path):52s} "
              f"{fields[1]}x{fields[2]} flags=0x{fields[3]:x} d9={fields[8]} "
              f"d11={fields[10]} fmt={fields[11]} lvl0={fields[12]} "
              f"actual={e[ctex_path][1]-56} tag={payload[8:16]}")
        samples += 1
        if samples > 14:
            break
    if samples > 14:
        break