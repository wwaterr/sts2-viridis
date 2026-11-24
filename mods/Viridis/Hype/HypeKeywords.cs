using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Viridis.Hype;

/// <summary>入脑：回合开始时获得层数点同接，并把一张锡纸帽加入手牌。</summary>
[RegisterOwnedCardKeyword("Earworm", IconPath = "res://Viridis/images/powers/kaozi.ctex", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class EarwormKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Earworm").GetModCardKeyword();
}

/// <summary>反踩：每受到一次伤害获得 1 同接，并把一张独轮车加入手牌。</summary>
[RegisterOwnedCardKeyword("Backlash", IconPath = "res://Viridis/images/powers/anti_stomp.ctex", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class BacklashKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Backlash").GetModCardKeyword();
}

/// <summary>串子：回合开始时随机获得活力或拉伸。</summary>
[RegisterOwnedCardKeyword("Chuanzi", IconPath = "res://Viridis/images/powers/chuanzi.ctex", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class ChuanziKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Chuanzi").GetModCardKeyword();
}

/// <summary>栽入：本牌被栽培时触发的额外效果。</summary>
[RegisterOwnedCardKeyword("PlantIn", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class PlantInKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "PlantIn").GetModCardKeyword();
}

/// <summary>黑听：入脑与反踩均为 0 的状态。</summary>
[RegisterOwnedCardKeyword("Lurking", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class LurkingKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Lurking").GetModCardKeyword();
}
