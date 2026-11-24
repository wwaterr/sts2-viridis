using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;
using Viridis.Cultivate;

namespace Viridis.Cards;

/// <summary>她在和谁说话：回合结束前同接达到 8（6）时，下回合开始时生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class SheIsTalking : CultivateCard
{
    public SheIsTalking() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(CultivateKeyword.Value),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<SheIsTalkingPower>(8m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<SheIsTalkingPower>(ctx, play, play.Player.Creature, DynamicVars["SheIsTalkingPower"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["SheIsTalkingPower"].UpgradeValueBy(-2m);
}
