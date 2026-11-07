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
/// 园艺工具：在你的回合开始时，额外抽 1 张牌，然后栽培 1 张手牌。
/// </summary>
[RegisterRelic(typeof(ViridisRelicPool))]
public sealed class GardeningTools : RelicModel
{
    public override LocString Title => new("relics", "GARDENING_TOOLS.title");

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override string IconBaseName => "gardening_tools";

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        try
        {
            if (player?.Creature?.CombatState is null)
                return;

            await CardPileCmd.Draw(choiceContext, 1m, player);
            Log.Info("[Cultivate] Gardening Tools: drew 1, prompting cultivate");

            await CultivateSystem.CultivateFromHand(choiceContext, player, this);
        }
        catch (Exception e)
        {
            // 回合开始钩子里不能抛异常，否则整个战斗循环会死锁
            Log.Error($"[Cultivate] Gardening Tools failed: {e}");
        }
    }
}