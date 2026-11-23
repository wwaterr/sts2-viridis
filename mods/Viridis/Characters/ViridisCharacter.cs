using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using Viridis.Pools;

namespace Viridis.Characters;

/// <summary>
/// 小松绿 / Viridis。
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

    public override float AttackAnimDelay => 1f / 3f;

    public override float CastAnimDelay => 0.2f;

    /// <summary>
    /// 未定制的角色资源沿用铁甲战士。
    /// </summary>
    public override string PlaceholderCharacterId => "IRONCLAD";

    // ---------- 角色展示用美术 ----------
    // 文件放在 mods/Viridis/assets/characters/viridis/，
    // 打包后资源路径是 res://<modId>/<相对路径>。
    private const string ArtRoot = "res://Viridis/characters/viridis";

    public override string? CustomVisualsPath => $"{ArtRoot}/combat/viridis.tscn";

    public override CreatureAnimator GenerateAnimator(MegaSprite controller, Creature creature)
    {
        var idle = new AnimState("idle_loop", isLooping: true);
        var attack = new AnimState("attack");
        attack.AddNextState(idle);

        var animator = new CreatureAnimator(idle, controller);
        animator.AddAnyState(CreatureAnimator.idleTrigger, idle);
        animator.AddAnyState(CreatureAnimator.attackTrigger, attack);
        return animator;
    }

    /// <summary>选人界面的角色肖像（未解锁用这张）。换卡面外的"角色展示图"就换它。</summary>
    public override string? CustomCharacterSelectIconPath => $"{ArtRoot}/character_select_icon.ctex";

    /// <summary>选人界面未解锁时的肖像（灰版）。</summary>
    public override string? CustomCharacterSelectLockedIconPath => $"{ArtRoot}/character_select_icon_locked.ctex";

    /// <summary>选人界面背景。</summary>
    public override string? CustomCharacterSelectBgPath => $"{ArtRoot}/character_select_bg.ctex";

    /// <summary>存档/顶栏用的小头像。</summary>
    public override string? CustomIconTexturePath => $"{ArtRoot}/character_icon.ctex";

    // NTopBarPortrait 实例化 Character.Icon 的场景。
    public override string? CustomIconPath => $"{ArtRoot}/character_icon.tscn";

    /// <summary>小头像描边层。</summary>
    public override string? CustomIconOutlineTexturePath => $"{ArtRoot}/character_icon_outline.ctex";

    /// <summary>地图上的角色标记。</summary>
    public override string? CustomMapMarkerPath => $"{ArtRoot}/map_marker.ctex";

    /// <summary>战斗界面左下角的能量球，改用叶片图标。</summary>
    public override string? CustomEnergyCounterPath
        => "res://Viridis/scenes/combat/energy_counters/viridis_energy_counter.tscn";

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
        => new[] { typeof(Viridis.Relics.GardeningTools) };

    public override List<string> GetArchitectAttackVfx() => new();
}
