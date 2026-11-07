using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
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
    public override string Title => "裸子植物";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.Grow(ctx, play.Player, 1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Transplant : CultivateCard
{
    public Transplant() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string Title => "移栽";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateNow(ctx, play.Player, this);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class RootNetwork : CultivateCard
{
    public RootNetwork() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    public override string Title => "根系网络";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<RootNetworkPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Mycorrhiza : CultivateCard
{
    public Mycorrhiza() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    public override string Title => "菌根";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<MycorrhizaPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class VineEntangle : CultivateCard
{
    public VineEntangle() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    public override string Title => "藤蔓缠绕";

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
    public override string Title => "分株";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateMultiple(ctx, play.Player, this, 2, 1);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class PineconeBurst : CultivateCard
{
    public PineconeBurst() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }
    public override string Title => "松塔爆裂";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await HitAll(ctx, play, 8m + 2m * Pile(play));
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Sap : CultivateCard
{
    public Sap() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string Title => "树液";

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
    public override string Title => "顶芽";

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
    public override string Title => "年轮刻印";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<TreeRingMarkPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LeafWhirl : CultivateCard
{
    public LeafWhirl() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    public override string Title => "落叶回旋";

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
    public override string Title => "苔原";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<TundraPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class Sucker : CultivateCard
{
    public Sucker() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string Title => "吸芽";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateFromDrawPile(ctx, play.Player, this);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class FernSpread : CultivateCard
{
    public FernSpread() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string Title => "蕨类蔓延";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivateMultiple(ctx, play.Player, this, 2);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class LandedRoot : CultivateCard
{
    public LandedRoot() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    public override string Title => "落地生根";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<LandedRootPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class OldTreeRoots : CultivateCard
{
    public OldTreeRoots() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string Title => "老树盘根";

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
    public override string Title => "花粉";

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
    public override string Title => "木栓层";

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
    public override string Title => "顶端生长";

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
    public override string Title => "花芽分化";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<FlowerBudPower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class MyceliumSpread : CultivateCard
{
    public MyceliumSpread() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string Title => "菌丝蔓延";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.HalveAll(ctx, play.Player);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class NeedleRain : CultivateCard
{
    public NeedleRain() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }
    public override string Title => "松针雨";

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
    public override string Title => "泥炭";

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
    public override string Title => "主干";

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
    public override string Title => "落叶层";

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
    public override string Title => "树脂封存";

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
    public override string Title => "幼苗移栽";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
        await Apply<DelayedGrowPower>(ctx, play, play.Player.Creature, 1m);
    }
}