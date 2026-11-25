// 已按改版要求暂时注释停用（锡纸帽）
/*
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Viridis.Cards;

/// <summary>锡纸帽：入脑衍生。获得 入脑×2 格挡；本回合每栽培一张牌费用 -1；栽入抽牌。虚无，消耗。</summary>
[RegisterCard(typeof(TokenCardPool))]
public sealed class FoilHat : PlaceholderArtCard
{
    public FoilHat() : base(2, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(EarwormKeyword.Value),
        HoverTipFactory.FromKeyword(PlantInKeyword.Value),
    };

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Ethereal, CardKeyword.Exhaust, PlantInKeyword.Value };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new IntVar("Multiplier", 2m), new CardsVar(1) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int stacks = HypeSystem.EarwormStacks(play.Player);
        await Block(ctx, play, stacks * (int)DynamicVars["Multiplier"].BaseValue);
    }

    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
        => await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, player);

    protected override void OnUpgrade()
    {
        DynamicVars["Multiplier"].UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
*/
