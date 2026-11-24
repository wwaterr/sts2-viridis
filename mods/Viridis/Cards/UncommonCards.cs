using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Pools;

namespace Viridis.Cards;

// ---------- 罕见牌 ----------

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Gymnosperm : CultivateCard
{
    public Gymnosperm() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var player = play.Player;
        var field = player.Creature.GetPower<CultivateFieldPower>();
        if (field is null || !field.Pending.Any(planting => !planting.Permanent))
            return;

        // 选项只用于选择界面，不加入任何牌堆或牌表。
        var accelerate = ModelDb.Card<GymnospermAccelerateOption>().ToMutable();
        accelerate.Owner = player;
        var delay = ModelDb.Card<GymnospermDelayOption>().ToMutable();
        delay.Owner = player;
        var prefs = new CardSelectorPrefs(new LocString("cards", "VIRIDIS_CARD_GYMNOSPERM.select_prompt"), 1)
        {
            Cancelable = false,
            RequireManualConfirmation = false
        };

        var choices = new[] { accelerate, delay };
        var selected = (await CardSelectCmd.FromSimpleGrid(ctx, choices, player, prefs)).FirstOrDefault();
        if (ReferenceEquals(selected, accelerate))
            await CultivateSystem.Grow(ctx, player, 1);
        else if (ReferenceEquals(selected, delay))
            await CultivateSystem.DelayAll(ctx, player, 1);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(TokenCardPool))]
public sealed class GymnospermAccelerateOption : PlaceholderArtCard
{
    public GymnospermAccelerateOption() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    public override bool CanBeGeneratedInCombat => false;
}

[RegisterCard(typeof(TokenCardPool))]
public sealed class GymnospermDelayOption : PlaceholderArtCard
{
    public GymnospermDelayOption() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    public override bool CanBeGeneratedInCombat => false;
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Transplant : CultivateCard
{
    public Transplant() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateNow(ctx, play.Player, this);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class RootNetwork : CultivateCard
{
    public RootNetwork() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<RootNetworkPower>(1m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var owner = play.Player.Creature;
        decimal increase = DynamicVars["RootNetworkPower"].BaseValue - owner.GetPowerAmount<RootNetworkPower>();
        if (increase > 0m)
            await Apply<RootNetworkPower>(ctx, play, owner, increase);
    }

    protected override void OnUpgrade() => DynamicVars["RootNetworkPower"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Mycorrhiza : CultivateCard
{
    public Mycorrhiza() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<MycorrhizaPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class VineEntangle : CultivateCard
{
    public VineEntangle() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(12m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        await Apply<WeakPower>(ctx, play, play.Target, 2m);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Division : CultivateCard
{
    public Division() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateMultiple(ctx, play.Player, this, 2, 1);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class PineconeBurst : CultivateCard
{
    public PineconeBurst() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new CalculationBaseVar(8m),
            new ExtraDamageVar(2m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => CultivateSystem.PileCount(card.Owner))
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await HitAll(ctx, play, DynamicVars.CalculatedDamage.Calculate(play.Target));

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Sap : CultivateCard
{
    public Sap() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new EnergyVar(1) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Energy(ctx, play, DynamicVars.Energy.IntValue);

    protected override void OnUpgrade() => DynamicVars.Energy.UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class TerminalBud : CultivateCard
{
    public TerminalBud() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(9m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);

        if (Pile(play) > 0)
            await CardPileCmd.Draw(ctx, 1m, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class TreeRingMark : CultivateCard
{
    public TreeRingMark() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<TreeRingMarkPower>(2m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var owner = play.Player.Creature;
        decimal increase = DynamicVars["TreeRingMarkPower"].BaseValue - owner.GetPowerAmount<TreeRingMarkPower>();
        if (increase > 0m)
            await Apply<TreeRingMarkPower>(ctx, play, owner, increase);
    }

    protected override void OnUpgrade() => DynamicVars["TreeRingMarkPower"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafWhirl : CultivateCard
{
    public LeafWhirl() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(5m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);

        // 收获最早进入牌库的一张
        var power = play.Player.Creature.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return;

        var first = power.Pending[0];
        first.TurnsLeft = 0;
        await CultivateSystem.HarvestAll(ctx, play.Player, power);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Tundra : CultivateCard
{
    public Tundra() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new PowerVar<TundraPower>(3m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<TundraPower>(ctx, play, play.Player.Creature, DynamicVars["TundraPower"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["TundraPower"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Sucker : CultivateCard
{
    public Sucker() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateFromDrawPile(ctx, play.Player, this);

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class FernSpread : CultivateCard
{
    public FernSpread() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new CardsVar(2) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1);
        var discarded = (await CardSelectCmd.FromHandForDiscard(ctx, play.Player, prefs, null, this)).FirstOrDefault();
        if (discarded is not null)
            await CardCmd.Discard(ctx, discarded);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LandedRoot : CultivateCard
{
    public LandedRoot() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<LandedRootPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class OldTreeRoots : CultivateCard
{
    public OldTreeRoots() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new BlockVar(12m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await Apply<BlockNextTurnPower>(ctx, play, play.Player.Creature, 4m);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Pollen : CultivateCard
{
    public Pollen() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<VulnerablePower>(2m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Apply<VulnerablePower>(ctx, play, play.Target, DynamicVars["VulnerablePower"].BaseValue);

        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }

    protected override void OnUpgrade() => DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class CorkLayer : CultivateCard
{
    public CorkLayer() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new BlockVar(7m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await Apply<ArtifactPower>(ctx, play, play.Player.Creature, 1m);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ApicalGrowth : CultivateCard
{
    public ApicalGrowth() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new CalculationBaseVar(14m),
            new ExtraDamageVar(6m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => CultivateSystem.PileCount(card.Owner) >= 3 ? 1 : 0)
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.CalculatedDamage.Calculate(play.Target));
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class FlowerBud : CultivateCard
{
    protected override bool ShowsGrowKeyword => true;

    public FlowerBud() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<FlowerBudPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MyceliumSpread : CultivateCard
{
    public MyceliumSpread() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DamageVar(5m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CombatState is not { } combat)
            return;

        var hits = ResolveEnergyXValue();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(hits)
            .FromCard(this, play).TargetingAllOpponents(combat).Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class NeedleRain : CultivateCard
{
    public NeedleRain() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(5m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await HitAll(ctx, play, DynamicVars.Damage.BaseValue, hits: 2);

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Peat : CultivateCard
{
    public Peat() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new BlockVar(6m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await Apply<DrawCardsNextTurnPower>(ctx, play, play.Player.Creature, 1m);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Trunk : CultivateCard
{
    public Trunk() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(24m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);

        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafLitter : CultivateCard
{
    public LeafLitter() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new CalculationBaseVar(5m),
            new CalculationExtraVar(1m),
            new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) => CultivateSystem.PileCount(card.Owner))
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.CalculatedBlock.Calculate(play.Target));
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ResinSeal : CultivateCard
{
    public ResinSeal() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new PowerVar<WeakPower>(2m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Apply<WeakPower>(ctx, play, play.Target, DynamicVars["WeakPower"].BaseValue);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars["WeakPower"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SeedlingTransplant : CultivateCard
{
    public SeedlingTransplant() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new BlockVar(6m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        var pile = PileType.Discard.GetPile(play.Player);
        var prefs = new CardSelectorPrefs(new LocString("cards", "HEADBUTT.selectionScreenPrompt"), 0, 1);
        var chosen = (await CardSelectCmd.FromCombatPile(ctx, pile, play.Player, prefs)).FirstOrDefault();
        if (chosen is not null)
            await CardPileCmd.Add(chosen, PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
