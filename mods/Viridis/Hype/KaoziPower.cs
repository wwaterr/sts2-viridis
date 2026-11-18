using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Viridis.Hype;

/// <summary>考子：回合开始时增加等同于层数的同接。</summary>
public sealed class KaoziPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return Task.CompletedTask;

        HypeSystem.Add(player, Amount);
        return Task.CompletedTask;
    }
}
