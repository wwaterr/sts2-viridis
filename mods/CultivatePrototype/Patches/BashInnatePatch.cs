using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace CultivatePrototype;

/// <summary>
/// 给痛击(Bash)加固有，方便测试 X=2 的费用折扣。
/// 注意：Bash 没有自己重写 CanonicalKeywords，必须挂到基类 CardModel 上再按类型过滤。
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_CanonicalKeywords")]
public static class BashInnatePatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (__instance is not Bash)
            return;

        var list = __result?.ToList() ?? new List<CardKeyword>();
        if (!list.Contains(CardKeyword.Innate))
            list.Add(CardKeyword.Innate);
        __result = list;
    }
}