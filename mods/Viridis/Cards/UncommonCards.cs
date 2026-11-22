using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Pools;

namespace Viridis.Cards;

// ---------- 罕见牌 ----------

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Gymnosperm : CultivateCard
{
    public Gymnosperm() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var player = play.Player;
        var field = player.Creature.GetPower<CultivateFieldPower>();
        if (field is null || !field.Pending.Any(planting => !planting.Permanent))
            return;

        // 选项只用于选择界面，不加入任何牌堆或牌表。
        var accelerate = ModelDb.Card<GymnospermAccelerateOption>().ToMutable();
        accelerate.Owner = player;
        var delay = ModelDb.Card<GymnospermDelayOption>().ToMutable();
        delay.Owner = player;
        var prefs = new CardSelectorPrefs(new LocString("cards", "VIRIDIS_CARD_GYMNOSPERM.select_prompt"), 1)
        {
            Cancelable = false,
            RequireManualConfirmation = false
        };

        var choices = new[] { accelerate, delay };
        var selected = (await CardSelectCmd.FromSimpleGrid(ctx, choices, player, prefs)).FirstOrDefault();
        if (ReferenceEquals(selected, accelerate))
            await CultivateSystem.Grow(ctx, player, 1);
        else if (ReferenceEquals(selected, delay))
            await CultivateSystem.DelayAll(ctx, player, 1);
    }
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

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateNow(ctx, play.Player, this);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class RootNetwork : CultivateCard
{
    public RootNetwork() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<RootNetworkPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Mycorrhiza : CultivateCard
{
    public Mycorrhiza() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<MycorrhizaPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class VineEntangle : CultivateCard
{
    public VineEntangle() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, 8m);
        await Apply<WeakPower>(ctx, play, play.Target, 2m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Division : CultivateCard
{
    public Division() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateMultiple(ctx, play.Player, this, 2, 1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class PineconeBurst : CultivateCard
{
    public PineconeBurst() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await HitAll(ctx, play, 8m + 2m * Pile(play));
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Sap : CultivateCard
{
    public Sap() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Energy(ctx, play, 2);
        await Apply<NoCultivatePower>(ctx, play, play.Player.Creature, 1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class TerminalBud : CultivateCard
{
    public TerminalBud() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, 9m);

        if (Pile(play) > 0)
            await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class TreeRingMark : CultivateCard
{
    public TreeRingMark() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<TreeRingMarkPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafWhirl : CultivateCard
{
    public LeafWhirl() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, 5m);

        // 收获最早进入牌库的一张
        var power = play.Player.Creature.GetPower<CultivateFieldPower>();
        if (power is null || power.Pending.Count == 0)
            return;

        var first = power.Pending[0];
        first.TurnsLeft = 0;
        await CultivateSystem.HarvestAll(ctx, play.Player, power);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Tundra : CultivateCard
{
    public Tundra() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<TundraPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Sucker : CultivateCard
{
    public Sucker() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateFromDrawPile(ctx, play.Player, this);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class FernSpread : CultivateCard
{
    public FernSpread() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateMultiple(ctx, play.Player, this, 2);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LandedRoot : CultivateCard
{
    public LandedRoot() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<LandedRootPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class OldTreeRoots : CultivateCard
{
    public OldTreeRoots() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, 8m);
        await Apply<NextCultivateFastPower>(ctx, play, play.Player.Creature, 1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Pollen : CultivateCard
{
    public Pollen() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Apply<VulnerablePower>(ctx, play, play.Target, 2m);

        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class CorkLayer : CultivateCard
{
    public CorkLayer() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, 7m);
        await Apply<ArtifactPower>(ctx, play, play.Player.Creature, 1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ApicalGrowth : CultivateCard
{
    public ApicalGrowth() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        decimal dmg = Pile(play) >= 3 ? 18m : 12m;
        await Hit(ctx, play, play.Target, dmg);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class FlowerBud : CultivateCard
{
    public FlowerBud() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<FlowerBudPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MyceliumSpread : CultivateCard
{
    public MyceliumSpread() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.HalveAll(ctx, play.Player);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class NeedleRain : CultivateCard
{
    public NeedleRain() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await HitAll(ctx, play, 5m);
        await HitAll(ctx, play, 5m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Peat : CultivateCard
{
    public Peat() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, 6m);
        await Apply<DelayedDrawPower>(ctx, play, play.Player.Creature, 1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Trunk : CultivateCard
{
    public Trunk() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, 20m);

        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafLitter : CultivateCard
{
    public LeafLitter() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, 4m + Pile(play));
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ResinSeal : CultivateCard
{
    public ResinSeal() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.DelayAll(ctx, play.Player, 2);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class SeedlingTransplant : CultivateCard
{
    public SeedlingTransplant() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
        await Apply<DelayedGrowPower>(ctx, play, play.Player.Creature, 1m);
    }
}