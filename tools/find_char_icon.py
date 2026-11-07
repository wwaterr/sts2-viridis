import struct, sys, re

P = r"D:\all\steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"

f = open(P, "rb")
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


def ctex_info(name):
    ofs, size = e[name]
    f.seek(ofs)
    d = f.read(60)
    if d[:4] != b"GST2":
        return None
    fl = struct.unpack_from("<14I", d, 4)
    return f"{fl[1]}x{fl[2]} flags=0x{fl[3]:x} fmt={fl[11]} filesize={size}"


patterns = [r"character_icon", r"char_select", r"select_button", r"top_panel"]
rx = re.compile("|".join(patterns), re.I)
hits = sorted(nm for nm in e if rx.search(nm) and nm.endswith(".ctex"))
print(f"ctex hits: {len(hits)}")
for nm in hits[:40]:
    info = ctex_info(nm)
    print(f"  {nm}\n      {info}")