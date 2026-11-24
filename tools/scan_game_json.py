from sts2_paths import game_pack

import struct, os
from collections import Counter
p = game_pack()
f = open(p, "rb"); h = f.read(112)
file_base, dir_off = struct.unpack_from("<QQ", h, 24)
f.seek(dir_off); (count,) = struct.unpack("<I", f.read(4)); f.seek(dir_off + 4)
json_ents = []
for _ in range(count):
    (plen,) = struct.unpack("<I", f.read(4)); raw = f.read(plen)
    name = raw.split(b"\x00")[0].decode("utf-8","replace")
    ofs, size = struct.unpack("<QQ", f.read(16)); f.read(20)
    if name.endswith(".json") or name.endswith(".csv") or name.endswith(".tres"):
        json_ents.append((name, size))
non_loc = [e for e in json_ents if not e[0].startswith("localization/")]
print("total json/csv/tres:", len(json_ents), "non-localization:", len(non_loc))
print("top dirs:", Counter(e[0].split("/")[0] for e in non_loc).most_common(10))
for e in sorted(non_loc)[:25]: print(f"  {e[1]:>8}  {e[0]}")
