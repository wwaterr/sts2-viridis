using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Viridis.Cards;
using Viridis.Characters;
using Viridis.Powers;

namespace Viridis.Hype;

/// <summary>同接/入脑/反踩体系的共用入口。</summary>
public static class HypeSystem
{
    public const int MaxPolarityStacks = 8;

    public static int GetAmount(Player? player)
        => (int)(player?.Creature?.GetPower<HypeCounterPower>()?.Amount ?? 0);

    /// <summary>入脑层数。</summary>
    public static int EarwormStacks(Player? player)
        => (int)(player?.Creature?.GetPower<EarwormPower>()?.Amount ?? 0);

    /// <summary>反踩层数。</summary>
    public static int BacklashStacks(Player? player)
        => (int)(player?.Creature?.GetPower<BacklashPower>()?.Amount ?? 0);

    /// <summary>有符号极性值（正=入脑，负=反踩，0=黑听）。</summary>
    public static int NetPolarity(Player? player)
        => Math.Clamp(EarwormStacks(player) - BacklashStacks(player), -MaxPolarityStacks, MaxPolarityStacks);

    /// <summary>黑听：入脑与反踩均为 0。</summary>
    public static bool IsLurking(Player? player)
        => player is not null && EarwormStacks(player) == 0 && BacklashStacks(player) == 0;

    /// <summary>同接获取倍率（全力绿）。</summary>
    public static int Multiplier(Player? player)
    {
        int value = (int)(player?.Creature?.GetPower<AllGreenPower>()?.Amount ?? 1);
        return value < 1 ? 1 : value;
    }

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

    /// <summary>增加同接（会经过全力绿的倍率）。</summary>
    public static void Add(Player? player, int amount)
    {
        if (player is null || amount <= 0)
            return;

        var power = player.Creature?.GetPower<HypeCounterPower>();
        if (power is null)
            return;

        int gained = amount * Multiplier(player);
        power.SetAmount((int)power.Amount + gained);
        HypeCounterUI.Refresh(player);

        var coinStream = player.Creature?.GetPower<CoinStreamPower>();
        if (coinStream is not null)
            TaskHelper.RunSafely(coinStream.OnHypeGained(gained));

        var watcher = player.Creature?.GetPower<SheIsTalkingPower>();
        watcher?.NotifyHype((int)power.Amount);
    }

    /// <summary>生成一张衍生牌并加入手牌；直播间皇帝会在此时把它升级。</summary>
    public static async Task GiveToken<T>(PlayerChoiceContext ctx, Player player) where T : CardModel
    {
        var state = player.Creature?.CombatState;
        if (state is null)
            return;

        var card = state.CreateCard<T>(player);
        if (!card.IsUpgraded && player.Creature?.GetPower<StreamEmperorPower>() is not null)
            CardCmd.Upgrade(card, CardPreviewStyle.None);

        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player, CardPilePosition.Top);
    }

    /// <summary>栽培成功后的手牌联动：锡纸帽本回合每栽培一张减 1 费。</summary>
    public static void OnCultivated(Player player)
    {
        try
        {
            foreach (var card in PileType.Hand.GetPile(player).Cards)
            {
                if (card is FoilHat)
                    card.EnergyCost.UpgradeBy(-1);
            }
        }
        catch
        {
            // 栽培流程里不能因为费用调整失败而中断
        }
    }

    /// <summary>取走全部同接并清零，返回取走的数量。</summary>
    public static int TakeAll(Player? player)
    {
        var power = player?.Creature?.GetPower<HypeCounterPower>();
        if (power is null)
            return 0;

        int value = (int)power.Amount;
        power.SetAmount(0);
        if (player is not null)
            HypeCounterUI.Refresh(player);

        return value;
    }

    /// <summary>获得入脑：先抵掉等量反踩，剩余部分上限 8，溢出转化为拉伸。</summary>
    public static async Task GainEarworm(PlayerChoiceContext ctx, Player player, int amount)
    {
        if (amount <= 0)
            return;

        int remain = await ReduceOpposite<BacklashPower>(ctx, player, amount);
        if (remain <= 0)
        {
            PolarityBarUI.Refresh(player);
            return;
        }

        int current = EarwormStacks(player);
        int room = Math.Max(0, MaxPolarityStacks - current);
        int apply = Math.Min(remain, room);
        int overflow = remain - apply;

        if (apply > 0)
            await PowerCmd.Apply<EarwormPower>(ctx, player.Creature, apply, player.Creature, null);

        if (overflow > 0)
            await PowerCmd.Apply<StretchPower>(ctx, player.Creature, overflow, player.Creature, null);

        PolarityBarUI.Refresh(player);
    }

    /// <summary>获得反踩：先抵掉等量入脑，剩余部分上限 8，溢出转化为活力。</summary>
    public static async Task GainBacklash(PlayerChoiceContext ctx, Player player, int amount)
    {
        if (amount <= 0)
            return;

        int remain = await ReduceOpposite<EarwormPower>(ctx, player, amount);
        if (remain <= 0)
        {
            PolarityBarUI.Refresh(player);
            return;
        }

        int current = BacklashStacks(player);
        int room = Math.Max(0, MaxPolarityStacks - current);
        int apply = Math.Min(remain, room);
        int overflow = remain - apply;

        if (apply > 0)
            await PowerCmd.Apply<BacklashPower>(ctx, player.Creature, apply, player.Creature, null);

        if (overflow > 0)
            await PowerCmd.Apply<VigorPower>(ctx, player.Creature, overflow, player.Creature, null);

        PolarityBarUI.Refresh(player);
    }

    private static async Task<int> ReduceOpposite<TOpposite>(PlayerChoiceContext ctx, Player player, int amount)
        where TOpposite : PowerModel
    {
        var opposite = player.Creature?.GetPower<TOpposite>();
        if (opposite is null)
            return amount;

        int current = (int)opposite.Amount;
        int reduced = Math.Min(current, amount);
        int left = current - reduced;
        if (left <= 0)
            await PowerCmd.Remove(opposite);
        else
            opposite.SetAmount(left);

        return amount - reduced;
    }
}
