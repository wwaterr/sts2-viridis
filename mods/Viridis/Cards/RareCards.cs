using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Pools;

namespace Viridis.Cards;

// ---------- 稀有牌 ----------

[RegisterCard(typeof(ViridisCardPool))]
public sealed class AncientTree : CultivateCard
{
    public AncientTree() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<AncientTreePower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ForestLord : CultivateCard
{
    public ForestLord() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<ForestLordPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class AllGrowth : CultivateCard
{
    public AllGrowth() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.Grow(ctx, play.Player, 3);
        await CardPileCmd.Draw(ctx, 2m, play.Player);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class RainOfChange : CultivateCard
{
    public RainOfChange() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = await CultivateSystem.ClearPileAsync(ctx, play.Player);
        var enemies = play.Player.Creature.CombatState?.HittableEnemies;
        if (n <= 0 || enemies is null || enemies.Count == 0)
            return;

        for (int i = 0; i < n; i++)
            await Hit(ctx, play, enemies[Random.Shared.Next(enemies.Count)], 8m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Canopy : CultivateCard
{
    public Canopy() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<CanopyPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SporeCloud : CultivateCard
{
    public SporeCloud() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await ApplyAll<WeakPower>(ctx, play, 3m);
        await ApplyAll<VulnerablePower>(ctx, play, 2m);
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Clone : CultivateCard
{
    public Clone() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        CultivateSystem.DuplicateOne(play.Player);
        return Task.CompletedTask;
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SpringRitual : CultivateCard
{
    public SpringRitual() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.SetAllTurns(ctx, play.Player, 0);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class StranglerFig : CultivateCard
{
    public StranglerFig() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, 15m);
        await Apply<VulnerablePower>(ctx, play, play.Target, Pile(play));
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class DeepRoot : CultivateCard
{
    public DeepRoot() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<DeepRootPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SpringCycle : CultivateCard
{
    public SpringCycle() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<SpringCyclePower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafStorm : CultivateCard
{
    public LeafStorm() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = Pile(play);
        for (int i = 0; i < n; i++)
            await HitAll(ctx, play, 4m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Truffle : CultivateCard
{
    public Truffle() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Energy(ctx, play, 2);
        await CultivateSystem.CultivateMultiple(ctx, play.Player, this, 2);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MotherPlant : CultivateCard
{
    public MotherPlant() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<MotherPlantPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ExtremeGrowth : CultivateCard
{
    public ExtremeGrowth() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.SetAllTurns(ctx, play.Player, 0);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafVein : CultivateCard
{
    public LeafVein() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<LeafVeinPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Lignify : CultivateCard
{
    public Lignify() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, 20m);
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Symbiosis : CultivateCard
{
    public Symbiosis() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<SymbiosisPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class HumusCycle : CultivateCard
{
    public HumusCycle() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<HumusCyclePower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MillennialPine : CultivateCard
{
    public MillennialPine() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = Pile(play);
        if (n <= 0)
            return;

        await Block(ctx, play, 6m * n);
        await Energy(ctx, play, n);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class GrowthPeriod : CultivateCard
{
    public GrowthPeriod() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<GrowthPeriodPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Catkin : CultivateCard
{
    public Catkin() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, 6m);

        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
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
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class PhotosyntheticStorm : CultivateCard
{
    public PhotosyntheticStorm() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        int n = Pile(play);
        for (int i = 0; i < n; i++)
            await Hit(ctx, play, play.Target, 10m);
    }
}