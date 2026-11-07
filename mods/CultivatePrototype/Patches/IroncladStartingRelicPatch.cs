using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace CultivatePrototype;

/// <summary>把园艺工具塞进初始遗物，省得去刷遗物奖励。</summary>
[HarmonyPatch(typeof(Ironclad), "get_StartingRelics")]
public static class IroncladStartingRelicPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref IReadOnlyList<RelicModel> __result)
    {
        try
        {
            var list = __result?.ToList() ?? new List<RelicModel>();

            if (list.Any(r => r is Relics.GardeningTools))
                return;

            var relic = ModelDb.RelicPool<IroncladRelicPool>()
                            .AllRelics
                            .OfType<Relics.GardeningTools>()
                            .FirstOrDefault()
                        ?? new Relics.GardeningTools();

            list.Add(relic);
            __result = list;
            Log.Info($"[CultivatePrototype] StartingRelics patched: {list.Count} relics (+ GardeningTools)");
        }
        catch (Exception e)
        {
            Log.Error($"[CultivatePrototype] StartingRelics patch failed: {e}");
        }
    }
}