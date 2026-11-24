using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

/// <summary>头部大主播：获得 5（6）点格挡和 3（4）层覆甲。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class HeadStreamer : CultivateCard
{
    public HeadStreamer() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<PlatingPower>(3),
    };

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new BlockVar(5m, ValueProp.Move), new PowerVar<PlatingPower>(3m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await Apply<PlatingPower>(ctx, play, play.Player.Creature, DynamicVars["PlatingPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
        DynamicVars["PlatingPower"].UpgradeValueBy(1m);
    }
}
