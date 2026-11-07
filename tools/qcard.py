"""Quick card search: python tools/qcard.py <keyword> [--pool x] [--type y] [--rarity z]"""
import sqlite3
import sys
from pathlib import Path

DB = Path(__file__).resolve().parents[1] / "refs" / "db" / "cards.sqlite"

args = sys.argv[1:]
filters, kw = [], []
i = 0
while i < len(args):
    a = args[i]
    if a.startswith("--"):
        key = a[2:]
        val = args[i + 1] if i + 1 < len(args) else ""
        filters.append((key, val))
        i += 2
    else:
        kw.append(a)
        i += 1

sql = "select id, title_zh, title_en, cost, type, rarity, target, pools, vars, upgrade from cards where 1=1"
params = []
if kw:
    clause = " and (" + " or ".join(["(title_zh like ? or title_en like ? or id like ? or desc_zh like ?)"] * len(kw)) + ")"
    for k in kw:
        params += [f"%{k}%", f"%{k}%", f"%{k.upper()}%", f"%{k}%"]
    sql += clause
for key, val in filters:
    sql += f" and {key} like ?"
    params.append(f"%{val}%")
sql += " order by pools, type, cast(cost as int)"

con = sqlite3.connect(DB)
con.row_factory = sqlite3.Row
rows = con.execute(sql, params).fetchall()
print(f"{len(rows)} match(es)")
for r in rows:
    print(f"\n{r['title_zh']} / {r['title_en']}   [{r['id']}]")
    print(f"  {r['cost']}费 {r['type']} {r['rarity']} {r['target']}  pool={r['pools']}")
    if r["vars"]:
        print(f"  vars: {r['vars'][:120]}")
    if r["upgrade"]:
        print(f"  upg:  {r['upgrade'][:100]}")