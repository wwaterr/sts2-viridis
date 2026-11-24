from sts2_paths import game_pack

import struct, os, json
p = game_pack()
f = open(p, "rb")
h = f.read(112)
file_base, dir_off = struct.unpack_from("<QQ", h, 24)
f.seek(dir_off)
(count,) = struct.unpack("<I", f.read(4))
f.seek(dir_off + 4)
ents = {}
for _ in range(count):
    (plen,) = struct.unpack("<I", f.read(4))
    raw = f.read(plen); name = raw.split(b"\x00")[0].decode()
    ofs, size = struct.unpack("<QQ", f.read(16)); f.read(20)
    ents[name] = (file_base + ofs, size)

want = ["cards", "relics", "potions", "powers", "characters", "card_library", "card_keywords", "orbs", "afflictions", "enchantments"]
os.makedirs("refs/gamedata/en", exist_ok=True)
os.makedirs("refs/gamedata/zhs", exist_ok=True)
for lang in ("eng", "zhs"):
    for w in want:
        k = f"localization/{lang}/{w}.json"
        if k not in ents: print("MISSING", k); continue
        ofs, size = ents[k]
        f.seek(ofs); data = f.read(size)
        open(f"refs/gamedata/{lang}/{w}.json", "wb").write(data)
        print(f"{k:38s} {size:>7} bytes")

