from sts2_paths import repo_path, workshop_root

import re
p = workshop_root() / "3815352089/ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
text = d[42318000:42324000].decode('utf-8', 'replace')
lines = [l.rstrip() for l in text.splitlines() if re.match(r'^\s*"[A-Z0-9_\.]+"\s*:', l)]
open(repo_path("refs/loc_character.txt"), "w", encoding="utf-8").write("\n".join(lines))
print(len(lines))
