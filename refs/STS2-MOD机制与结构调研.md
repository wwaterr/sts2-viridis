# 杀戮尖塔 2 MOD 机制与结构调研

> 记录时间：2026-11-07 ｜ 游戏 v0.111.0（commit 41cef1ea，Steam 分支 `public-beta`）
> 引擎 Godot 4.5.1 (.NET 9) ｜ AppID 2868840
> 动画部分单独一份：[STS2-角色动画Spine调研.md](STS2-角色动画Spine调研.md)

---

## 1. 文件到底存在哪

Steam 安装路径以安装该游戏的库目录为起点。

| 内容 | 路径 |
|---|---|
| 游戏本体 | `steamapps/common/Slay the Spire 2/` |
| **创意工坊 MOD** | `steamapps/workshop/content/2868840/<PublishedFileID>/` |
| 工坊订阅清单 | `steamapps/workshop/appworkshop_2868840.acf` |
| 本地（非工坊）MOD | `steamapps/common/Slay the Spire 2/mods/<任意子目录>/` |
| MOD 启用状态 / 加载顺序 | `%APPDATA%\SlayTheSpire2\steam\<SteamID>\settings.save` → `mod_settings.mod_list` |
| 加载日志（排错必备） | `%APPDATA%\SlayTheSpire2\logs\godot.log` |
| Modded 存档（独立于原版） | `%APPDATA%\SlayTheSpire2\steam\<SteamID>\modded\` |

要点：**加载器会递归扫描 `mods/` 和每个工坊物品目录，凡目录里出现 `*.json` 清单就当成一个 MOD**（日志原文：`Found mod manifest file ...`）。工坊目录结构与本地 `mods/` 完全一致，可直接当参考工程。

本机现有 16 个工坊物品、约 921 MB。

---

## 2. 已安装 MOD 一览（本机）

| ID | 类型 | 说明 |
|---|---|---|
| Koishi | 角色 | 古明地恋，含新先古之民，依赖自研 EcoLib |
| Remilia | 角色 | 蕾米莉亚，依赖 RitsuLib |
| ThirtySevenCharacterMod | 角色 | 37，依赖 RitsuLib（结构最干净，推荐精读） |
| figure_Saya | 角色 | 沙耶 |
| ATA_IronClad | 皮肤/换皮 | 铁甲战士美化（模型+卡图替换） |
| Mechaneko_Defect | 皮肤 | 故障机器人皮肤 |
| OrchisNecrobinderSkinMod | 皮肤 | 亡灵契约师 Spine 替换 |
| regentSkin / silentSkin | 皮肤 | 储君 / 静默猎手立绘 |
| RegentCardsAnimeRework / NecrobinderCardPortraits / NSFW_SilentCards | 卡图 | 纯资源替换 |
| RegentFX | 特效 | 卡面特效还原 |
| NecrobinderOstyAnimMod | 动画 | 奥斯提同步攻击动画 |
| RestSiteFondleCursorMod | 杂项 | 篝火光标 |
| NSFW-Event | 文本/图片 | 事件替换 |
| STS2-RitsuLib | 框架库 | 0.6.10，角色/内容脚手架 |
| SignatureLib / KoishiSignaturePack | 框架/扩展 | 卡牌签名 |

角色 MOD 三个参考样本：`Koishi`（重度，自研框架）、`Remilia`（RitsuLib）、`ThirtySevenCharacterMod`（RitsuLib，最适合作模板）。

---

## 3. 一个 MOD 的物理结构

```
<ModId>/
├── <ModId>.json      # 清单，必须
├── <ModId>.pck       # Godot 资源包（图/场景/本地化），可选
└── <ModId>.dll       # C# 程序集，可选
```

清单字段（实测，注意 `ModManifest` 里其实**没有** `pck_name`，写了也会被忽略）：

```json
{
  "id": "ThirtySevenCharacterMod",
  "name": "ThirtySeven Mod",
  "author": "...",
  "description": "...",
  "version": "1.2.3",
  "min_game_version": "0.110.0",
  "has_pck": true,
  "has_dll": true,
  "affects_gameplay": true,
  "dependencies": [ { "id": "STS2-RitsuLib", "min_version": "0.5.2" } ]
}
```

依赖会被拓扑排序后先加载（`ModManager.SortModList`）。文件名按 `<id>` 命名最稳。

---

## 4. 代码侧：游戏提供的 MOD API

游戏自带 `data_sts2_windows_x86_64\sts2.dll`（9.7 MB）、`sts2.xml`（5.6 MB，XML 注释）、`0Harmony.dll`。

### 4.1 入口签名（已用元数据确认真实签名）

```csharp
[ModInitializer("Initialize")]      // 参数 = 要调用的方法名
public class ModEntry
{
    public static void Initialize() { }   // 必须 static / void / 无参
}
```

- 没有 `[ModInitializer]` 的 DLL 会被当作 Harmony 补丁集，自动 `Harmony.PatchAll`。
- 日志：`MegaCrit.Sts2.Core.Logging.Log.Info(string text, int skipFrames = 2)`，另有 `Debug/Warn/Error/Load/VeryDebug`，以及实例化的 `Logger`。

### 4.2 注册内容

- `ModHelper.AddModelToPool<TPool, TModel>()` —— 把卡/遗物/药水塞进对应池
- `ModHelper.SubscribeForRunStateHooks(id, del)` / `SubscribeForCombatStateHooks(id, del)`
- `ModHelper.ConcatModelsFromMods(...)`、`IterateAllRunStateSubscribers(...)`
- 本地化：`ModManager.GetModdedLocTables("zhs", "cards.json")`，MOD 在 pck 里放 `localization/zhs/*.json` 按文件名合并

### 4.3 关键命名空间

`MegaCrit.Sts2.Core.Models`（CharacterModel / CardModel / RelicModel / PowerModel / Pools）、`Entities`、`Commands`、`Runs`、`Combat`、`Localization`、`Nodes`（Godot 节点）、`Bindings.MegaSpine`（见动画文档）。

`sts2.xml` 注释覆盖不全（`CharacterModel` 只有 2 个成员有注释），实际开发配合 **ILSpy / dnSpy 反编译 `sts2.dll`** 查签名。

### 4.4 两个社区框架

| 框架 | 作者 | 特点 |
|---|---|---|
| **BaseLib** | Alchyr | 老牌，配套 `dotnet new` 模板（Character 模板开箱即用） |
| **RitsuLib** | BAKAOLC | 本机已装 0.6.10；`[RegisterCharacter]`、`ModCharacterTemplate<T>`、`RegisterCharacterStarterCard/Relic`、角色视觉脚手架、自动注册 |

RitsuLib 写法示意（来自其文档）：

```csharp
[RegisterCharacter]
public sealed class MyCharacter : ModCharacterTemplate<MyCardPool, MyRelicPool, MyPotionPool> { ... }

[RegisterCharacterStarterCard(typeof(MyCharacter), 5)]
public sealed class MyStrike : ModCardTemplate { ... }
```

> RitsuLib 版本核对（2026-11-07）：GitHub 最新正式版 v0.6.10、NuGet `STS2.RitsuLib` 0.6.10、本机 0.6.10，三者一致；自带 `compat/0.111.0` 正好对应本机游戏版本。

---

## 5. 角色 MOD 的内容组成（以 ThirtySeven 为样本）

pck 内路径（`res://` 之外还有一份去掉前缀的副本）：

```
<ModId>/images/cards/<CardName>.png
<ModId>/images/powers/<PowerName>.png
<ModId>/images/relics/<RelicName>.png
<ModId>/images/potions/<PotionName>.png
<ModId>/images/characters/
    <ModId>_character_icon.png / _icon_outline.png
    <ModId>_character_play.png
    <ModId>_character_select.png / _select_locked.png
    <ModId>_select_bg.png / _select_character.png
    <ModId>_map_marker.png
    <ModId>_energy_orb_layer_1..5.png
    energy_big.png / energy_text.png
    multiplayer_hand_*.png
<ModId>/images/transitions/<ModId>_transition_function.png
<ModId>/scenes/characters/
    <ModId>_character.tscn            # 战斗内角色
    <ModId>_character_select_bg.tscn
    <ModId>_energy_counter.tscn
    <ModId>_icon.tscn
    <ModId>_merchant.tscn             # 商店立绘
    <ModId>_rest_site.tscn            # 篝火立绘
<ModId>/spine/*.tres + *.png          # Spine 骨骼（见动画文档）
<ModId>/localization/{eng,zhs}/*.json
<ModId>/audio/<ModId>_select_voice.mp3
<ModId>/materials/*.tres  <ModId>/shaders/*.gdshader
```

本地化文件名（固定这 10 个）：`characters.json`、`cards.json`、`powers.json`、`relics.json`、`potions.json`、`enchantments.json`、`events.json`、`ancients.json`、`card_keywords.json`、`static_hover_tips.json`。

代码工程目录（对应 `<Mod>Code/`）：

```
Characters/  Cards/  Powers/  Relics/  Potions/  Enchantments/  Events/
Capabilities/  Keywords/  Patches/  Utils/  Systems/
```

> Koishi / Remilia 的 pck 里还塞了 `.cs` 源码与 Roslyn 依赖（`PortableRuntimeIdentifierGraph.json`），属于运行时编译/热重载的开发者用法，不是必需。

---

## 5.5 API 踩坑记录（实测，2026-11-07 原型验证）

做栽培机制原型时踩到并解决的坑，都是"看文档绝对看不出来"的：

| 现象 | 真实原因 | 正确做法 |
|---|---|---|
| `KeyNotFoundException: 'CARD_POOL.IRONCLAD_CARD_POOL'` | `[ModInitializer]` 阶段 **ModelDb 还没初始化** | 初始化函数里禁止调用任何 `ModelDb.*` |
| `DuplicateModelException: ID POWER.XXX` | mod 程序集里的 Power 类型**已被自动扫描注册** | 不要 `new`；用 `PowerCmd.Apply<TPower>(...)` 泛型，让引擎克隆实例 |
| `CanonicalModelException: ... used in incorrect place` | 牌池 `AllCards` 返回的是**不可变规范实例** | 拿实例前不能直接 `CreateClone` |
| `NullReferenceException at CreateClone` | `CreateClone` 内部是 `this.CardScope.CloneCard(this)`，规范卡不在战斗牌堆 → CardScope 为 null | 用 `ICombatState.CreateCard(canonical, player)` 生成战斗实例 |
| 回合开始钩子里抛异常 → **结束回合按钮消失、无法出牌** | 异常从 `AfterPlayerTurnStart` 冒到 `CombatManager` 的 turn loop，整场战斗死锁 | 钩子内所有逻辑必须 try/catch 包住 |
| 栽培诅咒后收获为空 | 状态/诅咒/任务/衍生牌在**独立牌池** | 查询要并上 `StatusCardPool`/`CurseCardPool`/`QuestCardPool`/`TokenCardPool` |
| 用了 `[ModInitializer]` 但 Harmony 没生效 | 有该特性时加载器**不会**自动 `PatchAll` | 在初始化函数里手动 `new Harmony(id).PatchAll(asm)` |
| 测试卡刷不到 | 只进卡池要等随机奖励 | Harmony 补丁改 `CharacterModel.StartingDeck`（属性 getter，如 `Ironclad.get_StartingDeck`） |
| 补丁挂 `NCombatPileCardSelectScreen._Ready` 完全不执行 | Godot 回调 `_Ready` 在**每个子类各自重写**，子类不会调用 `base._Ready()` | 改挂所有选牌界面共用、且子类未重写的 `NCardGridSelectionScreen.CardsSelected()` |
| `Harmony.PatchAll(Type)` 不存在（只有 `PatchAll(Assembly)`） | HarmonyX 的 API 限制 | 用 `harmony.CreateClassProcessor(type).Patch()` 逐类型打补丁，一个失败不影响其它 |
| 补丁类里一处报错导致其它补丁全失效 | `PatchAll` 是整批处理，抛异常即中断 | 逐类型 try/catch |
| `CardPlay.ResultPile` 无法赋值（CS8852） | 属性是 `init`-only | 反射写 `<ResultPile>k__BackingField`；或事后 `CardPileCmd.Add(card, PileType.Hand)` |
| 想退回能量但不知道玩家实际花了多少 | — | `cardPlay.Resources.EnergySpent` 记录实际花费 |
| `MutableModelException: Mutable model ... used in incorrect place` + 开局卡死 | `CharacterModel.StartingDeck` 只接受**规范（不可变）模型**，引擎自己克隆 | 不要往 StartingDeck 塞 `ToMutable()`；改用 `Hook.BeforeCombatStart` 在抽牌堆里升级 |
| 手牌选择没有可取消的界面 | 手牌选择不是独立界面，而是 `NPlayerHand.SelectCards` 在原手牌上做的 | 相关节点：`%SelectModeBackstop`、`%SelectModeConfirmButton`、`%SelectionHeader`；完成/取消靠 `_selectionCompletionSource` |

有用的 API：

- `PlayerCombatState.Hand/DrawPile/DiscardPile/ExhaustPile` → `CardPile.Cards`
- `CardPileCmd.RemoveFromCombat(card)` / `AddGeneratedCardToCombat(card, PileType, player)` / `Add(...)`
- `PowerCmd.Apply<T>(ctx, creature, amount, applier, cardSource)`
- `Creature.GetPower<T>()`
- `CardEnergyCost.Canonical`（原始费用）、`AddUntilPlayed(offset, reduceOnly)`（首次打出前改费）
- `CardRarity` 枚举 + `CardRarityExtensions.GetNextHighestRarityWithWrapping()`（顺序与自定义阶梯不同，慎用）
- `AbstractModel.AfterPlayerTurnStart(choiceContext, player)` —— 任何 Model（Power/遗物）都能重写
- `CardKeyword.Innate` —— 开局必进手牌，调试用

---

## 6. 环境与工具链

- 游戏运行时是 .NET 9 → 项目目标框架 `net9.0`。
- 可在仓库根目录的 `.dotnet/` 安装独立的 .NET 9 SDK。
- Godot 4.5.1：只在编辑 `.tscn`、导入图片、打包 `.pck` 时需要；纯 DLL 逻辑 MOD 可以不装。
- 打包 pck：Godot 工程 `--export-pack`，或社区 pck 工具。
- 启动参数 `nomods` 可禁用全部 MOD；游戏目录放 `steam_appid.txt`（内容 `2868840`）可脱离 Steam 启动。
- 一旦启用 MOD，游戏切到 Modded 模式：独立存档槽、成就/统计上传关闭、联机要求双方 MOD 列表一致。

### 已建好的空 MOD 工程（可直接开工）

```
mods/EmptyCharacterMod/
├── EmptyCharacterMod.csproj   # net9.0，引用 sts2.dll（Private=false）
├── EmptyCharacterMod.json     # 清单
├── ModEntry.cs                # [ModInitializer(nameof(Initialize))]
├── build.ps1                  # 编译，-Deploy 直接装进游戏 mods/
└── README.md
```

```powershell
Set-Location mods/EmptyCharacterMod
$gameDir = Read-Host '游戏安装目录'
$dataDir = Join-Path $gameDir 'data_sts2_windows_x86_64'
.\build.ps1 -Deploy -Sts2Dir $dataDir
# 验证
Select-String -Path "$env:APPDATA\SlayTheSpire2\logs\godot.log" -Pattern "EmptyCharacterMod"
```

---

## 7. 建议的上手路线

1. **跑通空 MOD**（已完成，见上）——确认列表可见、日志无报错。
2. **最小角色闭环**：角色模型 + 卡池/遗物池/药水池 + 初始牌组与初始遗物 + 选人界面 + 图标/立绘/能量球 + 中文英文本地化。
3. **再做差异化**：自定义关键词、能力（Power）、卡牌效果、事件/先古之民、Spine 动画。
4. 参考顺序：`ThirtySeven`（结构）→ `Remilia`（RitsuLib 用法）→ `Koishi`（大型角色分模块）。

---

## 8. 参考链接

- BaseLib 模板：https://github.com/Alchyr/ModTemplate-StS2
- BaseLib Wiki：https://alchyr.github.io/BaseLib-Wiki/
- RitsuLib：https://github.com/BAKAOLC/STS2-RitsuLib
- RitsuLib 模板：https://github.com/NothingFumo/RitsuLibModTemplate
- 社区教程合集：https://github.com/GlitchedReme/SlayTheSpire2ModdingTutorials
- 最小示例：https://github.com/jiegec/STS2FirstMod
- Harmony：https://harmony.pardeike.net/articles/intro.html
- Godot 文档（中文）：https://docs.godotengine.org/zh-cn/4.x/

---

## 附：本目录数据文件

| 文件 | 内容 |
|---|---|
| `raw/ThirtySevenCharacterMod.pck.paths.txt` | 该 mod 的 pck 全部路径（结构范本） |
| `raw/Remilia.pck.paths.txt`、`raw/Koishi.pck.paths.txt` | 另两个角色 mod 的路径 |
| `raw/vanilla_ironclad_scene.txt` | 原版铁甲战士 Spine 数据资源与动画混合表 |
| `raw/vanilla_spine_scan.txt` | 原版 pck 中 Spine 资产命名与导入器 |
| `tools/pck_scrape.py` | 从 pck 抽取路径清单的脚本（可复用于其它 pck） |