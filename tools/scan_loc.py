from sts2_paths import workshop_root

import re
p = workshop_root() / "3815352089/ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
print("size", len(d))
# look for localization json content: keys like "SOME_ID.title" or {"id": {"name": ...}}
for pat in [rb'"title"', rb'\.title', rb'"cards"', rb'"characters"']:
    hits = [m.start() for m in re.finditer(pat, d)]
    print(pat, len(hits), hits[:5])
i = d.find(b'"title"')
if i > 0:
    print(repr(d[max(0,i-600):i+600]))
