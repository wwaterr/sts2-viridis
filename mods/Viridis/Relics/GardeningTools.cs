using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Cultivate;

namespace Viridis.Relics;

/// <summary>
/// 园丁手套：战斗的第一回合，额外抽 1 张牌，然后栽培 1 张手牌（每场战斗只触发一次）。
/// 欧洛巴斯之触可将其升级为园艺工具。
/// </summary>
[RegisterRelic(typeof(ViridisRelicPool))]
[RegisterTouchOfOrobasRefinement(typeof(GardeningToolsUpgraded))]
public sealed class GardeningTools : RelicModel
{
    /// <summary>上一次触发所属的战斗；换战斗后自动重置，实现「每场战斗一次」。</summary>
    private object? _lastCombat;

    private bool _usedThisCombat;

    public override LocString Title => new("relics", "VIRIDIS_RELIC_GARDENING_TOOLS.title");

    public override RelicRarity Rarity => RelicRarity.Starter;

    private const string IconArt = "res://Viridis/images/relics/gardener_gloves";
    public override string PackedIconPath => IconArt + ".ctex";
    protected override string PackedIconOutlinePath => IconArt + "_outline.ctex";
    protected override string BigIconPath => IconArt + "_big.ctex";

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        try
        {
            var combat = player?.Creature?.CombatState;
            if (combat is null)
                return;

            // 每进入一场新战斗，重置一次性开关
            if (!ReferenceEquals(_lastCombat, combat))
            {
                _lastCombat = combat;
                _usedThisCombat = false;
            }

            if (_usedThisCombat)
                return;

            _usedThisCombat = true;

            await CardPileCmd.Draw(choiceContext, 1m, player!);
            Log.Info("[Cultivate] Gardening Tools: first turn, drew 1, prompting cultivate");

            await CultivateSystem.CultivateFromHand(choiceContext, player!, this);
        }
        catch (Exception e)
        {
            // 回合开始钩子里不能抛异常，否则整个战斗循环会死锁
            Log.Error($"[Cultivate] Gardening Tools failed: {e}");
        }
    }
}
