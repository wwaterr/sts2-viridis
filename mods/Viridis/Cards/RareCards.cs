using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Pools;

namespace Viridis.Cards;

// ---------- 稀有牌 ----------

[RegisterCard(typeof(ViridisCardPool))]
public sealed class AncientTree : CultivateCard
{
    public AncientTree() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new IntVar("BonusBlock", 0m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var owner = play.Player.Creature;
        var power = await PowerCmd.Apply<AncientTreePower>(ctx, owner, 1m, owner, this);
        if (power is not null)
            power.BonusBlock = Math.Max(power.BonusBlock, DynamicVars["BonusBlock"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars["BonusBlock"].UpgradeValueBy(2m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ForestLord : CultivateCard
{
    public ForestLord() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<ForestLordPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class AllGrowth : CultivateCard
{
    public AllGrowth() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.Grow(ctx, play.Player, 3);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class RainOfChange : CultivateCard
{
    public RainOfChange() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(8m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = await CultivateSystem.ClearPileAsync(ctx, play.Player);
        var combat = play.Player.Creature.CombatState;
        if (combat is null)
            return;

        for (int i = 0; i < n; i++)
        {
            var target = play.Player.RunState.Rng.CombatTargets.NextItem(combat.HittableEnemies);
            if (target is null)
                break;

            await Hit(ctx, play, target, DynamicVars.Damage.BaseValue);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Canopy : CultivateCard
{
    public Canopy() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<CanopyPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SporeCloud : CultivateCard
{
    public SporeCloud() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WeakPower>(3m), new PowerVar<VulnerablePower>(2m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await ApplyAll<WeakPower>(ctx, play, DynamicVars["WeakPower"].BaseValue);
        await ApplyAll<VulnerablePower>(ctx, play, DynamicVars["VulnerablePower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["WeakPower"].UpgradeValueBy(1m);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Clone : CultivateCard
{
    public Clone() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var prefs = new CardSelectorPrefs(new LocString("cards", "VIRIDIS_CARD_CLONE.selectionScreenPrompt"), 1);
        var selected = await CardSelectCmd.FromHand(ctx, play.Player, prefs, null, this);
        if (selected.FirstOrDefault() is { } card)
            await CardPileCmd.AddGeneratedCardToCombat(card.CreateClone(), PileType.Hand, play.Player);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SpringRitual : CultivateCard
{
    public SpringRitual() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.SetAllTurns(ctx, play.Player, 0);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class StranglerFig : CultivateCard
{
    public StranglerFig() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(15m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        await Apply<VulnerablePower>(ctx, play, play.Target, Pile(play));
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class DeepRoot : CultivateCard
{
    public DeepRoot() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<DeepRootPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SpringCycle : CultivateCard
{
    public SpringCycle() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<SpringCyclePower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafStorm : CultivateCard
{
    public LeafStorm() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(4m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int hits = 1 + Pile(play);
        for (int i = 0; i < hits; i++)
            await HitAll(ctx, play, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Truffle : CultivateCard
{
    public Truffle() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<EnergyNextTurnPower>(ctx, play, play.Player.Creature, 2m);
        await Apply<DrawCardsNextTurnPower>(ctx, play, play.Player.Creature, 1m);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MotherPlant : CultivateCard
{
    public MotherPlant() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<MotherPlantPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ExtremeGrowth : CultivateCard
{
    public ExtremeGrowth() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.SetAllTurns(ctx, play.Player, 0);

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafVein : CultivateCard
{
    public LeafVein() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<LeafVeinPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Lignify : CultivateCard
{
    public Lignify() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new BlockVar(20m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Block(ctx, play, DynamicVars.Block.BaseValue);

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Symbiosis : CultivateCard
{
    public Symbiosis() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SymbiosisPower>(3m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<SymbiosisPower>(ctx, play, play.Player.Creature, DynamicVars["SymbiosisPower"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["SymbiosisPower"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class HumusCycle : CultivateCard
{
    public HumusCycle() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<HumusCyclePower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MillennialPine : CultivateCard
{
    public MillennialPine() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override bool HasEnergyCostX => true;
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("BlockPerEnergy", 7m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int x = ResolveEnergyXValue();
        if (x > 0)
            await Block(ctx, play, x * DynamicVars["BlockPerEnergy"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars["BlockPerEnergy"].UpgradeValueBy(2m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class GrowthPeriod : CultivateCard
{
    public GrowthPeriod() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<MachineLearningPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Catkin : CultivateCard
{
    public Catkin() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(6m, ValueProp.Move) };

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);

        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class WitherBloom : CultivateCard
{
    public WitherBloom() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = await CultivateSystem.ClearPileAsync(ctx, play.Player);
        if (n <= 0)
            return;

        await PowerCmd.Apply<StrengthPower>(ctx, play.Player.Creature, 2m * n, play.Player.Creature, this);
        await PowerCmd.Apply<DexterityPower>(ctx, play.Player.Creature, 2m * n, play.Player.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class PhotosyntheticStorm : CultivateCard
{
    public PhotosyntheticStorm() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(10m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        int hits = 1 + Pile(play);
        for (int i = 0; i < hits && play.Target.IsAlive; i++)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
