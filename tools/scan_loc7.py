from sts2_paths import repo_path, workshop_root

import re
p = workshop_root() / "3815352089/ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
keys = set()
for m in re.finditer(rb'"(THIRTY_SEVEN_CHARACTER_MOD_[A-Z0-9_\.]*)"\s*:', d):
    keys.add(m.group(1).decode())
keys = sorted(keys)
chars = [k for k in keys if "_CHARACTER_" in k]
open(repo_path("refs/loc_keys.txt"), "w", encoding="utf-8").write("\n".join(chars))
print("total keys", len(keys), "character keys", len(chars))
