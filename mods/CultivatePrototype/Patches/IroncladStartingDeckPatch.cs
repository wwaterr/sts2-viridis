using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using CultivatePrototype.Cards;

namespace CultivatePrototype;

/// <summary>把测试卡塞进铁甲的初始牌组，省得去刷卡牌奖励。</summary>
[HarmonyPatch(typeof(Ironclad), "get_StartingDeck")]
public static class IroncladStartingDeckPatch
{
    public const int Copies = 3;

    [HarmonyPostfix]
    public static void Postfix(ref IEnumerable<CardModel> __result)
    {
        try
        {
            var cards = __result?.ToList() ?? new List<CardModel>();
            int before = cards.Count;

            var canonical = FindCanonical() ?? new CultivateTestCard();

            for (int i = 0; i < Copies; i++)
            {
                // 每张给独立实例，避免同一引用被去重
                cards.Add(i == 0 ? canonical : SafeClone(canonical));
            }

            __result = cards;
            Log.Info($"[CultivatePrototype] StartingDeck patched: {before} -> {cards.Count} (+{Copies})");
        }
        catch (Exception e)
        {
            Log.Error($"[CultivatePrototype] StartingDeck patch failed: {e}");
        }
    }

    private static CardModel SafeClone(CardModel card)
    {
        try { return card.CreateClone(); }
        catch { return card; }
    }

    private static CardModel? FindCanonical()
    {
        try
        {
            return ModelDb.CardPool<IroncladCardPool>().AllCards.OfType<CultivateTestCard>().FirstOrDefault();
        }
        catch (Exception e)
        {
            Log.Warn($"[CultivatePrototype] pool lookup failed, falling back to new(): {e.GetType().Name}");
            return null;
        }
    }
}