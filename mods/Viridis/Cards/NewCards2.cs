using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Hype;
using Viridis.Pools;
using Viridis.Powers;

namespace Viridis.Cards;

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ClimbHill : PlaceholderArtCard, IHypeCard
{
    public ClimbHill() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FlightPower>(1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var player = play.Player;
        await Apply<FlightPower>(ctx, play, player.Creature, DynamicVars["FlightPower"].BaseValue);

        if (player.Creature.GetPower<KaoziPower>() is { Amount: > 0 } kaozi)
            await PowerCmd.Apply<KaoziPower>(ctx, player.Creature, kaozi.Amount, player.Creature, this);

        if (!IsUpgraded && player.Creature.GetPower<AntiStompPower>() is { Amount: > 0 } anti)
            await PowerCmd.Apply<AntiStompPower>(ctx, player.Creature, anti.Amount, player.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["FlightPower"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class StreamEmperor : PlaceholderArtCard
{
    public StreamEmperor() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<EmperorPower>(ctx, play, play.Player.Creature, 1m);

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class FruitSmash : PlaceholderArtCard, IHypeCard
{
    public FruitSmash() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("KaoziGain", 5m),
        new IntVar("Multiplier", 2m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var player = play.Player;
        await PowerCmd.Apply<KaoziPower>(
            ctx, player.Creature, DynamicVars["KaoziGain"].BaseValue, player.Creature, this);

        var planting = await CultivateSystem.SelectPlanting(ctx, player, "VIRIDIS_CARD_SELECT_CULTIVATE_PROMPT");
        if (planting is null || !await CultivateSystem.Wither(ctx, player, planting))
            return;

        var kaozi = player.Creature.GetPower<KaoziPower>()?.Amount ?? 0;
        if (kaozi <= 0)
            return;
        await Hit(ctx, play, play.Target!, kaozi * DynamicVars["Multiplier"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars["Multiplier"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Overwater : PlaceholderArtCard
{
    public Overwater() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OverwaterPower>(1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<OverwaterPower>(ctx, play, play.Player.Creature, DynamicVars["OverwaterPower"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["OverwaterPower"].UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class WowXiaolv : PlaceholderArtCard, IHypeCard
{
    public WowXiaolv() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new IntVar("Bonus", 3m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var count = CountRelevant(play.Player);
        var damage = DynamicVars.Damage.BaseValue + DynamicVars["Bonus"].BaseValue * count;
        await Hit(ctx, play, play.Target!, damage);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }

    private static int CountRelevant(Player player)
    {
        var state = player.PlayerCombatState;
        if (state is null)
            return 0;
        var count = state.Hand.Cards
            .Concat(state.DrawPile.Cards)
            .Concat(state.DiscardPile.Cards)
            .Count(IsRelevant);

        var pending = player.Creature.GetPower<CultivateFieldPower>()?.Pending;
        if (pending is not null)
            count += pending.Count(p =>
                ModelDb.GetByIdOrNull<CardModel>(p.SourceId) is { } card && IsRelevant(card));
        return count;
    }

    private static bool IsRelevant(CardModel card)
    {
        if (card.Rarity == CardRarity.Ancient)
            return true;
        var text = card.Description.GetRawText();
        return text.Contains("栽培", StringComparison.Ordinal)
            || text.Contains("Cultivate", StringComparison.OrdinalIgnoreCase);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class TrueGymnosperm : PlaceholderArtCard
{
    public TrueGymnosperm() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        base.CanonicalKeywords.Append(CardKeyword.Exhaust);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var candidates = ModelDb.AllCards
            .Where(c => c.Keywords.Contains(GymnospermKeyword.Value))
            .Where(c => c.Rarity != CardRarity.Token)
            .ToList();
        if (candidates.Count == 0)
            return;
        var source = candidates[play.Player.RunState.Rng.CombatCardSelection.NextInt(candidates.Count)];
        var card = play.Player.Creature.CombatState!.CreateCard(source, play.Player);
        if (IsUpgraded)
            CardCmd.Upgrade(card, CardPreviewStyle.None);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, play.Player);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class FullGreen : PlaceholderArtCard, IHypeCard
{
    public FullGreen() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        base.CanonicalKeywords.Append(CardKeyword.Exhaust);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var state = play.Player.PlayerCombatState;
        if (state is null)
            return;
        var cards = state.Hand.Cards
            .Concat(state.DrawPile.Cards)
            .Concat(state.DiscardPile.Cards)
            .Where(c => c != this && HasHypeWord(c))
            .ToList();

        foreach (var card in cards)
        {
            if (card.Pile?.Type != PileType.Hand)
                await CardPileCmd.Add(card, PileType.Hand);
            card.EnergyCost.SetUntilPlayed(0);
        }
    }

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);

    private static bool HasHypeWord(CardModel card)
    {
        var text = card.Description.GetRawText();
        return text.Contains("同接", StringComparison.Ordinal)
            || text.Contains("考子", StringComparison.Ordinal)
            || text.Contains("Viewer", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Kaozi", StringComparison.OrdinalIgnoreCase);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Day0 : PlaceholderArtCard
{
    public Day0() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        base.CanonicalKeywords.Append(CardKeyword.Exhaust);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2),
        new CardsVar(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var player = play.Player;
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, player);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, player);
        await Apply<Day0Power>(ctx, play, player.Creature, 1m);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
