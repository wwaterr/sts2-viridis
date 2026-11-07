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

            // 兜底：没有可选目标时直接空放，不弹选择界面、不报错
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
                Cancelable = true,
                RequireManualConfirmation = false
            };

            IEnumerable<CardModel>? selected;
            CancelButtonPatch.RequestCancelable = true;
            try
            {
                selected = await CardSelectCmd.FromHand(
                    choiceContext, player, prefs, c => candidates.Contains(c), this);
            }
            finally
            {
                CancelButtonPatch.RequestCancelable = false;
            }

            var target = selected?.FirstOrDefault();
            if (target is null)
            {
                // 取消选择：这张牌回到手牌，并把已花费的能量退回去
                int refund = cardPlay.Resources.EnergySpent;

                // ResultPile 是 init-only，只能反射写回退字段
                bool redirected = TrySetResultPile(cardPlay, PileType.Hand);
                if (!redirected)
                    await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Top);

                if (refund > 0)
                    await PlayerCmd.GainEnergy(refund, player);

                Log.Info($"[Cultivate] selection cancelled -> returned to hand (redirect={redirected}), refunded {refund} energy");
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

    private static bool TrySetResultPile(CardPlay play, PileType pile)
    {
        try
        {
            var field = typeof(CardPlay).GetField(
                "<ResultPile>k__BackingField",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            if (field is null)
            {
                Log.Warn("[Cultivate] ResultPile backing field not found");
                return false;
            }

            field.SetValue(play, pile);
            return true;
        }
        catch (Exception e)
        {
            Log.Warn($"[Cultivate] redirect ResultPile failed: {e.GetType().Name}");
            return false;
        }
    }
}