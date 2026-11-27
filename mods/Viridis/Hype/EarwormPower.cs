using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Powers;

namespace Viridis.Hype;

/// <summary>
/// 入脑：回合开始时，获得 X 点拉伸、3 点同接，并失去 1 层入脑。
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

        await PowerCmd.Apply<StretchPower>(choiceContext, player.Creature, stacks, player.Creature, null);
        HypeSystem.Add(player, 3);

        bool droppedToZero = stacks <= 1;
        if (droppedToZero)
            await PowerCmd.Remove(this);
        else
            SetAmount(stacks - 1);

        PolarityBarUI.Refresh(player);
        if (droppedToZero)
            await HypeSystem.NotifyPolarityReachedZero(choiceContext, player);
    }
}
