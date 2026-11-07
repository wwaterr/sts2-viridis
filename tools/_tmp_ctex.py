import struct, re
P = r"D:\all\steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"
f=open(P,"rb"); h=f.read(112)
fb,do=struct.unpack_from("<QQ",h,24); f.seek(do); (n,)=struct.unpack("<I",f.read(4)); f.seek(do+4)
ents={}
for _ in range(n):
    (pl,)=struct.unpack("<I",f.read(4)); raw=f.read(pl)
    name=raw.split(b"\x00")[0].decode("utf-8","replace")
    ofs,size=struct.unpack("<QQ",f.read(16)); f.read(20); ents[name]=(fb+ofs,size)

for key in [k for k in ents if "ironclad_4.png" in k and k.endswith(".ctex")]:
    ofs,size = ents[key]
    f.seek(ofs); data=f.read(64)
    print(key, size)
    print(" hex:", data[:40].hex())
    magic,ver,w,hh,flags,mip,fmt = struct.unpack_from("<7I", data, 0)
    print(f" magic={data[:4]} ver={ver} {w}x{hh} flags=0x{flags:x} mip={mip} fmt={fmt}")
    print(" payload starts:", data[28:40])
