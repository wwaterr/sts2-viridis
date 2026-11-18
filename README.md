# Viridis（小松绿）— Slay the Spire 2 角色 MOD

一个《杀戮尖塔 2》的自定义角色 MOD：角色 + 卡池 / 遗物池 / 药水池 + 中英文本地化，
配套的调研资料、构建工具与美术参考都放在同一个仓库里。

当前状态：102 张卡（含 2 张先古）、栽培关键词、初始遗物与欧洛巴斯升级已接入；
角色立绘仍复用铁甲战士占位，两张先古卡图已使用自制素材。
新增卡如果没有专属卡图，继承 `Cards/PlaceholderArtCard`，默认使用 `images/cards/card_placeholder.ctex`；已有卡牌保持原样。

## 最近更新（相对 0.2.1）

- 栽培：非先古产物只从 `ViridisCardPool` 选，诅咒产出的状态从全状态池选，先古牌仍收同名 ×2；X>1 的产物费用固定降为 1。
- 关键词：注册「栽培」「永久栽培」两个模组关键词，18 张栽培动作卡在描述内用金色文字标记。
- 初始卡组：落叶改为微小种子，育苗改为种植；打击/防御补上升级（6→9、5→8）。
- 遗物：初始遗物改名园丁手套，新增欧洛巴斯之触升级园艺工具。
- 先古牌：移除世界树/始祖孢子，新增小松绿（古老牙齿转化）与山绿篱（尘封魔典）。
- 兼容：给尘封魔典加直接发放兜底，避免调试工具绕过事件时拿到空 `AncientCard`。
- 兼容：适配《杀戮尖塔 2》v0.113.1 与 RitsuLib 0.6.14（compat 目标 0.113.0）。
- 卡牌：新增钢镚友好直播间（0 费罕见技能）、最后一首（3 费稀有技能）与衍生牌天籁；新增「黏住」附魔。
- 卡牌：新增慢启动歌回（3 费稀有能力，感染→天籁的递增转化）；小松绿卡池卡框改为 `#8eb056`。
- 卡牌：新增初始攻击枼绿素出击（0 费，伤害等同当前同接，升级后保留）；初始牌组五张基础牌统一改用「图片暂缺」占位卡图。
- 卡牌：新增同接体系卡开播！、突击直播！、呼呼大睡；后两张在回合内打出过牌后不可打出。
- 体系：考子、反踩、串子注册为关键词并配状态图标；钢镚友好直播间改为同接每增加 1 点获得 2 金币；突击直播！与呼呼大睡降为罕见。
- 能量图标：小松绿卡池使用专属绿叶图标，带浅绿描边（卡面 74×74，富文本 24×24），由 `ViridisCardPool` 的 `BigEnergyIconPath` / `TextEnergyIconPath` 提供。
- 战斗能量球：改用自定义 `viridis_energy_counter.tscn`，以 128×128 叶片图标替代原版铁甲能量球。

已知问题：`characters.json` 仍缺 `titleObject`、`pronounObject`、`pronounPossessive`、`pronounSubject` 四个键，RitsuLib 会以键名占位。

## 仓库结构

| 路径 | 说明 |
|---|---|
| `mods/Viridis/` | **MOD 本体**（C# 源码、清单、本地化、构建脚本） |
| `mods/EmptyCharacterMod/` | 最小可跑通的空角色 MOD，当模板用 |
| `mods/CultivatePrototype/` | 栽培机制的独立原型 |
| `design/` | 玩法设计文档（`栽培机制Spec.md`） |
| `tools/` | 逆向 / 打包 / 数据提取脚本（PCK 解析、卡牌库构建、API dump 等） |
| `refs/` | 调研文档 + 游戏数据摘录（`db/`、`gamedata/`、`raw/`） |
| `ART_INDEX.md` | 美术资源抽取索引（尺寸、目录、缺口） |

## 不在仓库里的东西

为了体积和版权，以下内容**没有**进 Git，需要单独找我要：

| 内容 | 大小 | 说明 |
|---|---|---|
| `art/` | 415 MB | 从 `SlayTheSpire2.pck` 抽出的原画，可跑 `extract_sts2_art.py` 重新生成 |
| `refs/decompiled/` | 1.4 MB | 反编译的游戏程序集源码，属游戏本体内容，不外传 |
| `.dotnet/` | 726 MB | 工作区内置的 .NET 9 SDK，各自本地装 |

美术可以自己重新抽：

```powershell
python extract_sts2_art.py      # 需要本机装有游戏，见 ART_INDEX.md
```

## 环境准备

- Windows + PowerShell 7
- **.NET 9 SDK**：装到系统 PATH，或者解压到仓库根的 `.dotnet/`（`build.ps1` 会自动优先用它）
- **Slay the Spire 2**（Steam，beta 分支 `public-beta`，本篇基于 v0.113.1）
- **RitsuLib**：Steam 创意工坊物品 `3747602295`，v0.6.14，MOD 的运行依赖
- **Python 3**：打包本地化用的 `tools/pck_pack.py`

## 编译与部署

```powershell
cd mods\Viridis
.\build.ps1 -Deploy
```

产物：`Viridis.dll` + `Viridis.json` + `Viridis.pck` → 游戏的 `mods\Viridis\`。

路径默认值写死在 `Viridis.csproj` 里（我本机的 Steam 目录），**换机器要覆盖**：

```powershell
.\build.ps1 -Deploy `
  -Sts2Dir "D:\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64" `
  -RitsuLibDir "D:\Steam\steamapps\workshop\content\2868840\3747602295" `
  -ModsDir "D:\Steam\steamapps\common\Slay the Spire 2\mods"
```

## 协作约定

- `main` 保持可编译；改动走 `feat/xxx`、`fix/xxx` 分支，开 PR 合并。
- 提交信息用 `<类型>: <一句话>`（`feat` / `fix` / `docs` / `art` / `chore`）。
- 动 `mods/` 之前先 `git pull`，避免 `Viridis.csproj` 里本机路径互相覆盖。
- 二进制产物（`bin/`、`obj/`、`*.pck`、`*.dll`）一律不进库，由脚本生成。

## 自动检查

每次 push 到 `main` 或开 PR，`.github/workflows/check.yml` 会跑：

1. `python tools/check_loc.py` —— 清单字段、版本号格式、**中英文本键对齐**，以及每个
   `[RegisterCard]` / `[RegisterCharacter]` 类是否都有对应的 `title` / `description`
2. `python tools/pck_pack.py` 把 `mods/Viridis/assets` 打成 `Viridis.pck`，产物作为
   artifact 保留 7 天

CI **不编译 DLL**：编译要引用游戏自带的 `sts2.dll`、`GodotSharp.dll` 和工坊里的
RitsuLib，这些都不能进仓库、GitHub runner 上也拿不到。DLL 在本机 `build.ps1` 产出。

本地跑同样的检查：

```powershell
python tools/check_loc.py
```

## 版权说明

`art/` 与 `refs/decompiled/` 的内容来自《杀戮尖塔 2》游戏本体，仅作学习与 MOD 开发参考，
**不要公开传播**。本仓库保持私有；MOD 对外发布时仓库只含自己写的代码与美术。