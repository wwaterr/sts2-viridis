using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Cultivate;

namespace Viridis.Cards;

/// <summary>腐殖土：4 格挡；生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Humus : CultivateCard
{
    public Humus() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, 4m);
        await CultivateSystem.Grow(ctx, play.Player, 1);
    }
}

/// <summary>蚯蚓松土：生长 1；抽 1 张。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Earthworm : CultivateCard
{
    public Earthworm() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.Grow(ctx, play.Player, 1);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}

/// <summary>催芽：栽培 1 张手牌；生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Germinate : CultivateCard
{
    public Germinate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
        await CultivateSystem.Grow(ctx, play.Player, 1);
    }
}

/// <summary>春化：生长 2。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Vernalization : CultivateCard
{
    public Vernalization() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.Grow(ctx, play.Player, 2);
}

/// <summary>硼砂：生长 1；抽 2 张。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Borax : CultivateCard
{
    public Borax() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.Grow(ctx, play.Player, 1);
        await CardPileCmd.Draw(ctx, 2m, play.Player);
    }
}

/// <summary>拔节：7 伤害；生长 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Bolting : CultivateCard
{
    public Bolting() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, 7m);

        await CultivateSystem.Grow(ctx, play.Player, 1);
    }
}

/// <summary>腐熟：把栽培牌库所有牌的剩余回合设为 1。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Compost : CultivateCard
{
    public Compost() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        Log.Info("[Cultivate] compost: set all turns to 1");
        await CultivateSystem.SetAllTurns(ctx, play.Player, 1);
    }
}