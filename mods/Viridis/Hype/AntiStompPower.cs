using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Hype;

/// <summary>反踩：每回合前 X 次打出的牌不会获得同接。</summary>
public sealed class AntiStompPower : ModPowerTemplate
{
    private int _blockedThisTurn;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile =>
        new("res://Viridis/images/powers/anti_stomp.ctex", "res://Viridis/images/powers/anti_stomp.ctex");

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
            _blockedThisTurn = 0;

        return Task.CompletedTask;
    }

    /// <summary>由 HypeCounterPower 调用，保证不受能力钩子执行顺序影响。</summary>
    public bool TryBlockCard()
    {
        if (_blockedThisTurn >= Amount)
            return false;

        _blockedThisTurn++;
        return true;
    }
}
