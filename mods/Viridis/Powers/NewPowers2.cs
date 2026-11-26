using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Viridis.Cultivate;
using Viridis.Hype;

namespace Viridis.Powers;

/// <summary>雪松：被攻击命中后获得格挡。</summary>
public sealed class CedarPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || !props.HasFlag(ValueProp.Move) || result.UnblockedDamage <= 0m)
            return;
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, null);
    }
}

/// <summary>飞行：受到的攻击伤害减半；每次受到未被格挡的攻击伤害消耗 1 层，回合结束不衰减。</summary>
public sealed class FlightPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
        => target == Owner && props.HasFlag(ValueProp.Move) ? 0.5m : 1m;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || !props.HasFlag(ValueProp.Move) || result.UnblockedDamage <= 0m)
            return;
        await PowerCmd.Decrement(this);
    }
}

/// <summary>直播间皇帝：回合开始扣考子、加反踩、回能并额外抽牌。</summary>
public sealed class EmperorPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
        await PowerCmd.Apply<KaoziPower>(choiceContext, Owner, -2m, Owner, null);
        await PowerCmd.Apply<AntiStompPower>(choiceContext, Owner, 2m, Owner, null);
        await PlayerCmd.GainEnergy(1m, player);
        await CardPileCmd.Draw(choiceContext, 1m, player);
    }
}

/// <summary>养啥死啥：栽培后立即令该作物枯萎，回能并抽牌。</summary>
public sealed class OverwaterPower : CultivatePowerBase
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner.Player)
            return;
        var power = player.Creature.GetPower<CultivateFieldPower>();
        var target = power?.Pending.LastOrDefault(p => !p.Permanent);
        if (target is null)
            return;
        if (!await CultivateSystem.Wither(ctx, player, target))
            return;
        await PlayerCmd.GainEnergy(Amount, player);
        await CardPileCmd.Draw(ctx, 1m, player);
    }
}

/// <summary>Day0：下回合少抽 2 张、少 2 点能量。</summary>
public sealed class Day0Power : PowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyHandDraw(Player player, decimal count)
        => player == Owner.Player ? Math.Max(0m, count - 2m) : count;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
        await PlayerCmd.LoseEnergy(2m, player);
        await PowerCmd.Remove(this);
    }
}
