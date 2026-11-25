using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Hype;
using Viridis.Pools;

namespace Viridis.Cards;

// ---------- 第三组普通牌 ----------

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ToiletPaper : PlaceholderArtCard
{
    public ToiletPaper() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);
        await Apply<AntiStompPower>(ctx, play, play.Player.Creature, 3m);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Protection : PlaceholderArtCard
{
    public Protection() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        int viewer = HypeSystem.GetAmount(play.Player);
        if (viewer > 0)
            await Block(ctx, play, viewer);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Applause : PlaceholderArtCard
{
    public Applause() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
        new PowerVar<WeakPower>(2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        await Apply<WeakPower>(ctx, play, play.Target, DynamicVars["WeakPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["WeakPower"].UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Documentary : PlaceholderArtCard
{
    public Documentary() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(13m, ValueProp.Move),
        new PowerVar<KaoziPower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await Apply<KaoziPower>(ctx, play, play.Player.Creature, DynamicVars["KaoziPower"].BaseValue);
        await Apply<NextCultivateEnergyPower>(ctx, play, play.Player.Creature, 1m);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["KaoziPower"].UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class BrainDeadGame : PlaceholderArtCard
{
    public BrainDeadGame() : base(3, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int multiplier = IsUpgraded ? 6 : 4;
        decimal damage = HypeSystem.GetAmount(play.Player) * (decimal)multiplier;
        if (damage > 0)
            await HitAll(ctx, play, damage);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Confrontation : PlaceholderArtCard
{
    public Confrontation() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
        new PowerVar<AntiStompPower>(2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        await Apply<AntiStompPower>(ctx, play, play.Player.Creature, DynamicVars["AntiStompPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["AntiStompPower"].UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SingAnywhere : PlaceholderArtCard
{
    public SingAnywhere() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);

        var combatState = play.Player.Creature.CombatState;
        if (combatState is null)
            return;

        var melody = combatState.CreateCard<HeavenlyMelody>(play.Player);
        PileType pile = Random.Shared.Next(3) switch
        {
            0 => PileType.Draw,
            1 => PileType.Hand,
            _ => PileType.Discard
        };
        await CardPileCmd.AddGeneratedCardToCombat(melody, pile, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
