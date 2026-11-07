using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace EmptyCharacterMod;

/// <summary>
/// MOD 入口。游戏扫描到本 DLL 后，会找带 [ModInitializer] 的类型，
/// 并调用特性里写的方法名（这里就是 Initialize）。
/// </summary>
[ModInitializer(nameof(Initialize))]
public class ModEntry
{
    public const string ModId = "EmptyCharacterMod";

    public static void Initialize()
    {
        Log.Info($"[{ModId}] Initialize() called - empty mod loaded OK.");
    }
}