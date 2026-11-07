"""Build a card/meta database for Slay the Spire 2 from:
   - decompiled model classes (refs/decompiled)
   - extracted game localization (refs/gamedata/{eng,zhs})

Outputs: refs/db/cards.csv, refs/db/cards.sqlite, refs/db/README.md
"""
import csv
import json
import re
import sqlite3
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DECOMP = ROOT / "refs" / "decompiled" / "MegaCrit" / "Sts2" / "Core" / "Models"
LOC = ROOT / "refs" / "gamedata"
OUT = ROOT / "refs" / "db"

POOL_CHARACTER = {
    "IroncladCardPool": "ironclad",
    "SilentCardPool": "silent",
    "DefectCardPool": "defect",
    "RegentCardPool": "regent",
    "NecrobinderCardPool": "necrobinder",
    "ColorlessCardPool": "colorless",
    "CurseCardPool": "curse",
    "StatusCardPool": "status",
    "TokenCardPool": "token",
    "EventCardPool": "event",
    "QuestCardPool": "quest",
    "DeprecatedCardPool": "deprecated",
    "DeprivedCardPool": "deprived",
}

CLASS_RE = re.compile(r"public (?:sealed |abstract |partial )*class (\w+)\s*:\s*([\w<>]+)")
CTOR_RE = re.compile(r"public \w+\(\)\s*:\s*base\(([^)]*)\)", re.S)
CANON_RE = re.compile(r"CanonicalVars\s*=>\s*(.+?);", re.S)
UPGRADE_RE = re.compile(r"protected override void OnUpgrade\(\)\s*\{(.*?)\n\t\}", re.S)
PLAY_RE = re.compile(r"protected override async Task OnPlay\([^)]*\)\s*\{(.*?)\n\t\}", re.S)
BOOL_RE = re.compile(r"public override bool (\w+) => (true|false);")


def snake(name: str) -> str:
    return re.sub(r"(?<!^)(?=[A-Z])", "_", name).upper()


def tidy(expr: str) -> str:
    expr = re.sub(r"new <>z__\w+List<DynamicVar>\(", "new[] { ", expr)
    expr = expr.replace("<>z__ReadOnly", "")
    return re.sub(r"\s+", " ", expr).strip()


def load_loc():
    out = {}
    for lang in ("zhs", "eng"):
        p = LOC / lang / "cards.json"
        out[lang] = json.loads(p.read_text(encoding="utf-8")) if p.exists() else {}
    return out


def parse_pools():
    """class -> [character pools]"""
    owner = {}
    for f in sorted((DECOMP / "CardPools").glob("*.cs")):
        character = POOL_CHARACTER.get(f.stem)
        if not character:
            continue
        for m in re.finditer(r"ModelDb\.Card<(\w+)>\(\)", f.read_text(encoding="utf-8")):
            owner.setdefault(m.group(1), []).append(character)
    return owner


def parse_cards():
    rows = []
    for f in sorted((DECOMP / "Cards").glob("*.cs")):
        src = f.read_text(encoding="utf-8")
        cm = CLASS_RE.search(src)
        if not cm:
            continue
        cls, base = cm.group(1), cm.group(2)
        cost = ctype = rarity = target = ""
        args = CTOR_RE.search(src)
        if args:
            parts = [a.strip() for a in args.group(1).split(",")]
            if len(parts) >= 4 and all(parts[i].isdigit() or parts[i].lstrip("-").isdigit() for i in range(1, 1)):
                pass
            vals = [p for p in parts]
            if len(vals) >= 4:
                cost = vals[0]
                ctype = vals[1].replace("CardType.", "")
                rarity = vals[2].replace("CardRarity.", "")
                target = vals[3].replace("TargetType.", "")
        flags = {k: v == "true" for k, v in BOOL_RE.findall(src)}
        canon = CANON_RE.search(src)
        upg = UPGRADE_RE.search(src)
        play = PLAY_RE.search(src)
        rows.append({
            "class": cls,
            "id": f"{snake(cls)}",
            "base": base,
            "cost": cost,
            "type": ctype,
            "rarity": rarity,
            "target": target,
            "gains_block": flags.get("GainsBlock", False),
            "vars": tidy(canon.group(1)) if canon else "",
            "upgrade": tidy(upg.group(1)) if upg else "",
            "play": tidy(play.group(1))[:4000] if play else "",
            "file": str(f.relative_to(ROOT)).replace("\\", "/"),
        })
    return rows


def build():
    loc = load_loc()
    pools = parse_pools()
    rows = parse_cards()

    for r in rows:
        r["pools"] = ",".join(sorted(set(pools.get(r["class"], []))))
        for lang in ("zhs", "eng"):
            suffix = "zh" if lang == "zhs" else "en"
            d = loc[lang]
            r[f"title_{suffix}"] = d.get(f"{r['id']}.title", "")
            r[f"desc_{suffix}"] = d.get(f"{r['id']}.description", "")

    OUT.mkdir(parents=True, exist_ok=True)
    cols = ["id", "class", "pools", "cost", "type", "rarity", "target", "gains_block",
            "vars", "upgrade", "title_zh", "desc_zh", "title_en", "desc_en", "play", "file"]

    with (OUT / "cards.csv").open("w", encoding="utf-8-sig", newline="") as fh:
        w = csv.DictWriter(fh, fieldnames=cols)
        w.writeheader()
        for r in sorted(rows, key=lambda x: (x["pools"], x["type"], x["rarity"], x["id"])):
            w.writerow({k: r.get(k, "") for k in cols})

    db = OUT / "cards.sqlite"
    if db.exists():
        db.unlink()
    con = sqlite3.connect(db)
    con.execute("""CREATE TABLE cards (
        id TEXT PRIMARY KEY, class TEXT, pools TEXT, cost TEXT, type TEXT, rarity TEXT,
        target TEXT, gains_block INTEGER, vars TEXT, upgrade TEXT,
        title_zh TEXT, desc_zh TEXT, title_en TEXT, desc_en TEXT, play TEXT, file TEXT)""")
    con.executemany(
        "INSERT OR REPLACE INTO cards VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)",
        [(r["id"], r["class"], r["pools"], r["cost"], r["type"], r["rarity"], r["target"],
          int(r["gains_block"]), r["vars"], r["upgrade"], r["title_zh"], r["desc_zh"],
          r["title_en"], r["desc_en"], r["play"], r["file"]) for r in rows])
    con.execute("CREATE INDEX idx_pools ON cards(pools)")
    con.commit()
    con.close()

    # stats
    from collections import Counter
    by_pool = Counter(r["pools"] for r in rows)
    missing_cost = sum(1 for r in rows if not r["cost"])
    missing_title = sum(1 for r in rows if not r["title_zh"])
    print(f"cards: {len(rows)}  |  no ctor args: {missing_cost}  |  no zh title: {missing_title}")
    for k, v in by_pool.most_common(8):
        print(f"  {k or '(none)':24s} {v}")
    return rows


if __name__ == "__main__":
    build()