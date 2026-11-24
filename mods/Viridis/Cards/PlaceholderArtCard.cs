using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Cards;

/// <summary>
/// 没有专属卡图的卡牌基类：统一使用「图片暂缺」占位卡图。
/// 新增卡如果没有自己的 AssetProfile，就继承这个类，不要改已有的卡。
/// </summary>
public abstract class PlaceholderArtCard : CultivateCard
{
    protected PlaceholderArtCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target)
    {
    }

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: $"{ModEntry.ResPath}/images/cards/card_placeholder.ctex", BannerTexturePath: $"{ModEntry.ResPath}/images/ui/card_banner.ctex");
}
