using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace CultivatePrototype;

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

    public static async Task Cultivate(PlayerChoiceContext ctx, Player player, CardModel target)
    {
        try
        {
            int canonical = target.EnergyCost.Canonical;
            int x = Math.Clamp(canonical, 1, 3);
            bool isAncient = target.Rarity == CardRarity.Ancient;

            var planting = new Planting(
                target.Id,
                TargetRarity(target.Rarity),
                x,
                isAncient ? 2 : 1,
                target.Rarity);

            await CardPileCmd.RemoveFromCombat(target);

            var power = await GetOrApply(ctx, player, target);
            if (power is null)
            {
                Log.Error("[Cultivate] failed to get field power");
                return;
            }

            power.Pending.Add(planting);
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
    }

    private static async Task HarvestOne(PlayerChoiceContext ctx, Player player, Planting p)
    {
        var canonical = p.SourceRarity == CardRarity.Ancient
            ? FindById(p.SourceId)
            : Pick(p.TargetRarity, p.X);

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

        if (p.X > 1)
            card.EnergyCost.AddUntilPlayed(1 - p.X, reduceOnly: true);

        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        Log.Info($"[Cultivate] harvested {card.Id} (X={p.X}, target={p.TargetRarity})");
    }

    private static IEnumerable<CardModel> Pool()
    {
        foreach (var c in ModelDb.CardPool<IroncladCardPool>().AllCards) yield return c;
        foreach (var c in ModelDb.CardPool<EventCardPool>().AllCards) yield return c;
        // 状态/诅咒/任务/衍生牌各自在独立池里，栽培它们才收得到
        foreach (var c in ModelDb.CardPool<StatusCardPool>().AllCards) yield return c;
        foreach (var c in ModelDb.CardPool<CurseCardPool>().AllCards) yield return c;
        foreach (var c in ModelDb.CardPool<QuestCardPool>().AllCards) yield return c;
        foreach (var c in ModelDb.CardPool<TokenCardPool>().AllCards) yield return c;
    }

    private static CardModel? FindById(ModelId id) => Pool().FirstOrDefault(c => c.Id == id);

    private static CardModel? Pick(CardRarity? rarity, int x)
    {
        var all = Pool().Where(c => c.Rarity == rarity).ToList();

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
    public Planting(ModelId sourceId, CardRarity? targetRarity, int x, int count, CardRarity sourceRarity)
    {
        SourceId = sourceId;
        TargetRarity = targetRarity;
        X = x;
        Count = count;
        SourceRarity = sourceRarity;
        TurnsLeft = x;
    }

    public ModelId SourceId { get; }
    public CardRarity? TargetRarity { get; }
    public CardRarity SourceRarity { get; }
    public int X { get; }
    public int Count { get; }
    public int TurnsLeft { get; set; }
}