"""Generate human-readable reports from refs/db/cards.sqlite."""
import sqlite3
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DB = ROOT / "refs" / "db" / "cards.sqlite"
OUT = ROOT / "refs" / "db"

con = sqlite3.connect(DB)
con.row_factory = sqlite3.Row
rows = con.execute("select * from cards").fetchall()

counts = {r[0]: r[1] for r in con.execute("select pools, count(*) from cards group by pools")}
n = len(rows)

readme = f"""# StS2 卡牌机制数据库

来源：反编译 `sts2.dll` 的 `MegaCrit.Sts2.Core.Models.Cards`（ICSharpCode.Decompiler 11.2）+ 游戏本体 `localization/{{eng,zhs}}/cards.json`。

- 卡牌总数：**{n}**
- 文件：`cards.sqlite`（查询用）、`cards.csv`（Excel/表格用，UTF-8-BOM）
- 原始源码：`refs/decompiled/.../Models/Cards/*.cs`

## 表结构 `cards`

| 字段 | 说明 |
|---|---|
| `id` | 本地化键（类名转 UPPER_SNAKE），如 `STRIKE_IRONCLAD` |
| `class` | C# 类名，如 `StrikeIronclad` |
| `pools` | 所属卡池：ironclad / silent / defect / regent / necrobinder / colorless / curse / status / token / event / quest（可多个，逗号分隔）|
| `cost` | 费用，`-1` 表示 X 费，`0` 表示固有 0 费 |
| `type` | Attack / Skill / Power / Status / Curse / Quest |
| `rarity` | Basic / Common / Uncommon / Rare / Ancient / Event / Token / Status / Curse / Quest |
| `target` | Self / AnyEnemy / AllEnemies / RandomEnemy / AnyAlly / AllAllies / None 等 |
| `gains_block` | 是否格挡牌（`GainsBlock`）|
| `vars` | `CanonicalVars` 中的动态变量（伤害/格挡/层数等基础值）|
| `upgrade` | `OnUpgrade()` 内容（升级增量）|
| `title_zh` / `desc_zh` / `title_en` / `desc_en` | 本地化文本（描述里 `{{Damage:diff()}}` 是变量占位符）|
| `play` | `OnPlay()` 源码（截断 4000 字符），用于查实现写法 |
| `file` | 反编译源码路径 |

## 分布

按卡池：{", ".join(f"{k}={v}" for k, v in sorted(counts.items(), key=lambda x: -x[1]))}

## 常用查询

```sql
-- 某职业所有 1 费攻击牌
select id, title_zh, cost, rarity, vars from cards
where pools='ironclad' and type='Attack' and cost='1';

-- 中文搜卡
select id, title_zh, cost, type from cards where title_zh like '%击%';

-- 看某张卡的实现
select play from cards where class='Bash';
```

```powershell
python tools/qcard.py 打击          # 按中/英文名搜
python tools/qcard.py --pool silent --type Power
```
"""
(OUT / "README.md").write_text(readme, encoding="utf-8")

order = ["ironclad", "silent", "defect", "regent", "necrobinder", "colorless"]
lines = ["# 各职业卡表\n"]
for pool in order:
    sub = [r for r in rows if pool in (r["pools"] or "")]
    if not sub:
        continue
    lines.append(f"\n## {pool}（{len(sub)} 张）\n")
    lines.append("| 卡名 | id | 费 | 类型 | 稀有度 | 目标 | 变量 | 升级 |")
    lines.append("|---|---|---|---|---|---|---|---|")
    for r in sorted(sub, key=lambda x: (x["type"], x["rarity"], x["id"])):
        lines.append(
            f"| {r['title_zh']} | `{r['id']}` | {r['cost']} | {r['type']} | {r['rarity']} | "
            f"{r['target']} | {r['vars'][:60]} | {r['upgrade'][:40]} |")
(OUT / "cards_by_character.md").write_text("\n".join(lines), encoding="utf-8")
print(f"cards={n}; wrote README.md and cards_by_character.md")