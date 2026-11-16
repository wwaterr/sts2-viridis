using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Viridis.Patches;

/// <summary>
/// 自定义能量球场景里不直接引用 .ctex（场景加载器不认这个扩展名），
/// 改在 NEnergyCounter 就绪后从 pck 里读原始 PNG 生成 ImageTexture。
/// </summary>
[HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter._Ready))]
internal static class EnergyCounterLeafPatch
{
    private const string LeafPngPath = "res://Viridis/images/ui/energy_orb_icon.png";

    private static Texture2D? _leafTexture;

    private static void Postfix(NEnergyCounter __instance)
    {
        TextureRect? leaf = __instance.GetNodeOrNull<TextureRect>("Layers/LayerLeaf");
        if (leaf == null)
        {
            return;
        }

        _leafTexture ??= LoadLeafTexture();
        if (_leafTexture != null)
        {
            leaf.Texture = _leafTexture;
        }
    }

    private static Texture2D? LoadLeafTexture()
    {
        byte[] bytes = Godot.FileAccess.GetFileAsBytes(LeafPngPath);
        if (bytes.Length == 0)
        {
            return null;
        }

        Image image = new();
        if (image.LoadPngFromBuffer(bytes) != Error.Ok)
        {
            return null;
        }

        return ImageTexture.CreateFromImage(image);
    }
}
