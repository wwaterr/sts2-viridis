p = r"D:\all\steam\steamapps\workshop\content\2868840\3815352089\ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
needle = b'_CHARACTER_THIRTY_SEVEN_CHARACTER.'
i = 0; fields = []
while True:
    i = d.find(needle, i)
    if i < 0: break
    fields.append(d[i+len(needle):i+len(needle)+40].split(b'"')[0].decode('latin-1'))
    i += 1
from collections import Counter
for k, v in sorted(Counter(fields).items()):
    print(f"{v:>4}  {k}")
