using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Hype;

/// <summary>考子：回合开始时增加等同于层数的同接。</summary>
public sealed class KaoziPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override LocString Title => new("powers", "VIRIDIS_POWER_KAOZI_POWER.title");

    public override LocString Description => new("powers", "VIRIDIS_POWER_KAOZI_POWER.description");

    public override PowerAssetProfile AssetProfile =>
        new("res://Viridis/images/powers/kaozi.ctex", "res://Viridis/images/powers/kaozi.ctex");

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return Task.CompletedTask;

        HypeSystem.Add(player, Amount);
        return Task.CompletedTask;
    }
}
