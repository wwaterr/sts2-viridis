using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Enchantments;

/// <summary>
/// 黏住：被附魔的牌打出后按当前概率回到手牌；每次打出后概率降低 15%。
/// 初始概率 100%，即第一次一定回到手牌。
/// </summary>
[RegisterEnchantment]
public sealed class Sticky : ModEnchantmentTemplate
{
    public override EnchantmentAssetProfile AssetProfile =>
        new EnchantmentAssetProfile("res://images/enchantments/goopy.png");

    public override bool ShowAmount => true;

    public override bool HasExtraCardText => true;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != Card || !HasCard)
            return;

        if (Amount > 0 && Card.Owner.RunState.Rng.CombatCardSelection.NextFloat(100f) < Amount)
        {
            await CardPileCmd.Add(Card, PileType.Hand);
        }

        Amount = Math.Max(0, Amount - 15);
    }
}
