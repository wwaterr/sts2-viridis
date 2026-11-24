using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

/// <summary>登山：获得 1 层无实体；所有同接转化为反踩并发动效果；下回合获得 2 能量。消耗。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class MountainClimb : CultivateCard
{
    public MountainClimb() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(BacklashKeyword.Value),
        HoverTipFactory.FromKeyword(ViewerCountKeyword.Value),
        HoverTipFactory.FromPower<IntangiblePower>(1),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<IntangiblePower>(1m), new EnergyVar(2) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<IntangiblePower>(ctx, play, play.Player.Creature, 1m);

        int hype = HypeSystem.TakeAll(play.Player);
        if (hype > 0)
            await HypeSystem.GainBacklash(ctx, play.Player, hype);

        if (play.Player.Creature.GetPower<BacklashPower>() is { } backlash)
            await backlash.TriggerEffect(ctx, play.Player);

        await Apply<EnergyNextTurnPower>(ctx, play, play.Player.Creature, 2m);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
