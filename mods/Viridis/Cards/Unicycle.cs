// 已按改版要求暂时注释停用（独轮车）
/*
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Viridis.Cards;

/// <summary>独轮车：反踩衍生。造成 反踩 点伤害 2 次；栽入获得 反踩 点活力。消耗。</summary>
[RegisterCard(typeof(TokenCardPool))]
public sealed class Unicycle : PlaceholderArtCard
{
    public Unicycle() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(BacklashKeyword.Value),
        HoverTipFactory.FromKeyword(PlantInKeyword.Value),
        HoverTipFactory.FromPower<VigorPower>(1),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust, PlantInKeyword.Value };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new IntVar("Hits", 2m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        int stacks = HypeSystem.BacklashStacks(play.Player);
        int hits = (int)DynamicVars["Hits"].BaseValue;
        for (int i = 0; i < hits && play.Target.IsAlive; i++)
            await Hit(ctx, play, play.Target, stacks);
    }

    protected override void OnUpgrade() => DynamicVars["Hits"].UpgradeValueBy(1m);

    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
    {
        int stacks = HypeSystem.BacklashStacks(player);
        if (stacks <= 0)
            return;

        await PowerCmd.Apply<VigorPower>(ctx, player, stacks, player.Creature, this);
    }
}
*/
