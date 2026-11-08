using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Viridis.Cultivate;

/// <summary>栽培相关能力的基类：提供"栽培 / 收获 / 清库"三个事件钩子。</summary>
public abstract class CultivatePowerBase : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected static int Pile(Player player) => CultivateSystem.PileCount(player);

    public virtual Task OnCultivated(PlayerChoiceContext ctx, Player player) => Task.CompletedTask;

    public virtual Task OnHarvested(PlayerChoiceContext ctx, Player player, CardModel card) => Task.CompletedTask;

    public virtual Task OnPileCleared(PlayerChoiceContext ctx, Player player, int count) => Task.CompletedTask;

    protected async Task Block(Player player, decimal amount)
        => await CreatureCmd.GainBlock(player.Creature, amount, BlockProps.card, null!);

    protected async Task Draw(PlayerChoiceContext ctx, Player player, int count)
        => await CardPileCmd.Draw(ctx, count, player);
}

/// <summary>本回合力量（回合结束时移除）。</summary>
public sealed class TempStrengthPower : CultivatePowerBase
{
    public decimal Value { get; set; } = 1m;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        await PowerCmd.Apply<StrengthPower>(ctx, Owner, -Value, Owner, null);
        await PowerCmd.Remove(this);
    }
}

/// <summary>本回合敏捷（回合结束时移除）。</summary>
public sealed class TempDexterityPower : CultivatePowerBase
{
    public decimal Value { get; set; } = 1m;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        await PowerCmd.Apply<DexterityPower>(ctx, Owner, -Value, Owner, null);
        await PowerCmd.Remove(this);
    }
}

/// <summary>共生：回合开始时获得等同于库数的力量（本回合）。</summary>
public sealed class SymbiosisPower : CultivatePowerBase
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        int n = Pile(player);
        if (n <= 0)
            return;

        var p = await PowerCmd.Apply<TempStrengthPower>(ctx, player.Creature, 1m, player.Creature, null);
        p.Value = n;
        await PowerCmd.Apply<StrengthPower>(ctx, player.Creature, n, player.Creature, null);
    }
}

/// <summary>根系网络：回合开始时获得等同于库数的格挡。</summary>
public sealed class RootNetworkPower : CultivatePowerBase
{
    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext ctx, Player player)
    {
        int n = Pile(player);
        if (n > 0)
            await Block(player, n);
    }
}

/// <summary>千年古树：回合开始时获得库数格挡与 1 点能量。</summary>
public sealed class AncientTreePower : CultivatePowerBase
{
    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext ctx, Player player)
    {
        int n = Pile(player);
        if (n > 0)
            await Block(player, n);

        await PlayerCmd.GainEnergy(1m, player);
    }
}

/// <summary>冠层：回合开始时按库数获得能量（无上限）。</summary>
public sealed class CanopyPower : CultivatePowerBase
{
    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext ctx, Player player)
    {
        int n = Pile(player);
        if (n > 0)
            await PlayerCmd.GainEnergy(n, player);
    }
}

/// <summary>生长期：回合开始时生长 1。</summary>
public sealed class GrowthPeriodPower : CultivatePowerBase
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
        => await CultivateSystem.Grow(ctx, player, 1);
}

/// <summary>母株：回合开始时若栽培牌库为空，栽培 2 张手牌。</summary>
public sealed class MotherPlantPower : CultivatePowerBase
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (Pile(player) > 0)
            return;

        await CultivateSystem.CultivateFromHand(ctx, player, this);
        await CultivateSystem.CultivateFromHand(ctx, player, this);
    }
}

/// <summary>苔原：回合开始时若栽培牌库为空，栽培 1 张手牌。</summary>
public sealed class TundraPower : CultivatePowerBase
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (Pile(player) == 0)
            await CultivateSystem.CultivateFromHand(ctx, player, this);
    }
}

/// <summary>菌根：每当你栽培 1 张牌，抽 1 张牌。</summary>
public sealed class MycorrhizaPower : CultivatePowerBase
{
    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
        => await Draw(ctx, player, 1);
}

/// <summary>年轮刻印：每当你收获 1 张牌，获得 2 点格挡。</summary>
public sealed class TreeRingMarkPower : CultivatePowerBase
{
    public override async Task OnHarvested(PlayerChoiceContext ctx, Player player, CardModel card)
        => await Block(player, 2m);
}

/// <summary>叶脉：每当你收获 1 张牌，抽 1 张牌。</summary>
public sealed class LeafVeinPower : CultivatePowerBase
{
    public override async Task OnHarvested(PlayerChoiceContext ctx, Player player, CardModel card)
        => await Draw(ctx, player, 1);
}

/// <summary>森林之主：每当你收获 1 张牌，获得 2 点力量。</summary>
public sealed class ForestLordPower : CultivatePowerBase
{
    public override async Task OnHarvested(PlayerChoiceContext ctx, Player player, CardModel card)
        => await PowerCmd.Apply<StrengthPower>(ctx, player.Creature, 2m, player.Creature, null);
}

/// <summary>深根：每当你栽培 1 张牌，获得 1 点力量与 1 点敏捷。</summary>
public sealed class DeepRootPower : CultivatePowerBase
{
    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
    {
        await PowerCmd.Apply<StrengthPower>(ctx, player.Creature, 1m, player.Creature, null);
        await PowerCmd.Apply<DexterityPower>(ctx, player.Creature, 1m, player.Creature, null);
    }
}

/// <summary>花芽分化：每回合第一次栽培时，生长 1。</summary>
public sealed class FlowerBudPower : CultivatePowerBase
{
    private bool _used;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
        => _used = false;

    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
    {
        if (_used)
            return;

        _used = true;
        await CultivateSystem.Grow(ctx, player, 1);
    }
}

/// <summary>树液：本回合无法栽培（回合结束时移除）。</summary>
public sealed class NoCultivatePower : CultivatePowerBase
{
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
            await PowerCmd.Remove(this);
    }
}

/// <summary>老树盘根：你的下一次栽培剩余回合为 1（消耗）。</summary>
public sealed class NextCultivateFastPower : CultivatePowerBase
{
}

/// <summary>落地生根：当你打出攻击牌造成伤害时，获得 1 点格挡。</summary>
public sealed class LandedRootPower : CultivatePowerBase
{
    public override async Task AfterDamageGiven(PlayerChoiceContext ctx, Creature dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (cardSource is null || dealer != Owner)
            return;

        var player = Owner.Player;
        if (player is null || Pile(player) == 0)
            return;

        await Block(player, 1m);
    }
}

/// <summary>泥炭：回合结束时若栽培牌库非空，抽 1 张牌（然后移除）。</summary>
public sealed class DelayedDrawPower : CultivatePowerBase
{
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        var player = Owner.Player;
        if (player is not null && Pile(player) > 0)
            await Draw(ctx, player, 1);

        await PowerCmd.Remove(this);
    }
}

/// <summary>幼苗移栽：回合结束时生长 1（然后移除）。</summary>
public sealed class DelayedGrowPower : CultivatePowerBase
{
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        var player = Owner.Player;
        if (player is not null)
            await CultivateSystem.Grow(ctx, player, 1);

        await PowerCmd.Remove(this);
    }
}

/// <summary>年轮循环：每 3 个回合生长 3。</summary>
public sealed class SpringCyclePower : CultivatePowerBase
{
    private int _turns;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        _turns++;
        if (_turns % 3 != 0)
            return;

        await CultivateSystem.Grow(ctx, player, 3);
    }
}

/// <summary>世界树：回合开始时栽培 1 张手牌；每次收获额外获得 1 张同稀有度牌。</summary>
public sealed class WorldTreePower : CultivatePowerBase
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
        => await CultivateSystem.CultivateFromHand(ctx, player, this);
}

/// <summary>始祖孢子：栽培牌库被清空时，对所有敌人造成 20 点伤害。</summary>
public sealed class PrimordialSporePower : CultivatePowerBase
{
    public override async Task OnPileCleared(PlayerChoiceContext ctx, Player player, int count)
    {
        var enemies = player.Creature.CombatState?.HittableEnemies;
        if (enemies is null)
            return;

        foreach (var e in enemies.ToList())
        {
            try
            {
                await CreatureCmd.Damage(ctx, e, 20m, DamageProps.card, null, null);
            }
            catch (Exception ex)
            {
                Log.Error($"[Cultivate] primordial spore damage failed: {ex.Message}");
            }
        }
    }
}

/// <summary>腐殖循环：每当你收获 1 张牌，将其复制加入手牌。</summary>
public sealed class HumusCyclePower : CultivatePowerBase
{
}

/// <summary>栽培牌库为空时是否允许栽培（树液）。</summary>
public static class CultivateFlags
{
    public static bool CanCultivate(Player player)
        => player.Creature?.GetPower<NoCultivatePower>() is null;

    public static bool NextCultivateFast(Player player)
        => player.Creature?.GetPower<NextCultivateFastPower>() is not null;

    public static bool Has<T>(Player player) where T : PowerModel
        => player.Creature?.GetPower<T>() is not null;
}