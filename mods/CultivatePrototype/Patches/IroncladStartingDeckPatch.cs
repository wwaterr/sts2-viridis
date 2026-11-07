using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using CultivatePrototype.Cards;

namespace CultivatePrototype;

/// <summary>用设计好的起始牌组替换铁甲初始牌组，方便直接测小松绿的卡。</summary>
[HarmonyPatch(typeof(Ironclad), "get_StartingDeck")]
public static class IroncladStartingDeckPatch
{
    /// <summary>起始牌组：4 松针击 / 4 护根 / 1 育苗 / 1 落叶。</summary>
    private static readonly (Type Type, int Copies)[] Deck =
    {
        (typeof(PineNeedle), 4),
        (typeof(RootGuard), 4),
        (typeof(Seedling), 1),
        (typeof(FallenLeaf), 1)
    };

    [HarmonyPostfix]
    public static void Postfix(ref IEnumerable<CardModel> __result)
    {
        try
        {
            var pool = ModelDb.CardPool<IroncladCardPool>().AllCards;
            var cards = new List<CardModel>();

            foreach (var (type, copies) in Deck)
            {
                var canonical = pool.FirstOrDefault(c => c.GetType() == type);
                if (canonical is null)
                {
                    Log.Warn($"[CultivatePrototype] starting deck: {type.Name} not in pool");
                    continue;
                }

                for (int i = 0; i < copies; i++)
                    cards.Add(i == 0 ? canonical : SafeClone(canonical));
            }

            if (cards.Count == 0)
                return;

            __result = cards;
            Log.Info($"[CultivatePrototype] StartingDeck = {cards.Count} cards ({string.Join(", ", cards.Select(c => c.GetType().Name))})");
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
}