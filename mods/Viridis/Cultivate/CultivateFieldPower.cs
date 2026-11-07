using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Viridis.Cultivate;

/// <summary>鎸傚湪鐜╁韬笂鐨?鐢?锛氳褰曞凡鏍藉煿鐨勭墝涓庡€掕鏃躲€?/summary>
public sealed class CultivateFieldPower : PowerModel
{
    public List<Planting> Pending { get; } = new();

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Pending.Count == 0)
            return;

        foreach (var p in Pending)
            p.TurnsLeft--;

        await CultivateSystem.HarvestAll(choiceContext, player, this);

        // 倒计时变化后刷新牌堆显示
        CultivatePileUI.Refresh(player);
    }
}
