using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace CultivatePrototype;

/// <summary>
/// 给测试用的牌加固有，保证开局就在手上：
///  - 痛击(Bash)：整类加固有（Bash 没重写 CanonicalKeywords，必须挂基类再按类型过滤）
///  - 升级过的打击/痛击：补丁塞进初始牌组的那两张
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_CanonicalKeywords")]
public static class BashInnatePatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (!ShouldBeInnate(__instance))
            return;

        var list = __result?.ToList() ?? new List<CardKeyword>();
        if (!list.Contains(CardKeyword.Innate))
            list.Add(CardKeyword.Innate);
        __result = list;
    }

    private static bool ShouldBeInnate(CardModel card)
    {
        if (card is Bash)
            return true;

        // 只给补丁加进去的那两张升级牌
        if (!card.IsUpgraded)
            return false;

        var name = card.GetType().Name;
        return name is "StrikeIronclad" or "Bash";
    }
}