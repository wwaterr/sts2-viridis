using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Viridis.Cultivate;

/// <summary>“永久栽培”关键词：进入栽培区后不参与收获，始终留在区内。</summary>
[RegisterOwnedCardKeyword("PermanentCultivate", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class PermanentCultivateKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "PermanentCultivate").GetModCardKeyword();
}
