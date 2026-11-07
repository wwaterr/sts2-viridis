using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace Viridis;

/// <summary>MOD 入口：只负责打一行日志，内容注册由 RitsuLib 的特性扫描完成。</summary>
[ModInitializer(nameof(Initialize))]
public class ModEntry
{
    public const string ModId = "Viridis";

    /// <summary>模组 PCK 内的资源根路径。</summary>
    public const string ResPath = $"res://{ModId}";

    public static void Initialize()
    {
        Log.Info($"[{ModId}] Initialize() called - character scaffold loaded.");
    }
}