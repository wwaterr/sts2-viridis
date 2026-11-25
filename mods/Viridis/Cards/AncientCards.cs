using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Characters;
using Viridis.Cultivate;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

// ---------- 先古牌 ----------

/// <summary>
/// 小松绿：永久栽培（被动，只有被其它效果栽培时才生效）。
/// 打出时获得 3（5）格挡并随机造成 3（5）伤害，然后这两个数值翻倍，牌进入弃牌堆。
/// 在栽培区时每个己方回合结束再触发一次，触发后同样翻倍。
/// </summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class SmallPine : CultivateCard
{
    public SmallPine() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.RandomEnemy) { }

    public override bool GainsBlock => true;


    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: $"{ModEntry.ResPath}/images/cards/small_pine.ctex");

    protected override bool ShowsCultivateKeyword => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { PermanentCultivateKeyword.Value };

    public override bool CultivatesPermanently => true;

    public override decimal PermanentBlockValue => DynamicVars.Block.BaseValue;

    public override decimal PermanentDamageValue => DynamicVars.Damage.BaseValue;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new BlockVar(3m, ValueProp.Move),
            new DamageVar(3m, ValueProp.Move),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await PlayEffects(ctx, play.Player, play);
        DynamicVars.Block.BaseValue *= 2m;
        DynamicVars.Damage.BaseValue *= 2m;
    }

    private async Task PlayEffects(PlayerChoiceContext ctx, Player player, CardPlay? play)
    {
        await CreatureCmd.GainBlock(player.Creature, DynamicVars.Block.BaseValue, BlockProps.card, play);

        var state = player.Creature.CombatState;
        if (state is null)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingRandomOpponents(state)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}

/// <summary>
/// 山绿篱：获得 5（6）点无实体和 99 层中毒。由尘封魔典作为小松绿的先古候选获得。
/// </summary>
[RegisterDustyTomeCard(typeof(ViridisCharacter))]
[RegisterCard(typeof(ViridisCardPool))]
public sealed class MountainHedge : CultivateCard
{
    public MountainHedge() : base(4, CardType.Power, CardRarity.Ancient, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<IntangiblePower>(),
        HoverTipFactory.FromPower<PoisonPower>(),
    ];


    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: $"{ModEntry.ResPath}/images/cards/mountain_hedge.ctex");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new PowerVar<IntangiblePower>(5m),
            new PowerVar<PoisonPower>(99m),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var player = play.Player;
        await CreatureCmd.TriggerAnim(player.Creature, "PowerUp", player.Character.PowerUpAnimDelay);
        await Apply<IntangiblePower>(ctx, play, player.Creature, DynamicVars["IntangiblePower"].BaseValue);
        await Apply<PoisonPower>(ctx, play, player.Creature, DynamicVars["PoisonPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["IntangiblePower"].UpgradeValueBy(1m);
    }
}
