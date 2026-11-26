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
using Viridis.Hype;
using Viridis.Pools;
using Viridis.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

// ---------- 罕见牌 ----------

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Gymnosperm : CultivateCard
{
    public Gymnosperm() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<MuleDutyPower>(),
        HoverTipFactory.FromPower<KaoziPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<MuleDutyPower>(4m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<MuleDutyPower>(ctx, play, play.Player.Creature, DynamicVars["MuleDutyPower"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["MuleDutyPower"].UpgradeValueBy(2m);
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

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var hand = play.Player.PlayerCombatState?.Hand;
        if (hand is null || hand.Cards.Count == 0)
            return;

        var prefs = new CardSelectorPrefs(new LocString("cards", "VIRIDIS_CARD_TRANSPLANT_PROMPT"), 1)
        {
            Cancelable = false,
            RequireManualConfirmation = false
        };
        var selected = await CardSelectCmd.FromHand(ctx, play.Player, prefs, c => c != this, this);
        var card = selected?.FirstOrDefault();
        if (card is null)
            return;

        await CardCmd.AutoPlay(ctx, card, null);
        if (card.Pile is { IsCombatPile: true })
            await CultivateSystem.Cultivate(ctx, play.Player, card);
    }

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class RootNetwork : CultivateCard
{
    public RootNetwork() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<RootNetworkPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<RootNetworkPower>(3m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<RootNetworkPower>(ctx, play, play.Player.Creature, DynamicVars["RootNetworkPower"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["RootNetworkPower"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Mycorrhiza : CultivateCard
{
    public Mycorrhiza() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<MycorrhizaPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<MycorrhizaPower>(1m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<MycorrhizaPower>(ctx, play, play.Player.Creature, DynamicVars["MycorrhizaPower"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["MycorrhizaPower"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class VineEntangle : CultivateCard
{
    public VineEntangle() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(12m, ValueProp.Move),
            new IntVar("StrengthLoss", 6m),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        await Apply<ViridisTempStrengthDownPower>(ctx, play, play.Target, DynamicVars["StrengthLoss"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["StrengthLoss"].UpgradeValueBy(3m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Division : CultivateCard
{
    public Division() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Copies", 1m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateWithCopies(ctx, play.Player, this, DynamicVars["Copies"].IntValue);

    protected override void OnUpgrade() => DynamicVars["Copies"].UpgradeValueBy(1m);
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
        new[] { new DamageVar(6m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);

        await CardPileCmd.Draw(ctx, 1m, play.Player);

        var extra = CultivateSystem.PileCount(play.Player);
        if (extra > 0)
            await CardPileCmd.Draw(ctx, extra, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class TreeRingMark : CultivateCard
{
    public TreeRingMark() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<TreeRingMarkPower>(),
    ];

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

        var discard = play.Player.PlayerCombatState?.DiscardPile.Cards;
        if (discard is null || discard.Count == 0)
            return;

        var card = discard[Random.Shared.Next(discard.Count)];
        await CardPileCmd.Add(card, PileType.Hand);
        card.EnergyCost.SetUntilPlayed(0);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Tundra : CultivateCard
{
    public Tundra() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<TundraPower>(),
    ];

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

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new IntVar("Count", 1m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateFromAllPiles(
            ctx, play.Player, this, DynamicVars["Count"].IntValue);

    protected override void OnUpgrade() => DynamicVars["Count"].UpgradeValueBy(1m);
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

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<LandedRootPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<LandedRootPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class OldTreeRoots : CultivateCard
{
    public OldTreeRoots() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<OldTreeRootsPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<OldTreeRootsPower>(10m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<OldTreeRootsPower>(ctx, play, play.Player.Creature, DynamicVars["OldTreeRootsPower"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["OldTreeRootsPower"].UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Pollen : CultivateCard
{
    public Pollen() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    protected override bool ShowsCultivateKeyword => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(2m),
        new PowerVar<VulnerablePower>(2m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
        {
            await Apply<WeakPower>(ctx, play, play.Target, DynamicVars["WeakPower"].BaseValue);
            await Apply<VulnerablePower>(ctx, play, play.Target, DynamicVars["VulnerablePower"].BaseValue);
        }

        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["WeakPower"].UpgradeValueBy(1m);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class CorkLayer : CultivateCard
{
    public CorkLayer() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<ArtifactPower>(),
    ];

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
    protected override bool ShowsGrowKeyword => true;

    public ApicalGrowth() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(14m, ValueProp.Move),
            new PowerVar<StrengthPower>(1m),
            new IntVar("Grow", 1m),
        };

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        await Apply<StrengthPower>(ctx, play, play.Player.Creature, DynamicVars["StrengthPower"].BaseValue);
        await CultivateSystem.Grow(ctx, play.Player, (int)DynamicVars["Grow"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
        DynamicVars["Grow"].UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class FlowerBud : CultivateCard
{
    public FlowerBud() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<FlowerBudPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(6m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var power = await PowerCmd.Apply<FlowerBudPower>(ctx, play.Player.Creature,
            DynamicVars.Damage.BaseValue, play.Player.Creature, this);
        if (power is not null)
        {
            power.DamageValue = DynamicVars.Damage.BaseValue;
            power.CountHarvested = IsUpgraded;
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MyceliumSpread : CultivateCard
{
    public MyceliumSpread() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(4m, ValueProp.Move),
            new PowerVar<PoisonPower>(3m),
        };

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CombatState is not { } combat)
            return;

        var hits = ResolveEnergyXValue();
        for (var i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, play).TargetingAllOpponents(combat).Execute(ctx);
            await ApplyAll<PoisonPower>(ctx, play, DynamicVars["PoisonPower"].BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["PoisonPower"].UpgradeValueBy(2m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class NeedleRain : CultivateCard
{
    public NeedleRain() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(2m, ValueProp.Move),
            new IntVar("Hits", 8m),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await HitAll(ctx, play, DynamicVars.Damage.BaseValue, hits: (int)DynamicVars["Hits"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["Hits"].UpgradeValueBy(4m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Peat : CultivateCard
{
    public Peat() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<DrawCardsNextTurnPower>(),
    ];

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
    public Trunk() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(25m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await HitAll(ctx, play, DynamicVars.Damage.BaseValue);

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(7m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafLitter : CultivateCard
{
    public LeafLitter() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override bool ShowsGrowKeyword => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new BlockVar(15m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await CultivateSystem.Grow(ctx, play.Player, 1);

        var power = await PowerCmd.Apply<LeafLitterPower>(ctx, play.Player.Creature,
            DynamicVars.Block.BaseValue, play.Player.Creature, this);
        if (power is not null)
            power.BlockValue = DynamicVars.Block.BaseValue;
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ResinSeal : CultivateCard
{
    public ResinSeal() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(28m, ValueProp.Move),
            new PowerVar<VigorPower>(4m),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        if (play.Target.IsAlive)
            await Apply<VigorPower>(ctx, play, play.Target, DynamicVars["VigorPower"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
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
