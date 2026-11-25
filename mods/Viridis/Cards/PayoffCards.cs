using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Cultivate;

namespace Viridis.Cards;

file static class FieldCount
{
    public static int For(CardModel card) =>
        card.IsMutable && card.Owner is { } player ? CultivateSystem.PileCount(player) : 0;
}

/// <summary>气生根：5（8）格挡，每株作物额外 2 格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class AerialRoot : CultivateCard
{
    public AerialRoot() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        if (Pile(play) > 0)
            await Block(ctx, play, DynamicVars.Block.BaseValue);
        else
            await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}

/// <summary>顶端优势：5（8）伤害，有作物时额外 5 点。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class ApicalDominance : CultivateCard
{
    public ApicalDominance() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        if (Pile(play) > 0)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>引水：获得 1 能量；库数非空时抽 1 张。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class WaterDrawing : CultivateCard
{
    public WaterDrawing() : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await HitAll(ctx, play, DynamicVars.Damage.BaseValue);
        int energy = Pile(play);
        if (energy > 0)
            await Energy(ctx, play, energy);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}

/// <summary>煎蛋！：本回合获得 3（5）点力量。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class FriedEgg : CultivateCard
{
    public FriedEgg() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<TempStrengthPower>(),
    ];


    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(3m)];

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => Apply<TempStrengthPower>(ctx, play, play.Player.Creature, DynamicVars.Strength.BaseValue);

    protected override void OnUpgrade() => DynamicVars.Strength.UpgradeValueBy(2m);
}

/// <summary>喜多护佑：获得 7（10）点格挡，移除自己的虚弱。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class KitaBlessing : CultivateCard
{
    public KitaBlessing() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }


    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await PowerCmd.Remove(play.Player.Creature.GetPower<WeakPower>());
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

/// <summary>杀植物：令栽培区所有牌枯萎；每枯萎 1 张获得 1 能量与 3 格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class MeltToWater : CultivateCard
{
    public MeltToWater() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("BlockPerCrop", 3m)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(WitherKeyword.Value)
    };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = CultivateSystem.WitherAll(play.Player);
        if (n <= 0)
            return;

        await PlayerCmd.GainEnergy(n, play.Player);
        await Block(ctx, play, DynamicVars["BlockPerCrop"].BaseValue * n);
    }

    protected override void OnUpgrade() => DynamicVars["BlockPerCrop"].UpgradeValueBy(1m);
}

/// <summary>光合脉冲：3（4）伤害，每株作物多攻击一次。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class PhotosyntheticPulse : CultivateCard
{
    public PhotosyntheticPulse() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    private const string HitsKey = "CalculatedHits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new CalculationBaseVar(1m),
        new CalculationExtraVar(1m),
        new CalculatedVar(HitsKey).WithMultiplier((card, _) => FieldCount.For(card))
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        int hits = (int)((CalculatedVar)DynamicVars[HitsKey]).Calculate(play.Target);
        for (int i = 0; i < hits && play.Target.IsAlive; i++)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

/// <summary>荆刺：6（9）伤害，获得 2 格挡，每株作物额外 1 格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Thorns : CultivateCard
{
    public Thorns() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new CalculationBaseVar(2m),
        new CalculationExtraVar(1m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) => FieldCount.For(card))
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var block = DynamicVars.CalculatedBlock.Calculate(play.Target);
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);

        await Block(ctx, play, block);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>树脂护壳：14（17）格挡，每株作物额外 2 格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class ResinShell : CultivateCard
{
    public ResinShell() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(14m),
        new CalculationExtraVar(2m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) => FieldCount.For(card))
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Block(ctx, play, DynamicVars.CalculatedBlock.Calculate(play.Target));

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(3m);
}

/// <summary>叶绿素爆发：对单个敌人造成每株作物 6 点伤害，至少 12 点。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class ChlorophyllBurst : CultivateCard
{
    public ChlorophyllBurst() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(12m),
        new ExtraDamageVar(6m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => Math.Max(0, FieldCount.For(card) - 2))
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, DynamicVars.CalculatedDamage.Calculate(play.Target));
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(4m);
}

/// <summary>老根：10（13）格挡，每株作物额外 4 格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class OldRoots : CultivateCard
{
    public OldRoots() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(10m),
        new CalculationExtraVar(4m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) => FieldCount.For(card))
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Block(ctx, play, DynamicVars.CalculatedBlock.Calculate(play.Target));

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(3m);
}
