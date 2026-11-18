using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Viridis.Cultivate;

/// <summary>挂在玩家身上的能力：记录已栽培的牌与倒计时。</summary>
public sealed class CultivateFieldPower : PowerModel
{
    public List<Planting> Pending { get; } = new();

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>内部计数器，不显示在角色能力栏。</summary>
    protected override bool IsVisibleInternal => false;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Pending.Count == 0)
            return;

        var progress = 1 + (Owner.GetPower<FarmerFormPower>()?.Amount ?? 0);
        foreach (var p in Pending.Where(p => !p.Permanent))
            p.TurnsLeft -= progress;

        await CultivateSystem.HarvestAll(choiceContext, player, this);

        // 倒计时变化后刷新牌堆显示
        CultivatePileUI.Refresh(player);
    }

    /// <summary>永久栽培牌在己方回合结束时再触发一次。</summary>
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(Owner))
            return;

        if (!Pending.Any(p => p.Permanent))
            return;

        var player = Owner.Player;
        if (player is null)
            return;

        await CultivateSystem.TriggerPermanents(choiceContext, player, this);
    }
}
