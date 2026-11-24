using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Viridis.Cards;

/// <summary>厕纸：状态牌。抽 2（3）张牌，获得 2（3）层反踩。</summary>
[RegisterCard(typeof(StatusCardPool))]
public sealed class PaperTowel : PlaceholderArtCard
{
    public PaperTowel() : base(0, CardType.Status, CardRarity.Status, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(BacklashKeyword.Value),
    };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);
        await HypeSystem.GainBacklash(ctx, play.Player, (int)DynamicVars["Backlash"].BaseValue);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new CardsVar(2), new IntVar("Backlash", 2m) };

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
        DynamicVars["Backlash"].UpgradeValueBy(1m);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
}
