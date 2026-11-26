using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Cultivate;
using Viridis.Hype;
using Viridis.Pools;

namespace Viridis.Cards;

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LikeYou : PlaceholderArtCard, IHypeCard
{
    public LikeYou() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2),
        new PowerVar<KaoziPower>(2m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<KaoziPower>(),
        HoverTipFactory.FromKeyword(ViewerCountKeyword.Value),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Energy(ctx, play, (int)DynamicVars.Energy.BaseValue);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
        await Apply<KaoziPower>(ctx, play, play.Player.Creature, DynamicVars["KaoziPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
        DynamicVars["KaoziPower"].UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class HateYou : PlaceholderArtCard, IHypeCard
{
    public HateYou() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new PowerVar<VulnerablePower>(2m),
        new PowerVar<DebilitatePower>(2m),
        new PowerVar<KaoziPower>(3m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<DebilitatePower>(),
        HoverTipFactory.FromPower<KaoziPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Hit(ctx, play, play.Target!, DynamicVars.Damage.BaseValue);
        await Apply<VulnerablePower>(ctx, play, play.Target!, DynamicVars["VulnerablePower"].BaseValue);
        await Apply<DebilitatePower>(ctx, play, play.Target!, DynamicVars["DebilitatePower"].BaseValue);
        await Apply<KaoziPower>(ctx, play, play.Player.Creature, DynamicVars["KaoziPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["KaoziPower"].UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class EssentialFlirting : PlaceholderArtCard, IHypeCard
{
    public EssentialFlirting() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<EssentialFlirtingPower>(),
        HoverTipFactory.FromPower<KaoziPower>(),
        HoverTipFactory.FromPower<AntiStompPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (IsUpgraded)
        {
            var anti = play.Player.Creature.GetPower<AntiStompPower>();
            if (anti is { Amount: > 0 })
            {
                var amount = anti.Amount;
                anti.SetAmount(0);
                await Apply<KaoziPower>(ctx, play, play.Player.Creature, amount);
            }
        }

        await Apply<EssentialFlirtingPower>(ctx, play, play.Player.Creature, 1m);
    }
}

public sealed class EssentialFlirtingPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (power is not AntiStompPower || power.Owner != Owner || amount <= 0m)
            return;

        var current = power.Amount;
        var converted = Math.Min((int)amount, current);
        if (converted <= 0)
            return;

        power.SetAmount(current - converted);
        await PowerCmd.Apply<KaoziPower>(ctx, Owner, converted, Owner, null);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MountainClimbing : PlaceholderArtCard, IHypeCard
{
    public MountainClimbing() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<IntangiblePower>(1m),
        new IntVar("AntiStomp", 5m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<IntangiblePower>(),
        HoverTipFactory.FromPower<AntiStompPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<IntangiblePower>(ctx, play, play.Player.Creature, DynamicVars["IntangiblePower"].BaseValue);
        await Apply<AntiStompPower>(ctx, play, play.Player.Creature, DynamicVars["AntiStomp"].BaseValue);
        PlayerCmd.EndTurn(play.Player, canBackOut: false);
    }

    protected override void OnUpgrade() => DynamicVars["AntiStomp"].UpgradeValueBy(-2m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class StreetTreeStream : PlaceholderArtCard, IHypeCard
{
    public StreetTreeStream() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(ViewerCountKeyword.Value),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var damage = HypeSystem.GetAmount(play.Player) + DynamicVars.Damage.BaseValue;
        var hits = 1 + CountAncientCards(play.Player);
        await Hit(ctx, play, play.Target!, damage, hits);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5m);

    private static int CountAncientCards(Player player)
    {
        var count = 0;
        if (player.PlayerCombatState is { } combat)
        {
            foreach (var pile in combat.AllPiles)
            foreach (var card in pile.Cards)
                if (card.Rarity == CardRarity.Ancient && card.Pool is ViridisCardPool)
                    count++;
        }

        var cultivate = player.Creature.GetPower<CultivateFieldPower>();
        if (cultivate is not null)
        {
            foreach (var planting in cultivate.Pending)
            {
                var card = ModelDb.GetByIdOrNull<CardModel>(planting.SourceId);
                if (card is not null && card.Rarity == CardRarity.Ancient && card.Pool is ViridisCardPool)
                    count++;
            }
        }

        return count;
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class VtuberAppreciationStream : PlaceholderArtCard, IHypeCard
{
    public VtuberAppreciationStream() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<AntiStompPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, HypeSystem.GetAmount(play.Player) + DynamicVars.Block.BaseValue);
        await Apply<WeakPower>(ctx, play, play.Player.Creature, 2m);
        await Apply<AntiStompPower>(ctx, play, play.Player.Creature, 2m);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Slg : PlaceholderArtCard, IHypeCard
{
    public Slg() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var target = play.Target!;
        await CreatureCmd.GainBlock(target, DynamicVars.Block.BaseValue, BlockProps.card, play);
        await Block(ctx, play, target.Block * 2m);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MoeRadioStream : PlaceholderArtCard, IHypeCard
{
    public MoeRadioStream() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<KaoziPower>(3m),
        new IntVar("Viewer", 3m),
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<KaoziPower>(),
        HoverTipFactory.FromKeyword(ViewerCountKeyword.Value),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<KaoziPower>(ctx, play, play.Player.Creature, DynamicVars["KaoziPower"].BaseValue);
        HypeSystem.Add(play.Player, (int)DynamicVars["Viewer"].BaseValue);

        var enemies = CombatState?.HittableEnemies?.ToList();
        if (enemies is null || enemies.Count == 0)
            return;

        for (var i = 0; i < HypeSystem.GetAmount(play.Player); i++)
        {
            var target = enemies[Random.Shared.Next(enemies.Count)];
            var amount = 1m;
            switch (Random.Shared.Next(3))
            {
                case 0:
                    await Apply<FrailPower>(ctx, play, target, amount);
                    break;
                case 1:
                    await Apply<WeakPower>(ctx, play, target, amount);
                    break;
                default:
                    await Apply<VulnerablePower>(ctx, play, target, amount);
                    break;
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["KaoziPower"].UpgradeValueBy(1m);
        DynamicVars["Viewer"].UpgradeValueBy(1m);
    }
}
