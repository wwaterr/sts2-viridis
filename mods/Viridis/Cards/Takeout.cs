using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Viridis.Cards;

/// <summary>外卖：签到码衍生。获得 同接 点格挡（升级后 同接×2）。消耗。</summary>
[RegisterCard(typeof(TokenCardPool))]
public sealed class Takeout : PlaceholderArtCard
{
    public Takeout() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(ViewerCountKeyword.Value),
    };

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int hype = HypeSystem.GetAmount(play.Player);
        await Block(ctx, play, hype * (int)DynamicVars["Multiplier"].BaseValue);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new IntVar("Multiplier", 1m) };

    protected override void OnUpgrade() => DynamicVars["Multiplier"].UpgradeValueBy(1m);
}
