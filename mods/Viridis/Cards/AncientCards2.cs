using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Pools;
using Viridis.Powers;

namespace Viridis.Cards;

/// <summary>
/// 先古牌默认卡图：横版「图片暂缺」裁出的竖版版本。
/// 先古牌统一走 832×1248 竖版卡面。
/// </summary>
public abstract class AncientPlaceholderArtCard : PlaceholderArtCard
{
    protected AncientPlaceholderArtCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target)
    {
    }

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: $"{ModEntry.ResPath}/images/cards/card_placeholder_ancient.ctex");
}

/// <summary>珍珠梅：获得 15（20）格挡，移除自身所有负面状态，每移除一种抽 1 张牌。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Sorbaria : AncientPlaceholderArtCard
{
    public Sorbaria() : base(2, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new BlockVar(15m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);

        var player = play.Player;
        var debuffs = player.Creature.Powers
            .Where(power => power.Type == PowerType.Debuff)
            .ToList();

        foreach (var debuff in debuffs)
            await PowerCmd.Remove(debuff);

        if (debuffs.Count > 0)
            await CardPileCmd.Draw(ctx, debuffs.Count, player);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5m);
}

/// <summary>塔黄：对所有敌人造成当前格挡值 2（3）倍的伤害。保留。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class TowerRhubarb : AncientPlaceholderArtCard
{
    public TowerRhubarb() : base(3, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Retain };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var multiplier = IsUpgraded ? 3m : 2m;
        await HitAll(ctx, play, play.Player.Creature.Block * multiplier);
    }
}

/// <summary>小垫柳：获得等同于已损失生命值（升级后两倍）的格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class CushionWillow : AncientPlaceholderArtCard
{
    public CushionWillow() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var lostHp = play.Player.Creature.MaxHp - play.Player.Creature.CurrentHp;
        var multiplier = IsUpgraded ? 2m : 1m;
        await Block(ctx, play, lostHp * multiplier);
    }
}

/// <summary>苹果：获得 25（35）格挡，造成 25（35）伤害。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Apple : AncientPlaceholderArtCard
{
    public Apple() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new BlockVar(25m, ValueProp.Move),
            new DamageVar(25m, ValueProp.Move),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await Hit(ctx, play, play.Target!, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(10m);
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}

/// <summary>金光菊：造成 15（20）伤害，获得 2（3）能量，下回合开始再获得 2（3）能量。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class BlackEyedSusan : AncientPlaceholderArtCard
{
    public BlackEyedSusan() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<EnergyNextTurnPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(15m, ValueProp.Move),
            new EnergyVar(2),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Hit(ctx, play, play.Target!, DynamicVars.Damage.BaseValue);

        var energy = (int)DynamicVars.Energy.BaseValue;
        await Energy(ctx, play, energy);
        await Apply<EnergyNextTurnPower>(ctx, play, play.Player.Creature, energy);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}

/// <summary>美丽月见草：给予易伤，然后造成等同于目标攻击意图总和的伤害。消耗、虚无。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class EveningPrimrose : AncientPlaceholderArtCard
{
    public EveningPrimrose() : base(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust, CardKeyword.Ethereal };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<VulnerablePower>(2m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var target = play.Target!;
        await Apply<VulnerablePower>(ctx, play, target, DynamicVars["VulnerablePower"].BaseValue);
        await Hit(ctx, play, target, GetAttackIntentDamage(target, play.Player));
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VulnerablePower"].UpgradeValueBy(2m);
    }

    private static decimal GetAttackIntentDamage(Creature target, Player player)
    {
        var monster = target.Monster;
        if (monster == null)
        {
            return 0m;
        }

        return monster.NextMove.Intents
            .OfType<AttackIntent>()
            .Sum(intent => (decimal)intent.GetTotalDamage(new[] { player.Creature }, target));
    }
}

/// <summary>黄刺玫：每打出一张攻击牌，获得荆棘。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class YellowRose : AncientPlaceholderArtCard
{
    public YellowRose() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<YellowRosePower>(),
        HoverTipFactory.FromPower<ThornsPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<YellowRosePower>(1m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<YellowRosePower>(ctx, play, play.Player.Creature,
            DynamicVars["YellowRosePower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["YellowRosePower"].UpgradeValueBy(1m);
    }
}

/// <summary>云母蔓绿绒：抽牌，并在接下来若干回合保留手牌、能量和格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class MicaPhilodendron : AncientPlaceholderArtCard
{
    public MicaPhilodendron() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<MicaWardPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<MicaWardPower>(1m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CardPileCmd.Draw(ctx, 3m, play.Player);
        await Apply<MicaWardPower>(ctx, play, play.Player.Creature,
            DynamicVars["MicaWardPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MicaWardPower"].UpgradeValueBy(1m);
    }
}

/// <summary>苦苣苔：造成伤害并抽牌。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Bittercress : AncientPlaceholderArtCard
{
    public Bittercress() : base(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(12m, ValueProp.Move),
            new IntVar("Draw", 1m),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Hit(ctx, play, play.Target!, DynamicVars.Damage.BaseValue);
        await CardPileCmd.Draw(ctx, DynamicVars["Draw"].BaseValue, play.Player);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Draw"].UpgradeValueBy(1m);
    }
}
