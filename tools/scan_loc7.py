import re
p = r"D:\all\steam\steamapps\workshop\content\2868840\3815352089\ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
keys = set()
for m in re.finditer(rb'"(THIRTY_SEVEN_CHARACTER_MOD_[A-Z0-9_\.]*)"\s*:', d):
    keys.add(m.group(1).decode())
keys = sorted(keys)
chars = [k for k in keys if "_CHARACTER_" in k]
open(r"D:\study\ai\viridis\refs\loc_keys.txt", "w", encoding="utf-8").write("\n".join(chars))
print("total keys", len(keys), "character keys", len(chars))
