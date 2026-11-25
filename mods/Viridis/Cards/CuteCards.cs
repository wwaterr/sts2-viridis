using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using Viridis.Pools;
using Viridis.Powers;

namespace Viridis.Cards;

/// <summary>娘养：获得考子。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class NiangCare : PlaceholderArtCard, IHypeCard
{
    public NiangCare() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<KaoziPower>(4m) };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { KaoziKeyword.Value };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<KaoziPower>(ctx, play, play.Player.Creature, DynamicVars["KaoziPower"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars["KaoziPower"].UpgradeValueBy(2m);
}

/// <summary>娘摇：获得格挡与考子。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class NiangWave : PlaceholderArtCard, IHypeCard
{
    public NiangWave() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new BlockVar(5m, ValueProp.Move),
            new PowerVar<KaoziPower>(1m),
        };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { KaoziKeyword.Value };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Block(ctx, play, DynamicVars.Block.BaseValue);
        await Apply<KaoziPower>(ctx, play, play.Player.Creature, DynamicVars["KaoziPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["KaoziPower"].UpgradeValueBy(1m);
    }
}

/// <summary>娘笑：造成伤害并获得考子。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class NiangSmile : PlaceholderArtCard, IHypeCard
{
    public NiangSmile() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(6m, ValueProp.Move),
            new PowerVar<KaoziPower>(1m),
        };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { KaoziKeyword.Value };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        await Apply<KaoziPower>(ctx, play, play.Player.Creature, DynamicVars["KaoziPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["KaoziPower"].UpgradeValueBy(1m);
    }
}

/// <summary>可爱动静：将三张娘卡加入手牌，并让它们被打出时抽牌。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class CuteCommotion : PlaceholderArtCard, IHypeCard
{
    public CuteCommotion() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromCard<NiangSmile>(),
        HoverTipFactory.FromCard<NiangWave>(),
        HoverTipFactory.FromCard<NiangCare>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var state = play.Player.Creature.CombatState;
        if (state is null)
            return;

        var cards = new List<CardModel>
        {
            state.CreateCard<NiangSmile>(play.Player),
            state.CreateCard<NiangWave>(play.Player),
            state.CreateCard<NiangCare>(play.Player),
        };

        if (IsUpgraded)
        {
            foreach (var card in cards)
                CardCmd.Upgrade(card, CardPreviewStyle.None);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, play.Player);
        await Apply<CuteCommotionPower>(ctx, play, play.Player.Creature, 1m);
    }
}

/// <summary>互联网盆栽：用一组负面状态与再生换取强力续航。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class InternetBonsai : PlaceholderArtCard
{
    public InternetBonsai() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<FrailPower>(),
        HoverTipFactory.FromPower<RegenPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<RegenPower>(8m) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var player = play.Player.Creature;
        await Apply<VulnerablePower>(ctx, play, player, 99m);
        await Apply<WeakPower>(ctx, play, player, 99m);
        await Apply<FrailPower>(ctx, play, player, 99m);
        await Apply<RegenPower>(ctx, play, player, DynamicVars["RegenPower"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars["RegenPower"].UpgradeValueBy(2m);
}
