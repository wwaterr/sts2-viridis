using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Enchantments;
using Viridis.Hype;
using Viridis.Pools;
using Viridis.Powers;

namespace Viridis.Cards;

/// <summary>钢镚友好直播间：本回合每打出一张牌，获得 2 金币；消耗，升级后不再消耗。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class CoinStream : CultivateCard
{
    public CoinStream() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<CoinStreamPower>(ctx, play, play.Player.Creature, 1m);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}

/// <summary>“最后一首”：将一张歌回124加入手牌，并为其附上“黏住”；消耗，升级后获得保留。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class LastSong : CultivateCard
{
    public LastSong() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        HoverTipFactory.FromEnchantment<Sticky>(100)
            .Concat(new[] { HoverTipFactory.FromCard<HeavenlyMelody>() });

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var state = play.Player.Creature.CombatState;
        if (state is null)
            return;

        var melody = state.CreateCard<HeavenlyMelody>(play.Player);
        await CardPileCmd.AddGeneratedCardToCombat(melody, PileType.Hand, play.Player);
        CardCmd.Enchant<Sticky>(melody, 100m);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}

/// <summary>歌回124：所有敌人失去 1 点力量；获得能量并抽 1 张牌；消耗。</summary>
[RegisterCard(typeof(TokenCardPool))]
public sealed class HeavenlyMelody : CultivateCard
{
    public HeavenlyMelody() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new IntVar("Viewer", 2m),
            new PowerVar<KaoziPower>(2m),
            new EnergyVar(1)
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        HypeSystem.Add(play.Player, (int)DynamicVars["Viewer"].BaseValue);
        await Apply<KaoziPower>(ctx, play, play.Player.Creature, DynamicVars["KaoziPower"].BaseValue);
        await Energy(ctx, play, (int)DynamicVars.Energy.BaseValue);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Viewer"].UpgradeValueBy(1m);
        DynamicVars["KaoziPower"].UpgradeValueBy(1m);
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
