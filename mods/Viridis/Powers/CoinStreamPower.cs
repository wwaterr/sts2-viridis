using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Viridis.Powers;

/// <summary>钢镚的临时收益：本回合每打出一张牌获得 2 金币，回合结束移除。</summary>
public sealed class CoinStreamPower : PowerModel
{
    public override PowerType Type => PowerType.None;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner.Player)
            await PlayerCmd.GainGold(2m, Owner.Player);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
            await PowerCmd.Remove(this);
    }
}
