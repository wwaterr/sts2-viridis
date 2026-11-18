using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Viridis.Hype;

/// <summary>本回合已经累计的同接数；回合结束时清零。</summary>
public sealed class HypeCounterPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Player is null || cardPlay.Card.Owner != Owner.Player)
            return Task.CompletedTask;

        // 反踩在“打牌之后”才结算，但这里必须先判断，否则能力钩子顺序会决定结果。
        var antiStomp = Owner.GetPower<AntiStompPower>();
        if (antiStomp is not null && antiStomp.TryBlockCard())
        {
            HypeCounterUI.Refresh(Owner.Player);
            return Task.CompletedTask;
        }

        SetAmount(Amount + 1);
        HypeCounterUI.Refresh(Owner.Player);
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(Owner))
            return Task.CompletedTask;

        SetAmount(0);
        if (Owner.Player is not null)
            HypeCounterUI.Refresh(Owner.Player);

        return Task.CompletedTask;
    }
}
