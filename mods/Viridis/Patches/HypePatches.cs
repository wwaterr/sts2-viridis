using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Viridis.Hype;

/// <summary>战斗建立时给所有需要的玩家挂上隐藏的同接计数器。</summary>
[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.SetUpCombat))]
public static class HypeCombatSetupPatch
{
    [HarmonyPostfix]
    public static void Postfix(CombatState state)
    {
        foreach (var player in state.Players)
            HypeSystem.Ensure(player);
    }
}

/// <summary>本地玩家战斗 UI 建立后创建同接面板；这里再兜底一次计数器。</summary>
[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
public static class HypeCombatUiPatch
{
    [HarmonyPostfix]
    public static void Postfix(NCombatUi __instance, CombatState state)
    {
        var me = LocalContext.GetMe(state);
        if (me is null)
            return;

        HypeSystem.Ensure(me);
        HypeCounterUI.Attach(__instance, me);
    }
}

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Deactivate))]
public static class HypeCombatUiResetPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        HypeCounterUI.Reset();
    }
}
