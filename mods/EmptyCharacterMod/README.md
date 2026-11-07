# EmptyCharacterMod（空 MOD 脚手架）

一个**只有清单 + DLL** 的最小可加载 MOD，用来验证编译/部署/加载链路。没有任何内容。

## 结构

```
EmptyCharacterMod/
├── EmptyCharacterMod.csproj   # net9.0，引用游戏的 sts2.dll
├── EmptyCharacterMod.json     # MOD 清单（游戏靠它识别）
├── ModEntry.cs                # [ModInitializer] 入口
└── build.ps1                  # 编译（+ 可选部署）
```

## 编译

```powershell
# 只编译
.\build.ps1

# 编译并直接拷进游戏 mods 目录
.\build.ps1 -Deploy
```

默认引用 `D:\all\steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll`，
换机器/换目录时：

```powershell
.\build.ps1 -Deploy -Sts2Dir "X:\...\data_sts2_windows_x86_64" -ModsDir "X:\...\Slay the Spire 2\mods"
```

## 验证是否加载成功

1. 启动游戏 → 设置里的 MOD 列表应出现 `Empty Character Mod`，打开开关。首次开启会提示切换到 Modded 存档。
2. 到日志里搜一行即可：

```powershell
Select-String -Path "$env:APPDATA\SlayTheSpire2\logs\godot.log" -Pattern "EmptyCharacterMod"
```

看到 `[EmptyCharacterMod] Initialize() called - empty mod loaded OK.` 就说明整条链路通了。

## 目录约定（后续加内容时）

```
<ModId>.json   # 清单：id/name/version/has_pck/has_dll/dependencies/min_game_version
<ModId>.pck    # Godot 资源包（图、tscn、本地化 json、Spine）
<ModId>.dll    # 本工程产物
```

清单里的 `pck_name` 字段游戏其实不读（`ModManifest` 里没有），文件名按 `<id>` 命名最稳。

## 下一步

- 加角色：用 RitsuLib（已装 0.6.10）的 `[RegisterCharacter]` / `ModCharacterTemplate<T>`，
  或 BaseLib 的 Character 模板。
- 加资源：做一个 Godot 4.5 工程，按 `images/`、`scenes/characters/`、`localization/zhs/` 组织后导出 `.pck`。