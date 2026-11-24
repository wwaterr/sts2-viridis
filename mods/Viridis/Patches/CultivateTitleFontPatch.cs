using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Viridis.Patches;

/// <summary>
/// 游戏标题字体在“苔”字左下角会多出一块梯形。这里只在标题含“苔”时，
/// 把标题拆成“前缀 / 苔 / 后缀”三段：只有“苔”用系统中文字体，
/// 其余文字继续用原字体，其他卡牌的标题完全不动。
/// </summary>
[HarmonyPatch(typeof(NCard), "UpdateTitleLabel")]
internal static class CultivateTitleFontPatch
{
    private const string MixedTitleName = "ViridisMixedTitle";

    private static readonly SystemFont CjkGlyphFont = new()
    {
        FontNames = new[] { "Microsoft YaHei", "Microsoft YaHei UI", "SimHei" },
        AllowSystemFallback = true
    };

    private static void Postfix(NCard __instance)
    {
        Label? label = __instance.GetNodeOrNull<Label>("%TitleLabel");
        if (label?.GetParent() is not { } parent)
            return;

        if (parent.GetNodeOrNull<HBoxContainer>(MixedTitleName) is { } oldBox)
        {
            parent.RemoveChild(oldBox);
            oldBox.QueueFree();
        }

        label.Visible = true;
        string title = __instance.Model?.Title ?? string.Empty;
        int glyphIndex = title.IndexOf('苔');
        if (glyphIndex < 0)
            return;

        var box = new HBoxContainer
        {
            Name = MixedTitleName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Alignment = BoxContainer.AlignmentMode.Center,
            Position = label.Position,
            Size = label.Size
        };
        box.AddThemeConstantOverride("separation", 0);

        Font gameFont = label.GetThemeFont("font");
        int fontSize = label.GetThemeFontSize("font_size");

        AddPart(box, title[..glyphIndex], gameFont, label, fontSize);
        AddPart(box, "苔", CjkGlyphFont, label, fontSize);
        AddPart(box, title[(glyphIndex + 1)..], gameFont, label, fontSize);

        parent.AddChild(box);
        label.Visible = false;
    }

    private static void AddPart(HBoxContainer box, string text, Font font, Label source, int fontSize)
    {
        if (text.Length == 0)
            return;

        var part = new Label
        {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        part.AddThemeFontOverride("font", font);
        part.AddThemeFontSizeOverride("font_size", fontSize);
        part.AddThemeColorOverride("font_color", source.GetThemeColor("font_color"));
        part.AddThemeColorOverride("font_outline_color", source.GetThemeColor("font_outline_color"));
        part.AddThemeColorOverride("font_shadow_color", source.GetThemeColor("font_shadow_color"));
        part.AddThemeConstantOverride("outline_size", source.GetThemeConstant("outline_size"));
        part.AddThemeConstantOverride("shadow_offset_x", source.GetThemeConstant("shadow_offset_x"));
        part.AddThemeConstantOverride("shadow_offset_y", source.GetThemeConstant("shadow_offset_y"));
        part.AddThemeConstantOverride("shadow_outline_size", source.GetThemeConstant("shadow_outline_size"));
        box.AddChild(part);
    }
}
