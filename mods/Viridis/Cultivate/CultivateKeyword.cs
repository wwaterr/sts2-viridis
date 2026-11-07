using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Viridis.Cultivate;

/// <summary>模组关键词“栽培”：注册定义，并暴露原版 <see cref="CardKeyword"/> 铸造值供卡牌引用。</summary>
[RegisterOwnedCardKeyword("Cultivate", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.AfterCardDescription)]
public sealed class CultivateKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Cultivate").GetModCardKeyword();
}
