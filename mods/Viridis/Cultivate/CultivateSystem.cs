using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Viridis.Cards;
using Viridis.Pools;
using Viridis.Relics;

namespace Viridis.Cultivate;

/// <summary>栽培机制核心逻辑（原型版）。</summary>
public static class CultivateSystem
{
    public static int XOf(CardModel card) =>
        card.EnergyCost.CostsX ? 1 : Math.Clamp(card.EnergyCost.Canonical, 1, 3);

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
    public static Task<bool> CultivateFromHand(PlayerChoiceContext ctx, Player player, AbstractModel source, int extraTurns = 0)
        => PlantFromHand(ctx, player, source, extraTurns, harvestImmediately: false);

    /// <summary>栽培一张牌；若它有倒计时，立即收获它（移栽）。</summary>
    public static Task<bool> CultivateNow(PlayerChoiceContext ctx, Player player, AbstractModel source)
        => PlantFromHand(ctx, player, source, extraTurns: 0, harvestImmediately: true);

    private static async Task<bool> PlantFromHand(PlayerChoiceContext ctx, Player player, AbstractModel source, int extraTurns, bool harvestImmediately)
    {
        if (!CultivateFlags.CanCultivate(player))
        {
            Log.Info("[Cultivate] cannot cultivate this turn (树液)");
            return false;
        }

        var hand = player.PlayerCombatState?.Hand;
        if (hand is null || hand.Cards.Count == 0)
        {
            Log.Info("[Cultivate] no cultivable card in hand");
            return false;
        }

        var candidates = hand.Cards.ToList();
        var prefs = new CardSelectorPrefs(new LocString("cards", "VIRIDIS_CARD_CULTIVATE_HAND_PROMPT"), 1)
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

        return await PlantCard(ctx, player, target, hand, extraTurns, harvestImmediately);
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

        if (player.Relics.OfType<HarvestCharm>().FirstOrDefault() is { } charm)
            await charm.OnHarvested(player);
    }

    // ---------- 库数 / 生长 / 清库 ----------

    /// <summary>栽培区中的牌数（无上限）。</summary>
    public static int PileCount(Player player)
        => player.Creature?.GetPower<CultivateFieldPower>()?.Pending.Count ?? 0;

    /// <summary>生长 N：所有栽培牌剩余回合 -N，到 0 立即收获。</summary>
    public static Task Grow(PlayerChoiceContext ctx, Player player, int n)
        => Grow(ctx, player, n, cultivatedOnTurn: null);

    public static Task GrowCultivatedThisTurn(PlayerChoiceContext ctx, Player player, int n)
        => player.PlayerCombatState is { } combat
            ? Grow(ctx, player, n, combat.TurnNumber)
            : Task.CompletedTask;

    private static async Task Grow(PlayerChoiceContext ctx, Player player, int n, int? cultivatedOnTurn)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return;

        var crops = power.Pending.Where(p => !p.Permanent &&
            (cultivatedOnTurn is null || p.CultivatedOnTurn == cultivatedOnTurn));
        foreach (var p in crops)
            p.TurnsLeft = Math.Max(0, p.TurnsLeft - n);

        Log.Info($"[Cultivate] grow {n} -> {string.Join(", ", power.Pending.Select(p => $"{p.Title}:{p.TurnsLeft}"))}");

        await HarvestReady(ctx, player, power, crops);
    }

    /// <summary>把所有栽培牌的剩余回合设为指定值。</summary>
    public static async Task SetAllTurns(PlayerChoiceContext ctx, Player player, int turns)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return;

        foreach (var p in power.Pending.Where(p => !p.Permanent))
            p.TurnsLeft = Math.Max(0, turns);

        await HarvestAll(ctx, player, power);
        CultivatePileUI.Refresh(player);
    }

    /// <summary>令栽培区所有牌枯萎（不收获、不返还原卡），返回枯萎的张数。</summary>
    public static int WitherAll(Player player)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return 0;

        int n = power.Pending.Count;
        power.Pending.Clear();
        CultivatePileUI.Refresh(player);
        Log.Info($"[Cultivate] withered all ({n} cards)");
        return n;
    }

    /// <summary>枯萎：把一张栽培牌移出栽培区，不收获，也不返还原卡。不指定目标时随机移除一张。</summary>
    public static bool Wither(Player player, Planting? target = null)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return false;

        var planting = target ?? power.Pending[Random.Shared.Next(power.Pending.Count)];
        if (!power.Pending.Remove(planting))
            return false;

        CultivatePileUI.Refresh(player);
        Log.Info($"[Cultivate] withered {planting.Title}");
        return true;
    }

    /// <summary>从栽培区选择一张牌；返回 null 表示没有可选牌或玩家取消。</summary>
    public static async Task<Planting?> SelectPlanting(PlayerChoiceContext ctx, Player player, string promptKey)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return null;

        var pairs = new List<(Planting Planting, CardModel Card)>();
        foreach (var planting in power.Pending.ToList())
        {
            var canonical = ModelDb.GetByIdOrNull<CardModel>(planting.SourceId);
            if (canonical is null)
                continue;

            var display = player.RunState.CreateCard(canonical, player);
            if (planting.Upgraded && display.IsUpgradable)
            {
                display.UpgradeInternal();
                display.FinalizeUpgradeInternal();
            }

            pairs.Add((planting, display));
        }

        if (pairs.Count == 0)
            return null;

        var prefs = new CardSelectorPrefs(new LocString("cards", promptKey), 1)
        {
            Cancelable = false,
            RequireManualConfirmation = false
        };
        var selected = await CardSelectCmd.FromSimpleGrid(ctx, pairs.Select(pair => pair.Card).ToList(), player, prefs);
        var chosen = selected.FirstOrDefault();
        if (chosen is null)
            return null;

        foreach (var pair in pairs)
        {
            if (ReferenceEquals(pair.Card, chosen))
                return pair.Planting;
        }

        return null;
    }

    /// <summary>把栽培区中所有牌的剩余回合 +n（延缓）。</summary>
    public static async Task DelayAll(PlayerChoiceContext ctx, Player player, int n)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return;

        foreach (var p in power.Pending.Where(p => !p.Permanent))
            p.TurnsLeft += n;

        CultivatePileUI.Refresh(player);
        Log.Info($"[Cultivate] delay {n} -> {string.Join(", ", power.Pending.Select(p => $"{p.Title}:{p.TurnsLeft}"))}");
        await Task.CompletedTask;
    }

    /// <summary>把栽培区中所有牌的剩余回合减半（向上取整）。</summary>
    public static async Task HalveAll(PlayerChoiceContext ctx, Player player)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return;

        foreach (var p in power.Pending.Where(p => !p.Permanent))
            p.TurnsLeft = (int)Math.Ceiling(p.TurnsLeft / 2.0);

        await HarvestAll(ctx, player, power);
        CultivatePileUI.Refresh(player);
    }

    /// <summary>复制栽培区中的一张，复制品剩余回合为 1。</summary>
    public static int DuplicateOne(Player player)
    {
        var power = player.Creature?.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return 0;

        var candidates = power.Pending.Where(p => !p.Permanent).ToList();
        if (candidates.Count == 0)
            return 0;

        var src = candidates[Random.Shared.Next(candidates.Count)];
        var copy = new Planting(src.SourceId, src.TargetRarity, src.X, src.Count, src.SourceRarity, src.Upgraded, src.Title)
        {
            TurnsLeft = 1
        };
        power.Pending.Add(copy);
        CultivatePileUI.Refresh(player);
        Log.Info($"[Cultivate] duplicated {src.Title}, turns=1");
        return 1;
    }

    /// <summary>从抽牌堆选一张牌栽培。</summary>
    public static async Task<bool> CultivateFromDrawPile(PlayerChoiceContext ctx, Player player, AbstractModel source)
    {
        if (!CultivateFlags.CanCultivate(player))
            return false;

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

        return await PlantCard(ctx, player, target, pile);
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

    public static Task Cultivate(PlayerChoiceContext ctx, Player player, CardModel target)
        => target.Pile is { IsCombatPile: true } pile
            ? PlantCard(ctx, player, target, pile)
            : Task.CompletedTask;

    private static async Task<bool> PlantCard(PlayerChoiceContext ctx, Player player, CardModel target,
        CardPile sourcePile, int extraTurns = 0, bool harvestImmediately = false)
    {
        try
        {
            if (!CultivateFlags.CanCultivate(player))
            {
                Log.Info("[Cultivate] blocked by NoCultivatePower");
                return false;
            }

            // 永久栽培：不进收获倒计时，而是留在栽培区里
            if (target is CultivateCard { CultivatesPermanently: true } permanent)
            {
                return await CultivatePermanent(
                    ctx,
                    player,
                    target,
                    sourcePile,
                    permanent.PermanentBlockValue,
                    permanent.PermanentDamageValue);
            }

            int x = XOf(target);
            bool isAncient = target.Rarity == CardRarity.Ancient;

            var planting = new Planting(
                target.Id,
                TargetRarity(target.Rarity),
                x,
                isAncient ? 2 : 1,
                target.Rarity,
                target.IsUpgraded,
                target.Title + (target.IsUpgraded ? "+" : ""))
            {
                CultivatedOnTurn = player.PlayerCombatState?.TurnNumber
            };

            // 老树盘根：下一次栽培剩余回合为 1
            var fast = player.Creature?.GetPower<NextCultivateFastPower>();
            if (fast is not null)
                planting.TurnsLeft = 1;

            var power = await GetOrApply(ctx, player, target);
            if (power is null)
            {
                Log.Error("[Cultivate] failed to get field power");
                return false;
            }

            if (!await TryRemoveTarget(player, target, sourcePile))
                return false;

            power.Pending.Add(planting);
            if (fast is not null)
                await PowerCmd.Remove(fast);
            CultivatePileUI.Refresh(player);
            await NotifyCultivated(ctx, player);

            if (target is CultivateCard cultivatedCard)
                await cultivatedCard.OnCultivated(ctx, player);

            Log.Info($"[Cultivate] planted {target.Id} from {sourcePile.Type} X={x} count={planting.Count}");
            // 栽培触发可能已收获或移除这张牌。
            if ((harvestImmediately || extraTurns != 0) && power.Pending.Contains(planting))
            {
                if (harvestImmediately)
                {
                    planting.TurnsLeft = 0;
                    await HarvestReady(ctx, player, power, [planting]);
                }
                else
                {
                    planting.TurnsLeft += extraTurns;
                    CultivatePileUI.Refresh(player);
                }
            }
            return true;
        }
        catch (Exception e)
        {
            Log.Error($"[Cultivate] plant failed: {e}");
            return false;
        }
    }

    /// <summary>
    /// 永久栽培：把牌移入栽培区并永久保留，不参与倒计时和收获。
    /// 直接打出不会调用这里；只有其它栽培效果命中带永久栽培的牌时才转换。
    /// </summary>
    public static async Task<bool> CultivatePermanent(
        PlayerChoiceContext ctx,
        Player player,
        CardModel target,
        decimal block,
        decimal damage)
        => target.Pile is { IsCombatPile: true } pile
            && await CultivatePermanent(ctx, player, target, pile, block, damage);

    private static async Task<bool> CultivatePermanent(
        PlayerChoiceContext ctx,
        Player player,
        CardModel target,
        CardPile sourcePile,
        decimal block,
        decimal damage)
    {
        try
        {
            if (!CultivateFlags.CanCultivate(player))
            {
                Log.Info("[Cultivate] blocked by NoCultivatePower");
                return false;
            }

            var planting = new Planting(
                target.Id,
                null,
                1,
                1,
                target.Rarity,
                target.IsUpgraded,
                target.Title + (target.IsUpgraded ? "+" : ""))
            {
                Permanent = true,
                CultivatedOnTurn = player.PlayerCombatState?.TurnNumber,
                TurnsLeft = int.MaxValue,
                StoredBlock = block,
                StoredDamage = damage,
                Card = target,
            };

            var power = await GetOrApply(ctx, player, target);
            if (power is null)
            {
                Log.Error("[Cultivate] failed to get field power");
                return false;
            }

            if (!await TryRemoveTarget(player, target, sourcePile))
                return false;

            power.Pending.Add(planting);
            CultivatePileUI.Refresh(player);
            await NotifyCultivated(ctx, player);

            Log.Info($"[Cultivate] permanently planted {target.Id} from {sourcePile.Type} block={block} damage={damage}");
            return true;
        }
        catch (Exception e)
        {
            Log.Error($"[Cultivate] permanent plant failed: {e}");
            return false;
        }
    }

    private static async Task<bool> TryRemoveTarget(Player player, CardModel target, CardPile sourcePile)
    {
        if (target.Owner != player || target.Pile != sourcePile)
        {
            Log.Warn($"[Cultivate] {target.Id} is no longer eligible in {sourcePile.Type}");
            return false;
        }

        await CardPileCmd.RemoveFromCombat(target);
        if (target.Pile is null && target.HasBeenRemovedFromState)
            return true;

        Log.Error($"[Cultivate] {target.Id} did not leave combat (pile={target.Pile?.Type})");
        return false;
    }

    /// <summary>触发栽培区中所有永久栽培牌的效果。</summary>
    public static async Task TriggerPermanents(PlayerChoiceContext ctx, Player player, CultivateFieldPower power)
    {
        foreach (var p in power.Pending.Where(p => p.Permanent).ToList())
        {
            try
            {
                await TriggerPermanent(ctx, player, p);
            }
            catch (Exception e)
            {
                Log.Error($"[Cultivate] permanent trigger failed: {e}");
            }
        }

        CultivatePileUI.Refresh(player);
    }

    private static async Task TriggerPermanent(PlayerChoiceContext ctx, Player player, Planting p)
    {
        await CreatureCmd.GainBlock(player.Creature, p.StoredBlock, BlockProps.card, null);

        var state = player.Creature.CombatState;
        if (state is not null && p.Card is not null)
        {
            await DamageCmd.Attack(p.StoredDamage)
                .FromCard(p.Card, null)
                .TargetingRandomOpponents(state)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(ctx);
        }

        p.StoredBlock *= 2m;
        p.StoredDamage *= 2m;

        Log.Info($"[Cultivate] permanent trigger {p.Title}: block={p.StoredBlock / 2} damage={p.StoredDamage / 2} -> next {p.StoredBlock}/{p.StoredDamage}");
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

    public static Task HarvestAll(PlayerChoiceContext ctx, Player player, CultivateFieldPower power)
        => HarvestReady(ctx, player, power, power.Pending);

    private static async Task HarvestReady(PlayerChoiceContext ctx, Player player, CultivateFieldPower power, IEnumerable<Planting> crops)
    {
        var harvestBonus = player.Creature?.GetPower<FarmerFormPower>()?.Amount ?? 0;
        foreach (var p in crops.Where(p => !p.Permanent && p.TurnsLeft <= 0).ToList())
        {
            if (!power.Pending.Remove(p))
                continue;
            for (int i = 0; i < p.Count + harvestBonus; i++)
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
        // 收成按种下时的档位结算，催熟只改变 TurnsLeft。
        bool threeTurnHarvest = p.X == 3 && p.SourceRarity is
            CardRarity.Basic or CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare or CardRarity.Event or CardRarity.Token;
        var canonical = p.SourceRarity == CardRarity.Ancient
            ? ModelDb.GetByIdOrNull<CardModel>(p.SourceId)
            : Pick(p.TargetRarity, threeTurnHarvest ? 2 : p.X, p.X, p.SourceRarity == CardRarity.Curse);

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

        if (p.X > 1 && !card.EnergyCost.CostsX)
            card.EnergyCost.SetUntilPlayed(threeTurnHarvest ? 0 : 1, reduceOnly: true);

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
            var extra = Pick(canonical.Rarity, p.X, p.X, statusExtra);
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

    private static CardModel? Pick(CardRarity? rarity, int minCost, int maxCost, bool statusOnly)
    {
        // X 费可作材料，收获候选池只取固定费用的牌。
        var all = Pool(statusOnly).Where(c => c.Rarity == rarity && !c.EnergyCost.CostsX).ToList();

        // 先要求可生成，实在没有就放宽（原型期避免因为标志位导致空池）
        var generatable = all.Where(c => c.CanBeGeneratedInCombat).ToList();
        var source = generatable.Count > 0 ? generatable : all;

        var preferred = source.Where(c => c.EnergyCost.Canonical >= minCost && c.EnergyCost.Canonical <= maxCost).ToList();
        if (preferred.Count > 0)
            return preferred[Random.Shared.Next(preferred.Count)];

        // 回退：同稀有度任意费用
        if (source.Count > 0)
        {
            Log.Warn($"[Cultivate] no {rarity} card with cost in [{minCost}, {maxCost}], falling back to any cost");
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

    /// <summary>被栽培时的玩家回合号；直接复制进栽培区的牌没有栽培动作。</summary>
    public int? CultivatedOnTurn { get; init; }

    /// <summary>永久栽培：不参与倒计时和收获。</summary>
    public bool Permanent { get; set; }

    /// <summary>永久栽培当前使用的格挡值，每次触发后翻倍。</summary>
    public decimal StoredBlock { get; set; }

    /// <summary>永久栽培当前使用的伤害值，每次触发后翻倍。</summary>
    public decimal StoredDamage { get; set; }

    /// <summary>永久栽培对应的卡牌实例，用于伤害来源与卡面信息。</summary>
    public CardModel? Card { get; set; }
}
