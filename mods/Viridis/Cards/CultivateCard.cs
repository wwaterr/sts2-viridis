using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
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
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: $"{ModEntry.ResPath}/images/cards/{System.Text.Json.JsonNamingPolicy.SnakeCaseLower.ConvertName(GetType().Name)}.ctex");

    protected CultivateCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target)
    {
    }

    /// <summary>卡面是否显示“栽培”关键词；只给真正执行栽培的牌开启。</summary>
    protected virtual bool ShowsCultivateKeyword => false;

    /// <summary>卡面是否显示“生长”关键词；只给描述里带生长效果的牌开启。</summary>
    protected virtual bool ShowsGrowKeyword => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var keywords = base.CanonicalKeywords.ToList();
            if (ShowsCultivateKeyword)
                keywords.Add(CultivateKeyword.Value);
            if (ShowsGrowKeyword)
                keywords.Add(GrowKeyword.Value);
            return keywords;
        }
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        (ShowsCultivateKeyword, ShowsGrowKeyword) switch
        {
            (true, _) => [CultivateTips.Harvest],
            (_, true) => [CultivateTips.Cultivate],
            _ => base.AdditionalHoverTips
        };

    /// <summary>这张牌被栽培时触发；默认无效果。</summary>
    public virtual Task OnCultivated(PlayerChoiceContext ctx, Player player) => Task.CompletedTask;

    /// <summary>这张牌是否走“永久栽培”路径（不进入收获倒计时）。</summary>
    public virtual bool CultivatesPermanently => false;

    /// <summary>永久栽培时首次触发使用的格挡值。</summary>
    public virtual decimal PermanentBlockValue => 0m;

    /// <summary>永久栽培时首次触发使用的伤害值。</summary>
    public virtual decimal PermanentDamageValue => 0m;

    /// <summary>当前栽培区中的牌数。</summary>
    protected static int Pile(CardPlay play) => CultivateSystem.PileCount(play.Player);

    protected async Task Hit(PlayerChoiceContext ctx, CardPlay play, Creature target, decimal amount, int hits = 1)
    {
        await DamageCmd.Attack(amount).WithHitCount(hits)
            .FromCard(this, play).Targeting(target).Execute(ctx);
    }

    protected async Task HitAll(PlayerChoiceContext ctx, CardPlay play, decimal amount, int hits = 1)
    {
        if (CombatState is not { } combat)
            return;

        await DamageCmd.Attack(amount).WithHitCount(hits)
            .FromCard(this, play).TargetingAllOpponents(combat).Execute(ctx);
    }

    protected async Task Block(PlayerChoiceContext ctx, CardPlay play, decimal amount)
    {
        await CreatureCmd.GainBlock(play.Player.Creature, amount, BlockProps.card, play);
    }

    /// <summary>给目标施加能力（易伤 / 虚弱 / 力量等）。</summary>
    protected async Task Apply<T>(PlayerChoiceContext ctx, CardPlay play, Creature target, decimal amount)
        where T : PowerModel
    {
        await PowerCmd.Apply<T>(ctx, target, amount, play.Player.Creature, this);
    }

    /// <summary>给所有敌人施加能力。</summary>
    protected async Task ApplyAll<T>(PlayerChoiceContext ctx, CardPlay play, decimal amount)
        where T : PowerModel
    {
        var enemies = play.Player.Creature.CombatState?.HittableEnemies;
        if (enemies is null)
            return;

        foreach (var e in enemies.ToList())
            await Apply<T>(ctx, play, e, amount);
    }

    protected async Task Energy(PlayerChoiceContext ctx, CardPlay play, int amount)
        => await PlayerCmd.GainEnergy(amount, play.Player);
}
