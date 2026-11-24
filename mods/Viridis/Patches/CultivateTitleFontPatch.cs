using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Viridis.Patches;

/// <summary>
/// 游戏标题字体在“苔”字左下角会多出一块梯形。这里只对包含“苔”的标题
/// 临时改用系统中文字体，避免回退字体图集把该字形画坏；其余标题不动。
/// </summary>
[HarmonyPatch(typeof(NCard), "UpdateTitleLabel")]
internal static class CultivateTitleFontPatch
{
    private static readonly SystemFont CjkTitleFont = new()
    {
        FontNames = new[] { "Microsoft YaHei", "Microsoft YaHei UI", "SimHei" },
        AllowSystemFallback = true
    };

    private static void Postfix(NCard __instance)
    {
        Label? label = __instance.GetNodeOrNull<Label>("%TitleLabel");
        if (label is null)
            return;

        if (__instance.Model?.Title is { } title && title.Contains('苔'))
            label.AddThemeFontOverride("font", CjkTitleFont);
        else if (ReferenceEquals(label.GetThemeFont("font"), CjkTitleFont))
            label.RemoveThemeFontOverride("font");
    }
}
