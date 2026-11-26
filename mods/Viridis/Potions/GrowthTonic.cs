using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Cultivate;
using Viridis.Pools;

namespace Viridis.Potions;

[RegisterPotion(typeof(ViridisPotionPool))]
public sealed class GrowthTonic : ModPotionTemplate
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [CultivateTips.Cultivate, CultivateTips.Grow, CultivateTips.Harvest];

    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    public override PotionAssetProfile AssetProfile => new(
        ImagePath: $"{ModEntry.ResPath}/images/potions/growth_tonic.ctex",
        OutlinePath: $"{ModEntry.ResPath}/images/potions/growth_tonic_outline.ctex");

    protected override Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        => CultivateSystem.Grow(choiceContext, Owner, 1);
}
