import re

p = r"D:\all\steam\steamapps\workshop\content\2868840\3815352089\ThirtySevenCharacterMod.pck"
d = open(p, 'rb').read()
out = []
w = out.append

w(f"size {len(d)}")

def show(label, off, before=250, after=450):
    seg = d[max(0, off - before):off + after]
    txt = ''.join(chr(b) if 32 <= b < 127 or b == 10 else '.' for b in seg)
    w(f"\n===== {label} @ {off} =====")
    w(txt)

for kw in (b'SpineSprite', b'.atlas', b'spine_loader'):
    offs = [m.start() for m in re.finditer(re.escape(kw), d)]
    w(f"\n### {kw!r} hits: {len(offs)} -> {offs[:8]}")

# data regions (skip the directory block near EOF)
show("SpineSprite data", 42466025)
show(".atlas data", 42743631)
show("spine_loader data", 43803381)
show("spine_loader data 2", 43819271)

# any text mentioning skeleton/atlas resource names
for kw in (b'306604_37', b'ThirtySevenCharacterMod_character.tscn'):
    offs = [m.start() for m in re.finditer(re.escape(kw), d)]
    w(f"\n### {kw!r} hits: {len(offs)} -> {offs[:10]}")

open(r"D:\study\ai\viridis\refs\spine_scan.txt", "w", encoding="utf-8").write("\n".join(out))
print("written")