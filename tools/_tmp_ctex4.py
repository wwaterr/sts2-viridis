import struct, os, re
W=r"D:\all\steam\steamapps\workshop\content\2868840"
pck=os.path.join(W,"3747497501","RegentFX.pck")
f=open(pck,"rb"); h=f.read(112)
fb,do=struct.unpack_from("<QQ",h,24); f.seek(do); (n,)=struct.unpack("<I",f.read(4)); f.seek(do+4)
e={}
for _ in range(n):
    (pl,)=struct.unpack("<I",f.read(4)); raw=f.read(pl)
    name=raw.split(b"\x00")[0].decode("utf-8","replace")
    ofs,size=struct.unpack("<QQ",f.read(16)); f.read(20); e[name]=(fb+ofs,size)
target=[k for k in e if k.endswith("fn0001.png-9483bd725fea4353a8fe0ffd38ad6862.ctex")][0]
ofs,size=e[target]; f.seek(ofs); d=f.read(min(size,120))
print("size",size)
print("hdr hex:", d[:60].hex())
print("bytes 52..96:", d[52:96])
imp=[k for k in e if k.endswith("fn0001.png.import")][0]
ofs,size=e[imp]; f.seek(ofs); print("\nIMPORT:\n"+f.read(size).decode("utf-8","replace"))
