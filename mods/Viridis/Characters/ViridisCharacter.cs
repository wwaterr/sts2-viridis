using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using Viridis.Pools;

namespace Viridis.Characters;

/// <summary>
/// 小松绿 / Viridis —— 占位角色。
/// 视觉暂时借用原版角色（PlaceholderCharacterId），美术后续用 pck 里的纯黑图替换。
/// </summary>
[RegisterCharacter]
public sealed class ViridisCharacter
    : ModCharacterTemplate<ViridisCardPool, ViridisRelicPool, ViridisPotionPool>
{
    /// <summary>名字配色：松绿。</summary>
    public override Color NameColor => new(0.20f, 0.47f, 0.32f);

    public override CharacterGender Gender => CharacterGender.Feminine;

    public override int StartingHp => 72;

    public override int StartingGold => 99;

    public override float AttackAnimDelay => 0.2f;

    public override float CastAnimDelay => 0.2f;

    /// <summary>
    /// 没有自定义 Spine/场景时，暂时复用某个原版角色的视觉表现。
    /// 可用值参考原版角色 Id（如 IRONCLAD / SILENT / DEFECT / REGENT / NECROBINDER）。
    /// </summary>
    public override string PlaceholderCharacterId => "IRONCLAD";

    /// <summary>
    /// 默认值会让角色挂到"纪元/时间线"解锁体系里，选人界面就不显示。
    /// 框架阶段直接关掉，角色即时可选。
    /// </summary>
    public override bool RequiresEpochAndTimeline => false;

    /// <summary>确保出现在原版选人界面（而不是只出现在 MOD 专属入口）。</summary>
    public override bool HideFromVanillaCharacterSelect => false;

    /// <summary>
    /// 初始遗物：选人界面会读取 StartingRelics[0] 做预览，
    /// 为空会抛 ArgumentOutOfRange。暂借用原版"燃烧之血"占位。
    /// </summary>
    protected override IEnumerable<Type> StartingRelicTypes
        => new[] { typeof(MegaCrit.Sts2.Core.Models.Relics.BurningBlood) };

    public override List<string> GetArchitectAttackVfx() => new();
}