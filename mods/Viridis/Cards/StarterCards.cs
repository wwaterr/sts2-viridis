using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Cultivate;
using Viridis.Characters;

namespace Viridis.Cards;

/// <summary>打击：1 费 6 伤害。</summary>
[RegisterCard(typeof(ViridisCardPool))]
[RegisterCharacterStarterCard(typeof(ViridisCharacter), 4)]
public sealed class PineNeedle : CultivateCard
{
    public PineNeedle() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    public override string Title => "打击";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, 6m);
    }
}

/// <summary>防御：1 费 5 格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
[RegisterCharacterStarterCard(typeof(ViridisCharacter), 4)]
public sealed class RootGuard : CultivateCard
{
    public RootGuard() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    public override string Title => "防御";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Block(ctx, play, 5m);
}

/// <summary>育苗：1 费，栽培 1 张手牌。</summary>
[RegisterCard(typeof(ViridisCardPool))]
[RegisterCharacterStarterCard(typeof(ViridisCharacter), 1)]
public sealed class Seedling : CultivateCard
{
    public Seedling() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    public override string Title => "育苗";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
}

/// <summary>落叶：1 费 4 伤害；栽培牌库非空时抽 1 张。</summary>
[RegisterCard(typeof(ViridisCardPool))]
[RegisterCharacterStarterCard(typeof(ViridisCharacter), 1)]
public sealed class FallenLeaf : CultivateCard
{
    public FallenLeaf() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    public override string Title => "落叶";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, 4m);

        if (Pile(play) > 0)
            await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}