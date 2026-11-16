using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Pools;

/// <summary>卡池：卡牌通过 [RegisterCard(typeof(ViridisCardPool))] 自动归入。</summary>
public sealed class ViridisCardPool : TypeListCardPoolModel
{
    public override string Title => "Viridis";

    // 自定义能量配色；大图标与富文本小图标由 RitsuLib 的池覆盖接口提供。
    public override string EnergyColorName => "viridis";

    public override string? BigEnergyIconPath => "res://Viridis/images/ui/energy_icon_big.ctex";

    public override string? TextEnergyIconPath => "res://Viridis/images/ui/energy_icon_small.ctex";

    public override Vector2I? TextEnergyIconSize => new Vector2I(24, 24);

    public override Color DeckEntryCardColor => new("8eb056");

    // 卡框沿用原版绿色边框材质，按截图实测反解出的 HSV 参数重着色到 #8eb056 的观感。
    private static ShaderMaterial? _frameMaterial;

    public override Material? PoolFrameMaterial
    {
        get
        {
            if (_frameMaterial != null)
                return _frameMaterial;

            var source = GD.Load<ShaderMaterial>("res://materials/cards/frames/card_frame_green_mat.tres");
            if (source == null)
                return null;

            var material = (ShaderMaterial)source.Duplicate();
            material.SetShaderParameter("h", 0.2684f);
            material.SetShaderParameter("s", 0.3181f);
            material.SetShaderParameter("v", 1.7422f);
            _frameMaterial = material;
            return material;
        }
    }

    public override bool IsColorless => false;
}

/// <summary>遗物池（暂时为空，只保证角色能创建）。</summary>
public sealed class ViridisRelicPool : TypeListRelicPoolModel
{
    public override string EnergyColorName => "ironclad";
}

/// <summary>药水池（暂时为空，只保证角色能创建）。</summary>
public sealed class ViridisPotionPool : TypeListPotionPoolModel
{
    public override string EnergyColorName => "ironclad";
}