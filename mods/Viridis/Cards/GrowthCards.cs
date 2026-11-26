using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Cultivate;

namespace Viridis.Cards;

/// <summary>腐殖土：4 格挡；生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Humus : CultivateCard
{
    protected override bool ShowsGrowKeyword => true;

    public Humus() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await CultivateSystem.Grow(ctx, play.Player, 1);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

/// <summary>催芽：栽培 1 张手牌；生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Germinate : CultivateCard
{
    protected override bool ShowsGrowKeyword => true;

    public Germinate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Growth", 1m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
        await CultivateSystem.Grow(ctx, play.Player, DynamicVars["Growth"].IntValue);
    }

    protected override void OnUpgrade() => DynamicVars["Growth"].UpgradeValueBy(1m);
}

/// <summary>春化：生长 2。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Vernalization : CultivateCard
{
    protected override bool ShowsGrowKeyword => true;

    public Vernalization() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Growth", 2m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.Grow(ctx, play.Player, DynamicVars["Growth"].IntValue);

    protected override void OnUpgrade() => DynamicVars["Growth"].UpgradeValueBy(1m);
}

/// <summary>硼砂：抽 2（3）张牌。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Borax : CultivateCard
{
    public Borax() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }


    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

/// <summary>拔节：7 伤害；生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Bolting : CultivateCard
{
    protected override bool ShowsGrowKeyword => true;

    public Bolting() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);

        await CultivateSystem.Grow(ctx, play.Player, 1);
    }
}

/// <summary>腐熟：将栽培区内的倒计时设为 1 回合，然后抽 1 张牌。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Compost : CultivateCard
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [CultivateTips.Cultivate];

    public Compost() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        Log.Info("[Cultivate] compost: set all turns to 1");
        await CultivateSystem.SetAllTurns(ctx, play.Player, 1);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}
