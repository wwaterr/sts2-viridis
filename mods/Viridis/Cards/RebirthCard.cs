using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Powers;

namespace Viridis.Cards;

/// <summary>转世：获得若干层转世，枯萎时复制栽培牌，并在自己的回合内阻止致死。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Rebirth : PlaceholderArtCard
{
    public Rebirth()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<RebirthPower>(3m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<RebirthPower>(ctx, play, play.Player.Creature, DynamicVars["RebirthPower"].BaseValue);
    }

    protected override void OnUpgrade()
        => DynamicVars["RebirthPower"].UpgradeValueBy(2m);
}
