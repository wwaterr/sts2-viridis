using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Cards;

namespace Viridis.Hype;

/// <summary>
/// 入脑：回合开始时获得等同于层数的同接，将一张「锡纸帽」加入手牌，然后失去 1 层。
/// 上限 8 层，溢出转化为拉伸。
/// </summary>
public sealed class EarwormPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override LocString Title => new("card_keywords", "VIRIDIS_KEYWORD_EARWORM.title");

    public override LocString Description => new("card_keywords", "VIRIDIS_KEYWORD_EARWORM.description");

    public override PowerAssetProfile AssetProfile =>
        new("res://Viridis/images/powers/kaozi.ctex", "res://Viridis/images/powers/kaozi.ctex");

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

        HypeSystem.Add(player, stacks);
        await HypeSystem.GiveToken<FoilHat>(choiceContext, player);

        if (stacks <= 1)
            await PowerCmd.Remove(this);
        else
            SetAmount(stacks - 1);

        PolarityBarUI.Refresh(player);
    }
}
