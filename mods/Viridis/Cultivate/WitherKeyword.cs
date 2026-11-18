using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Viridis.Cultivate;

/// <summary>枯萎：把一张牌移出栽培区，不收获，也不返还原卡。</summary>
[RegisterOwnedCardKeyword("Wither", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class WitherKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Wither").GetModCardKeyword();
}
