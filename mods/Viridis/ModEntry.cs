using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace Viridis;

/// <summary>MOD 入口：内容注册由 RitsuLib 的特性扫描完成，这里只补一个尘封魔典的兜底补丁。</summary>
[ModInitializer(nameof(Initialize))]
public class ModEntry
{
    public const string ModId = "Viridis";

    /// <summary>模组 PCK 内的资源根路径。</summary>
    public const string ResPath = $"res://{ModId}";

    private static Harmony? _harmony;

    public static void Initialize()
    {
        try
        {
            _harmony = new Harmony($"{ModId}.DustyTomeFallback");
            _harmony.PatchAll(typeof(ModEntry).Assembly);
        }
        catch (Exception ex)
        {
            Log.Error($"[{ModId}] DustyTome fallback patch failed: {ex}");
        }

        // 入脑/反踩隐藏跨局变量（Profile 作用域存档）
        Hype.PolaritySave.EnsureRegistered();

        Log.Info($"[{ModId}] Initialize() called - character scaffold loaded.");
    }
}