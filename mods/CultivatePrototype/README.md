# Cultivate Prototype

栽培机制的验证用 mod。目前只验证**链路**，不是最终效果。

## 已部署

```
<游戏目录>\mods\CultivatePrototype\
├── CultivatePrototype.dll
└── CultivatePrototype.json
```

## 怎么测

1. 启动游戏，主菜单能看到 `Cultivate Prototype`。
2. 开一局**铁甲战士**（新开局才生效，老存档不会变）。
3. **初始牌组里直接有 3 张 Cultivate (test)**，第一回合基本就能抽到。
4. 打出它 → 会**随机**埋掉手牌里的一张牌（原型先不做选牌 UI）。
5. 之后每回合开始倒计时减 1；归零时收获 1 张牌入手。

## 看日志

```powershell
Select-String -Path "$env:APPDATA\SlayTheSpire2\logs\godot.log" -Pattern "Cultivate" | Select-Object -Last 30
```

预期能看到：

```
[CultivatePrototype] loaded, CultivateTestCard registered into IroncladCardPool
[Cultivate] random target = ...
[Cultivate] planted ... cost=2 X=2 count=1
[Cultivate] harvested ... (X=2, ...)
```

## 验证进度（2026-11-07）

- [x] 卡牌注册进角色池
- [x] 开局获得测试卡（Harmony 改 `Ironclad.StartingDeck`）
- [x] 栽培动作：目标移出战斗 + 状态记录
- [x] 回合开始倒计时
- [x] 收获：按稀有度/费用筛选 → 生成 → 置入手牌
- [x] X>1 的费用折扣（`AddUntilPlayed`）
- [x] 选牌 UI（游戏原生手牌选择）
- [x] 升级继承（`CurrentUpgradeLevel` + `FinalizeUpgradeInternal`）
- [ ] 手牌满时溢出到弃牌堆
- [ ] 先古牌收获同名牌 ×2
- [ ] 本地化（卡名/描述/能力图标）

## 当前版本的已知限制

| 项 | 状态 |
|---|---|
| 选牌 UI | 已在用游戏原生手牌选择；取消则无效果 |
| 升级继承 | 已实现（收获牌恢复 `+` 状态） |
| 卡面文字 | 占位（描述为空，显示 loc key） |
| 先古 ×2 | 已实现（收同名牌 2 张），未验证 |
| 费用折扣 | 已实现并验证（X>1 收获牌首张 1 费） |
| 手牌满 | 依赖游戏的 `AddGeneratedCardToCombat` 行为，待验证 |

## 代码结构

| 文件 | 作用 |
|---|---|
| `ModEntry.cs` | 注册测试卡进铁甲池、逐类型打补丁 |
| `Cards/CultivateTestCard.cs` | 测试卡，打出后选手牌中的一张栽培 |
| `CultivateSystem.cs` | 栽培/收获核心逻辑、稀有度映射、池筛选 |
| `Powers/CultivateFieldPower.cs` | 场地能力，记倒计时，回合开始结算 |
| `Patches/IroncladStartingDeckPatch.cs` | 给初始牌组加测试卡 |
| `Patches/StarterUpgradePatch.cs` | 战斗开始时升级打击/痛击各一张（测试用） |
| `Patches/BashInnatePatch.cs` | 给测试用牌加固有 |

## 构建

```powershell
Set-Location mods/CultivatePrototype
$gameDir = Read-Host '游戏安装目录'
.\build.ps1 -Deploy -Sts2Dir $gameDir
```