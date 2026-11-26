using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Viridis.Cards;

namespace Viridis.Patches;

/// <summary>
/// SLG 允许把格挡给敌人，也允许给多人模式下的其他玩家。
/// 原版 TargetType 没有“敌人或队友”这个合并枚举，所以这里在
/// 单目标瞄准、目标校验和卡牌合法性三处一起放开限制。
/// </summary>
public static class SlgTargeting
{
    public static bool Active;
}

[HarmonyPatch(typeof(NTargetManager), "StartTargeting",
    new[] { typeof(TargetType), typeof(Control), typeof(TargetMode), typeof(Func<bool>), typeof(Func<Node, bool>) })]
public static class SlgStartTargetingPatch
{
    private static void Prefix(Control control)
    {
        SlgTargeting.Active = control is NCard { Model: Slg };
    }
}

[HarmonyPatch(typeof(NTargetManager), "AllowedToTargetCreature")]
public static class SlgAllowedTargetPatch
{
    private static bool Prefix(Creature creature, ref bool __result)
    {
        if (!SlgTargeting.Active)
            return true;

        if (!creature.IsAlive)
        {
            __result = false;
            return false;
        }

        if (creature.Side == CombatSide.Enemy)
        {
            __result = true;
            return false;
        }

        __result = creature.IsPlayer && !LocalContext.IsMe(creature.Player);
        return false;
    }
}

[HarmonyPatch(typeof(NTargetManager), "FinishTargeting")]
public static class SlgFinishTargetingPatch
{
    private static void Postfix() => SlgTargeting.Active = false;
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.IsValidTarget))]
public static class SlgValidTargetPatch
{
    private static bool Prefix(CardModel __instance, Creature? target, ref bool __result)
    {
        if (__instance is not Slg)
            return true;

        __result = target != null
                   && target.IsAlive
                   && target != __instance.Owner.Creature
                   && (target.Side == CombatSide.Enemy || target.IsPlayer);
        return false;
    }
}
