# Viridis（小松绿）— Slay the Spire 2 角色 MOD

一个《杀戮尖塔 2》的自定义角色 MOD：角色 + 卡池 / 遗物池 / 药水池 + 中英文本地化，
配套的调研资料、构建工具与美术参考都放在同一个仓库里。

当前状态：角色框架已跑通（`mods/Viridis` 可编译、可部署），美术仍是占位（复用铁甲战士立绘）。

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
- **Slay the Spire 2**（Steam，beta 分支 `public-beta`，本篇基于 v0.111.0）
- **RitsuLib**：Steam 创意工坊物品 `3747602295`，MOD 的运行依赖
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