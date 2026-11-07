# 实测查询结果

总行数: 596

## 1) 单卡查询：痛击 (Bash)
```
id         = BASH
class      = Bash
pools      = ironclad
cost       = 2
type       = Attack
rarity     = Basic
target     = AnyEnemy
title_zh   = 痛击
desc_zh    = 造成{Damage:diff()}点伤害。
给予{VulnerablePower:diff()}层[gold]易伤[/gold]。
vars       = new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(8m, ValueProp.Move), new PowerVar<VulnerablePower>(2m) })
upgrade    = DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars.Vulnerable.UpgradeValueBy(1m);
```

## 2) 跨职业检索：所有 0 费攻击牌（前 12 张）

| 卡名 | id | 职业 | 稀有度 | 变量 |
|---|---|---|---|---|
| 流星锤 | `BOLAS` | colorless | Rare | new[] { new DamageVar(3m, ValueProp.Move)) |
| 闪亮登场 | `DRAMATIC_ENTRANCE` | colorless | Uncommon | new[] { new DamageVar(11m, ValueProp.Move)) |
| 亮剑 | `FLASH_OF_STEEL` | colorless | Uncommon | new Array<DynamicVar>(new DynamicVar[2] { new Dama |
| 万向斩 | `OMNISLICE` | colorless | Uncommon | new[] { new DamageVar(8m, ValueProp.Move)) |
| 连射 | `VOLLEY` | colorless | Uncommon | new[] { new DamageVar(10m, ValueProp.Move)) |
| 光束射线 | `BEAM_CELL` | defect | Common | new Array<DynamicVar>(new DynamicVar[2] { new Dama |
| 爪击 | `CLAW` | defect | Common | new Array<DynamicVar>(new DynamicVar[2] { new Dama |
| 超越光速 | `FTL` | defect | Uncommon | new Array<DynamicVar>(new DynamicVar[3] { new Dama |
| 眼部攻击 | `GO_FOR_THE_EYES` | defect | Common | new Array<DynamicVar>(new DynamicVar[2] { new Dama |
| 螺旋钻击 | `HELIX_DRILL` | defect | Rare | new Array<DynamicVar>(new DynamicVar[4] { new Dama |
| 特斯拉线圈 | `TESLA_COIL` | defect | Uncommon | new[] { new DamageVar(3m, ValueProp.Move)) |
| 异鸟扑击 | `BYRD_SWOOP` | event | Event | new[] { new DamageVar(14m, ValueProp.Move)) |

匹配总数: 47

## 3) 关键词检索：中文描述里含「中毒」

| 卡名 | 职业 | 费用 | 描述 |
|---|---|---|---|
| 触媒 | silent | 1 | [gold]中毒[/gold]会额外触发{Accelerant:diff()}次。 |
| 弹跳药瓶 | silent | 2 | 随机给予敌人{PoisonPower:diff()}层[gold]中毒[/gold]{Repeat:diff()}次。 |
| 咕嘟冒泡 | silent | 1 | 如果敌方拥有[gold]中毒[/gold]，则给予{PoisonPower:diff()}层[gold]中毒[/gold]。 |
| 调制 | silent | 0 | 选择另一名玩家，他在本回合内造成未被格挡的伤害时，给予{ConcoctPower:diff()}层[gold]中毒[/gold]。 |
| 腐蚀波 | silent | 1 | 打出此牌后，你在本回合每抽到一张牌，就给予所有敌人{CorrosiveWave:diff()}层[gold]中毒[/gold]。 |
| 致命毒药 | silent | 1 | 给予{PoisonPower:diff()}层[gold]中毒[/gold]。 |
| 涂毒 | silent | 2 | 每有一次攻击造成未被格挡的伤害，就给予{EnvenomPower:diff()}层[gold]中毒[/gold]。 |
| 迷雾 | silent | 2 | 给予所有敌人{PoisonPower:diff()}层[gold]中毒[/gold]和{WeakPower:diff()}层[gold]虚弱 |

匹配总数: 13

## 4) 实现源码：Bash 的 OnPlay
```csharp
ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target"); await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target) .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3") .Execute(choiceContext); await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
```