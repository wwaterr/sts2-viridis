using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models.CardPools;
using CultivatePrototype.Cards;

namespace CultivatePrototype;

[ModInitializer(nameof(Initialize))]
public static class ModEntry
{
    public const string ModId = "CultivatePrototype";

    public static void Initialize()
    {
        // 注意：此阶段 ModelDb 还没初始化，禁止调用 ModelDb.*（会抛 KeyNotFoundException）
        ApplyPatches();

        try
        {
            ModHelper.AddModelToPool(typeof(IroncladCardPool), typeof(CultivateTestCard));
            Log.Info($"[{ModId}] CultivateTestCard added to IroncladCardPool");
        }
        catch (Exception e)
        {
            Log.Error($"[{ModId}] AddModelToPool failed: {e}");
        }
    }

    /// <summary>
    /// 逐个类型打补丁：一个失败不影响其它补丁。
    /// </summary>
    private static void ApplyPatches()
    {
        var harmony = new HarmonyLib.Harmony(ModId);
        var patchTypes = typeof(ModEntry).Assembly
            .GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(HarmonyLib.HarmonyPatch), true).Any())
            .ToList();

        foreach (var t in patchTypes)
        {
            try
            {
                // Harmony 只提供 PatchAll(Assembly)，单类型要用 ClassProcessor
                harmony.CreateClassProcessor(t).Patch();
                Log.Info($"[{ModId}] patched {t.Name}");
            }
            catch (Exception e)
            {
                Log.Error($"[{ModId}] patch {t.Name} failed: {e}");
            }
        }
    }
}