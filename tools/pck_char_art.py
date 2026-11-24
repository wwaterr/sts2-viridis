from sts2_paths import game_pack

import struct, sys, os, re

P = game_pack()


def load_dir(f):
    f.seek(0)
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
    return ents


def ctex_size(data):
    """Godot 4 CompressedTexture2D (.ctex): GST2 header."""
    magic = data[:4]
    if magic == b"GST2":
        ver, w, h, flags, mip = struct.unpack_from("<IIIII", data, 4)
        fmt = struct.unpack_from("<I", data, 24)[0]
        return f"GST2 v{ver} {w}x{h} flags=0x{flags:x} mipmaps={mip} format={fmt}"
    return "unknown header " + data[:16].hex()


def main():
    f = open(P, "rb")
    ents = load_dir(f)

    wants = [
        r"animations/character_select/ironclad/.*\.(atlas|import)$",
        r"animations/character_select/ironclad/.*png\.import$",
        r"\.godot/imported/characterselect_ironclad\.png.*\.ctex$",
        r"\.godot/imported/character_select_necrobinder_bg\.png.*\.ctex$",
        r"animations/character_select/necrobinder/.*_bg\.png\.import$",
    ]
    for pat in wants:
        rx = re.compile(pat)
        for name in sorted(ents):
            if not rx.search(name):
                continue
            ofs, size = ents[name]
            f.seek(ofs)
            data = f.read(size)
            print(f"\n=== {name}  ({size} bytes, from pck at {ofs})")
            if name.endswith((".ctex",)):
                print("   ", ctex_size(data))
            elif name.endswith((".import", ".atlas")):
                text = data.decode("utf-8", "replace")
                print("\n".join("    " + ln for ln in text.splitlines()[:22]))
            elif name.endswith(".png"):
                print("   PNG" if data[:8] == b"\x89PNG\r\n\x1a\n" else "   not png")


main()