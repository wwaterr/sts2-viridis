import re
p = r"D:\all\steam\steamapps\workshop\content\2868840\3815352089\ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
hits = [m.start() for m in re.finditer(rb'\.title"', d)]
print("title hits", len(hits))
for h in hits[:3]:
    print(repr(d[h-60:h+40]))
print("prefix counts:")
for pat in [rb'"THIRTY_SEVEN', rb'THIRTY_SEVEN_CHARACTER_MOD_CARD_', rb'_CARD_ACCEPTANCE']:
    print(pat, len(re.findall(pat, d)))
