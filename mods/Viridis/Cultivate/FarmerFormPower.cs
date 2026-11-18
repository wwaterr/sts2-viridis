using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Cultivate;

/// <summary>老农形态：每层让每回合的栽培进度 +1，收获时的产物数量 +1。</summary>
public sealed class FarmerFormPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override LocString Title => new("card_keywords", "VIRIDIS_KEYWORD_FARMER_FORM.title");

    public override LocString Description => new("card_keywords", "VIRIDIS_KEYWORD_FARMER_FORM.description");

    public override PowerAssetProfile AssetProfile =>
        new("res://Viridis/images/powers/farmer_form.ctex", "res://Viridis/images/powers/farmer_form.ctex");
}
