using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Viridis.Pools;

namespace Viridis.Cultivate;

/// <summary>栽培机制核心逻辑（原型版）。</summary>
public static class CultivateSystem
{
    public static int XOf(CardModel card) => Math.Clamp(card.EnergyCost.Canonical, 1, 3);

    public static CardRarity? TargetRarity(CardRarity r) => r switch
    {
        CardRarity.Basic or CardRarity.Common or CardRarity.Event or CardRarity.Token => CardRarity.Uncommon,
        CardRarity.Uncommon => CardRarity.Rare,
        CardRarity.Rare => CardRarity.Ancient,
        CardRarity.Ancient => CardRarity.Ancient,
        CardRarity.Status or CardRarity.Quest => CardRarity.Common,
        CardRarity.Curse => CardRarity.Status,
        _ => null
    };

    /// <summary>弹出手牌选择并栽培选中的那张；没有可选目标时返回 false。</summary>
    public static async Task<bool> CultivateFromHand(PlayerChoiceContext ctx, Player player, AbstractModel source, int extraTurns = 0)
    {
        if (!CultivateFlags.CanCultivate(player))
        {
            Log.Info("[Cultivate] cannot cultivate this turn (树液)");
            return false;
        }

        var hand = player.PlayerCombatState?.Hand.Cards;
        var candidates = hand?
            .Where(c => c.Pile?.Type == PileType.Hand)
            .ToList() ?? new List<CardModel>();

        if (candidates.Count == 0)
        {
            Log.Info("[Cultivate] no cultivable card in hand");
            return false;
        }

        var prefs = new CardSelectorPrefs(new LocString("cards", "CULTIVATE_TEST.select_prompt"), 1)
        {
            Cancelable = false,
            RequireManualConfirmation = false
        };

        var selected = await CardSelectCmd.FromHand(ctx, player, prefs, c => candidates.Contains(c), source);
        var target = selected?.FirstOrDefault();

        if (target is null)
        {
            Log.Info("[Cultivate] nothing selected");
            return false;
        }

        await Cultivate(ctx, player, target);

        if (extraTurns != 0)
        {
            var power = player.Creature?.GetPower<CultivateFieldPower>();
            var last = power?.Pending.LastOrDefault();
            if (last is not null)
            {
                last.TurnsLeft += extraTurns;
                CultivatePileUI.Refresh(player);
            }
        }

        return true;
    }

    // ---------- 事件通知 ----------

    private static async Task NotifyCultivated(PlayerChoiceContext ctx, Player player)
    {
        foreach (var p in player.Creature.Powers.OfType<CultivatePowerBase>().ToList())
        {
            try
            {
                await p.OnCultivated(ctx, player);
            }
            catch (Exception e)
            {
                Log.Error($"[Cultivate] {p.GetType().Name}.OnCultivated failed: {e.Message}");
            }
        }
    }

    private static async Task NotifyHarvested(PlayerChoiceContext ctx, Player player, CardModel card)
    {
        foreach (var p in player.Creature.Powers.OfType<CultivatePowerBase>().ToList())
        {
            try
            {
                await p.OnHarvested(ctx, player, card);
            }
            catch (Exception e)
            {
                Log.Error($"[Cultivate] {p.GetType().Name}.OnHarvested failed: {e.Message}");
            }
        }
    }

    private static async Task NotifyCleared(PlayerChoiceContext ctx, Player player, int count)
    {
        foreach (var p in player.Creature.Powers.OfType<CultivatePowerBase>().ToList())
        {
            try
            {
                await p.OnPileCleared(ctx, player, count);
            }
            catch (Exception e)
            {
                Log.Error($"[Cultivate] {p.GetType().Name}.OnPileCleared failed: {e.Message}");
            }
        }
    }

    // ---------- 库数 / 生长 / 清库 ----------

    /// <summary>栽培牌库中的牌数（无上限）。</summary>
    public static int PileCount(Player player)
        => player.Creature?.GetPower<CultivateFieldPower>()?.Pending.Count ?? 0;

    /// <summary>生长 N：所有栽培牌剩余回合 -N，到 0 立即收获。</summary>
    public static async Task Grow(PlayerChoiceContext ctx, Player player, int n)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return;

        foreach (var p in power.Pending)
            p.TurnsLeft = Math.Max(0, p.TurnsLeft - n);

        Log.Info($"[Cultivate] grow {n} -> {string.Join(", ", power.Pending.Select(p => $"{p.Title}:{p.TurnsLeft}"))}");

        await HarvestAll(ctx, player, power);
        CultivatePileUI.Refresh(player);
    }

    /// <summary>把所有栽培牌的剩余回合设为指定值。</summary>
    public static async Task SetAllTurns(PlayerChoiceContext ctx, Player player, int turns)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return;

        foreach (var p in power.Pending)
            p.TurnsLeft = Math.Max(0, turns);

        await HarvestAll(ctx, player, power);
        CultivatePileUI.Refresh(player);
    }

    /// <summary>清空栽培牌库（不收获），返回清掉的张数。</summary>
    public static async Task<int> ClearPileAsync(PlayerChoiceContext ctx, Player player)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null)
            return 0;

        int n = power.Pending.Count;
        power.Pending.Clear();
        CultivatePileUI.Refresh(player);
        Log.Info($"[Cultivate] pile cleared ({n} cards)");

        if (n > 0)
            await NotifyCleared(ctx, player, n);

        return n;
    }

    /// <summary>把栽培牌库中所有牌的剩余回合 +n（延缓）。</summary>
    public static async Task DelayAll(PlayerChoiceContext ctx, Player player, int n)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return;

        foreach (var p in power.Pending)
            p.TurnsLeft += n;

        CultivatePileUI.Refresh(player);
        Log.Info($"[Cultivate] delay {n} -> {string.Join(", ", power.Pending.Select(p => $"{p.Title}:{p.TurnsLeft}"))}");
        await Task.CompletedTask;
    }

    /// <summary>把栽培牌库中所有牌的剩余回合减半（向上取整）。</summary>
    public static async Task HalveAll(PlayerChoiceContext ctx, Player player)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return;

        foreach (var p in power.Pending)
            p.TurnsLeft = (int)Math.Ceiling(p.TurnsLeft / 2.0);

        await HarvestAll(ctx, player, power);
        CultivatePileUI.Refresh(player);
    }

    /// <summary>复制栽培牌库中的一张，复制品剩余回合为 1。</summary>
    public static int DuplicateOne(Player player)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return 0;

        var src = power.Pending[Random.Shared.Next(power.Pending.Count)];
        var copy = new Planting(src.SourceId, src.TargetRarity, 1, src.Count, src.SourceRarity, src.Upgraded, src.Title);
        power.Pending.Add(copy);
        CultivatePileUI.Refresh(player);
        Log.Info($"[Cultivate] duplicated {src.Title}, turns=1");
        return 1;
    }

    /// <summary>从抽牌堆选一张牌栽培。</summary>
    public static async Task<bool> CultivateFromDrawPile(PlayerChoiceContext ctx, Player player, AbstractModel source)
    {
        var pile = player.PlayerCombatState?.DrawPile;
        if (pile is null || pile.Cards.Count == 0)
            return false;

        var prefs = new CardSelectorPrefs(new LocString("cards", "CULTIVATE_TEST.select_prompt"), 1)
        {
            Cancelable = false,
            RequireManualConfirmation = false
        };

        var selected = await CardSelectCmd.FromCombatPile(ctx, pile, player, prefs);
        var target = selected?.FirstOrDefault();
        if (target is null)
            return false;

        await Cultivate(ctx, player, target);
        return true;
    }

    /// <summary>连续栽培若干张手牌。</summary>
    public static async Task<int> CultivateMultiple(PlayerChoiceContext ctx, Player player, AbstractModel source, int count, int extraTurns = 0)
    {
        int done = 0;
        for (int i = 0; i < count; i++)
        {
            if (!await CultivateFromHand(ctx, player, source, extraTurns))
                break;
            done++;
        }
        return done;
    }

    /// <summary>栽培并立即收获（移栽）。</summary>
    public static async Task<bool> CultivateNow(PlayerChoiceContext ctx, Player player, AbstractModel source)
    {
        if (!await CultivateFromHand(ctx, player, source))
            return false;

        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return false;

        var last = power.Pending[^1];
        power.Pending.Remove(last);
        last.TurnsLeft = 0;
        power.Pending.Add(last);
        await HarvestAll(ctx, player, power);
        return true;
    }

    public static async Task Cultivate(PlayerChoiceContext ctx, Player player, CardModel target)
    {
        try
        {
            if (!CultivateFlags.CanCultivate(player))
            {
                Log.Info("[Cultivate] blocked by NoCultivatePower");
                return;
            }

            int canonical = target.EnergyCost.Canonical;
            int x = Math.Clamp(canonical, 1, 3);
            bool isAncient = target.Rarity == CardRarity.Ancient;

            var planting = new Planting(
                target.Id,
                TargetRarity(target.Rarity),
                x,
                isAncient ? 2 : 1,
                target.Rarity,
                target.IsUpgraded,
                target.Title + (target.IsUpgraded ? "+" : ""));

            // 老树盘根：下一次栽培剩余回合为 1
            var fast = player.Creature?.GetPower<NextCultivateFastPower>();
            if (fast is not null)
            {
                planting.TurnsLeft = 1;
                await PowerCmd.Remove(fast);
            }

            await CardPileCmd.RemoveFromCombat(target);

            var power = await GetOrApply(ctx, player, target);
            if (power is null)
            {
                Log.Error("[Cultivate] failed to get field power");
                return;
            }

            power.Pending.Add(planting);
            CultivatePileUI.Refresh(player);
            await NotifyCultivated(ctx, player);
            Log.Info($"[Cultivate] planted {target.Id} cost={canonical} X={x} count={planting.Count}");
        }
        catch (Exception e)
        {
            Log.Error($"[Cultivate] plant failed: {e}");
        }
    }

    private static async Task<CultivateFieldPower?> GetOrApply(PlayerChoiceContext ctx, Player player, CardModel source)
    {
        var existing = player.Creature.GetPower<CultivateFieldPower>();
        if (existing is not null)
            return existing;

        // 用泛型 Apply 让引擎从 ModelDb 克隆实例；自己 new 会触发 DuplicateModelException
        await PowerCmd.Apply<CultivateFieldPower>(ctx, player.Creature, 1m, player.Creature, source);
        return player.Creature.GetPower<CultivateFieldPower>();
    }

    public static async Task HarvestAll(PlayerChoiceContext ctx, Player player, CultivateFieldPower power)
    {
        foreach (var p in power.Pending.Where(p => p.TurnsLeft <= 0).ToList())
        {
            power.Pending.Remove(p);
            for (int i = 0; i < p.Count; i++)
            {
                // 钩子里抛异常会直接搞死战斗循环，必须自己吞掉
                try
                {
                    await HarvestOne(ctx, player, p);
                }
                catch (Exception e)
                {
                    Log.Error($"[Cultivate] harvest failed: {e}");
                }
            }
        }

        CultivatePileUI.Refresh(player);
    }

    private static async Task HarvestOne(PlayerChoiceContext ctx, Player player, Planting p)
    {
        var canonical = p.SourceRarity == CardRarity.Ancient
            ? ModelDb.GetByIdOrNull<CardModel>(p.SourceId)
            : Pick(p.TargetRarity, p.X, p.SourceRarity == CardRarity.Curse);

        if (canonical is null)
        {
            Log.Warn($"[Cultivate] empty pool: target={p.TargetRarity} X={p.X}");
            return;
        }

        Log.Info($"[Cultivate] step1 canonical = {canonical.Id}");

        // 不能用 ToMutable + CreateCloneForPlayer：规范卡的 CardScope 为 null，会 NRE
        // 正确做法是让战斗的 card scope 按规范卡创建实例
        var combat = player.Creature.CombatState;
        if (combat is null)
        {
            Log.Warn("[Cultivate] no combat state");
            return;
        }

        var card = combat.CreateCard(canonical, player);
        Log.Info($"[Cultivate] step2 created = {card?.Id} owner={(card?.Owner is null ? "null" : "ok")}");
        if (card is null) return;

        // 升级继承：被栽培的牌是升级版，收获的牌也是升级版
        if (p.Upgraded && !card.IsUpgraded)
        {
            try
            {
                CardCmd.Upgrade(card);
                Log.Info($"[Cultivate] inherited upgrade -> {card.Id} upgraded={card.IsUpgraded}");
            }
            catch (Exception e)
            {
                Log.Warn($"[Cultivate] upgrade inherit failed: {e.GetType().Name}");
            }
        }

        if (p.X > 1)
            card.EnergyCost.SetUntilPlayed(1, reduceOnly: true);

        // 手牌满则溢出到弃牌堆
        var hand = player.PlayerCombatState?.Hand;
        bool handHasRoom = hand is not null && hand.Cards.Count < CardPile.MaxCardsInHand;
        var dest = handHasRoom ? PileType.Hand : PileType.Discard;

        if (!handHasRoom)
            Log.Info($"[Cultivate] hand full ({hand?.Cards.Count}/{CardPile.MaxCardsInHand}) -> overflow to discard");

        await CardPileCmd.AddGeneratedCardToCombat(card, dest, player);
        Log.Info($"[Cultivate] harvested {card.Id} (X={p.X}, target={p.TargetRarity}, pile={dest})");

        // 世界树：额外获得 1 张同稀有度的牌
        if (player.Creature?.GetPower<WorldTreePower>() is not null)
        {
            bool statusExtra = canonical.Rarity == CardRarity.Status;
            var extra = Pick(canonical.Rarity, p.X, statusExtra) ?? Pick(canonical.Rarity, p.X, statusExtra);
            if (extra is not null && combat.CreateCard(extra, player) is { } extraCard)
            {
                await CardPileCmd.AddGeneratedCardToCombat(extraCard, PileType.Hand, player);
                Log.Info($"[Cultivate] world tree extra -> {extraCard.Id}");
            }
        }

        // 腐殖循环：收获的牌复制一张加入手牌
        if (player.Creature?.GetPower<HumusCyclePower>() is not null)
        {
            if (combat.CreateCard(canonical, player) is { } copyCard)
            {
                await CardPileCmd.AddGeneratedCardToCombat(copyCard, PileType.Hand, player);
                Log.Info($"[Cultivate] humus cycle copy -> {copyCard.Id}");
            }
        }

        await NotifyHarvested(ctx, player, card);
    }

    /// <summary>收获候选池：小松绿角色卡池；诅咒产出的状态改从所有状态牌里选。</summary>
    private static IEnumerable<CardModel> Pool(bool statusOnly)
    {
        if (statusOnly)
        {
            foreach (var c in ModelDb.CardPool<StatusCardPool>().AllCards)
                yield return c;

            yield break;
        }

        foreach (var c in ModelDb.CardPool<ViridisCardPool>().AllCards)
            yield return c;
    }

    private static CardModel? Pick(CardRarity? rarity, int x, bool statusOnly)
    {
        var all = Pool(statusOnly).Where(c => c.Rarity == rarity).ToList();

        // 先要求可生成，实在没有就放宽（原型期避免因为标志位导致空池）
        var generatable = all.Where(c => c.CanBeGeneratedInCombat).ToList();
        var source = generatable.Count > 0 ? generatable : all;

        var exact = source.Where(c => c.EnergyCost.Canonical == x).ToList();
        if (exact.Count > 0)
            return exact[Random.Shared.Next(exact.Count)];

        // 回退：同稀有度任意费用
        if (source.Count > 0)
        {
            Log.Warn($"[Cultivate] no {rarity} card with cost {x}, falling back to any cost");
            return source[Random.Shared.Next(source.Count)];
        }

        return null;
    }
}

public sealed class Planting
{
    public Planting(ModelId sourceId, CardRarity? targetRarity, int x, int count, CardRarity sourceRarity, bool upgraded = false, string title = "")
    {
        SourceId = sourceId;
        TargetRarity = targetRarity;
        X = x;
        Count = count;
        SourceRarity = sourceRarity;
        Upgraded = upgraded;
        Title = title;
        TurnsLeft = x;
    }

    public ModelId SourceId { get; }
    public CardRarity? TargetRarity { get; }
    public CardRarity SourceRarity { get; }
    public int X { get; }
    public int Count { get; }
    public bool Upgraded { get; }
    public string Title { get; }
    public int TurnsLeft { get; set; }
}