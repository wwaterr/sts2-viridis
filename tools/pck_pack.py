"""Minimal Godot 4 (.pck, format v3) writer, reverse-engineered from real StS2 mod pcks.

Layout:
  header (104 bytes) -> FILE_BASE (aligned to 16, = 112)
  data area: file bytes, 16-byte aligned, offsets relative to FILE_BASE
  directory: u32 count, then per file:
      u32 path_len (utf8 + NUL, padded to 4)
      path bytes
      u64 offset (relative), u64 size
      16 bytes md5
      u32 flags (0)

Header fields:
  0  u32 magic 'GDPC'
  4  u32 format version = 3
  8  u32 engine major, 12 minor, 16 patch
  20 u32 flags (2 = offsets relative to file base)
  24 u64 file_base
  32 u64 dir_offset (absolute)
  40 64 bytes reserved
"""
import hashlib
import struct
import sys
from pathlib import Path

MAGIC = 0x43504447  # 'GDPC'
PACK_VERSION = 3
ENGINE = (4, 5, 1)
FLAG_REL_FILEBASE = 2
HEADER_SIZE = 104
FILE_BASE = 112  # 104 aligned up to 16


def align(n, a=16):
    return (n + a - 1) // a * a


def build_pck(entries, out_path):
    """entries: list of (pack_path: str, data: bytes)"""
    body = bytearray()
    records = []

    for pack_path, data in entries:
        offset = len(body)
        body += data
        pad = align(len(body)) - len(body)
        body += b"\x00" * pad
        records.append((pack_path, offset, len(data), hashlib.md5(data).digest()))

    dir_offset = FILE_BASE + len(body)

    directory = bytearray(struct.pack("<I", len(records)))
    for pack_path, offset, size, digest in records:
        raw = pack_path.encode("utf-8")
        plen = align(len(raw) + 1)  # include NUL, pad to 4
        directory += struct.pack("<I", plen)
        directory += raw + b"\x00" * (plen - len(raw))
        directory += struct.pack("<QQ", offset, size)
        directory += digest
        directory += struct.pack("<I", 0)

    header = bytearray(HEADER_SIZE)
    struct.pack_into("<IIIIII", header, 0, MAGIC, PACK_VERSION, *ENGINE, FLAG_REL_FILEBASE)
    struct.pack_into("<Q", header, 24, FILE_BASE)
    struct.pack_into("<Q", header, 32, dir_offset)

    out = Path(out_path)
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_bytes(bytes(header) + b"\x00" * (FILE_BASE - HEADER_SIZE) + bytes(body) + bytes(directory))
    return len(records), len(body), dir_offset


def collect(root, prefix):
    root = Path(root)
    items = []
    for f in sorted(root.rglob("*")):
        if f.is_file():
            rel = f.relative_to(root).as_posix()
            items.append((f"{prefix}/{rel}", f.read_bytes()))
    return items


if __name__ == "__main__":
    if len(sys.argv) < 4:
        print("usage: pck_pack.py <out.pck> <packPrefix> <sourceDir> [<packPrefix2> <sourceDir2> ...]")
        sys.exit(1)
    out = sys.argv[1]
    entries = []
    args = sys.argv[2:]
    for i in range(0, len(args), 2):
        entries.extend(collect(args[i + 1], args[i]))
    count, body_len, dir_off = build_pck(entries, out)
    print(f"packed {count} files, body={body_len} bytes, dir@{dir_off} -> {out}")