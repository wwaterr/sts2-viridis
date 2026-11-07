import struct, re
P = r"D:\all\steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"
f = open(P,"rb"); h=f.read(112)
fb,do = struct.unpack_from("<QQ",h,24)
f.seek(do); (n,)=struct.unpack("<I",f.read(4)); f.seek(do+4)
ents=[]
for _ in range(n):
    (pl,)=struct.unpack("<I",f.read(4)); raw=f.read(pl)
    name=raw.split(b"\x00")[0].decode("utf-8","replace")
    ofs,size=struct.unpack("<QQ",f.read(16)); f.read(20); ents.append((name,fb+ofs,size))
for name,ofs,size in sorted(ents):
    if not re.search(r"character_select_\w+_bg\.png|characterselect_\w+\.png", name): continue
    if ".ctex" not in name: continue
    f.seek(ofs); d=f.read(28)
    if d[:4]==b"GST2":
        ver,w,hh,flags,mip=struct.unpack_from("<IIIII",d,4)
        base=re.sub(r"^\.godot/imported/|-[0-9a-f]{32}\..*$","",name)
        print(f"{base:44s} {w}x{hh}  {size/1048576:.1f} MB")
