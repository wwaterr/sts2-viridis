using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using Viridis.Pools;

namespace Viridis.Cards;

/// <summary>直播间皇帝：1 费稀有能力。（固有。）当锡纸帽和独轮车进入你的手牌时，将它们升级。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class StreamEmperor : CultivateCard
{
    public StreamEmperor() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    /*
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<FoilHat>(),
        HoverTipFactory.FromCard<Unicycle>(),
    };
    */

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<StreamEmperorPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}
