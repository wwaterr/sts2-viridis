from sts2_paths import workshop_root

import mmap, struct
p = workshop_root() / "3815352089/ThirtySevenCharacterMod.pck"
f = open(p,'rb'); m = mmap.mmap(f.fileno(),0,access=mmap.ACCESS_READ)
key = b'res://ThirtySevenCharacterModCode/Cards/Acceptance.cs'
i = m.find(key)
seg = bytes(m[i-8:i+128])
print(seg.hex(' '))
print(repr(seg))
hdr = bytes(m[:96])
print('hdr:', hdr.hex(' '))
print('file_base', struct.unpack('<Q', m[24:32])[0])
m.close(); f.close()
