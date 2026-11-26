using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Pools;
using Viridis.Powers;

namespace Viridis.Cards;

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Cedar : AncientPlaceholderArtCard
{
    public Cedar() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self) { }

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        base.CanonicalKeywords.Append(GymnospermKeyword.Value);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(20m, ValueProp.Move),
        new PowerVar<CedarPower>(5m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await Apply<CedarPower>(ctx, play, play.Player.Creature, DynamicVars["CedarPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5m);
        DynamicVars["CedarPower"].UpgradeValueBy(2m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Ginkgo : AncientPlaceholderArtCard
{
    public Ginkgo() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        base.CanonicalKeywords.Append(CardKeyword.Retain).Append(GymnospermKeyword.Value);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(30m, ValueProp.Move),
        new IntVar("Multiplier", 3m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var hand = play.Player.PlayerCombatState?.Hand.Cards;
        var onlyCard = hand is not null && hand.All(c => c == this);
        var damage = onlyCard
            ? DynamicVars.Damage.BaseValue * DynamicVars["Multiplier"].BaseValue
            : DynamicVars.Damage.BaseValue;
        await Hit(ctx, play, play.Target!, damage);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
        DynamicVars["Multiplier"].UpgradeValueBy(1m);
    }
}
