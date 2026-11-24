using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

/// <summary>全力绿：你获得的同接量加倍（升级后三倍）。黑听：入脑 3（4）并发动效果。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class AllGreen : CultivateCard
{
    public AllGreen() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(EarwormKeyword.Value),
        HoverTipFactory.FromKeyword(LurkingKeyword.Value),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<AllGreenPower>(2m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool lurking = HypeSystem.IsLurking(play.Player);

        await Apply<AllGreenPower>(ctx, play, play.Player.Creature, DynamicVars["AllGreenPower"].BaseValue);

        if (!lurking)
            return;

        await HypeSystem.GainEarworm(ctx, play.Player, IsUpgraded ? 4 : 3);
        if (play.Player.Creature.GetPower<EarwormPower>() is { } earworm)
            await earworm.TriggerEffect(ctx, play.Player);
    }

    protected override void OnUpgrade() => DynamicVars["AllGreenPower"].UpgradeValueBy(1m);
}
