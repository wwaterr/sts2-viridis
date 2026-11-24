using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Cultivate;

namespace Viridis.Hype;

/// <summary>
/// 她在和谁说话：本回合同接达到阈值后，下回合开始时生长 1。
/// 层数即阈值（基础 8，升级后 6）。
/// </summary>
public sealed class SheIsTalkingPower : ModPowerTemplate
{
    private bool _pending;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override LocString Title => new("powers", "VIRIDIS_POWER_SHE_IS_TALKING.title");

    public override LocString Description => new("powers", "VIRIDIS_POWER_SHE_IS_TALKING.description");

    /// <summary>由 HypeSystem 在每次同接变化后调用。</summary>
    public void NotifyHype(int total)
    {
        if (total >= Amount)
            _pending = true;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || !_pending)
            return;

        _pending = false;
        await CultivateSystem.Grow(choiceContext, player, 1);
    }
}
