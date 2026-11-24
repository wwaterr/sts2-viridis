using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Viridis.Cards;

namespace Viridis.Hype;

/// <summary>战斗建立时给所有需要的玩家挂上隐藏的同接计数器。</summary>
[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.SetUpCombat))]
public static class HypeCombatSetupPatch
{
    [HarmonyPostfix]
    public static void Postfix(CombatState state)
    {
        foreach (var player in state.Players)
        {
            HypeSystem.Ensure(player);
            GuWuSystem.Ensure(player);
        }
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
        PolarityBarUI.Reset();
    }
}

/// <summary>人物 HP 条上方挂载并跟随定位「入脑 / 反踩」双极滑条。</summary>
[HarmonyPatch(typeof(NCreatureStateDisplay))]
public static class PolarityBarDisplayPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCreatureStateDisplay.SetCreature))]
    public static void AfterSetCreature(
        NCreatureStateDisplay __instance,
        NHealthBar ____healthBar,
        Creature creature)
    {
        PolarityBarUI.Attach(__instance, ____healthBar, creature);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCreatureStateDisplay.SetCreatureBounds))]
    public static void AfterSetCreatureBounds(
        NCreatureStateDisplay __instance,
        NHealthBar ____healthBar)
    {
        PolarityBarUI.Reposition(__instance, ____healthBar);
    }

    [HarmonyPostfix]
    [HarmonyPatch("RefreshValues")]
    public static void AfterRefreshValues(
        NCreatureStateDisplay __instance,
        NHealthBar ____healthBar,
        Creature? ____creature)
    {
        PolarityBarUI.Reposition(__instance, ____healthBar);
        if (____creature?.Player is { } player)
            PolarityBarUI.Refresh(player);
    }
}

/// <summary>入脑 / 反踩层数变化时实时刷新人物血条上方滑条。</summary>
[HarmonyPatch(typeof(NCreature))]
public static class PolarityBarCreaturePowerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("OnPowerApplied")]
    public static void AfterPowerApplied(NCreature __instance, PowerModel power)
    {
        if (power is EarwormPower or BacklashPower && __instance.Entity?.Player is { } player)
            PolarityBarUI.Refresh(player);
    }

    [HarmonyPostfix]
    [HarmonyPatch("OnPowerIncreased")]
    public static void AfterPowerIncreased(NCreature __instance, PowerModel power)
    {
        if (power is EarwormPower or BacklashPower && __instance.Entity?.Player is { } player)
            PolarityBarUI.Refresh(player);
    }

    [HarmonyPostfix]
    [HarmonyPatch("OnPowerRemoved")]
    public static void AfterPowerRemoved(NCreature __instance, PowerModel power)
    {
        if (power is EarwormPower or BacklashPower && __instance.Entity?.Player is { } player)
            PolarityBarUI.Refresh(player);
    }
}
