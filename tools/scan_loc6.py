import re
p = r"D:\all\steam\steamapps\workshop\content\2868840\3815352089\ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
seen = []
for m in re.finditer(rb'THIRTY_SEVEN_CHARACTER_MOD_CHARACTER', d):
    s = d[max(0, m.start()-160):m.start()+220].decode('utf-8', 'replace')
    for frag in re.findall(r'[A-Z0-9_\.]{20,}', s):
        if 'CHARACTER' in frag and frag not in seen:
            seen.append(frag)
open(r"D:\study\ai\viridis\refs\loc_character.txt", "w", encoding="utf-8").write("\n".join(seen[:80]))
print(len(seen))
