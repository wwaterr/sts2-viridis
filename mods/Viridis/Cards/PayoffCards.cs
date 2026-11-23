using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Cultivate;

namespace Viridis.Cards;

/// <summary>气生根：5 格挡；库数每有 1 张额外 2 点。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class AerialRoot : CultivateCard
{
    public AerialRoot() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Block(ctx, play, 5m + 2m * Pile(play));
}

/// <summary>顶端优势：5 伤害；库数非空时 +3。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class ApicalDominance : CultivateCard
{
    public ApicalDominance() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        decimal dmg = Pile(play) > 0 ? 8m : 5m;
        await Hit(ctx, play, play.Target, dmg);
    }
}

/// <summary>年轮：6 伤害 + 库数。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class TreeRing : CultivateCard
{
    public TreeRing() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, 6m + Pile(play));
    }
}

/// <summary>引水：获得 1 能量；库数非空时抽 1 张。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class WaterDrawing : CultivateCard
{
    public WaterDrawing() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await PlayerCmd.GainEnergy(1m, play.Player);

        if (Pile(play) > 0)
            await CardPileCmd.Draw(ctx, 1m, play.Player);
    }
}

/// <summary>煎蛋！：叶绿素之力 —— 获得等同于库数的力量。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class FriedEgg : CultivateCard
{
    public FriedEgg() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = Pile(play);
        if (n <= 0)
            return;

        var temp = await PowerCmd.Apply<TempStrengthPower>(ctx, play.Player.Creature, 1m, play.Player.Creature, this);
        temp.Value = n;
        await PowerCmd.Apply<StrengthPower>(ctx, play.Player.Creature, n, play.Player.Creature, this);
    }
}

/// <summary>喜多护佑：获得等同于库数的敏捷。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class KitaBlessing : CultivateCard
{
    public KitaBlessing() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = Pile(play);
        if (n <= 0)
            return;

        var temp = await PowerCmd.Apply<TempDexterityPower>(ctx, play.Player.Creature, 1m, play.Player.Creature, this);
        temp.Value = n;
        await PowerCmd.Apply<DexterityPower>(ctx, play.Player.Creature, n, play.Player.Creature, this);
    }
}

/// <summary>杀植物：清空栽培牌库；每清 1 张获得 1 能量与 3 格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class MeltToWater : CultivateCard
{
    public MeltToWater() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = await CultivateSystem.ClearPileAsync(ctx, play.Player);
        if (n <= 0)
            return;

        await PlayerCmd.GainEnergy(n, play.Player);
        await Block(ctx, play, 3m * n);
    }
}

/// <summary>光合脉冲：造成 3 点伤害，次数 = 库数。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class PhotosyntheticPulse : CultivateCard
{
    public PhotosyntheticPulse() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        int n = Pile(play);
        for (int i = 0; i < n; i++)
            await Hit(ctx, play, play.Target, 3m);
    }
}

/// <summary>荆刺：6 伤害；获得等同于库数的格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class Thorns : CultivateCard
{
    public Thorns() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, 6m);

        await Block(ctx, play, Pile(play));
    }
}

/// <summary>树脂护壳：14 格挡；库数每有 1 张额外 2 点。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class ResinShell : CultivateCard
{
    public ResinShell() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Block(ctx, play, 14m + 2m * Pile(play));
}

/// <summary>叶绿素爆发：造成 库数 ×6 伤害。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class ChlorophyllBurst : CultivateCard
{
    public ChlorophyllBurst() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        int n = Pile(play);
        if (n <= 0)
            return;

        await Hit(ctx, play, play.Target, 6m * n);
    }
}

/// <summary>老根：获得 库数 ×4 格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class OldRoots : CultivateCard
{
    public OldRoots() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int n = Pile(play);
        if (n <= 0)
            return;

        await Block(ctx, play, 4m * n);
    }
}