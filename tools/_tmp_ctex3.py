import struct, sys
P = r"D:\all\steam\steamapps\workshop\content\2868840\3747508952\figure_Saya.pck"
f=open(P,"rb"); h=f.read(112)
fb,do=struct.unpack_from("<QQ",h,24); f.seek(do); (n,)=struct.unpack("<I",f.read(4)); f.seek(do+4)
ents={}
for _ in range(n):
    (pl,)=struct.unpack("<I",f.read(4)); raw=f.read(pl)
    name=raw.split(b"\x00")[0].decode("utf-8","replace")
    ofs,size=struct.unpack("<QQ",f.read(16)); f.read(20); ents[name]=(fb+ofs,size)
for name in [k for k in ents if k.endswith(".ctex") and ("spine" in k or "bg" in k)][:4]:
    ofs,size=ents[name]; f.seek(ofs); d=f.read(min(size,4096))
    riff=d.find(b"RIFF")
    print(name, "size",size,"riff",riff)
    print("  hdr0-24:", d[:24].hex())
    print("  hdr24-riff:", d[24:riff].hex())
    if riff>0: print("  payload size field vs actual:", int.from_bytes(d[riff-4:riff],'little'), "actual", size-riff)
