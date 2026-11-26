using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Pools;

namespace Viridis.Cards;

/// <summary>小众品味：消耗手牌中的普通/基础等牌，换取煎蛋与黄鼠狼。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class NicheTaste : PlaceholderArtCard
{
    public NicheTaste() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromCard<FriedEgg>(),
        HoverTipFactory.FromCard<Weasel>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var hand = play.Player.PlayerCombatState?.Hand;
        if (hand is not null)
        {
            var targets = hand.Cards
                .Where(c => c.Rarity is not (CardRarity.Uncommon or CardRarity.Rare or CardRarity.Ancient))
                .ToList();

            foreach (var card in targets)
                await CardCmd.Exhaust(ctx, card);
        }

        var combat = play.Player.Creature.CombatState;
        if (combat is null)
            return;

        var cards = new List<CardModel>();

        var egg = combat.CreateCard<FriedEgg>(play.Player);
        if (IsUpgraded)
            CardCmd.Upgrade(egg, CardPreviewStyle.None);
        cards.Add(egg);

        var weasel = combat.CreateCard<Weasel>(play.Player);
        if (IsUpgraded)
            CardCmd.Upgrade(weasel, CardPreviewStyle.None);
        cards.Add(weasel);

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, play.Player);
    }
}

/// <summary>黄鼠狼！：本回合获得敏捷，保留并消耗。</summary>
[RegisterCard(typeof(TokenCardPool))]
public sealed class Weasel : PlaceholderArtCard
{
    public Weasel() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Retain, CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<TempDexterityPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TempDexterityPower>(5m)];

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => Apply<TempDexterityPower>(ctx, play, play.Player.Creature, DynamicVars["TempDexterityPower"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["TempDexterityPower"].UpgradeValueBy(2m);
}
