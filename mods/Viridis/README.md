# Viridis（小松绿）角色 MOD — 框架版

角色框架已跑通：角色 + 卡池/遗物池/药水池 + 10 张空白初始卡 + 中英文本地化。

## 结构

```
Viridis/
├── Viridis.csproj               # net9.0，引用 sts2.dll + RitsuLib
├── Viridis.json                 # 清单（依赖 STS2-RitsuLib）
├── ModEntry.cs                  # [ModInitializer]，只打日志
├── Characters/ViridisCharacter.cs
├── Pools/ViridisPools.cs        # 卡池 / 遗物池 / 药水池
├── Cards/BlankCard.cs           # 空白卡（初始牌组 10 张由特性生成）
├── assets/localization/{zhs,eng}/{characters,cards}.json
└── build.ps1                    # 编译 + 打包 pck + 部署
```

编译部署：

```powershell
.\build.ps1 -Deploy
```

产物：`Viridis.dll` + `Viridis.json` + `Viridis.pck` → 游戏 `mods\Viridis\`。

## 关键约定

- **卡牌本地化键**：`<MOD大写>_CARD_<类名大写下划线>.title/.description`
  例：类 `BlankCard` → `VIRIDIS_CARD_BLANK_CARD.title`
- **角色本地化键**：`<MOD大写>_CHARACTER_<角色类名大写>.title/.description/.flavor/...`
  例：`VIRIDIS_CHARACTER_VIRIDIS_CHARACTER.title`
- **注册方式**：类上加 `[RegisterCharacter]` / `[RegisterCard(typeof(ViridisCardPool))]`，
  初始牌用 `[RegisterCharacterStarterCard(typeof(ViridisCharacter), 数量)]`，RitsuLib 自动扫描。
- **pck 路径**：`Viridis/localization/zhs/cards.json`（无 `res://` 前缀，运行时挂在 res:// 下）。

## 当前占位

- 视觉：`PlaceholderCharacterId = "IRONCLAD"`，直接复用原版铁甲战士的立绘/场景。
  换成自己的图需要 Godot 导入成 `.ctex` 再打进 pck（见下）。
- 卡面：空白卡没有自定义立绘，用原版卡框。

## 加美术（下一步）

1. 装 Godot 4.5.1，新建工程，把纯黑 PNG 放进 `Viridis/images/...` 让其生成 `.import` + `.godot/imported/*.ctex`。
2. 用 `tools/pck_pack.py`（或 Godot 的导出）把整个工程目录打成 pck。
3. 角色类里改成 `CustomVisualsPath` / `AssetProfile` 指向自己的场景与贴图。

## 已知未验证

- 尚未在游戏里实际加载（首次进游戏需在 MOD 列表里打开本 MOD）。
- 本地化键名是按真实 MOD 反推的，若显示为原始键名，说明字段名要调。