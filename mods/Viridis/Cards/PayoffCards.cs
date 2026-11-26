using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Cultivate;
using Viridis.Hype;
using Viridis.Powers;

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
[RegisterCard(typeof(TokenCardPool))]
public sealed class FriedEgg : CultivateCard
{
    public FriedEgg() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Retain, CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<TempStrengthPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TempStrengthPower>(5m)];

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => Apply<TempStrengthPower>(ctx, play, play.Player.Creature, DynamicVars["TempStrengthPower"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["TempStrengthPower"].UpgradeValueBy(2m);
}

/// <summary>喜多：获得 5（8）点活力与拉伸。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class KitaBlessing : CultivateCard
{
    public KitaBlessing() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>(),
        HoverTipFactory.FromPower<StretchPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var amount = HypeSystem.GetAmount(play.Player);
        await Apply<VigorPower>(ctx, play, play.Player.Creature, amount);
        await Apply<StretchPower>(ctx, play, play.Player.Creature, amount);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}

/// <summary>杀植物：令栽培区所有牌枯萎；每枯萎 1 张获得 1 能量与 3 格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class MeltToWater : PlaceholderArtCard
{
    public MeltToWater() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(WitherKeyword.Value)
    };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = await CultivateSystem.WitherAll(ctx, play.Player);
        if (n <= 0)
            return;

        await PlayerCmd.GainEnergy(n, play.Player);
        await Block(ctx, play, DynamicVars.Block.BaseValue * n);
        await CardPileCmd.Draw(ctx, n, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

/// <summary>荆刺：6（9）伤害，获得 2 格挡，每株作物额外 1 格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Thorns : CultivateCard
{
    public Thorns() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<ThornsPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new IntVar("Hits", 3m),
        new IntVar("Thorns", 3m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue, hits: (int)DynamicVars["Hits"].BaseValue);
        await Apply<ViridisTempThornsPower>(ctx, play, play.Player.Creature, DynamicVars["Thorns"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["Hits"].UpgradeValueBy(1m);
        DynamicVars["Thorns"].UpgradeValueBy(1m);
    }
}

/// <summary>树脂！！！：造成 28（34）点伤害，给予目标 4 点活力。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class ResinShell : CultivateCard
{
    public ResinShell() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(28m, ValueProp.Move),
        new PowerVar<VigorPower>(4m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        if (play.Target.IsAlive)
            await Apply<VigorPower>(ctx, play, play.Target, DynamicVars["VigorPower"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}

/// <summary>枼绿素爆发：按当前同接造成伤害，每有 1 层考子重复 1 次。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class ChlorophyllBurst : CultivateCard
{
    public ChlorophyllBurst() : base(3, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var player = play.Player;
        var enemies = player.Creature.CombatState?.HittableEnemies;
        if (enemies is null || enemies.Count == 0)
            return;

        int damage = HypeSystem.GetAmount(player);
        if (damage <= 0)
            return;

        int kaozi = player.Creature.GetPower<KaoziPower>()?.Amount ?? 0;
        for (int i = 0; i <= kaozi; i++)
        {
            var target = player.RunState.Rng.CombatTargets.NextItem(enemies);
            if (target is null)
                break;

            await Hit(ctx, play, target, damage);
        }
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
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
