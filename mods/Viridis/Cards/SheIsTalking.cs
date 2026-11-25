using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Hype;
using Viridis.Pools;

namespace Viridis.Cards;

/// <summary>她在和谁说话：1 费罕见能力。每当回合结束前同接达到 8（6），次回合开始时生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class SheIsTalking : CultivateCard
{
    public SheIsTalking() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(ViewerCountKeyword.Value),
        HoverTipFactory.FromKeyword(CultivateKeyword.Value),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<SheIsTalkingPower>(8m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int threshold = (int)DynamicVars["SheIsTalkingPower"].BaseValue;
        var existing = play.Player.Creature.GetPower<SheIsTalkingPower>();
        if (existing is null)
            await Apply<SheIsTalkingPower>(ctx, play, play.Player.Creature, threshold);
        else if (threshold < existing.Amount)
            existing.SetAmount(threshold);

        play.Player.Creature.GetPower<SheIsTalkingPower>()?.NotifyHype(HypeSystem.GetAmount(play.Player));
    }

    protected override void OnUpgrade() => DynamicVars["SheIsTalkingPower"].UpgradeValueBy(-2m);
}
