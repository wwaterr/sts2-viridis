using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using Viridis.Pools;

namespace Viridis.Cards;

/// <summary>全力绿：2 费罕见能力。你获得的同接量加倍（变为 3 倍）。入脑 3（4）。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class AllGreen : CultivateCard
{
    public AllGreen() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(ViewerCountKeyword.Value),
        HoverTipFactory.FromKeyword(EarwormKeyword.Value),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new PowerVar<AllGreenPower>(2m),
            new PowerVar<EarwormPower>(3m),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int targetMultiplier = (int)DynamicVars["AllGreenPower"].BaseValue;
        var existing = play.Player.Creature.GetPower<AllGreenPower>();
        if (existing is null)
            await Apply<AllGreenPower>(ctx, play, play.Player.Creature, targetMultiplier);
        else if (targetMultiplier > existing.Amount)
            existing.SetAmount(targetMultiplier);

        await HypeSystem.GainEarworm(ctx, play.Player, (int)DynamicVars["EarwormPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AllGreenPower"].UpgradeValueBy(1m);
        DynamicVars["EarwormPower"].UpgradeValueBy(1m);
    }
}
