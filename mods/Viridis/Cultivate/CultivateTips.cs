using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Viridis.Cultivate;

[RegisterOwnedCardKeyword("Harvest", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class HarvestKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Harvest").GetModCardKeyword();
}

// Referenced mechanics belong in hover tips. Only the card's own keywords belong in CanonicalKeywords.
internal static class CultivateTips
{
    internal static IHoverTip Cultivate => HoverTipFactory.FromKeyword(CultivateKeyword.Value);
    internal static IHoverTip Grow => HoverTipFactory.FromKeyword(GrowKeyword.Value);
    internal static IHoverTip Harvest => HoverTipFactory.FromKeyword(HarvestKeyword.Value);
}
