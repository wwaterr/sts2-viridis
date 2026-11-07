using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace CultivatePrototype;

/// <summary>
/// 战斗开始时把牌组里的打击/痛击各升级一张，方便测试「升级继承」。
/// 不能直接往 StartingDeck 塞可变实例——初始牌组只接受规范(不可变)模型，
/// 否则开局会抛 MutableModelException。
/// </summary>
[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Hooks.Hook), "BeforeCombatStart")]
public static class StarterUpgradePatch
{
    private const int HowMany = 2;

    [HarmonyPrefix]
    public static void Prefix(ICombatState combatState)
    {
        try
        {
            foreach (var player in combatState.Players)
            {
                var state = player.PlayerCombatState;
                if (state is null)
                    continue;

                var all = state.AllPiles.SelectMany(p => p.Cards).ToList();
                var targets = all
                    .Where(c => !c.IsUpgraded && c.GetType().Name is "StrikeIronclad" or "Bash")
                    .Take(HowMany)
                    .ToList();

                foreach (var card in targets)
                    UpgradeNow(card);

                Log.Info($"[CultivatePrototype] combat start: {all.Count} cards, upgraded {targets.Count} " +
                         $"[{string.Join(", ", targets.Select(t => t.Id.ToString()))}]");
            }
        }
        catch (Exception e)
        {
            Log.Error($"[CultivatePrototype] starter upgrade failed: {e}");
        }
    }

    private static void UpgradeNow(CardModel card)
    {
        // CurrentUpgradeLevel 的 setter 非 public，用反射写
        typeof(CardModel)
            .GetProperty("CurrentUpgradeLevel")
            ?.SetValue(card, 1);

        typeof(CardModel)
            .GetMethod("FinalizeUpgradeInternal", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.Invoke(card, null);
    }
}