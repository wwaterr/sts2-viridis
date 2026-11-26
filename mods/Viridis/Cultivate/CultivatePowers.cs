using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Hype;
using Viridis.Powers;

namespace Viridis.Cultivate;

/// <summary>栽培相关能力的基类：提供"栽培 / 收获 / 清库"三个事件钩子。</summary>
public abstract class CultivatePowerBase : ModPowerTemplate
{
    public override LocString Title => new("powers", $"VIRIDIS_POWER_{Id.Entry}.title");

    public override LocString Description => new("powers", $"VIRIDIS_POWER_{Id.Entry}.description");

    public override PowerAssetProfile AssetProfile =>
        new($"{ModEntry.ResPath}/images/ui/cultivate_pile.ctex", $"{ModEntry.ResPath}/images/ui/cultivate_pile.ctex");

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected static int Pile(Player player) => CultivateSystem.PileCount(player);

    public virtual Task OnCultivated(PlayerChoiceContext ctx, Player player) => Task.CompletedTask;

    public virtual Task OnHarvested(PlayerChoiceContext ctx, Player player, CardModel card) => Task.CompletedTask;

    public virtual Task OnPileCleared(PlayerChoiceContext ctx, Player player, int count) => Task.CompletedTask;

    public virtual Task OnWithered(PlayerChoiceContext ctx, Player player, Planting planting) => Task.CompletedTask;

    protected async Task Block(Player player, decimal amount)
        => await CreatureCmd.GainBlock(player.Creature, amount, BlockProps.card, null!);

    protected async Task Draw(PlayerChoiceContext ctx, Player player, int count)
        => await CardPileCmd.Draw(ctx, count, player);
}

/// <summary>本回合力量（回合结束时移除）。</summary>
public sealed class TempStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Viridis.Cards.FriedEgg>();
}

/// <summary>本回合敏捷（回合结束时移除）。</summary>
public sealed class TempDexterityPower : TemporaryDexterityPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Viridis.Cards.Weasel>();
}

/// <summary>共生：每次栽培成功后，对一个随机敌人造成等同于层数的伤害。</summary>
public sealed class SymbiosisPower : CultivatePowerBase
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
    {
        if (Owner != player.Creature || Owner.CombatState?.HittableEnemies is not { Count: > 0 } enemies)
            return;

        var target = player.RunState.Rng.CombatTargets.NextItem(enemies);
        if (target is null)
            return;

        Flash();
        await CreatureCmd.Damage(ctx, target, Amount, ValueProp.Unpowered, Owner);
    }
}

/// <summary>根系网络：自己的回合结束时，栽培区每有1张牌便获得等同层数的格挡。</summary>
public sealed class RootNetworkPower : CultivatePowerBase
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        var player = Owner.Player;
        if (player is null)
            return;

        int n = Pile(player);
        if (n > 0)
            await Block(player, Amount * n);
    }
}

/// <summary>千年古树：自己的回合开始时，获得每株 1 点格挡、升级额外格挡与 1 点能量。</summary>
public sealed class AncientTreePower : CultivatePowerBase
{
    public decimal BonusBlock { get; set; }

    public override LocString Description
    {
        get
        {
            var description = new LocString("powers", "VIRIDIS_POWER_ANCIENT_TREE_POWER.description");
            description.Add("HasBonusBlock", BonusBlock > 0m);
            description.Add("BonusBlock", BonusBlock);
            return description;
        }
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext ctx, Player player)
    {
        if (Owner.Player != player)
            return;

        decimal block = Pile(player) + BonusBlock;
        if (block > 0m)
            await Block(player, block);

        await PlayerCmd.GainEnergy(1m, player);
    }
}

/// <summary>冠层：回合开始时按栽培区牌数与本次收获数获得能量。</summary>
public sealed class CanopyPower : CultivatePowerBase
{
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _harvested;

    public override Task OnHarvested(PlayerChoiceContext ctx, Player player, CardModel card)
    {
        if (player == Owner.Player)
            _harvested++;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner.Player)
            return;

        int n = Pile(player) + _harvested;
        _harvested = 0;

        if (n > 0)
            await PlayerCmd.GainEnergy(n * Amount, player);
    }
}

/// <summary>母株：回合开始时若栽培牌库为空，栽培 2 张手牌。</summary>
public sealed class MotherPlantPower : CultivatePowerBase
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner.Player || Pile(player) > 0)
            return;

        await CultivateSystem.CultivateFromHand(ctx, player, this);
        await CultivateSystem.CultivateFromHand(ctx, player, this);
    }
}

/// <summary>苔原：自己的回合开始时获得等同层数的格挡。</summary>
public sealed class TundraPower : CultivatePowerBase
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (player.Creature == Owner)
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}

/// <summary>菌根：每当你栽培 1 张牌，抽等同层数的牌。</summary>
public sealed class MycorrhizaPower : CultivatePowerBase
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
        => await Draw(ctx, player, Amount);
}

/// <summary>年轮刻印：每张主收获品产生一次格挡。</summary>
public sealed class TreeRingMarkPower : CultivatePowerBase
{
    public override PowerStackType StackType => PowerStackType.Counter;


    public override async Task OnHarvested(PlayerChoiceContext ctx, Player player, CardModel card)
        => await Block(player, Amount);
}

/// <summary>叶脉：每当你打出一张先古牌，获得 1 点能量。</summary>
public sealed class LeafVeinPower : CultivatePowerBase
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner.Player && cardPlay.Card.Rarity == CardRarity.Ancient)
            await PlayerCmd.GainEnergy(1m, Owner.Player);
    }
}

/// <summary>丰饶：每当你收获 1 张牌，随机获得 1 点力量或 1 点敏捷；升级后两者都获得。</summary>
public sealed class ForestLordPower : CultivatePowerBase
{
    public bool Both { get; set; }

    public override async Task OnHarvested(PlayerChoiceContext ctx, Player player, CardModel card)
    {
        if (player != Owner.Player)
            return;

        if (Both)
        {
            await PowerCmd.Apply<StrengthPower>(ctx, player.Creature, 1m, player.Creature, null);
            await PowerCmd.Apply<DexterityPower>(ctx, player.Creature, 1m, player.Creature, null);
            return;
        }

        if (player.RunState.Rng.Niche.NextBool())
            await PowerCmd.Apply<StrengthPower>(ctx, player.Creature, 1m, player.Creature, null);
        else
            await PowerCmd.Apply<DexterityPower>(ctx, player.Creature, 1m, player.Creature, null);
    }
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
    public decimal DamageValue { get; set; }
    public bool CountHarvested { get; set; }
    private int _harvested;

    public override async Task OnHarvested(PlayerChoiceContext ctx, Player player, CardModel card)
    {
        if (CountHarvested && player == Owner.Player)
            _harvested++;
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner.Player)
            return;

        int targets = Pile(player) + (CountHarvested ? _harvested : 0);
        if (targets <= 0)
        {
            await PowerCmd.Remove(this);
            return;
        }

        var enemies = Owner.CombatState?.HittableEnemies;
        if (enemies is null || enemies.Count == 0)
        {
            await PowerCmd.Remove(this);
            return;
        }

        Flash();
        for (int i = 0; i < targets; i++)
        {
            var target = player.RunState.Rng.CombatTargets.NextItem(enemies);
            if (target is null)
                break;

            await CreatureCmd.Damage(ctx, target, DamageValue, ValueProp.Unpowered, Owner);
        }

        await PowerCmd.Remove(this);
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

/// <summary>纪录片回：下一次栽培时获得 1 点能量并抽 1 张牌。</summary>
public sealed class NextCultivateEnergyPower : CultivatePowerBase
{
    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
    {
        await PlayerCmd.GainEnergy(1m, player);
        await Draw(ctx, player, 1);
        await PowerCmd.Remove(this);
    }
}

/// <summary>落地生根：自己的牌每命中一名敌人，获得 1 点格挡。</summary>
public sealed class LandedRootPower : CultivatePowerBase
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
        => await PowerCmd.Apply<PlatingPower>(ctx, Owner, Amount, Owner, null);
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

/// <summary>年轮循环：每两次自己的回合开始，生长 3。</summary>
public sealed class SpringCyclePower : CultivatePowerBase
{
    private int _turns;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (Owner.Player != player)
            return;

        _turns++;
        if (_turns % 2 != 0)
            return;

        await CultivateSystem.Grow(ctx, player, 3);
    }
}

/// <summary>世界树：回合开始时栽培 1 张手牌；每次收获额外获得 1 张同稀有度牌。</summary>
public sealed class WorldTreePower : CultivatePowerBase
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (player == Owner.Player)
            await CultivateSystem.CultivateFromHand(ctx, player, this);
    }
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
/// <summary>骡子职务：你的回合开始时获得等同层数的考子。</summary>
public sealed class MuleDutyPower : CultivatePowerBase
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (player == Owner.Player)
            await PowerCmd.Apply<KaoziPower>(ctx, player.Creature, Amount, Owner, null);
    }
}

/// <summary>落叶层：下个回合开始时获得格挡并生长 1。</summary>
public sealed class LeafLitterPower : CultivatePowerBase
{
    public decimal BlockValue { get; set; }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner.Player)
            return;

        await Block(player, BlockValue);
        await CultivateSystem.Grow(ctx, player, 1);
        await PowerCmd.Remove(this);
    }
}

/// <summary>老树盘根：回合结束时获得格挡；受到未被格挡的伤害时移除。</summary>
public sealed class OldTreeRootsPower : CultivatePowerBase
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
            await Block(Owner.Player!, Amount);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext ctx, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && result.UnblockedDamage > 0m)
            await PowerCmd.Remove(this);
    }
}

public static class CultivateFlags
{
    public static bool CanCultivate(Player player)
        => player.Creature?.GetPower<NoCultivatePower>() is null;

    public static bool NextCultivateFast(Player player)
        => player.Creature?.GetPower<NextCultivateFastPower>() is not null;

    public static bool Has<T>(Player player) where T : PowerModel
        => player.Creature?.GetPower<T>() is not null;
}
