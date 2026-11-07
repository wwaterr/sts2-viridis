# 杀戮尖塔 2 角色 MOD —— 资料索引

调研基于本机实际安装（游戏 v0.111.0，Steam 分支 `public-beta`，AppID 2868840）。

## 文档

| 文档 | 内容 |
|---|---|
| [STS2-MOD机制与结构调研.md](STS2-MOD机制与结构调研.md) | 文件位置、MOD 清单格式、加载流程、C# API 与两个框架、角色内容组成（图/场景/本地化）、工具链、上手路线 |
| [STS2-角色动画Spine调研.md](STS2-角色动画Spine调研.md) | Spine 版本约束、原版资产结构、5 个战斗动画与过渡表、场景节点、`MegaSpine` 接口、打包注意、替代方案、验收清单与常见坑 |

## 工程

| 路径 | 说明 |
|---|---|
| [`../mods/EmptyCharacterMod`](../mods/EmptyCharacterMod) | 已跑通的空 MOD（清单 + DLL + 构建脚本），`build.ps1 -Deploy` 可直接装进游戏 |
| [`../tools/pck_scrape.py`](../tools/pck_scrape.py) | 从 Godot `.pck` 抽取资源路径清单 |
| [`../tools/ApiDump`](../tools/ApiDump) | 反查 `sts2.dll` / RitsuLib 程序集公开 API 的小工具 |
| `../.dotnet` | 工作区内的 .NET 9 SDK（9.0.320），未安装到系统 |

## raw/ 原始数据

| 文件 | 内容 |
|---|---|
| `ThirtySevenCharacterMod.pck.paths.txt` | 角色 MOD 的 pck 全量路径（结构范本，930 条） |
| `Remilia.pck.paths.txt` / `Koishi.pck.paths.txt` | 另两个角色 MOD 的路径清单 |
| `vanilla_ironclad_scene.txt` | 原版铁甲战士 Spine 数据资源、全部动画名与混合表 |
| `vanilla_spine_scan.txt` | 原版 pck 中 Spine 资产命名与 `spine.skel/.atlas` 导入器 |
| `vanilla_character_paths.txt` | 原版角色动画目录命名 `animations/characters/<id>/...` |
| `spine_version.txt` | 运行时版本字面量 `4.2.36` 的出现位置与上下文 |
| `spine_scan.txt` | mod pck 内 Spine 资源引用上下文 |
| `vanilla_scene_paths.txt` | 原版场景路径探测（无结果，保留作记录） |
| `loc_character.txt` / `loc_keys.txt` | 37 本地化键名片段（扫描残留） |