using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Viridis.Hype;

/// <summary>“同接”计数。它本身不是可消耗资源，而是本回合内累计的观众数。</summary>
[RegisterOwnedCardKeyword("ViewerCount", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class ViewerCountKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "ViewerCount").GetModCardKeyword();
}
