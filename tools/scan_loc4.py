import re
p = r"D:\all\steam\steamapps\workshop\content\2868840\3815352089\ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
out = []
i = 42319439
out.append(d[i-1200:i+2500].decode('utf-8', 'replace'))
open(r"D:\study\ai\viridis\refs\loc_character.txt", "w", encoding="utf-8").write("\n=====\n".join(out))
print("ok")
