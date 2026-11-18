using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Viridis.Hype;

/// <summary>串子：回合开始时随机获得 X 次 1 点活力或 1 点拉伸。</summary>
public sealed class ChuanziPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        for (int i = 0; i < Amount; i++)
        {
            if (player.RunState.Rng.Niche.NextBool())
                await PowerCmd.Apply<VigorPower>(choiceContext, player.Creature, 1m, player.Creature, null);
            else
                await PowerCmd.Apply<StretchPower>(choiceContext, player.Creature, 1m, player.Creature, null);
        }
    }
}
