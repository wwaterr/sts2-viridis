using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Characters;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

/// <summary>
/// 周表：0 费初始技能。［固无］从「厕纸」和「晚上好！」中选择一张获得。
/// </summary>
[RegisterCard(typeof(ViridisCardPool))]
[RegisterCharacterStarterCard(typeof(ViridisCharacter), 1)]
public sealed class WeeklySchedule : CultivateCard
{
    public WeeklySchedule() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<PaperTowel>(),
        HoverTipFactory.FromCard<EveningGreeting>(),
        HoverTipFactory.FromKeyword(GuWuKeyword.Value),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { GuWuKeyword.Value };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var state = play.Player.Creature.CombatState;
        if (state is null)
            return;

        var options = new List<CardModel>
        {
            state.CreateCard<PaperTowel>(play.Player),
            state.CreateCard<EveningGreeting>(play.Player),
        };

        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, options, play.Player, canSkip: false);
        if (chosen is null)
            return;

        await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, play.Player, CardPilePosition.Top);
    }

    /// <summary>升级：把「固无」换成「固有」——起手必定抽到。</summary>
    protected override void OnUpgrade()
    {
        RemoveKeyword(GuWuKeyword.Value);
        AddKeyword(CardKeyword.Innate);
    }
}
