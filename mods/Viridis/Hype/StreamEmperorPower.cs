using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Cultivate;

namespace Viridis.Hype;

/// <summary>
/// 直播间皇帝：每回合首次获得入脑或反踩时，生长 Amount；每回合入脑或反踩首次降为 0 时，生长 Amount。
/// 入脑与反踩合并计算，每层每回合最多触发 2 次（获得与归零各 1 次）。
/// </summary>
public sealed class StreamEmperorPower : ModPowerTemplate
{
    private int _gainedRound;
    private int _zeroedRound;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override LocString Title => new("powers", "VIRIDIS_POWER_STREAM_EMPEROR.title");

    public override LocString Description => new("powers", "VIRIDIS_POWER_STREAM_EMPEROR.description");

    public async Task OnPolarityGained(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner.Player || Amount <= 0)
            return;

        int round = Owner.CombatState?.RoundNumber ?? 0;
        if (round > 0 && _gainedRound == round)
            return;

        _gainedRound = round;
        Flash();
        await CultivateSystem.Grow(ctx, player, Amount);
    }

    public async Task OnPolarityReachedZero(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner.Player || Amount <= 0)
            return;

        int round = Owner.CombatState?.RoundNumber ?? 0;
        if (round > 0 && _zeroedRound == round)
            return;

        _zeroedRound = round;
        Flash();
        await CultivateSystem.Grow(ctx, player, Amount);
    }
}
