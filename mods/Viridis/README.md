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

## 图片资产

小松绿使用持铲战斗场景，带呼吸待机和平刺。选人肖像、顶栏头像、地图标记、卡图与 UI 图标使用各自的资源路径。逐项尺寸、透明通道、参考图输入方式、转换命令和打包路径见[图片资产规格](../../design/IMAGE_ASSETS.md)。

## 已知未验证

- 尚未在游戏里实际加载（首次进游戏需在 MOD 列表里打开本 MOD）。
- 本地化键名是按真实 MOD 反推的，若显示为原始键名，说明字段名要调。
