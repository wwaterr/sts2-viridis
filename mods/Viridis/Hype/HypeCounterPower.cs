using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Viridis.Hype;

/// <summary>本回合已经累计的同接数；玩家回合结束时清零。同时在战斗结束时保存入脑/反踩跨局余额。</summary>
public sealed class HypeCounterPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Player is null || cardPlay.Card.Owner != Owner.Player)
            return Task.CompletedTask;

        HypeSystem.Add(Owner.Player, 1);
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

    public override Task AfterCombatEnd(CombatRoom _)
    {
        if (Owner.Player is not null)
            HypeSystem.SaveCarryover(Owner.Player);

        return Task.CompletedTask;
    }
}
