using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Pools;

/// <summary>卡池：卡牌通过 [RegisterCard(typeof(ViridisCardPool))] 自动归入。</summary>
public sealed class ViridisCardPool : TypeListCardPoolModel
{
    public override string Title => "Viridis";

    // 必须是原版已有的能量配色（ironclad/defect/silent/regent/necrobinder/colorless/quest），
    // 否则会去找不存在的 card/energy_<name> 图集精灵。
    public override string EnergyColorName => "ironclad";

    public override Color DeckEntryCardColor => new(0.20f, 0.47f, 0.32f);

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