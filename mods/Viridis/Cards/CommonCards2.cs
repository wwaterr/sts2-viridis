using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Pools;

namespace Viridis.Cards;

// ---------- 普通牌（第二组） ----------

[RegisterCard(typeof(ViridisCardPool))]
public sealed class VineWhip : CultivateCard
{
    public VineWhip() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, 7m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Photosynthesis : CultivateCard
{
    public Photosynthesis() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Energy(ctx, play, 1);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SporePowder : CultivateCard
{
    public SporePowder() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await HitAll(ctx, play, 4m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Bark : CultivateCard
{
    public Bark() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Block(ctx, play, 8m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Cutting : CultivateCard
{
    public Cutting() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Aphid : CultivateCard
{
    public Aphid() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, 5m);
        await Apply<WeakPower>(ctx, play, play.Target, 1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Dew : CultivateCard
{
    public Dew() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, 3m);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Nettle : CultivateCard
{
    public Nettle() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, 4m);
        await Hit(ctx, play, play.Target, 4m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Resin : CultivateCard
{
    public Resin() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, 6m);
        await CultivateSystem.Grow(ctx, play.Player, 1);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MossCover : CultivateCard
{
    public MossCover() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, 5m);
        if (Pile(play) > 0)
            await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Bud : CultivateCard
{
    public Bud() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ResinTrap : CultivateCard
{
    public ResinTrap() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, 10m);
        await Apply<VulnerablePower>(ctx, play, play.Target, 2m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafReturn : CultivateCard
{
    public LeafReturn() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var pile = play.Player.PlayerCombatState?.DiscardPile;
        if (pile is null || pile.Cards.Count == 0)
            return;

        var prefs = new MegaCrit.Sts2.Core.CardSelection.CardSelectorPrefs(
            new MegaCrit.Sts2.Core.Localization.LocString("cards", "CULTIVATE_TEST.select_prompt"), 1)
        {
            Cancelable = false,
            RequireManualConfirmation = false
        };

        var selected = await CardSelectCmd.FromCombatPile(ctx, pile, play.Player, prefs);
        var card = selected?.FirstOrDefault();
        if (card is null)
            return;

        await CardPileCmd.Add(card, PileType.Hand);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class FernLeaf : CultivateCard
{
    public FernLeaf() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, 3m);

        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}