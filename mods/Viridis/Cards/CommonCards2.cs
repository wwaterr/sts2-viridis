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
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

// ---------- 普通牌（第二组） ----------

[RegisterCard(typeof(ViridisCardPool))]
public sealed class VineWhip : CultivateCard
{
    public VineWhip() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
        {
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
            await Apply<VulnerablePower>(ctx, play, play.Target, 2m);
        }
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Photosynthesis : CultivateCard
{
    public Photosynthesis() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int times = 1 + Pile(play);
        for (int i = 0; i < times; i++)
        {
            await Energy(ctx, play, DynamicVars.Energy.IntValue);
            await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SporePowder : CultivateCard
{
    public SporePowder() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await HitAll(ctx, play, DynamicVars.Damage.BaseValue);
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Bark : CultivateCard
{
    public Bark() : base(3, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(25m, ValueProp.Move)];

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5m);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Block(ctx, play, DynamicVars.Block.BaseValue);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Cutting : CultivateCard
{
    public Cutting() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Dew : CultivateCard
{
    public Dew() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3m, ValueProp.Move)];

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2m);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Resin : CultivateCard
{
    public Resin() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        var drawn = await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, play.Player);
        foreach (var card in drawn)
            card.AddKeyword(CardKeyword.Retain);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Bud : CultivateCard
{
    public Bud() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Energy(ctx, play, DynamicVars.Energy.IntValue);
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafReturn : CultivateCard
{
    public LeafReturn() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { ViewerCountKeyword.Value };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var pile = play.Player.PlayerCombatState?.DiscardPile;
        if (pile is null || pile.Cards.Count == 0)
            return;

        int count = Math.Min(pile.Cards.Count, 1 + HypeSystem.GetAmount(play.Player) / 4);
        var prefs = new MegaCrit.Sts2.Core.CardSelection.CardSelectorPrefs(
            new MegaCrit.Sts2.Core.Localization.LocString("cards", "HOLOGRAM.selectionScreenPrompt"), count)
        {
            Cancelable = true,
            RequireManualConfirmation = false
        };

        var selected = await CardSelectCmd.FromCombatPile(ctx, pile, play.Player, prefs);
        if (selected is null)
            return;

        foreach (var card in selected)
            await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
