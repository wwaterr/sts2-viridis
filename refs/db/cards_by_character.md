# 各职业卡表


## ironclad（90 张）

| 卡名 | id | 费 | 类型 | 稀有度 | 目标 | 变量 | 升级 |
|---|---|---|---|---|---|---|---|
| 破击 | `BREAK` | 1 | Attack | Ancient | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(20m, | DynamicVars.Damage.UpgradeValueBy(10m);  |
| 痛击 | `BASH` | 2 | Attack | Basic | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(8m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 打击 | `STRIKE_IRONCLAD` | 1 | Attack | Basic | AnyEnemy | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 愤怒 | `ANGER` | 0 | Attack | Common | AnyEnemy | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 全身撞击 | `BODY_SLAM` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | EnergyCost.UpgradeBy(-1); |
| 突破 | `BREAKTHROUGH` | 1 | Attack | Common | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(9m,  | DynamicVars.Damage.UpgradeValueBy(4m); |
| 余烬 | `CINDER` | 2 | Attack | Common | AnyEnemy | new[] { new DamageVar(18m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(6m); |
| 头槌 | `HEADBUTT` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(9m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 铁斩波 | `IRON_WAVE` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(5m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 熔融之拳 | `MOLTEN_FIST` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(10m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 完美打击 | `PERFECTED_STRIKE` | 2 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.ExtraDamage.UpgradeValueBy(1 |
| 剑柄打击 | `POMMEL_STRIKE` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(9m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 预备打击 | `SETUP_STRIKE` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(7m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 飞剑回旋镖 | `SWORD_BOOMERANG` | 1 | Attack | Common | RandomEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(3m,  | DynamicVars.Repeat.UpgradeValueBy(1m); |
| 闪电霹雳 | `THUNDERCLAP` | 1 | Attack | Common | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(4m,  | DynamicVars.Damage.UpgradeValueBy(3m); |
| 双重打击 | `TWIN_STRIKE` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(5m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 焚烧 | `CONFLAGRATION` | 1 | Attack | Rare | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(2m,  | DynamicVars.Repeat.UpgradeValueBy(1m); |
| 狂宴 | `FEED` | 1 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(10m, | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 恶魔之焰 | `FIEND_FIRE` | 2 | Attack | Rare | AnyEnemy | new[] { new DamageVar(7m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 凌虐 | `MANGLE` | 3 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(20m, | DynamicVars.Damage.UpgradeValueBy(6m); D |
| 午夜 | `MIDNIGHT` | 12 | Attack | Rare | AnyEnemy | new[] { new DamageVar(60m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(12m); |
| 契约终结 | `PACTS_END` | 0 | Attack | Rare | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(18m, | DynamicVars.Damage.UpgradeValueBy(6m); |
| 扯碎 | `TEAR_ASUNDER` | 2 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[5] { new DamageVar(5m,  | DynamicVars.Damage.UpgradeValueBy(2m); |
| 痛殴 | `THRASH` | 1 | Attack | Rare | AnyEnemy | new[] { new DamageVar(4m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 灰烬打击 | `ASHEN_STRIKE` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.ExtraDamage.UpgradeValueBy(1 |
| 重锤 | `BLUDGEON` | 3 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(32m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(10m); |
| 欺凌 | `BULLY` | 0 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.ExtraDamage.UpgradeValueBy(1 |
| 拆卸 | `DISMANTLE` | 1 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(8m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 与我一战！ | `FIGHT_ME` | 2 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DamageVar(5m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 御血术 | `HEMOKINESIS` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new HpLossVar(2m), | DynamicVars.Damage.UpgradeValueBy(5m); |
| 彼岸咆哮 | `HOWL_FROM_BEYOND` | 3 | Attack | Uncommon | AllEnemies | new[] { new DamageVar(18m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(6m); |
| 群情激愤 | `OUTRAGE` | 0 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(9m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 劫掠 | `PILLAGE` | 1 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 暴走 | `RAMPAGE` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(10m, | DynamicVars["Increase"].UpgradeValueBy(5 |
| 怨恨 | `SPITE` | 0 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(5m,  | DynamicVars.Repeat.UpgradeValueBy(1m); |
| 踩踏 | `STOMP` | 3 | Attack | Uncommon | AllEnemies | new[] { new DamageVar(12m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 无情猛攻 | `UNRELENTING` | 2 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(14m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(6m); |
| 上勾拳 | `UPPERCUT` | 2 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(13m, | DynamicVars["Power"].UpgradeValueBy(1m); |
| 旋风斩 | `WHIRLWIND` | 0 | Attack | Uncommon | AllEnemies | new[] { new DamageVar(5m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 腐化 | `CORRUPTION` | 3 | Power | Ancient | Self | new[] { new DynamicVar("Power", 1m)) | EnergyCost.UpgradeBy(-1); |
| 好勇斗狠 | `AGGRESSION` | 1 | Power | Rare | Self |  | AddKeyword(CardKeyword.Innate); |
| 壁垒 | `BARRICADE` | 3 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 绯红披风 | `CRIMSON_MANTLE` | 1 | Power | Rare | Self | new[] { new PowerVar<CrimsonMantlePower>(7m)) | DynamicVars["CrimsonMantlePower"].Upgrad |
| 黑暗之拥 | `DARK_EMBRACE` | 2 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 恶魔形态 | `DEMON_FORM` | 3 | Power | Rare | Self | new[] { new PowerVar<StrengthPower>(3m)) | DynamicVars["StrengthPower"].UpgradeValu |
| 地狱狂徒 | `HELLRAISER` | 2 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 势不可当 | `JUGGERNAUT` | 2 | Power | Rare | Self | new[] { new PowerVar<JuggernautPower>(6m)) | DynamicVars["JuggernautPower"].UpgradeVa |
| 薪火之源 | `PYRE` | 2 | Power | Rare | Self | new[] { new EnergyVar(1)) | DynamicVars.Energy.UpgradeValueBy(1m); |
| 肉盾 | `TANK` | 1 | Power | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new DynamicVar("Da | EnergyCost.UpgradeBy(-1); |
| 坚定不移 | `UNMOVABLE` | 2 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 残酷 | `CRUELTY` | 1 | Power | Uncommon | Self | new[] { new PowerVar<CrueltyPower>(25m)) | DynamicVars["CrueltyPower"].UpgradeValue |
| 无惧疼痛 | `FEEL_NO_PAIN` | 1 | Power | Uncommon | Self | new[] { new DynamicVar("Power", 3m)) | DynamicVars["Power"].UpgradeValueBy(1m); |
| 狱火 | `INFERNO` | 1 | Power | Uncommon | Self | new[] { new PowerVar<InfernoPower>(6m)) | DynamicVars["InfernoPower"].UpgradeValue |
| 燃烧 | `INFLAME` | 1 | Power | Uncommon | Self | new[] { new PowerVar<StrengthPower>(2m)) | DynamicVars["StrengthPower"].UpgradeValu |
| 杂耍 | `JUGGLING` | 1 | Power | Uncommon | Self |  | AddKeyword(CardKeyword.Innate); |
| 撕裂 | `RUPTURE` | 1 | Power | Uncommon | Self | new[] { new PowerVar<StrengthPower>(1m)) | DynamicVars.Strength.UpgradeValueBy(1m); |
| 惊逃 | `STAMPEDE` | 2 | Power | Uncommon | Self | new[] { new DynamicVar("Power", 1m)) | EnergyCost.UpgradeBy(-1); |
| 岩石铠甲 | `STONE_ARMOR` | 1 | Power | Uncommon | Self | new[] { new PowerVar<PlatingPower>(4m)) | DynamicVars["PlatingPower"].UpgradeValue |
| 凶恶 | `VICIOUS` | 1 | Power | Uncommon | Self | new[] { new CardsVar(1)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 防御 | `DEFEND_IRONCLAD` | 1 | Skill | Basic | Self | new[] { new BlockVar(5m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 武装 | `ARMAMENTS` | 1 | Skill | Common | Self | new[] { new BlockVar(5m, ValueProp.Move)) |  |
| 血墙 | `BLOOD_WALL` | 2 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new HpLossVar(2m), | DynamicVars.Block.UpgradeValueBy(4m); |
| 破灭 | `HAVOC` | 1 | Skill | Common | Self |  | EnergyCost.UpgradeBy(-1); |
| 耸肩无视 | `SHRUG_IT_OFF` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(8m, V | DynamicVars.Block.UpgradeValueBy(3m); |
| 挑衅 | `TAUNT` | 1 | Skill | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(6m, V | DynamicVars.Block.UpgradeValueBy(1m); Dy |
| 战栗 | `TREMBLE` | 1 | Skill | Common | AnyEnemy | new[] { new PowerVar<VulnerablePower>(3m)) | DynamicVars.Vulnerable.UpgradeValueBy(1m |
| 坚毅 | `TRUE_GRIT` | 1 | Skill | Common | Self | new[] { new BlockVar(7m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(2m); |
| 烙印 | `BRAND` | 0 | Skill | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new HpLossVar(1m), | DynamicVars.Strength.UpgradeValueBy(1m); |
| 倾泻 | `CASCADE` | -1 | Skill | Rare | Self |  |  |
| 主宰 | `DOMINATE` | 1 | Skill | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<Vulne | DynamicVars["VulnerablePower"].UpgradeVa |
| 岿然不动 | `IMPERVIOUS` | 2 | Skill | Rare | Self | new[] { new BlockVar(30m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(10m); |
| 时候未到 | `NOT_YET` | 2 | Skill | Rare | Self | new[] { new HealVar(10m)) | DynamicVars.Heal.UpgradeValueBy(3m); |
| 祭品 | `OFFERING` | 0 | Skill | Rare | Self | new Array<DynamicVar>(new DynamicVar[3] { new HpLossVar(6m), | DynamicVars.Cards.UpgradeValueBy(2m); |
| 连环拳 | `ONE_TWO_PUNCH` | 1 | Skill | Rare | Self | new[] { new DynamicVar("Attacks", 1m)) | DynamicVars["Attacks"].UpgradeValueBy(1m |
| 原始力量 | `PRIMAL_FORCE` | 0 | Skill | Rare | Self |  |  |
| 添柴 | `STOKE` | 1 | Skill | Rare | Self |  |  |
| 战斗专注 | `BATTLE_TRANCE` | 0 | Skill | Uncommon | Self | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 炽焰 | `BLAZE` | 2 | Skill | Uncommon | AnyAlly | new[] { new PowerVar<StrengthPower>(5m)) | DynamicVars.Strength.UpgradeValueBy(2m); |
| 放血 | `BLOODLETTING` | 0 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new HpLossVar(3m), | DynamicVars.Energy.UpgradeValueBy(1m); |
| 燃烧契约 | `BURNING_PACT` | 1 | Skill | Uncommon | Self | new[] { new CardsVar(2)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 巨像 | `COLOSSUS` | 1 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(4m, V | DynamicVars.Block.UpgradeValueBy(3m); |
| 恶魔护盾 | `DEMONIC_SHIELD` | 0 | Skill | Uncommon | AnyAlly | new Array<DynamicVar>(new DynamicVar[4] { new CalculationBas | RemoveKeyword(CardKeyword.Exhaust); |
| 战鼓 | `DRUM_OF_BATTLE` | 1 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new CardsVar(2), n | DynamicVars.Energy.UpgradeValueBy(1m); |
| 邪眼 | `EVIL_EYE` | 1 | Skill | Uncommon | Self | new[] { new BlockVar(8m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 跃跃欲试 | `EXPECT_A_FIGHT` | 3 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.CalculationBase.UpgradeValue |
| 火焰屏障 | `FLAME_BARRIER` | 2 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(12m,  | DynamicVars.Block.UpgradeValueBy(4m); Dy |
| 被遗忘的仪式 | `FORGOTTEN_RITUAL` | 1 | Skill | Uncommon | Self | new[] { new EnergyVar(3)) | DynamicVars.Energy.UpgradeValueBy(1m); |
| 地狱之刃 | `INFERNAL_BLADE` | 1 | Skill | Uncommon | Self |  | EnergyCost.UpgradeBy(-1); |
| 狂怒 | `RAGE` | 0 | Skill | Uncommon | Self | new[] { new DynamicVar("Power", 3m)) | DynamicVars["Power"].UpgradeValueBy(2m); |
| 重振精神 | `SECOND_WIND` | 1 | Skill | Uncommon | Self | new[] { new BlockVar(5m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(2m); |

## silent（91 张）

| 卡名 | id | 费 | 类型 | 稀有度 | 目标 | 变量 | 升级 |
|---|---|---|---|---|---|---|---|
| 压制 | `SUPPRESS` | 0 | Attack | Ancient | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(11m, | DynamicVars.Damage.UpgradeValueBy(6m); D |
| 中和 | `NEUTRALIZE` | 0 | Attack | Basic | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(3m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 打击 | `STRIKE_SILENT` | 1 | Attack | Basic | AnyEnemy | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 匕首雨 | `DAGGER_SPRAY` | 1 | Attack | Common | AllEnemies | new[] { new DamageVar(4m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 投掷匕首 | `DAGGER_THROW` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(9m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 翻越撑击 | `FLICK_FLACK` | 1 | Attack | Common | AllEnemies | new[] { new DamageVar(7m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 先制打击 | `LEADING_STRIKE` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new CardsVar("Shiv | DynamicVars.Damage.UpgradeValueBy(3m); |
| 带毒刺击 | `POISONED_STAB` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(6m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 猎杀者 | `PREDATOR` | 2 | Attack | Common | AnyEnemy | new[] { new DamageVar(15m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(5m); |
| 连续反弹 | `RICOCHET` | 2 | Attack | Common | RandomEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(3m,  | DynamicVars.Repeat.UpgradeValueBy(1m); |
| 切割 | `SLICE` | 0 | Attack | Common | AnyEnemy | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 突然一拳 | `SUCKER_PUNCH` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(8m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 刺杀 | `ASSASSINATE` | 0 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(10m, | DynamicVars.Damage.UpgradeValueBy(3m); D |
| 华丽收场 | `GRAND_FINALE` | 0 | Attack | Rare | AllEnemies | new[] { new DamageVar(60m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(15m); |
| 谋杀 | `MURDER` | 3 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | EnergyCost.UpgradeBy(-1); |
| 狩猎 | `THE_HUNT` | 1 | Attack | Rare | AnyEnemy | new[] { new DamageVar(10m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(5m); |
| 背刺 | `BACKSTAB` | 0 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(11m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 冲刺 | `DASH` | 2 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(10m, | DynamicVars.Damage.UpgradeValueBy(3m); D |
| 回响斩击 | `ECHOING_SLASH` | 1 | Attack | Uncommon | AllEnemies | new[] { new DamageVar(10m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 终结技 | `FINISHER` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DamageVar(6m,  | DynamicVars.Damage.UpgradeValueBy(2m); |
| 飞镖 | `FLECHETTES` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DamageVar(5m,  | DynamicVars.Damage.UpgradeValueBy(2m); |
| 铭记死亡 | `MEMENTO_MORI` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.CalculationBase.UpgradeValue |
| 精密瞄准 | `PINPOINT` | 3 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(15m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 猛扑 | `POUNCE` | 2 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(14m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(6m); |
| 精确切击 | `PRECISE_CUT` | 0 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.CalculationBase.UpgradeValue |
| 串刺 | `SKEWER` | 0 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(8m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 紧勒 | `STRANGLE` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(8m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 幽魂形态 | `WRAITH_FORM` | 3 | Power | Ancient | Self | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<Intan | DynamicVars["IntangiblePower"].UpgradeVa |
| 磨蚀 | `ABRASIVE` | 3 | Power | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<Thorn | DynamicVars["ThornsPower"].UpgradeValueB |
| 余像 | `AFTERIMAGE` | 1 | Power | Rare | Self | new[] { new PowerVar<AfterimagePower>(1m)) | AddKeyword(CardKeyword.Innate); |
| 涂毒 | `ENVENOM` | 2 | Power | Rare | Self | new[] { new PowerVar<EnvenomPower>(1m)) | DynamicVars["EnvenomPower"].UpgradeValue |
| 刀扇 | `FAN_OF_KNIVES` | 2 | Power | Rare | Self | new[] { new CardsVar("Shivs", 4)) | DynamicVars["Shivs"].UpgradeValueBy(1m); |
| 谋划专家 | `MASTER_PLANNER` | 2 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 群蛇形态 | `SERPENT_FORM` | 3 | Power | Rare | Self | new[] { new PowerVar<SerpentFormPower>(4m)) | DynamicVars["SerpentFormPower"].UpgradeV |
| 鬼祟 | `SNEAKY` | 2 | Power | Rare | Self | new[] { new PowerVar<SneakyPower>(1m)) | DynamicVars["SneakyPower"].UpgradeValueB |
| 必备工具 | `TOOLS_OF_THE_TRADE` | 1 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 跟踪 | `TRACKING` | 2 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 计划妥当 | `WELL_LAID_PLANS` | 2 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 触媒 | `ACCELERANT` | 1 | Power | Uncommon | Self | new[] { new DynamicVar("Accelerant", 1m)) | DynamicVars["Accelerant"].UpgradeValueBy |
| 精准 | `ACCURACY` | 1 | Power | Uncommon | Self | new[] { new PowerVar<AccuracyPower>(4m)) | DynamicVars["AccuracyPower"].UpgradeValu |
| 灵动步法 | `FOOTWORK` | 1 | Power | Uncommon | Self | new[] { new PowerVar<DexterityPower>(2m)) | DynamicVars.Dexterity.UpgradeValueBy(1m) |
| 无尽刀刃 | `INFINITE_BLADES` | 1 | Power | Uncommon | Self |  | AddKeyword(CardKeyword.Innate); |
| 毒雾 | `NOXIOUS_FUMES` | 1 | Power | Uncommon | Self | new[] { new DynamicVar("PoisonPerTurn", 2m)) | DynamicVars["PoisonPerTurn"].UpgradeValu |
| 幻影之刃 | `PHANTOM_BLADES` | 1 | Power | Uncommon | Self | new[] { new PowerVar<PhantomBladesPower>(9m)) | DynamicVars["PhantomBladesPower"].Upgrad |
| 速行者 | `SPEEDSTER` | 2 | Power | Uncommon | Self | new[] { new PowerVar<SpeedsterPower>(2m)) | AddKeyword(CardKeyword.Innate); |
| 防御 | `DEFEND_SILENT` | 1 | Skill | Basic | Self | new[] { new BlockVar(5m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 生存者 | `SURVIVOR` | 1 | Skill | Basic | Self | new[] { new BlockVar(8m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 预判 | `ANTICIPATE` | 0 | Skill | Common | Self | new[] { new PowerVar<DexterityPower>(2m)) | DynamicVars.Dexterity.UpgradeValueBy(2m) |
| 后空翻 | `BACKFLIP` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(5m, V | DynamicVars.Block.UpgradeValueBy(3m); |
| 刀刃之舞 | `BLADE_DANCE` | 1 | Skill | Common | Self | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 斗篷与匕首 | `CLOAK_AND_DAGGER` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(6m, V | DynamicVars.Cards.UpgradeValueBy(1m); |
| 致命毒药 | `DEADLY_POISON` | 1 | Skill | Common | AnyEnemy | new[] { new PowerVar<PoisonPower>(5m)) | DynamicVars.Poison.UpgradeValueBy(2m); |
| 偏折 | `DEFLECT` | 0 | Skill | Common | Self | new[] { new BlockVar(4m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 闪躲翻滚 | `DODGE_AND_ROLL` | 1 | Skill | Common | Self | new[] { new BlockVar(4m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(2m); |
| 尖啸 | `PIERCING_WAIL` | 1 | Skill | Common | AllEnemies | new[] { new DynamicVar("StrengthLoss", 6m)) | DynamicVars["StrengthLoss"].UpgradeValue |
| 早有准备 | `PREPARED` | 0 | Skill | Common | Self | new[] { new CardsVar(1)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 蛇咬 | `SNAKEBITE` | 2 | Skill | Common | AnyEnemy | new[] { new PowerVar<PoisonPower>(7m)) | DynamicVars.Poison.UpgradeValueBy(3m); |
| 触不可及 | `UNTOUCHABLE` | 2 | Skill | Common | Self | new[] { new BlockVar(6m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 肾上腺素 | `ADRENALINE` | 0 | Skill | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new EnergyVar(1),  | DynamicVars.Energy.UpgradeValueBy(1m); |
| 墨之刃 | `BLADE_OF_INK` | 1 | Skill | Rare | Self | new[] { new CardsVar(2)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 子弹时间 | `BULLET_TIME` | 3 | Skill | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 爆发 | `BURST` | 1 | Skill | Rare | Self | new[] { new DynamicVar("Skills", 1m)) | DynamicVars["Skills"].UpgradeValueBy(1m) |
| 腐蚀波 | `CORROSIVE_WAVE` | 1 | Skill | Rare | Self | new[] { new DynamicVar("CorrosiveWave", 2m)) | DynamicVars["CorrosiveWave"].UpgradeValu |
| 夹击 | `FLANKING` | 2 | Skill | Rare | AnyEnemy |  | EnergyCost.UpgradeBy(-1); |
| 刀刃陷阱 | `KNIFE_TRAP` | 2 | Skill | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas |  |
| 萎靡 | `MALAISE` | 0 | Skill | Rare | AnyEnemy |  |  |
| 夜魇 | `NIGHTMARE` | 3 | Skill | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 毒性爆发 | `OUTBREAK` | 3 | Skill | Rare | AllEnemies | new[] { new PowerVar<PoisonPower>(9m)) | DynamicVars.Poison.UpgradeValueBy(3m); |
| 融入暗影 | `SHADOWMELD` | 1 | Skill | Rare | Self | new[] { new DynamicVar("Power", 1m)) | EnergyCost.UpgradeBy(-1); |
| 暗影步 | `SHADOW_STEP` | 1 | Skill | Rare | Self | new[] { new CardsVar(3)) | EnergyCost.UpgradeBy(-1); |
| 钢铁风暴 | `STORM_OF_STEEL` | 1 | Skill | Rare | Self |  |  |
| 杂技 | `ACROBATICS` | 1 | Skill | Uncommon | Self | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 刀刃交响曲 | `BLADE_SYMPHONY` | 2 | Skill | Uncommon | AllAllies | new[] { new CardsVar(2)) | EnergyCost.UpgradeBy(-1); |
| 残影 | `BLUR` | 1 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(5m, V | DynamicVars.Block.UpgradeValueBy(3m); |
| 弹跳药瓶 | `BOUNCING_FLASK` | 2 | Skill | Uncommon | RandomEnemy | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<Poiso | DynamicVars.Repeat.UpgradeValueBy(1m); |
| 咕嘟冒泡 | `BUBBLE_BUBBLE` | 1 | Skill | Uncommon | AnyEnemy | new[] { new PowerVar<PoisonPower>(9m)) | DynamicVars.Poison.UpgradeValueBy(3m); |
| 计算下注 | `CALCULATED_GAMBLE` | 0 | Skill | Uncommon | Self |  | AddKeyword(CardKeyword.Retain); |
| 调制 | `CONCOCT` | 0 | Skill | Uncommon | AnyAlly | new[] { new PowerVar<ConcoctPower>(3m)) | DynamicVars["ConcoctPower"].UpgradeValue |
| 逃脱计划 | `ESCAPE_PLAN` | 0 | Skill | Uncommon | Self | new[] { new BlockVar(3m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(2m); |
| 独门技术 | `EXPERTISE` | 1 | Skill | Uncommon | Self | new[] { new CardsVar(2)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 暴露 | `EXPOSE` | 0 | Skill | Uncommon | AnyEnemy | new[] { new DynamicVar("Power", 2m)) | DynamicVars["Power"].UpgradeValueBy(1m); |
| 消影 | `FADE` | 0 | Skill | Uncommon | AnyAlly | new[] { new PowerVar<DexterityPower>(6m)) | DynamicVars.Dexterity.UpgradeValueBy(3m) |
| 手上技法 | `HAND_TRICK` | 1 | Skill | Uncommon | Self | new[] { new BlockVar(7m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 迷雾 | `HAZE` | 2 | Skill | Uncommon | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<Poiso | DynamicVars.Poison.UpgradeValueBy(2m); D |
| 隐秘匕首 | `HIDDEN_DAGGERS` | 0 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new CardsVar(2), n |  |
| 扫腿 | `LEG_SWEEP` | 2 | Skill | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(11m,  | DynamicVars.Block.UpgradeValueBy(3m); Dy |
| 蜃景 | `MIRAGE` | 1 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | RemoveKeyword(CardKeyword.Exhaust); |
| 本能反应 | `REFLEX` | 3 | Skill | Uncommon | Self | new[] { new CardsVar(2)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 侧步 | `SIDESTEP` | 0 | Skill | Uncommon | Self | new[] { new EnergyVar(1)) | DynamicVars.Energy.UpgradeValueBy(1m); |
| 战术大师 | `TACTICIAN` | 3 | Skill | Uncommon | Self | new[] { new EnergyVar(1)) | DynamicVars.Energy.UpgradeValueBy(1m); |
| 袖里乾坤 | `UP_MY_SLEEVE` | 2 | Skill | Uncommon | Self | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(1m); |

## defect（91 张）

| 卡名 | id | 费 | 类型 | 稀有度 | 目标 | 变量 | 升级 |
|---|---|---|---|---|---|---|---|
| 打击 | `STRIKE_DEFECT` | 1 | Attack | Basic | AnyEnemy | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 球状闪电 | `BALL_LIGHTNING` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(7m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 弹幕齐射 | `BARRAGE` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DamageVar(5m,  | DynamicVars.Damage.UpgradeValueBy(2m); |
| 光束射线 | `BEAM_CELL` | 0 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(3m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 爪击 | `CLAW` | 0 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(3m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 寒流 | `COLD_SNAP` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 编译冲击 | `COMPILE_DRIVER` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DamageVar(7m,  | DynamicVars.Damage.UpgradeValueBy(3m); |
| 集中打击 | `FOCUSED_STRIKE` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(9m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 眼部攻击 | `GO_FOR_THE_EYES` | 0 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(3m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 污秽攻击 | `GUNK_UP` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(4m,  | DynamicVars.Damage.UpgradeValueBy(1m); |
| 趁势打击 | `MOMENTUM_STRIKE` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(11m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 扫荡射线 | `SWEEPING_BEAM` | 1 | Attack | Common | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(6m,  | DynamicVars.Damage.UpgradeValueBy(3m); |
| 骚动 | `UPROAR` | 2 | Attack | Common | AnyEnemy | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 适应打击 | `ADAPTIVE_STRIKE` | 2 | Attack | Rare | AnyEnemy | new[] { new DamageVar(18m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(5m); |
| 万物一心 | `ALL_FOR_ONE` | 2 | Attack | Rare | AnyEnemy | new[] { new DamageVar(10m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 散射炮 | `FLAK_CANNON` | 2 | Attack | Rare | RandomEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DamageVar(8m,  | DynamicVars.Damage.UpgradeValueBy(3m); |
| 螺旋钻击 | `HELIX_DRILL` | 0 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DamageVar(3m,  | DynamicVars.Damage.UpgradeValueBy(2m); |
| 超能光束 | `HYPERBEAM` | 2 | Attack | Rare | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(24m, | DynamicVars.Damage.UpgradeValueBy(6m); |
| 冰之长枪 | `ICE_LANCE` | 3 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(19m, | DynamicVars.Damage.UpgradeValueBy(5m); |
| 陨石打击 | `METEOR_STRIKE` | 5 | Attack | Rare | AnyEnemy | new[] { new DamageVar(24m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(6m); |
| 打碎 | `SHATTER` | 1 | Attack | Rare | AllEnemies | new[] { new DamageVar(7m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 超越光速 | `FTL` | 0 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new DamageVar(5m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 空值 | `NULL` | 2 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(10m, | DynamicVars.Damage.UpgradeValueBy(3m); D |
| 折射 | `REFRACT` | 3 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new RepeatVar(2),  | DynamicVars.Damage.UpgradeValueBy(3m); |
| 火箭飞拳 | `ROCKET_PUNCH` | 2 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(13m, | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 刮削 | `SCRAPE` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(7m,  | DynamicVars.Damage.UpgradeValueBy(3m); D |
| 分离 | `SUNDER` | 3 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(26m, | DynamicVars.Damage.UpgradeValueBy(8m); |
| 人工合成 | `SYNTHESIS` | 2 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(14m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(6m); |
| 特斯拉线圈 | `TESLA_COIL` | 0 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(3m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(1m); |
| 偏差认知 | `BIASED_COGNITION` | 1 | Power | Ancient | Self | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<Focus | DynamicVars["FocusPower"].UpgradeValueBy |
| 缓冲 | `BUFFER` | 2 | Power | Rare | Self | new[] { new PowerVar<BufferPower>(1m)) | DynamicVars["BufferPower"].UpgradeValueB |
| 吞噬暗影 | `CONSUMING_SHADOW` | 2 | Power | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new RepeatVar(2),  | DynamicVars.Repeat.UpgradeValueBy(1m); |
| 冷却剂 | `COOLANT` | 1 | Power | Rare | Self | new[] { new PowerVar<CoolantPower>(2m)) | DynamicVars["CoolantPower"].UpgradeValue |
| 创造性AI | `CREATIVE_AI` | 3 | Power | Rare | Self | new[] { new DynamicVar("CreativeAi", 1m)) | EnergyCost.UpgradeBy(-1); |
| 碎片整理 | `DEFRAGMENT` | 1 | Power | Rare | Self | new[] { new PowerVar<FocusPower>(1m)) | DynamicVars["FocusPower"].UpgradeValueBy |
| 回响形态 | `ECHO_FORM` | 3 | Power | Rare | Self | new[] { new DynamicVar("EchoForm", 1m)) | RemoveKeyword(CardKeyword.Ethereal); |
| 机器学习 | `MACHINE_LEARNING` | 1 | Power | Rare | Self | new[] { new CardsVar(1)) | AddKeyword(CardKeyword.Innate); |
| 一心化万 | `ONE_FOR_ALL` | 1 | Power | Rare | AllAllies | new[] { new PowerVar<OneForAllPower>(3m)) | DynamicVars["OneForAllPower"].UpgradeVal |
| 旋转工艺 | `SPINNER` | 1 | Power | Rare | Self | new[] { new PowerVar<SpinnerPower>(1m)) |  |
| 化废为宝 | `TRASH_TO_TREASURE` | 1 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 暴涨 | `BULK_UP` | 2 | Power | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[3] { new DynamicVar("Or | DynamicVars.Strength.UpgradeValueBy(1m); |
| 扩容 | `CAPACITOR` | 1 | Power | Uncommon | Self | new[] { new RepeatVar(2)) | DynamicVars.Repeat.UpgradeValueBy(1m); |
| 野性 | `FERAL` | 2 | Power | Uncommon | Self | new[] { new PowerVar<FeralPower>(1m)) | EnergyCost.UpgradeBy(-1); |
| 冰雹风暴 | `HAILSTORM` | 1 | Power | Uncommon | Self | new[] { new PowerVar<HailstormPower>(6m)) | DynamicVars["HailstormPower"].UpgradeVal |
| 迭代 | `ITERATION` | 1 | Power | Uncommon | Self | new[] { new PowerVar<IterationPower>(2m)) | DynamicVars["IterationPower"].UpgradeVal |
| 循环 | `LOOP` | 1 | Power | Uncommon | Self | new[] { new DynamicVar("Loop", 1m)) | DynamicVars["Loop"].UpgradeValueBy(1m); |
| 烟囱 | `SMOKESTACK` | 1 | Power | Uncommon | Self | new[] { new PowerVar<SmokestackPower>(5m)) | DynamicVars["SmokestackPower"].UpgradeVa |
| 雷暴 | `STORM` | 1 | Power | Uncommon | Self | new[] { new PowerVar<StormPower>(1m)) | DynamicVars["StormPower"].UpgradeValueBy |
| 子程序 | `SUBROUTINE` | 1 | Power | Uncommon | Self |  | EnergyCost.UpgradeBy(-1); |
| 雷霆 | `THUNDER` | 1 | Power | Uncommon | Self | new[] { new PowerVar<ThunderPower>(8m)) | DynamicVars["ThunderPower"].UpgradeValue |
| 四重释放 | `QUADCAST` | 1 | Skill | Ancient | Self | new[] { new RepeatVar(4)) | EnergyCost.UpgradeBy(-1); |
| 防御 | `DEFEND_DEFECT` | 1 | Skill | Basic | Self | new[] { new BlockVar(5m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 双重释放 | `DUALCAST` | 1 | Skill | Basic | Self |  | EnergyCost.UpgradeBy(-1); |
| 电击 | `ZAP` | 1 | Skill | Basic | Self |  | EnergyCost.UpgradeBy(-1); |
| 高速脱离 | `BOOST_AWAY` | 0 | Skill | Common | Self | new[] { new BlockVar(6m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 充电 | `CHARGE_BATTERY` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(7m, V | DynamicVars.Block.UpgradeValueBy(3m); |
| 冷静头脑 | `COOLHEADED` | 1 | Skill | Common | Self | new[] { new CardsVar(1)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 全息影像 | `HOLOGRAM` | 1 | Skill | Common | Self | new[] { new BlockVar(3m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(2m); Re |
| 热修复 | `HOTFIX` | 0 | Skill | Common | Self | new[] { new PowerVar<FocusPower>(2m)) | RemoveKeyword(CardKeyword.Exhaust); |
| 飞跃 | `LEAP` | 1 | Skill | Common | Self | new[] { new BlockVar(9m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 引雷针 | `LIGHTNING_ROD` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(4m, V | DynamicVars.Block.UpgradeValueBy(3m); |
| 内核加速 | `TURBO` | 0 | Skill | Common | Self | new[] { new EnergyVar(2)) | DynamicVars.Energy.UpgradeValueBy(1m); |
| 遗传算法 | `GENETIC_ALGORITHM` | 1 | Skill | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(Curre | DynamicVars["Increase"].UpgradeValueBy(1 |
| 模仿学习 | `IMITATION_LEARNING` | 1 | Skill | Rare | AnyAlly | new[] { new PowerVar<ImitationLearningPower>(2m)) | DynamicVars["ImitationLearningPower"].Up |
| 模组改造 | `MODDED` | 0 | Skill | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new RepeatVar(1),  | DynamicVars.Cards.UpgradeValueBy(1m); |
| 多重释放 | `MULTI_CAST` | 0 | Skill | Rare | Self |  |  |
| 彩虹 | `RAINBOW` | 2 | Skill | Rare | Self |  | RemoveKeyword(CardKeyword.Exhaust); |
| 重启 | `REBOOT` | 0 | Skill | Rare | Self | new[] { new CardsVar(4)) | DynamicVars.Cards.UpgradeValueBy(2m); |
| 信号增强 | `SIGNAL_BOOST` | 1 | Skill | Rare | Self | new[] { new PowerVar<SignalBoostPower>(1m)) | EnergyCost.UpgradeBy(-1); |
| 超临界态 | `SUPERCRITICAL` | 0 | Skill | Rare | Self | new[] { new EnergyVar(4)) | DynamicVars.Energy.UpgradeValueBy(2m); |
| 电流相生 | `VOLTAIC` | 3 | Skill | Rare | Self | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | RemoveKeyword(CardKeyword.Exhaust); |
| 启动流程 | `BOOT_SEQUENCE` | 0 | Skill | Uncommon | Self | new[] { new BlockVar(10m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 混沌 | `CHAOS` | 1 | Skill | Uncommon | Self | new[] { new RepeatVar(1)) | DynamicVars.Repeat.UpgradeValueBy(1m); |
| 冰寒 | `CHILL` | 0 | Skill | Uncommon | Self |  | RemoveKeyword(CardKeyword.Exhaust); |
| 压缩 | `COMPACT` | 1 | Skill | Uncommon | Self | new[] { new BlockVar(6m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(1m); |
| 漆黑 | `DARKNESS` | 1 | Skill | Uncommon | Self |  |  |
| 双倍能量 | `DOUBLE_ENERGY` | 1 | Skill | Uncommon | Self |  | EnergyCost.UpgradeBy(-1); |
| 能量涌动 | `ENERGY_SURGE` | 1 | Skill | Uncommon | AllAllies | new[] { new EnergyVar(2)) | DynamicVars.Energy.UpgradeValueBy(1m); |
| 强撑 | `FIGHT_THROUGH` | 1 | Skill | Uncommon | Self | new[] { new BlockVar(13m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(4m); |
| 聚变 | `FUSION` | 1 | Skill | Uncommon | Self |  | RemoveKeyword(CardKeyword.Exhaust); |
| 冰川 | `GLACIER` | 2 | Skill | Uncommon | Self | new[] { new BlockVar(6m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 玻璃工艺 | `GLASSWORK` | 1 | Skill | Uncommon | Self | new[] { new BlockVar(5m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 休眠 | `HIBERNATE` | 2 | Skill | Uncommon | Self | new[] { new RepeatVar(2)) | DynamicVars.Repeat.UpgradeValueBy(1m); |
| 引火 | `IGNITION` | 1 | Skill | Uncommon | AnyAlly |  | RemoveKeyword(CardKeyword.Exhaust); |
| 超频 | `OVERCLOCK` | 0 | Skill | Uncommon | Self | new[] { new CardsVar(2)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 内存清理 | `SCAVENGE` | 1 | Skill | Uncommon | Self | new[] { new EnergyVar(2)) | DynamicVars.Energy.UpgradeValueBy(1m); |
| 暗影之盾 | `SHADOW_SHIELD` | 2 | Skill | Uncommon | Self | new[] { new BlockVar(11m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(4m); |
| 快速检索 | `SKIM` | 1 | Skill | Uncommon | Self | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 同步 | `SYNCHRONIZE` | 1 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.CalculationExtra.UpgradeValu |
| 暴风雨 | `TEMPEST` | 0 | Skill | Uncommon | Self |  |  |
| 白噪声 | `WHITE_NOISE` | 1 | Skill | Uncommon | Self |  | EnergyCost.UpgradeBy(-1); |

## regent（91 张）

| 卡名 | id | 费 | 类型 | 稀有度 | 目标 | 变量 | 升级 |
|---|---|---|---|---|---|---|---|
| 流星雨 | `METEOR_SHOWER` | 0 | Attack | Ancient | AllEnemies | new Array<DynamicVar>(new DynamicVar[3] { new DamageVar(14m, | DynamicVars.Damage.UpgradeValueBy(7m); |
| 陨星 | `FALLING_STAR` | 0 | Attack | Basic | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new DamageVar(8m,  | DynamicVars.Damage.UpgradeValueBy(4m); |
| 打击 | `STRIKE_REGENT` | 1 | Attack | Basic | AnyEnemy | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 星界脉冲 | `ASTRAL_PULSE` | 0 | Attack | Common | AllEnemies | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 天穹之力 | `CELESTIAL_MIGHT` | 2 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(6m,  | DynamicVars.Repeat.UpgradeValueBy(1m); |
| 碰撞轨迹 | `COLLISION_COURSE` | 0 | Attack | Common | AnyEnemy | new[] { new DamageVar(10m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 新月长矛 | `CRESCENT_SPEAR` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.ExtraDamage.UpgradeValueBy(1 |
| 下砸 | `CRUSH_UNDER` | 1 | Attack | Common | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(8m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 引导之星 | `GUIDING_STAR` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(12m, | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 光子切割 | `PHOTON_CUT` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new DamageVar(10m, | DynamicVars.Damage.UpgradeValueBy(3m); D |
| 太阳打击 | `SOLAR_STRIKE` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(9m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 战火铸就 | `WROUGHT_IN_WAR` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(7m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 锻打成型 | `BEAT_INTO_SHAPE` | 1 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DamageVar(5m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 轰击 | `BOMBARDMENT` | 3 | Attack | Rare | AnyEnemy | new[] { new DamageVar(18m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(6m); |
| 彗星 | `COMET` | 0 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new DamageVar(33m, | DynamicVars.Damage.UpgradeValueBy(11m); |
| 迫降 | `CRASH_LANDING` | 1 | Attack | Rare | AllEnemies | new[] { new DamageVar(21m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(5m); |
| 星灭 | `DYING_STAR` | 1 | Attack | Rare | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(9m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 天际钻头 | `HEAVENLY_DRILL` | 0 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(8m,  | DynamicVars.Damage.UpgradeValueBy(2m); |
| 传承之锤 | `HEIRLOOM_HAMMER` | 2 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(20m, | DynamicVars.Damage.UpgradeValueBy(5m); |
| 如此甚好 | `MAKE_IT_SO` | 0 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(6m,  | DynamicVars.Damage.UpgradeValueBy(3m); |
| 七星 | `SEVEN_STARS` | 2 | Attack | Rare | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(7m,  | EnergyCost.UpgradeBy(-1); |
| 葬送 | `DEVASTATE` | 1 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(35m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(10m); |
| 伽马爆破 | `GAMMA_BLAST` | 0 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new DamageVar(13m, | DynamicVars.Damage.UpgradeValueBy(5m); |
| 霸权 | `HEGEMONY` | 2 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(15m, | DynamicVars.Damage.UpgradeValueBy(3m); D |
| 王者之踢 | `KINGLY_KICK` | 4 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(27m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(8m); |
| 王者之拳 | `KINGLY_PUNCH` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(8m,  | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 决胜一击 | `KNOCKOUT_BLOW` | 3 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(30m, | DynamicVars.Damage.UpgradeValueBy(8m); |
| 月面射击 | `LUNAR_BLAST` | 0 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DamageVar(4m,  | DynamicVars.Damage.UpgradeValueBy(1m); |
| 辐射 | `RADIATE` | 0 | Attack | Uncommon | AllEnemies | new Array<DynamicVar>(new DynamicVar[5] { new DamageVar(3m,  | DynamicVars.Damage.UpgradeValueBy(1m); |
| 明耀打击 | `SHINING_STRIKE` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(8m,  | DynamicVars.Damage.UpgradeValueBy(3m); |
| 星尘 | `STARDUST` | 0 | Attack | Uncommon | RandomEnemy | new[] { new DamageVar(5m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 超质量体 | `SUPERMASSIVE` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.ExtraDamage.UpgradeValueBy(1 |
| 封印王座 | `THE_SEALED_THRONE` | 1 | Power | Ancient | Self |  | EnergyCost.UpgradeBy(-1); |
| 武器库 | `ARSENAL` | 1 | Power | Rare | Self | new[] { new PowerVar<ArsenalPower>(1m)) | AddKeyword(CardKeyword.Innate); |
| 创世纪 | `GENESIS` | 2 | Power | Rare | Self | new[] { new DynamicVar("StarsPerTurn", 2m)) | DynamicVars["StarsPerTurn"].UpgradeValue |
| 锤子时间 | `HAMMER_TIME` | 2 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 王之凝视 | `MONARCHS_GAZE` | 2 | Power | Rare | Self | new[] { new DynamicVar("StrengthLoss", 1m)) | EnergyCost.UpgradeBy(-1); |
| 中子护盾 | `NEUTRON_AEGIS` | 1 | Power | Rare | Self | new[] { new PowerVar<PlatingPower>(8m)) | DynamicVars["PlatingPower"].UpgradeValue |
| 王国资产 | `ROYALTIES` | 1 | Power | Rare | Self | new[] { new GoldVar(30)) | DynamicVars.Gold.UpgradeValueBy(10m); |
| 追踪之刃 | `SEEKING_EDGE` | 1 | Power | Rare | Self | new[] { new ForgeVar(7)) | DynamicVars.Forge.UpgradeValueBy(4m); |
| 剑圣 | `SWORD_SAGE` | 2 | Power | Rare | Self | new[] { new PowerVar<SwordSagePower>(1m)) | EnergyCost.UpgradeBy(-1); |
| 暴政 | `TYRANNY` | 1 | Power | Rare | Self |  | AddKeyword(CardKeyword.Innate); |
| 虚空形态 | `VOID_FORM` | 3 | Power | Rare | Self | new[] { new PowerVar<VoidFormPower>(2m)) | RemoveKeyword(CardKeyword.Ethereal); |
| 黑洞 | `BLACK_HOLE` | 1 | Power | Uncommon | Self | new[] { new PowerVar<BlackHolePower>(3m)) | DynamicVars["BlackHolePower"].UpgradeVal |
| 群星之子 | `CHILD_OF_THE_STARS` | 1 | Power | Uncommon | Self | new[] { new DynamicVar("BlockForStars", 2m)) | DynamicVars["BlockForStars"].UpgradeValu |
| 熔炉 | `FURNACE` | 1 | Power | Uncommon | Self | new[] { new ForgeVar(5)) | DynamicVars.Forge.UpgradeValueBy(2m); |
| 环绕轨道 | `ORBIT` | 2 | Power | Uncommon | Self | new[] { new EnergyVar(1)) | EnergyCost.UpgradeBy(-1); |
| 暗淡蓝点 | `PALE_BLUE_DOT` | 1 | Power | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new CardsVar(1), n | DynamicVars.Cards.UpgradeValueBy(1m); |
| 招架 | `PARRY` | 1 | Power | Uncommon | Self | new[] { new PowerVar<ParryPower>(10m)) | DynamicVars["ParryPower"].UpgradeValueBy |
| 创世之柱 | `PILLAR_OF_CREATION` | 1 | Power | Uncommon | Self | new[] { new BlockVar(2m, ValueProp.Unpowered)) | DynamicVars.Block.UpgradeValueBy(1m); |
| 光谱偏移 | `SPECTRUM_SHIFT` | 2 | Power | Uncommon | Self | new[] { new CardsVar(1)) | EnergyCost.UpgradeBy(-1); |
| 防御 | `DEFEND_REGENT` | 1 | Skill | Basic | Self | new[] { new BlockVar(5m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 崇拜 | `VENERATE` | 1 | Skill | Basic | Self | new[] { new StarsVar(2)) | DynamicVars.Stars.UpgradeValueBy(1m); |
| 下去！ | `BEGONE` | 1 | Skill | Common | Self |  |  |
| 群星斗篷 | `CLOAK_OF_STARS` | 0 | Skill | Common | Self | new[] { new BlockVar(7m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 宇宙冷漠 | `COSMIC_INDIFFERENCE` | 1 | Skill | Common | Self | new[] { new BlockVar(6m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 收集光辉 | `GATHER_LIGHT` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(8m, V | DynamicVars.Block.UpgradeValueBy(3m); |
| 流光溢彩 | `GLITTERSTREAM` | 2 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(11m,  | DynamicVars.Block.UpgradeValueBy(2m); Dy |
| 辉光 | `GLOW` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new StarsVar(1), n | DynamicVars.Stars.UpgradeValueBy(1m); |
| 隐秘藏品 | `HIDDEN_CACHE` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new StarsVar(1), n | DynamicVars["StarNextTurnPower"].Upgrade |
| 何人僭越 | `KNOW_THY_PLACE` | 0 | Skill | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<WeakP | RemoveKeyword(CardKeyword.Exhaust); |
| 星星点点 | `PATTER` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(8m, V | DynamicVars.Block.UpgradeValueBy(2m); Dy |
| 淬炼刀刃 | `REFINE_BLADE` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new ForgeVar(8), n | DynamicVars.Forge.UpgradeValueBy(4m); |
| 战利品 | `SPOILS_OF_BATTLE` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new ForgeVar(6), n | DynamicVars.Forge.UpgradeValueBy(3m); |
| 大爆炸 | `BIG_BANG` | 0 | Skill | Rare | Self | new Array<DynamicVar>(new DynamicVar[4] { new CardsVar(1), n | AddKeyword(CardKeyword.Innate); |
| 新生之喜 | `BUNDLE_OF_JOY` | 1 | Skill | Rare | Self | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 抉择，抉择 | `DECISIONS_DECISIONS` | 0 | Skill | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new CardsVar(3), n | DynamicVars.Cards.UpgradeValueBy(2m); |
| 既定事项 | `FOREGONE_CONCLUSION` | 1 | Skill | Rare | Self | new[] { new CardsVar(2)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 护驾！！！ | `GUARDS` | 2 | Skill | Rare | Self |  |  |
| 所向无敌 | `I_AM_INVINCIBLE` | 1 | Skill | Rare | Self | new[] { new BlockVar(10m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 铸剑者 | `THE_SMITH` | 1 | Skill | Rare | Self | new[] { new ForgeVar(30)) | DynamicVars.Forge.UpgradeValueBy(10m); |
| 指导 | `TUTOR` | 1 | Skill | Rare | AnyAlly |  | EnergyCost.UpgradeBy(-1); |
| 星位序列 | `ALIGNMENT` | 0 | Skill | Uncommon | Self | new[] { new EnergyVar(2)) | DynamicVars.Energy.UpgradeValueBy(1m); |
| 铸墙 | `BULWARK` | 2 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(12m,  | DynamicVars.Block.UpgradeValueBy(3m); Dy |
| 冲锋！！ | `CHARGE` | 1 | Skill | Uncommon | Self | new[] { new CardsVar(2)) |  |
| 征服者 | `CONQUEROR` | 1 | Skill | Uncommon | AnyEnemy | new[] { new ForgeVar(3)) | DynamicVars.Forge.UpgradeValueBy(2m); |
| 群星荟萃 | `CONSTELLATION` | 0 | Skill | Uncommon | AnyAlly | new Array<DynamicVar>(new DynamicVar[3] { new CardsVar(1), n | DynamicVars.Block.UpgradeValueBy(3m); |
| 汇流 | `CONVERGENCE` | 1 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new EnergyVar(1),  | DynamicVars.Stars.UpgradeValueBy(1m); |
| 微光 | `GLIMMER` | 1 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new CardsVar(3), n | DynamicVars.Cards.UpgradeValueBy(1m); |
| 慷慨捐助 | `LARGESSE` | 0 | Skill | Uncommon | AnyAlly |  |  |
| 君权自授 | `MANIFEST_AUTHORITY` | 1 | Skill | Uncommon | Self | new[] { new BlockVar(7m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(1m); |
| 独白 | `MONOLOGUE` | 0 | Skill | Uncommon | Self | new[] { new DynamicVar("Power", 1m)) | AddKeyword(CardKeyword.Retain); |
| 粒子墙 | `PARTICLE_WALL` | 0 | Skill | Uncommon | Self | new[] { new BlockVar(9m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 筹划 | `PLOT` | 1 | Skill | Uncommon | AllAllies | new[] { new CardsVar(2)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 预言 | `PROPHESIZE` | 2 | Skill | Uncommon | Self | new[] { new CardsVar(6)) | DynamicVars.Cards.UpgradeValueBy(3m); |
| 类星体 | `QUASAR` | 0 | Skill | Uncommon | Self |  |  |
| 倒映 | `REFLECT` | 1 | Skill | Uncommon | Self | new[] { new BlockVar(15m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(5m); |
| 共鸣 | `RESONANCE` | 1 | Skill | Uncommon | AllEnemies | new[] { new PowerVar<StrengthPower>(1m)) | DynamicVars["StrengthPower"].UpgradeValu |
| 胜券在王 | `ROYAL_GAMBLE` | 0 | Skill | Uncommon | Self | new[] { new StarsVar(9)) | AddKeyword(CardKeyword.Retain); |
| 征召上前 | `SUMMON_FORTH` | 1 | Skill | Uncommon | Self | new[] { new ForgeVar(8)) | DynamicVars.Forge.UpgradeValueBy(3m); |
| 地形改造 | `TERRAFORMING` | 1 | Skill | Uncommon | Self | new[] { new PowerVar<VigorPower>(7m)) | DynamicVars["VigorPower"].UpgradeValueBy |

## necrobinder（91 张）

| 卡名 | id | 费 | 类型 | 稀有度 | 目标 | 变量 | 升级 |
|---|---|---|---|---|---|---|---|
| 守护者 | `PROTECTOR` | 1 | Attack | Ancient | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | EnergyCost.UpgradeBy(-1); DynamicVars.Ca |
| 打击 | `STRIKE_NECROBINDER` | 1 | Attack | Basic | AnyEnemy | new[] { new DamageVar(6m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 出击 | `UNLEASH` | 1 | Attack | Basic | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.CalculationBase.UpgradeValue |
| 荒疫打击 | `BLIGHT_STRIKE` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(8m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 玷污 | `DEFILE` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(13m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 能量汲取 | `DRAIN_POWER` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(10m, | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 恐惧 | `FEAR` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(7m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 重压 | `FLATTEN` | 2 | Attack | Common | AnyEnemy | new[] { new OstyDamageVar(12m, ValueProp.Move)) | DynamicVars.OstyDamage.UpgradeValueBy(4m |
| 坟冢爆射 | `GRAVEBLAST` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(4m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); R |
| 戳击 | `POKE` | 0 | Attack | Common | AnyEnemy | new[] { new OstyDamageVar(6m, ValueProp.Move)) | DynamicVars.OstyDamage.UpgradeValueBy(3m |
| 收割 | `REAP` | 3 | Attack | Common | AnyEnemy | new[] { new DamageVar(27m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(6m); |
| 剥夺 | `REAVE` | 1 | Attack | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(10m, | DynamicVars.Damage.UpgradeValueBy(3m); |
| 雕琢打击 | `SCULPTING_STRIKE` | 1 | Attack | Common | AnyEnemy | new[] { new DamageVar(9m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 响指 | `SNAP` | 1 | Attack | Common | AnyEnemy | new[] { new OstyDamageVar(7m, ValueProp.Move)) | DynamicVars.OstyDamage.UpgradeValueBy(3m |
| 播种 | `SOW` | 1 | Attack | Common | AllEnemies | new[] { new DamageVar(8m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 女妖之嚎 | `BANSHEES_CRY` | 9 | Attack | Rare | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(33m, | EnergyCost.UpgradeBy(-2); |
| 根除 | `ERADICATE` | 0 | Attack | Rare | AnyEnemy | new[] { new DamageVar(11m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 吊杀 | `HANG` | 1 | Attack | Rare | AnyEnemy | new[] { new DamageVar(10m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 苦难 | `MISERY` | 0 | Attack | Rare | AnyEnemy | new[] { new DamageVar(7m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); A |
| 灵魂风暴 | `SOUL_STORM` | 1 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.ExtraDamage.UpgradeValueBy(2 |
| 榨取 | `SQUEEZE` | 3 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.CalculationBase.UpgradeValue |
| 巨镰 | `THE_SCYTHE` | 2 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(Curr | DynamicVars["Increase"].UpgradeValueBy(2 |
| 大限已至 | `TIMES_UP` | 2 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | AddKeyword(CardKeyword.Retain); |
| 碎骨 | `BONE_SHARDS` | 1 | Attack | Uncommon | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new OstyDamageVar( | DynamicVars.OstyDamage.UpgradeValueBy(3m |
| 埋葬 | `BURY` | 4 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(52m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(11m); |
| 死亡行军 | `DEATH_MARCH` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.CalculationBase.UpgradeValue |
| 摧残 | `DEBILITATE` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(10m, | DynamicVars.Damage.UpgradeValueBy(2m); D |
| 取回 | `FETCH` | 0 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new OstyDamageVar( | DynamicVars.OstyDamage.UpgradeValueBy(3m |
| 击掌 | `HIGH_FIVE` | 2 | Attack | Uncommon | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new OstyDamageVar( | DynamicVars.OstyDamage.UpgradeValueBy(2m |
| 亡魂牵引 | `PULL_FROM_BELOW` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DamageVar(5m,  | DynamicVars.Damage.UpgradeValueBy(2m); |
| 猛晃 | `RATTLE` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new OstyDamageVar( | DynamicVars.OstyDamage.UpgradeValueBy(2m |
| 得力助手 | `RIGHT_HAND_HAND` | 0 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new OstyDamageVar( | DynamicVars.OstyDamage.UpgradeValueBy(2m |
| 切断 | `SEVERANCE` | 2 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(13m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(5m); |
| 紧追不放 | `SIC_EM` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new OstyDamageVar( | DynamicVars.OstyDamage.UpgradeValueBy(1m |
| 刺破帷幕 | `VEILPIERCER` | 1 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(10m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 禁忌魔典 | `FORBIDDEN_GRIMOIRE` | 2 | Power | Ancient | Self |  | EnergyCost.UpgradeBy(-1); |
| 不谐合曲 | `CACOPHONY` | 2 | Power | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new CardsVar(33),  | DynamicVars.Damage.UpgradeValueBy(33m); |
| 虚空之唤 | `CALL_OF_THE_VOID` | 1 | Power | Rare | Self | new[] { new CardsVar(1)) | AddKeyword(CardKeyword.Innate); |
| 领域 | `DEMESNE` | 3 | Power | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new EnergyVar(1),  | EnergyCost.UpgradeBy(-1); |
| 吞噬生命 | `DEVOUR_LIFE` | 1 | Power | Rare | Self | new[] { new PowerVar<DevourLifePower>(1m)) | DynamicVars["DevourLifePower"].UpgradeVa |
| 亡灵精通 | `NECRO_MASTERY` | 2 | Power | Rare | Self | new[] { new SummonVar(5m)) | DynamicVars.Summon.UpgradeValueBy(3m); |
| 精神过载 | `NEUROSURGE` | 0 | Power | Rare | Self | new Array<DynamicVar>(new DynamicVar[3] { new PowerVar<Neuro | DynamicVars.Energy.UpgradeValueBy(1m); |
| 死神形态 | `REAPER_FORM` | 3 | Power | Rare | Self |  | AddKeyword(CardKeyword.Retain); |
| 哨卫模式 | `SENTRY_MODE` | 2 | Power | Rare | Self | new[] { new PowerVar<SentryModePower>(1m)) | EnergyCost.UpgradeBy(-1); |
| 灰烬之灵 | `SPIRIT_OF_ASH` | 1 | Power | Rare | Self | new[] { new DynamicVar("BlockOnExhaust", 4m)) | DynamicVars["BlockOnExhaust"].UpgradeVal |
| 钙化 | `CALCIFY` | 1 | Power | Uncommon | Self | new[] { new PowerVar<CalcifyPower>(4m)) | DynamicVars["CalcifyPower"].UpgradeValue |
| 倒数计时 | `COUNTDOWN` | 1 | Power | Uncommon | Self | new[] { new PowerVar<CountdownPower>(6m)) | DynamicVars["CountdownPower"].UpgradeVal |
| 死亡之舞 | `DANSE_MACABRE` | 1 | Power | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<Danse | DynamicVars["DanseMacabrePower"].Upgrade |
| 友谊 | `FRIENDSHIP` | 1 | Power | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<Stren | DynamicVars["StrengthPower"].UpgradeValu |
| 纠缠 | `HAUNT` | 1 | Power | Uncommon | Self | new[] { new HpLossVar(7m)) | DynamicVars.HpLoss.UpgradeValueBy(2m); |
| 致死性 | `LETHALITY` | 1 | Power | Uncommon | Self | new[] { new PowerVar<LethalityPower>(50m)) | DynamicVars["LethalityPower"].UpgradeVal |
| 书页风暴 | `PAGESTORM` | 1 | Power | Uncommon | Self | new[] { new CardsVar(1)) | EnergyCost.UpgradeBy(-1); |
| 厄运之衣 | `SHROUD` | 1 | Power | Uncommon | Self | new[] { new BlockVar(3m, ValueProp.Unpowered)) | DynamicVars.Block.UpgradeValueBy(1m); |
| 血肉戏法 | `SLEIGHT_OF_FLESH` | 2 | Power | Uncommon | Self | new[] { new PowerVar<SleightOfFleshPower>(9m)) | DynamicVars["SleightOfFleshPower"].Upgra |
| 灵魂绑定 | `SOULBOUND` | 1 | Power | Uncommon | AnyAlly |  | AddKeyword(CardKeyword.Innate); |
| 护卫 | `BODYGUARD` | 1 | Skill | Basic | Self | new[] { new SummonVar(5m)) | DynamicVars.Summon.UpgradeValueBy(2m); |
| 防御 | `DEFEND_NECROBINDER` | 1 | Skill | Basic | Self | new[] { new BlockVar(5m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(3m); |
| 来生 | `AFTERLIFE` | 1 | Skill | Common | Self | new[] { new SummonVar(6m)) | DynamicVars.Summon.UpgradeValueBy(3m); |
| 违逆 | `DEFY` | 1 | Skill | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(6m, V | DynamicVars.Block.UpgradeValueBy(3m); |
| 守墓人 | `GRAVE_WARDEN` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(8m, V | DynamicVars.Block.UpgradeValueBy(3m); |
| 唤起 | `INVOKE` | 1 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new SummonVar(2m), | DynamicVars.Summon.UpgradeValueBy(1m); D |
| 负能量脉冲 | `NEGATIVE_PULSE` | 1 | Skill | Common | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(5m, V | DynamicVars.Block.UpgradeValueBy(1m); Dy |
| 吸引仇恨 | `PULL_AGGRO` | 2 | Skill | Common | Self | new Array<DynamicVar>(new DynamicVar[2] { new SummonVar(4m), | DynamicVars.Summon.UpgradeValueBy(1m); D |
| 鞭打 | `SCOURGE` | 1 | Skill | Common | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<DoomP | DynamicVars.Doom.UpgradeValueBy(3m); Dyn |
| 鬼火 | `WISP` | 0 | Skill | Common | Self | new[] { new EnergyVar(1)) | AddKeyword(CardKeyword.Retain); |
| 幻景 | `EIDOLON` | 2 | Skill | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 末日降临 | `END_OF_DAYS` | 3 | Skill | Rare | AllEnemies | new[] { new PowerVar<DoomPower>(29m)) | DynamicVars.Doom.UpgradeValueBy(8m); |
| 彼岸一瞥 | `GLIMPSE_BEYOND` | 1 | Skill | Rare | AllAllies | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 湮灭 | `OBLIVION` | 0 | Skill | Rare | AnyEnemy | new[] { new PowerVar<DoomPower>(3m)) | DynamicVars.Doom.UpgradeValueBy(1m); |
| 死者苏生 | `REANIMATE` | 3 | Skill | Rare | Self | new[] { new SummonVar(20m)) | DynamicVars.Summon.UpgradeValueBy(5m); |
| 牺牲 | `SACRIFICE` | 1 | Skill | Rare | Self | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | EnergyCost.UpgradeBy(-1); |
| 降灵 | `SEANCE` | 1 | Skill | Rare | Self | new[] { new CardsVar(1)) | EnergyCost.UpgradeBy(-1); |
| 命运同担 | `SHARED_FATE` | 0 | Skill | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DynamicVar("En | DynamicVars["EnemyStrengthLoss"].Upgrade |
| 重构 | `TRANSFIGURE` | 1 | Skill | Rare | Self | new[] { new EnergyVar(1)) | RemoveKeyword(CardKeyword.Exhaust); |
| 不死 | `UNDEATH` | 0 | Skill | Rare | Self | new[] { new BlockVar(7m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(2m); |
| 预借时间 | `BORROWED_TIME` | 1 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new EnergyVar(4),  | DynamicVars.Energy.UpgradeValueBy(2m); |
| 捕捉灵魂 | `CAPTURE_SPIRIT` | 1 | Skill | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(3m,  | DynamicVars.Damage.UpgradeValueBy(1m); D |
| 洁净 | `CLEANSE` | 1 | Skill | Uncommon | Self | new[] { new SummonVar(3m)) | DynamicVars.Summon.UpgradeValueBy(2m); |
| 死亡使者 | `DEATHBRINGER` | 2 | Skill | Uncommon | AllEnemies | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<DoomP | DynamicVars.Doom.UpgradeValueBy(5m); |
| 死亡之门 | `DEATHS_DOOR` | 1 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(6m, V | DynamicVars.Block.UpgradeValueBy(1m); |
| 拖延 | `DELAY` | 2 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(11m,  | DynamicVars.Block.UpgradeValueBy(2m); Dy |
| 挽歌 | `DIRGE` | 0 | Skill | Uncommon | Self | new[] { new SummonVar(3m)) | DynamicVars.Summon.UpgradeValueBy(1m); |
| 清淤 | `DREDGE` | 1 | Skill | Uncommon | Self | new[] { new CardsVar(3)) | AddKeyword(CardKeyword.Retain); |
| 弱化之触 | `ENFEEBLING_TOUCH` | 1 | Skill | Uncommon | AnyEnemy | new[] { new DynamicVar("StrengthLoss", 8m)) | DynamicVars["StrengthLoss"].UpgradeValue |
| 骸骨军团 | `LEGION_OF_BONE` | 2 | Skill | Uncommon | AllAllies | new[] { new SummonVar(6m)) | DynamicVars.Summon.UpgradeValueBy(2m); |
| 忧郁 | `MELANCHOLY` | 3 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(13m,  | DynamicVars.Block.UpgradeValueBy(4m); |
| 无处可逃 | `NO_ESCAPE` | 1 | Skill | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[4] { new DynamicVar("Do | DynamicVars.CalculationBase.UpgradeValue |
| 领会 | `PARSE` | 1 | Skill | Uncommon | Self | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 腐败 | `PUTREFY` | 1 | Skill | Uncommon | AnyEnemy | new[] { new DynamicVar("Power", 2m)) | DynamicVars["Power"].UpgradeValueBy(1m); |
| 增生 | `SPUR` | 1 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new SummonVar(3m), | DynamicVars.Summon.UpgradeValueBy(2m); D |
| 幽冥之界 | `UNDERWORLD` | 2 | Skill | Uncommon | Self |  | RemoveKeyword(CardKeyword.Exhaust); |

## colorless（65 张）

| 卡名 | id | 费 | 类型 | 稀有度 | 目标 | 变量 | 升级 |
|---|---|---|---|---|---|---|---|
| 流星锤 | `BOLAS` | 0 | Attack | Rare | AnyEnemy | new[] { new DamageVar(3m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(1m); |
| 金斧 | `GOLD_AXE` | 1 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | AddKeyword(CardKeyword.Retain); |
| 贪婪之手 | `HAND_OF_GREED` | 2 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(20m, | DynamicVars.Damage.UpgradeValueBy(5m); D |
| 大奖 | `JACKPOT` | 3 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(25m, | DynamicVars.Damage.UpgradeValueBy(5m); |
| 击倒 | `KNOCKDOWN` | 3 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(10m, | DynamicVars.Damage.UpgradeValueBy(4m); D |
| 撕碎 | `REND` | 1 | Attack | Rare | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.ExtraDamage.UpgradeValueBy(3 |
| 闪亮登场 | `DRAMATIC_ENTRANCE` | 0 | Attack | Uncommon | AllEnemies | new[] { new DamageVar(11m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 拳斗 | `FISTICUFFS` | 1 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(7m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(2m); |
| 亮剑 | `FLASH_OF_STEEL` | 0 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(5m,  | DynamicVars.Damage.UpgradeValueBy(3m); |
| 群起攻之 | `GANG_UP` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | DynamicVars.ExtraDamage.UpgradeValueBy(2 |
| 心灵震慑 | `MIND_BLAST` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | EnergyCost.UpgradeBy(-1); |
| 万向斩 | `OMNISLICE` | 0 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(8m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 箭雨 | `SALVO` | 1 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(12m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 探寻打击 | `SEEKER_STRIKE` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(9m,  | DynamicVars.Damage.UpgradeValueBy(3m); |
| 双打组合 | `TAG_TEAM` | 2 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(11m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 魔球 | `THE_BALL` | 1 | Attack | Uncommon | AnyEnemy | new Array<DynamicVar>(new DynamicVar[2] { new DamageVar(10m, | DynamicVars["Increase"].UpgradeValueBy(5 |
| 无休手斧 | `THRUMMING_HATCHET` | 1 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(11m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(3m); |
| 究极打击 | `ULTIMATE_STRIKE` | 1 | Attack | Uncommon | AnyEnemy | new[] { new DamageVar(14m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(6m); |
| 连射 | `VOLLEY` | 0 | Attack | Uncommon | RandomEnemy | new[] { new DamageVar(10m, ValueProp.Move)) | DynamicVars.Damage.UpgradeValueBy(4m); |
| 希望灯塔 | `BEACON_OF_HOPE` | 2 | Power | Rare | Self |  | AddKeyword(CardKeyword.Innate); |
| 劫难 | `CALAMITY` | 3 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 熵 | `ENTROPY` | 1 | Power | Rare | Self | new[] { new CardsVar(1)) | AddKeyword(CardKeyword.Innate); |
| 永恒铠甲 | `ETERNAL_ARMOR` | 3 | Power | Rare | Self | new[] { new PowerVar<PlatingPower>(9m)) | DynamicVars["PlatingPower"].UpgradeValue |
| 乱战 | `MAYHEM` | 2 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 怀旧 | `NOSTALGIA` | 1 | Power | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 滚石 | `ROLLING_BOULDER` | 3 | Power | Rare | Self | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<Rolli | DynamicVars["RollingBoulderPower"].Upgra |
| 自动化 | `AUTOMATION` | 1 | Power | Uncommon | Self | new[] { new EnergyVar(1)) | EnergyCost.UpgradeBy(-1); |
| 勒紧 | `FASTEN` | 1 | Power | Uncommon | Self | new[] { new DynamicVar("ExtraBlock", 4m)) | DynamicVars["ExtraBlock"].UpgradeValueBy |
| 神气制胜 | `PANACHE` | 0 | Power | Uncommon | Self | new[] { new DynamicVar("PanacheDamage", 10m)) | DynamicVars["PanacheDamage"].UpgradeValu |
| 准备时间 | `PREP_TIME` | 1 | Power | Uncommon | Self | new[] { new PowerVar<PrepTimePower>(4m)) | DynamicVars["PrepTimePower"].UpgradeValu |
| 非凡技艺 | `PROWESS` | 1 | Power | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new PowerVar<Stren | DynamicVars.Dexterity.UpgradeValueBy(1m) |
| 计策 | `STRATAGEM` | 1 | Power | Uncommon | Self |  | EnergyCost.UpgradeBy(-1); |
| 炼制药水 | `ALCHEMIZE` | 1 | Skill | Rare | Self |  | EnergyCost.UpgradeBy(-1); |
| 天选 | `ANOINTED` | 1 | Skill | Rare | Self |  | AddKeyword(CardKeyword.Retain); |
| 狠揍 | `BEAT_DOWN` | 3 | Skill | Rare | RandomEnemy | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 未掘宝石 | `HIDDEN_GEM` | 1 | Skill | Rare | Self | new[] { new IntVar("Replay", 2m)) | DynamicVars["Replay"].UpgradeValueBy(1m) |
| 战略大师 | `MASTER_OF_STRATEGY` | 0 | Skill | Rare | Self | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 拟态 | `MIMIC` | 1 | Skill | Rare | AnyAlly | new Array<DynamicVar>(new DynamicVar[3] { new CalculationBas | RemoveKeyword(CardKeyword.Exhaust); |
| 集结 | `RALLY` | 2 | Skill | Rare | AllAllies | new[] { new BlockVar(12m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(5m); |
| 潦草急就 | `SCRAWL` | 1 | Skill | Rare | Self |  | AddKeyword(CardKeyword.Retain); |
| 秘密技法 | `SECRET_TECHNIQUE` | 0 | Skill | Rare | Self |  | RemoveKeyword(CardKeyword.Exhaust); |
| 秘密武器 | `SECRET_WEAPON` | 0 | Skill | Rare | Self |  | RemoveKeyword(CardKeyword.Exhaust); |
| 飞溅 | `SPLASH` | 1 | Skill | Rare | Self |  |  |
| 孤注一掷 | `THE_GAMBIT` | 0 | Skill | Rare | Self | new[] { new BlockVar(50m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(25m); |
| 相信着你 | `BELIEVE_IN_YOU` | 0 | Skill | Uncommon | AnyAlly | new[] { new EnergyVar(2)) | DynamicVars.Energy.UpgradeValueBy(1m); |
| 横祸 | `CATASTROPHE` | 2 | Skill | Uncommon | Self | new[] { new CardsVar(2)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 协同配合 | `COORDINATE` | 1 | Skill | Uncommon | AnyAlly | new[] { new PowerVar<StrengthPower>(5m)) | DynamicVars.Strength.UpgradeValueBy(3m); |
| 黑暗镣铐 | `DARK_SHACKLES` | 0 | Skill | Uncommon | AnyEnemy | new[] { new DynamicVar("StrengthLoss", 9m)) | DynamicVars["StrengthLoss"].UpgradeValue |
| 发现 | `DISCOVERY` | 1 | Skill | Uncommon | Self |  | RemoveKeyword(CardKeyword.Exhaust); |
| 均衡 | `EQUILIBRIUM` | 2 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(13m,  | DynamicVars.Block.UpgradeValueBy(3m); |
| 妙计 | `FINESSE` | 0 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(4m, V | DynamicVars.Block.UpgradeValueBy(3m); |
| 抱团 | `HUDDLE_UP` | 1 | Skill | Uncommon | AllAllies | new[] { new CardsVar(2)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 急躁 | `IMPATIENCE` | 0 | Skill | Uncommon | Self | new[] { new CardsVar(2)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 拦截 | `INTERCEPT` | 1 | Skill | Uncommon | AnyAlly | new[] { new BlockVar(9m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(4m); |
| 花样百出 | `JACK_OF_ALL_TRADES` | 0 | Skill | Uncommon | Self | new[] { new CardsVar(1)) | DynamicVars.Cards.UpgradeValueBy(1m); |
| 托举 | `LIFT` | 1 | Skill | Uncommon | AnyAlly | new[] { new BlockVar(11m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(5m); |
| 应急按钮 | `PANIC_BUTTON` | 0 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new BlockVar(30m,  | DynamicVars.Block.UpgradeValueBy(10m); |
| 生产制造 | `PRODUCTION` | 0 | Skill | Uncommon | Self | new[] { new EnergyVar(2)) | DynamicVars.Energy.UpgradeValueBy(1m); |
| 延伸 | `PROLONG` | 0 | Skill | Uncommon | Self |  | RemoveKeyword(CardKeyword.Exhaust); |
| 净化 | `PURITY` | 0 | Skill | Uncommon | Self | new[] { new CardsVar(3)) | DynamicVars.Cards.UpgradeValueBy(2m); |
| 心神不宁 | `RESTLESSNESS` | 0 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new CardsVar(2), n | DynamicVars.Cards.UpgradeValueBy(1m); Dy |
| 震荡波 | `SHOCKWAVE` | 2 | Skill | Uncommon | AllEnemies | new[] { new DynamicVar("Power", 3m)) | DynamicVars["Power"].UpgradeValueBy(2m); |
| 炸弹 | `THE_BOMB` | 2 | Skill | Uncommon | Self | new Array<DynamicVar>(new DynamicVar[2] { new DynamicVar("Tu | DynamicVars["BombDamage"].UpgradeValueBy |
| 深谋远虑 | `THINKING_AHEAD` | 0 | Skill | Uncommon | Self | new[] { new CardsVar(2)) | RemoveKeyword(CardKeyword.Exhaust); |
| 究极防御 | `ULTIMATE_DEFEND` | 1 | Skill | Uncommon | Self | new[] { new BlockVar(11m, ValueProp.Move)) | DynamicVars.Block.UpgradeValueBy(4m); |