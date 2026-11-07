"""Strict reader/validator for the generated pck: re-parses header, data and directory."""

import hashlib
import struct
import sys


def read_pck(path):
    d = open(path, "rb").read()
    magic, ver, vmaj, vmin, vpat, flags = struct.unpack_from("<IIIIII", d, 0)
    assert magic == 0x43504447, "bad magic"
    assert ver == 3, f"unexpected pack version {ver}"
    (file_base,) = struct.unpack_from("<Q", d, 24)
    (dir_off,) = struct.unpack_from("<Q", d, 32)
    assert file_base <= dir_off <= len(d)
    (count,) = struct.unpack_from("<I", d, dir_off)

    entries = []
    off = dir_off + 4
    for _ in range(count):
        (plen,) = struct.unpack_from("<I", d, off)
        off += 4
        raw = d[off:off + plen]
        off += plen
        name = raw.split(b"\x00")[0].decode("utf-8")
        (fofs, size) = struct.unpack_from("<QQ", d, off)
        off += 16
        digest = d[off:off + 16]
        off += 16
        (eflags,) = struct.unpack_from("<I", d, off)
        off += 4
        entries.append((name, fofs, size, digest, eflags))

    assert off == len(d), f"directory does not end at EOF: {off} vs {len(d)}"
    return file_base, dir_off, entries, d


def verify(path):
    file_base, dir_off, entries, d = read_pck(path)
    bad = 0
    for name, fofs, size, digest, eflags in entries:
        start = file_base + fofs
        assert start + size <= dir_off, f"{name}: overlaps directory"
        actual = hashlib.md5(d[start:start + size]).digest()
        if actual != digest:
            bad += 1
            print(f"  MD5 MISMATCH {name}")
    print(f"OK {path}: {len(entries)} files, file_base={file_base}, dir@{dir_off}, size={len(d)}, md5_ok={bad == 0}")
    for e in entries[:5]:
        print(f"   {e[2]:>7}  {e[0]}")


if __name__ == "__main__":
    verify(sys.argv[1])