"""Heuristic path scrape for Godot pck: extract asset paths from the trailing directory."""
import re, sys, collections

pat = re.compile(rb'(?:res://)?[A-Za-z0-9_\-][A-Za-z0-9_\-./]*\.(?:cs|tscn|tres|png|ctex|jpg|jpeg|webp|ogg|wav|mp3|json|res|import|gd|svg|uid|bin|dll|pdb|txt|md|cfg|ini|font|ttf|otf|anim|material|shader|gdshader)\b')

for p in sys.argv[1:]:
    d = open(p, 'rb').read()
    seen = collections.OrderedDict()
    for m in pat.finditer(d):
        s = m.group().decode('ascii', 'ignore')
        if len(s) < 5:
            continue
        seen.setdefault(s, 0)
        seen[s] += 1
    print(f"== {p}  size={len(d):,}  unique_paths={len(seen)}")
    dirs = collections.Counter()
    for s in seen:
        dirs['/'.join(s.split('/')[:3])] += 1
    for k, v in sorted(dirs.items())[:40]:
        print(f"   {k}  ({v})")
    out = p.rsplit('\\', 1)[-1].rsplit('/', 1)[-1] + '.paths.txt'
    with open(out, 'w', encoding='utf-8') as f:
        for s, c in seen.items():
            f.write(f"{c:>4}  {s}\n")
    print("   ->", out)
