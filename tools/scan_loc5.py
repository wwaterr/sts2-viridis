import re
p = r"D:\all\steam\steamapps\workshop\content\2868840\3815352089\ThirtySevenCharacterMod.pck"
d = open(p, "rb").read()
text = d[42318000:42324000].decode('utf-8', 'replace')
lines = [l.rstrip() for l in text.splitlines() if re.match(r'^\s*"[A-Z0-9_\.]+"\s*:', l)]
open(r"D:\study\ai\viridis\refs\loc_character.txt", "w", encoding="utf-8").write("\n".join(lines))
print(len(lines))
