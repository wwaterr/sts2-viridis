using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Powers;

/// <summary>云母蔓绿绒：保留手牌、能量和格挡，持续若干个己方回合。</summary>
public sealed class MicaWardPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override LocString Title => new("powers", "VIRIDIS_POWER_MICA_WARD_POWER.title");

    public override LocString Description => new("powers", "VIRIDIS_POWER_MICA_WARD_POWER.description");

    public override PowerAssetProfile AssetProfile =>
        new("res://Viridis/images/powers/mica_ward.ctex", "res://Viridis/images/powers/mica_ward.ctex");

    public override bool ShouldFlush(Player player) => player != Owner.Player;

    public override bool ShouldClearBlock(Creature creature) => creature != Owner;

    public override bool ShouldPlayerResetEnergy(Player player) => player != Owner.Player;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == CombatSide.Player && participants.Contains(Owner))
        {
            await PowerCmd.Decrement(this);
        }
    }
}
