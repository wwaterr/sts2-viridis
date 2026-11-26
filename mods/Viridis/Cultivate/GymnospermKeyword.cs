using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Viridis.Cultivate;

/// <summary>裸子植物：只作为标签，供“真·裸子植物”等效果检索。</summary>
[RegisterOwnedCardKeyword("Gymnosperm", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class GymnospermKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Gymnosperm").GetModCardKeyword();
}
