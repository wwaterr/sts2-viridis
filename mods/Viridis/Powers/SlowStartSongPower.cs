using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Viridis.Cards;

namespace Viridis.Powers;

/// <summary>
/// 慢启动歌回的能力：每个己方回合开始，把 Amount 张状态牌变化为天籁（优先手牌），
/// 然后 Amount +1。第一次触发时 Amount 为 1。
/// </summary>
public sealed class SlowStartSongPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        var statuses = new List<CardModel>();
        CollectStatuses(PileType.Hand, player, statuses);
        CollectStatuses(PileType.Draw, player, statuses);
        CollectStatuses(PileType.Discard, player, statuses);
        CollectStatuses(PileType.Exhaust, player, statuses);

        if (statuses.Count > 0)
        {
            var count = Math.Min(Amount, statuses.Count);
            var transformations = new List<CardTransformation>();
            foreach (var original in statuses.Take(count))
            {
                var replacement = CombatState.CreateCard<HeavenlyMelody>(player);
                transformations.Add(new CardTransformation(original, replacement));
            }

            await CardCmd.Transform(transformations, null);
            Flash();
        }

        SetAmount(Amount + 1);
    }

    private static void CollectStatuses(PileType pileType, Player player, List<CardModel> output)
    {
        foreach (var card in pileType.GetPile(player).Cards)
        {
            if (card.Type == CardType.Status && card.IsTransformable)
                output.Add(card);
        }
    }
}
