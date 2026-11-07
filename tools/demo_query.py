import sqlite3

con = sqlite3.connect("refs/db/cards.sqlite")
con.row_factory = sqlite3.Row
out = []


def w(s=""):
    out.append(s)


w("# 实测查询结果\n")
w(f"总行数: {con.execute('select count(*) from cards').fetchone()[0]}\n")

w("## 1) 单卡查询：痛击 (Bash)\n```")
r = con.execute("select * from cards where class='Bash'").fetchone()
for k in ("id", "class", "pools", "cost", "type", "rarity", "target", "title_zh", "desc_zh", "vars", "upgrade"):
    w(f"{k:10s} = {r[k]}")
w("```\n")

w("## 2) 跨职业检索：所有 0 费攻击牌（前 12 张）\n")
w("| 卡名 | id | 职业 | 稀有度 | 变量 |")
w("|---|---|---|---|---|")
for x in con.execute("select title_zh,id,pools,rarity,vars from cards where type='Attack' and cost='0' order by pools limit 12"):
    w(f"| {x['title_zh']} | `{x['id']}` | {x['pools']} | {x['rarity']} | {x['vars'][:50]} |")
n2 = con.execute("select count(*) from cards where type='Attack' and cost='0'").fetchone()[0]
w(f"\n匹配总数: {n2}\n")

w("## 3) 关键词检索：中文描述里含「中毒」\n")
w("| 卡名 | 职业 | 费用 | 描述 |")
w("|---|---|---|---|")
for x in con.execute("select title_zh,pools,cost,desc_zh from cards where desc_zh like '%中毒%' limit 8"):
    d = x["desc_zh"].replace("\n", " / ")
    w(f"| {x['title_zh']} | {x['pools']} | {x['cost']} | {d[:70]} |")
n3 = con.execute("select count(*) from cards where desc_zh like '%中毒%'").fetchone()[0]
w(f"\n匹配总数: {n3}\n")

w("## 4) 实现源码：Bash 的 OnPlay\n```csharp")
w(con.execute("select play from cards where class='Bash'").fetchone()[0])
w("```")

open("refs/db/QUERY_DEMO.md", "w", encoding="utf-8").write("\n".join(out))
print("written lines:", len(out), "| counts:", n2, n3)