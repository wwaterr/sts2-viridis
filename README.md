# 小松绿 Viridis

小松绿是《杀戮尖塔 2》的自定义角色。每场战斗的第一回合，初始遗物「园丁手套」让你多抽一张牌，再选一张手牌种下。种哪张，决定了你这回合能打什么，也决定了几回合后能收获什么。

当前版本 **0.2.1**，包含 114 张卡牌（含 11 张先古牌）、专属遗物和药水，支持简体中文与英文。93 张牌已接入专属插画，另有 21 张使用占位图。小松绿在战斗中持铲待机，攻击时向前平刺。

## 0.2.1 本轮更新

- 重新分配 99 张常规奖励牌的稀有度，同步调整卡牌效果、升级和中英文说明。
- 「菌丝蔓延」每消耗 1 点能量就对全体敌人造成一次伤害。
- 「松脂」只催长本回合种下的作物。「幼苗移栽」可将弃牌堆中的一张牌放回抽牌堆顶。「生长期」每回合开始时额外抽一张牌。后两张牌和「菌丝蔓延」更新了插画。
- 「吸芽」从抽牌堆选一张牌栽培。「移栽」立即收获自己种下的非永久作物。联机时，母株和园艺遗物在持有者的回合触发。

[完整卡牌调整与验证记录](https://github.com/wwaterr/sts2-viridis/pull/15)

本试验分支采用了小松绿烂梗库中的三个牌名。「介壳虫」改名为「咋这样」（#001），「幼苗移栽」改名为「善意引导」（#015），「顶芽」改为「门槛姐」（#021）。打出「门槛姐」前已有至少 3 点同接时，这张牌会额外抽 1 张。[小松绿烂梗库](https://viridis.love/memes/)

## 安装测试包

现有测试包 `Viridis-0.2.1-PR15-4b702a0.zip` 对应试验前的提交 [`4b702a0`](https://github.com/wwaterr/sts2-viridis/commit/4b702a0b861b9e3ba0e675bf42af119dbaac897a)。要试玩三张梗卡和三回合栽培的新收获规则，请用本分支[从源码构建](#从源码构建)。

已验收的环境为 Windows、《杀戮尖塔 2》Steam 版 **v0.113.1** 和 **RitsuLib 0.6.14**。

1. 在 Steam 创意工坊订阅并启用 [RitsuLib](https://steamcommunity.com/sharedfiles/filedetails/?id=3747602295)。
2. 退出游戏。打开 Steam 中游戏的「浏览本地文件」，将测试包里的整个 `Viridis` 文件夹解压到游戏目录下的 `mods` 文件夹。
3. 确认 `mods/Viridis/` 下直接放着同一测试包里的三个文件 `Viridis.dll`、`Viridis.json` 和 `Viridis.pck`。更新旧版时一起替换这三个文件。启动游戏后，在模组列表启用 Viridis。

## 第一次玩小松绿

- **看收获时间。** 选中的原牌从原牌堆移出，放入栽培牌堆。等待时间按基础能量费用计算，最短 1 回合、最长 3 回合，X 费牌按 1 回合计算。你的回合开始时倒计时减 1。归零时，按收获规则生成新牌加入手牌。普通收获通常生成高一级稀有度的固定费用牌，手牌满时产物进入弃牌堆。
- **挑一张值得种的牌。** 在本试验分支中，栽培 2 费的「树脂护壳」，收获的牌首次打出至多 1 费。栽培 3 费的「主干」，优先从 2、3 费牌中随机收获一张，首次打出 0 费。此时打出升级的「春化」，可以当场收获。可收获的先古牌会生成两张同名牌。
- **把收获提前到这一回合。** 如果栽培库里有一张牌只差 1 回合，使用「生长药剂」就能立刻收获。若佩戴「初收护符」，且这是本场第一次收获，还会同时获得 8 点格挡。
- **留意当前同接。** 「枼绿素出击」的伤害等于当前同接。手里还有能增加同接的牌时，出牌顺序会直接影响这一击的伤害。

完整的栽培规则、特殊产物与卡牌设计可查阅[栽培机制说明](design/栽培机制Spec.md)和[小松绿卡牌设计手册](design/小松绿-卡牌设计手册.md)。

已知显示问题：个别事件文本中的角色称谓可能显示为本地化键名。相关字段见[角色文本](mods/Viridis/assets/localization/zhs/characters.json)。

## 从源码构建

在 Windows 上准备 PowerShell 7、.NET 9 SDK、Python 3，以及上述游戏和 RitsuLib。构建脚本会优先查找仓库根目录的 `.dotnet/`，也支持 PATH 中的 SDK。在仓库根目录运行以下命令。提示输入路径时，选择安装了游戏和 RitsuLib 的 Steam 库中的 `steamapps` 文件夹。

```powershell
$steamApps = Read-Host 'Steam 库的 steamapps 文件夹'
$gameDir = Join-Path $steamApps 'common\Slay the Spire 2'

python tools/check_loc.py
.\mods\Viridis\build.ps1 -Deploy `
  -Sts2Dir (Join-Path $gameDir 'data_sts2_windows_x86_64') `
  -RitsuLibDir (Join-Path $steamApps 'workshop\content\2868840\3747602295') `
  -ModsDir (Join-Path $gameDir 'mods')
```

`-Deploy` 会将三个运行文件复制到游戏的 `mods/Viridis/`。只需构建时可省略该参数，DLL 输出到 `mods/Viridis/bin/Release/net9.0/`，PCK 输出到 `mods/Viridis/bin/`，清单使用 [Viridis.json](mods/Viridis/Viridis.json)。

## 开发与美术资料

| 要做的事 | 从这里开始 |
| --- | --- |
| 修改卡牌、遗物、药水或栽培结算 | [MOD 源码](mods/Viridis/) |
| 调整卡图、头像、地图标记和 UI 图标 | [图片尺寸与接入步骤](design/IMAGE_ASSETS.md) |
| 修改持铲动作，从关键帧重建游戏资源 | [战斗动画制作说明](mods/Viridis/art_source/characters/viridis/shovel-fullbody/README.md) |
| 查阅游戏数据或定位原版参考图 | [调研资料](refs/README.md) · [美术索引](ART_INDEX.md) |

改动通过 PR 合入 `main`。CI 校验清单和本地化，并打包、检查 PCK；Release DLL 需要本机游戏和 RitsuLib 程序集才能构建。提交时保留源文件，构建生成的 DLL、PCK 和测试包留在本地。提取的游戏原画与反编译源码仅供本地开发参考，请勿传播。
