using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Cultivate;

namespace Viridis.Cards;

/// <summary>腐殖土：4 格挡；生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Humus : CultivateCard
{
    protected override bool ShowsGrowKeyword => true;

    public Humus() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await CultivateSystem.Grow(ctx, play.Player, 1);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

/// <summary>催芽：栽培 1 张手牌；生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Germinate : CultivateCard
{
    protected override bool ShowsGrowKeyword => true;

    public Germinate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Growth", 1m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
        await CultivateSystem.Grow(ctx, play.Player, DynamicVars["Growth"].IntValue);
    }

    protected override void OnUpgrade() => DynamicVars["Growth"].UpgradeValueBy(1m);
}

/// <summary>春化：生长 2。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Vernalization : CultivateCard
{
    protected override bool ShowsGrowKeyword => true;

    public Vernalization() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Growth", 2m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.Grow(ctx, play.Player, DynamicVars["Growth"].IntValue);

    protected override void OnUpgrade() => DynamicVars["Growth"].UpgradeValueBy(1m);
}

/// <summary>硼砂：抽 2（3）张牌。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Borax : CultivateCard
{
    public Borax() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }


    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

/// <summary>拔节：7 伤害；生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Bolting : CultivateCard
{
    public Bolting() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(12m, ValueProp.Move),
            new PowerVar<WeakPower>(1m),
            new PowerVar<VulnerablePower>(1m),
        };

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CombatState is not { } combat)
            return;

        var target = Owner.RunState.Rng.CombatTargets.NextItem(combat.HittableEnemies);
        if (target is null)
            return;

        await Hit(ctx, play, target, DynamicVars.Damage.BaseValue);
        await Apply<WeakPower>(ctx, play, target, DynamicVars["WeakPower"].BaseValue);
        await Apply<VulnerablePower>(ctx, play, target, DynamicVars["VulnerablePower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["WeakPower"].UpgradeValueBy(1m);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
    }
}

/// <summary>腐熟：把栽培区内牌的剩余回合设为 1，然后抽 1 张牌。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Compost : CultivateCard
{
    public Compost() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        Log.Info("[Cultivate] compost: set all turns to 1");
        await CultivateSystem.SetAllTurns(ctx, play.Player, 1);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}
