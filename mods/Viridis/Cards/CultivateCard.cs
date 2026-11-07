using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Cultivate;

namespace Viridis.Cards;

/// <summary>小松绿卡牌基类：封装伤害 / 格挡 / 库数。</summary>
public abstract class CultivateCard : ModCardTemplate
{
    protected CultivateCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target)
    {
    }

    /// <summary>当前栽培牌库中的牌数。</summary>
    protected static int Pile(CardPlay play) => CultivateSystem.PileCount(play.Player);

    protected async Task Hit(PlayerChoiceContext ctx, CardPlay play, Creature target, decimal amount)
    {
        await CreatureCmd.Damage(ctx, target, amount, DamageProps.card, this, play);
    }

    protected async Task HitAll(PlayerChoiceContext ctx, CardPlay play, decimal amount)
    {
        var enemies = play.Player.Creature.CombatState?.HittableEnemies;
        if (enemies is null)
            return;

        foreach (var e in enemies.ToList())
            await Hit(ctx, play, e, amount);
    }

    protected async Task Block(PlayerChoiceContext ctx, CardPlay play, decimal amount)
    {
        await CreatureCmd.GainBlock(play.Player.Creature, amount, BlockProps.card, play);
    }
}