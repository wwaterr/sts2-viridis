from sts2_paths import workshop_root

import re
p = workshop_root() / "3815352089/ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
for pat in [rb'THIRTY_SEVEN_CHARACTER_MOD_CHARACTER', rb'_CHARACTER_\.title', rb'CHARACTER_\.']:
    hits = [m.start() for m in re.finditer(pat, d)]
    print(pat, len(hits), hits[:3])
i = d.find(b'THIRTY_SEVEN_CHARACTER_MOD_CHARACTER_THIRTY_SEVEN')
print("---- character loc block ----")
print(d[i-400:i+1500].decode('utf-8', 'replace'))
