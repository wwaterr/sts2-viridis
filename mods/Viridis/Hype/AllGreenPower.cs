using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Hype;

/// <summary>全力绿：同接获取倍率。层数即倍率（2 = 加倍，3 = 三倍）。</summary>
public sealed class AllGreenPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override LocString Title => new("powers", "VIRIDIS_POWER_ALL_GREEN.title");

    public override LocString Description => new("powers", "VIRIDIS_POWER_ALL_GREEN.description");
}
