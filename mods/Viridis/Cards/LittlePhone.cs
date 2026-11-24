using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

/// <summary>小手机真好玩：获得 1 层人工制品与 2 层反踩。消耗。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class LittlePhone : CultivateCard
{
    public LittlePhone() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<ArtifactPower>(1),
        HoverTipFactory.FromKeyword(BacklashKeyword.Value),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<ArtifactPower>(1m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<ArtifactPower>(ctx, play, play.Player.Creature, DynamicVars["ArtifactPower"].BaseValue);
        await HypeSystem.GainBacklash(ctx, play.Player, 2);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
