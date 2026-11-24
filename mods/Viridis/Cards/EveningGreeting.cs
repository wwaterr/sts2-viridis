using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Viridis.Cards;

/// <summary>晚上好！：0 费衍生技能。获得 1 能量，入脑 2（3）。消耗。</summary>
[RegisterCard(typeof(TokenCardPool))]
public sealed class EveningGreeting : PlaceholderArtCard
{
    public EveningGreeting() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(EarwormKeyword.Value),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<EarwormPower>(2m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Energy(ctx, play, 1);
        await HypeSystem.GainEarworm(ctx, play.Player, (int)DynamicVars["EarwormPower"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars["EarwormPower"].UpgradeValueBy(1m);
}
