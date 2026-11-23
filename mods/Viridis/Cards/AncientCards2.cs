using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Pools;

namespace Viridis.Cards;

/// <summary>
/// 先古牌默认卡图：横版「图片暂缺」裁出的竖版版本。
/// 先古牌统一走 832×1248 竖版卡面。
/// </summary>
public abstract class AncientPlaceholderArtCard : PlaceholderArtCard
{
    protected AncientPlaceholderArtCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target)
    {
    }

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: $"{ModEntry.ResPath}/images/cards/card_placeholder_ancient.ctex");
}

/// <summary>珍珠梅：获得 15（20）格挡，移除自身所有负面状态，每移除一种抽 1 张牌。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Sorbaria : AncientPlaceholderArtCard
{
    public Sorbaria() : base(2, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new BlockVar(15m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);

        var player = play.Player;
        var debuffs = player.Creature.Powers
            .Where(power => power.Type == PowerType.Debuff)
            .ToList();

        foreach (var debuff in debuffs)
            await PowerCmd.Remove(debuff);

        if (debuffs.Count > 0)
            await CardPileCmd.Draw(ctx, debuffs.Count, player);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5m);
}

/// <summary>塔黄：对所有敌人造成当前格挡值 2（3）倍的伤害。保留。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class TowerRhubarb : AncientPlaceholderArtCard
{
    public TowerRhubarb() : base(3, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Retain };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var multiplier = IsUpgraded ? 3m : 2m;
        await HitAll(ctx, play, play.Player.Creature.Block * multiplier);
    }
}

/// <summary>小垫柳：获得等同于已损失生命值（升级后两倍）的格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class CushionWillow : AncientPlaceholderArtCard
{
    public CushionWillow() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var lostHp = play.Player.Creature.MaxHp - play.Player.Creature.CurrentHp;
        var multiplier = IsUpgraded ? 2m : 1m;
        await Block(ctx, play, lostHp * multiplier);
    }
}

/// <summary>苹果：获得 25（35）格挡，造成 25（35）伤害。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Apple : AncientPlaceholderArtCard
{
    public Apple() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new BlockVar(25m, ValueProp.Move),
            new DamageVar(25m, ValueProp.Move),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await Hit(ctx, play, play.Target!, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(10m);
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}

/// <summary>金光菊：造成 15（20）伤害，获得 2（3）能量，下回合开始再获得 2（3）能量。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class BlackEyedSusan : AncientPlaceholderArtCard
{
    public BlackEyedSusan() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(15m, ValueProp.Move),
            new EnergyVar(2),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Hit(ctx, play, play.Target!, DynamicVars.Damage.BaseValue);

        var energy = (int)DynamicVars.Energy.BaseValue;
        await Energy(ctx, play, energy);
        await Apply<EnergyNextTurnPower>(ctx, play, play.Player.Creature, energy);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
