using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Viridis.Cards;

/// <summary>不要叶绿素了：入脑把所有同接转化为格挡；反踩把所有同接转化为活力；黑听获得 5（7）层串子。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class NoChlorophyll : CultivateCard
{
    public NoChlorophyll() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(EarwormKeyword.Value),
        HoverTipFactory.FromKeyword(BacklashKeyword.Value),
        HoverTipFactory.FromKeyword(LurkingKeyword.Value),
        HoverTipFactory.FromKeyword(ChuanziKeyword.Value),
        HoverTipFactory.FromPower<VigorPower>(1),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new IntVar("Chuanzi", 5m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var player = play.Player;

        if (HypeSystem.EarwormStacks(player) > 0)
        {
            int hype = HypeSystem.TakeAll(player);
            if (hype > 0)
                await Block(ctx, play, hype);
            return;
        }

        if (HypeSystem.BacklashStacks(player) > 0)
        {
            int hype = HypeSystem.TakeAll(player);
            if (hype > 0)
                await Apply<VigorPower>(ctx, play, player.Creature, hype);
            return;
        }

        await Apply<ChuanziPower>(ctx, play, player.Creature, DynamicVars["Chuanzi"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars["Chuanzi"].UpgradeValueBy(2m);
}
