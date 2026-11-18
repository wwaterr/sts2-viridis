using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using Viridis.Characters;
using Viridis.Powers;

namespace Viridis.Hype;

/// <summary>同接体系的共用入口：可见性判断、计数读取、战斗开始时挂载计数器。</summary>
public static class HypeSystem
{
    public static int GetAmount(Player? player)
        => player?.Creature?.GetPower<HypeCounterPower>()?.Amount ?? 0;

    /// <summary>
    /// 小松绿始终显示；其它角色只有在任意战斗牌堆里出现同接相关卡时才显示。
    /// </summary>
    public static bool ShouldShow(Player? player)
    {
        if (player is null)
            return false;

        if (player.Character is ViridisCharacter)
            return true;

        var combatState = player.PlayerCombatState;
        return combatState is not null && combatState.AllCards.Any(IsHypeRelated);
    }

    public static bool IsHypeRelated(CardModel card)
        => card is IHypeCard || card.Keywords.Contains(ViewerCountKeyword.Value);

    /// <summary>确保该玩家有隐藏的同接计数器；重复调用无副作用。</summary>
    public static void Ensure(Player? player)
    {
        if (player?.Creature is null)
            return;

        if (player.Creature.GetPower<HypeCounterPower>() is not null)
            return;

        TaskHelper.RunSafely(ApplyAsync(player));
    }

    private static async Task ApplyAsync(Player player)
    {
        var power = await PowerCmd.Apply<HypeCounterPower>(
            new BlockingPlayerChoiceContext(),
            player.Creature,
            1m,
            player.Creature,
            null,
            silent: true);

        power?.SetAmount(0);
        HypeCounterUI.Refresh(player);
    }

    public static void Add(Player? player, int amount)
    {
        if (player is null || amount <= 0)
            return;

        var power = player.Creature?.GetPower<HypeCounterPower>();
        if (power is null)
            return;

        power.SetAmount(power.Amount + amount);
        HypeCounterUI.Refresh(player);

        var coinStream = player.Creature?.GetPower<CoinStreamPower>();
        if (coinStream is not null)
            TaskHelper.RunSafely(coinStream.OnHypeGained(amount));
    }
}
