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
