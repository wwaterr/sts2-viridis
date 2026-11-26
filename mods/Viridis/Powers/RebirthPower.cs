using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Cultivate;

namespace Viridis.Powers;

/// <summary>
/// 转世：枯萎栽培区的牌时消耗一层并重新栽培其复制；在自己的回合内阻止致死并在复活后清除全部能力。
/// </summary>
public sealed class RebirthPower : CultivatePowerBase
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile =>
        new($"{ModEntry.ResPath}/images/powers/rebirth.ctex", $"{ModEntry.ResPath}/images/powers/rebirth.ctex");

    public override async Task OnWithered(PlayerChoiceContext ctx, Player player, Planting planting)
    {
        if (Owner.Player != player || Amount <= 0)
            return;

        await PowerCmd.Decrement(this);
        if (Amount <= 0)
            await PowerCmd.Remove(this);

        var canonical = ModelDb.GetByIdOrNull<CardModel>(planting.SourceId);
        if (canonical is null)
        {
            Log.Warn($"[Rebirth] missing card {planting.SourceId}");
            return;
        }

        var copy = player.RunState.CreateCard(canonical, player);
        if (planting.Upgraded && copy.IsUpgradable)
        {
            copy.UpgradeInternal();
            copy.FinalizeUpgradeInternal();
        }

        Flash();
        await CultivateSystem.CultivateCopy(ctx, player, copy);
        Log.Info($"[Rebirth] recultivated {copy.Id} after {planting.Title} withered");
    }

    public override bool ShouldDie(Creature creature)
    {
        if (creature != Owner || Amount <= 0)
            return true;

        // 只在玩家自己的回合内生效；敌方回合的攻击致死不触发。
        var state = Owner.CombatState;
        return state is null || state.CurrentSide != CombatSide.Player;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (creature != Owner)
            return;

        Flash();
        var missing = Owner.MaxHp - Owner.CurrentHp;
        if (missing > 0)
            await CreatureCmd.Heal(Owner, missing);

        // “失去拥有的所有效果”：清除玩家身上的全部能力（增益、减益与隐藏能力），最后移除转世自身。
        foreach (var power in Owner.Powers.Where(p => p != this).ToList())
        {
            try
            {
                await PowerCmd.Remove(power);
            }
            catch (Exception e)
            {
                Log.Error($"[Rebirth] failed to remove {power.GetType().Name}: {e.Message}");
            }
        }

        await PowerCmd.Remove(this);
        Log.Info("[Rebirth] revived to full HP and cleared all powers");
    }
}
