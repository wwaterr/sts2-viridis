"""删掉 90 张卡硬编码的 Title，改回框架的本地化查表，并核对中英标题是否一致。

用前先 dry-run：python tools/strip_card_titles.py --check
真正改：     python tools/strip_card_titles.py
"""
from __future__ import annotations

import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
MOD = ROOT / "mods" / "Viridis"
CARDS = MOD / "Cards"

TITLE_RE = re.compile(r'^\s*public override string Title => "(?P<t>[^"]*)";\s*$')
CLASS_RE = re.compile(r"\bclass\s+(?P<c>\w+)")


def snake(name: str) -> str:
    return re.sub(r"(?<!^)(?=[A-Z])", "_", name).upper()


def load_json(lang: str, name: str) -> dict:
    p = MOD / "assets" / "localization" / lang / name
    return json.loads(p.read_text(encoding="utf-8")) if p.exists() else {}


zhs = load_json("zhs", "cards.json")
eng = load_json("eng", "cards.json")

dry = "--check" in sys.argv
rows: list[tuple[str, str, str, str, str]] = []
removed = 0

for cs in sorted(CARDS.glob("*.cs")):
    lines = cs.read_text(encoding="utf-8").splitlines(keepends=True)
    cur = None
    out: list[str] = []
    for line in lines:
        m = CLASS_RE.search(line)
        if m:
            cur = m.group("c")
        tm = TITLE_RE.match(line)
        if tm and cur:
            hard = tm.group("t")
            key = f"VIRIDIS_CARD_{snake(cur)}.title"
            z = zhs.get(key)
            e = eng.get(key)
            flag = "OK" if z == hard else "DIFF"
            rows.append((flag, cur, hard, z or "<缺失>", e or "<缺失>"))
            if not dry:
                removed += 1
                continue  # 丢掉这一行
        out.append(line)
    if not dry:
        cs.write_text("".join(out), encoding="utf-8", newline="")

print(f"共 {len(rows)} 张卡有硬编码 Title，英文标题缺失 {sum(1 for r in rows if r[4] == '<缺失>')} 个")
diff = [r for r in rows if r[0] == "DIFF"]
print(f"中文标题与本地化不一致：{len(diff)} 张")
for flag, cls, hard, z, e in diff:
    print(f"  {cls:24s} 代码={hard!s:8s} 中={z!s:12s} 英={e}")
print(f"已删除 {removed} 行" if not dry else "（dry-run，未写入）")