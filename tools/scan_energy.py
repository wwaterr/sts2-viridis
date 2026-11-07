import re
p = r"D:\all\steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"
names = set()
tail = b""
with open(p, "rb") as f:
    while True:
        chunk = f.read(1 << 24)
        if not chunk: break
        buf = tail + chunk
        for m in re.finditer(rb'card/energy_[a-z]{2,20}', buf):
            names.add(m.group().decode())
        tail = buf[-64:]
print(len(names))
for n in sorted(names): print(n)
