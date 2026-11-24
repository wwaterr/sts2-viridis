using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

/// <summary>签到码663645：将一张外卖（升级后为外卖+）置于抽牌堆顶；栽培 1；消耗。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class CheckinCode : CultivateCard
{
    public CheckinCode() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Takeout>(),
        HoverTipFactory.FromKeyword(CultivateKeyword.Value),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var state = play.Player.Creature.CombatState;
        if (state is not null)
        {
            var takeout = state.CreateCard<Takeout>(play.Player);
            if (IsUpgraded)
                CardCmd.Upgrade(takeout, CardPreviewStyle.None);

            await CardPileCmd.AddGeneratedCardToCombat(takeout, PileType.Draw, play.Player, CardPilePosition.Top);
        }

        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }
}
