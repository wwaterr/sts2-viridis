using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Pools;

namespace Viridis.Relics;

[RegisterRelic(typeof(ViridisRelicPool))]
public sealed class HarvestCharm : ModRelicTemplate
{
    private bool _triggeredThisCombat;

    public override RelicRarity Rarity => RelicRarity.Common;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{ModEntry.ResPath}/images/relics/harvest_charm.ctex",
        IconOutlinePath: $"{ModEntry.ResPath}/images/relics/harvest_charm_outline.ctex",
        BigIconPath: $"{ModEntry.ResPath}/images/relics/harvest_charm.ctex");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new BlockVar(8m, ValueProp.Unpowered) };

    public override Task BeforeCombatStart()
    {
        _triggeredThisCombat = false;
        return Task.CompletedTask;
    }

    internal async Task OnHarvested(Player player)
    {
        if (player != Owner || _triggeredThisCombat)
            return;

        _triggeredThisCombat = true;
        Flash();
        await CreatureCmd.GainBlock(player.Creature, DynamicVars.Block, null);
    }
}
