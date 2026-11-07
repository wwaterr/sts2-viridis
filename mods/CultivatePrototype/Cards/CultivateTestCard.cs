using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace CultivatePrototype.Cards;

/// <summary>测试卡：栽培手牌中的一张牌。手牌无牌可栽培时仍可打出，只是没有效果。</summary>
public sealed class CultivateTestCard : CardModel
{
    public CultivateTestCard() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    public override string Title => "Cultivate (test)";

    // 固有：保证开局就在手上，方便测试
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Innate };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        try
        {
            var player = cardPlay.Player;
            var hand = player.PlayerCombatState?.Hand.Cards;

            // 兜底：没有可选目标时直接空放，不弹选择、不报错
            var candidates = hand?
                .Where(c => c != this && c.Pile?.Type == PileType.Hand)
                .ToList() ?? new List<CardModel>();

            Log.Info($"[Cultivate] hand = [{string.Join(", ", candidates.Select(c => c.Id.ToString()))}]");

            if (candidates.Count == 0)
            {
                Log.Info("[Cultivate] no cultivable card in hand -> played with no effect");
                return;
            }

            var prefs = new CardSelectorPrefs(new LocString("cards", "CULTIVATE_TEST.select_prompt"), 1)
            {
                Cancelable = false,
                RequireManualConfirmation = false
            };

            var selected = await CardSelectCmd.FromHand(
                choiceContext, player, prefs, c => candidates.Contains(c), this);

            var target = selected?.FirstOrDefault();
            if (target is null)
            {
                // 没有选中任何牌：牌正常消耗，栽培无效
                Log.Info("[Cultivate] nothing selected -> played with no effect");
                return;
            }

            Log.Info($"[Cultivate] selected {target.Id}");
            await CultivateSystem.Cultivate(choiceContext, player, target);
        }
        catch (Exception e)
        {
            // 兜底：栽培链路出任何问题都不让这张牌把战斗搞崩
            Log.Error($"[Cultivate] play failed, treated as no-op: {e}");
        }
    }
}