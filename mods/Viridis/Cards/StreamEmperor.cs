using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Hype;
using Viridis.Pools;

namespace Viridis.Cards;

/// <summary>直播间皇帝：1 费稀有能力。（固有。）每回合首次获得入脑或反踩时，生长 1。每回合入脑或反踩首次降为 0 时，生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class StreamEmperor : CultivateCard
{
    public StreamEmperor() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(EarwormKeyword.Value),
        HoverTipFactory.FromKeyword(BacklashKeyword.Value),
        HoverTipFactory.FromKeyword(GrowKeyword.Value),
        HoverTipFactory.FromPower<StreamEmperorPower>(),
    };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<StreamEmperorPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}
