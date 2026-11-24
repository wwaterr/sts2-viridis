using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Cards;

namespace Viridis.Hype;

/// <summary>
/// 反踩：每受到一次伤害（含被格挡的部分）获得 1 同接；
/// 回合开始时将一张「独轮车」加入手牌，然后失去 1 层。上限 8 层，溢出转化为活力。
/// </summary>
public sealed class BacklashPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override LocString Title => new("card_keywords", "VIRIDIS_KEYWORD_BACKLASH.title");

    public override LocString Description => new("card_keywords", "VIRIDIS_KEYWORD_BACKLASH.description");

    public override PowerAssetProfile AssetProfile =>
        new("res://Viridis/images/powers/anti_stomp.ctex", "res://Viridis/images/powers/anti_stomp.ctex");

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Owner.Player is null || !ReferenceEquals(target, Owner))
            return Task.CompletedTask;

        if (result.TotalDamage <= 0 && result.BlockedDamage <= 0 && result.UnblockedDamage <= 0)
            return Task.CompletedTask;

        HypeSystem.Add(Owner.Player, 1);
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        await TriggerEffect(choiceContext, player);
    }

    public async Task TriggerEffect(PlayerChoiceContext choiceContext, Player player)
    {
        int stacks = Amount;
        if (stacks <= 0)
            return;

        await HypeSystem.GiveToken<Unicycle>(choiceContext, player);

        if (stacks <= 1)
            await PowerCmd.Remove(this);
        else
            SetAmount(stacks - 1);

        PolarityBarUI.Refresh(player);
    }
}
