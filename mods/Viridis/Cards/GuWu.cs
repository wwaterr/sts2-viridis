using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Viridis.Cards;

/// <summary>固无：这张牌不会出现在每局开始的手牌中。</summary>
[RegisterOwnedCardKeyword("Deferred", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
public sealed class GuWuKeyword
{
    public static readonly CardKeyword Value =
        ModContentRegistry.GetQualifiedKeywordId(ModEntry.ModId, "Deferred").GetModCardKeyword();
}

/// <summary>固无的全局执行者：开手抽牌前把带固无的牌压到抽牌堆底。</summary>
public sealed class GuWuPower : PowerModel
{
    public override PowerType Type => PowerType.None;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (Owner.Player is null || player != Owner.Player)
            return;

        // 只在战斗的第一个回合生效
        if (combatState.RoundNumber > 1)
            return;

        try
        {
            await GuWuSystem.PushDeferredToBottom(player);
        }
        catch (Exception e)
        {
            Log.Error($"[Viridis] GuWu before hand draw failed: {e.Message}");
        }
    }
}

/// <summary>固无的公共入口。</summary>
public static class GuWuSystem
{
    /// <summary>确保玩家身上挂着固无执行者；重复调用无副作用。</summary>
    public static void Ensure(Player? player)
    {
        if (player?.Creature is null)
            return;

        if (player.Creature.GetPower<GuWuPower>() is not null)
            return;

        TaskHelper.RunSafely(ApplyAsync(player));
    }

    private static async Task ApplyAsync(Player player)
    {
        await PowerCmd.Apply<GuWuPower>(
            new BlockingPlayerChoiceContext(),
            player.Creature,
            1m,
            player.Creature,
            null,
            silent: true);
    }

    /// <summary>把抽牌堆里带「固无」的牌移到最底下，使它进不了起手。</summary>
    public static async Task PushDeferredToBottom(Player player)
    {
        var pile = PileType.Draw.GetPile(player);
        var deferred = pile.Cards
            .Where(card => card.Keywords.Contains(GuWuKeyword.Value))
            .ToList();

        foreach (var card in deferred)
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Bottom, null, skipVisuals: true);
    }
}
