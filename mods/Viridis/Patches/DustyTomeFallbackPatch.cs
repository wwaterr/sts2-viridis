using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Viridis.Cards;
using Viridis.Characters;

namespace Viridis.Patches;

/// <summary>
/// 原版尘封魔典只在 Darv 事件生成选项时调用 SetupForPlayer。
/// 调试类模组（如 FreeLoadout）直接发放遗物会跳过这一步，导致 AfterObtained 拿到 null 并抛异常。
/// 这里在 AfterObtained 前补一次选择，保证直接发放时也能正常给牌。
/// </summary>
[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.AfterObtained))]
internal static class DustyTomeFallbackPatch
{
    private static void Prefix(DustyTome __instance)
    {
        if (__instance.AncientCard != null || __instance.Owner == null)
        {
            return;
        }

        __instance.SetupForPlayer(__instance.Owner);
        if (__instance.AncientCard == null && __instance.Owner.Character is ViridisCharacter)
        {
            __instance.AncientCard = ModelDb.GetId<MountainHedge>();
        }
    }
}
