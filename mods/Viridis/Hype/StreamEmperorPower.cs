using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Hype;

/// <summary>直播间皇帝：标记能力；由 <see cref="HypeSystem.GiveToken{T}"/> 检查，进手牌时升级衍生牌。</summary>
public sealed class StreamEmperorPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override LocString Title => new("powers", "VIRIDIS_POWER_STREAM_EMPEROR.title");

    public override LocString Description => new("powers", "VIRIDIS_POWER_STREAM_EMPEROR.description");
}
