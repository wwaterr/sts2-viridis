using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Viridis.Hype;

/// <summary>考子：回合开始时把层数转换成同接。</summary>
[RegisterOwnedCardKeyword("Kaozi", IconPath = "res://Viridis/images/powers/kaozi.ctex", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class KaoziKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Kaozi").GetModCardKeyword();
}

/// <summary>反踩：每回合前 X 次打牌不获得同接。</summary>
[RegisterOwnedCardKeyword("AntiStomp", IconPath = "res://Viridis/images/powers/anti_stomp.ctex", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class AntiStompKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "AntiStomp").GetModCardKeyword();
}

/// <summary>串子：回合开始时随机获得活力或拉伸。</summary>
[RegisterOwnedCardKeyword("Chuanzi", IconPath = "res://Viridis/images/powers/chuanzi.ctex", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class ChuanziKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Chuanzi").GetModCardKeyword();
}
