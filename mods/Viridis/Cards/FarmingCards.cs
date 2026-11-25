using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Cultivate;
using Viridis.Hype;
using Viridis.Pools;

namespace Viridis.Cards;

[RegisterCard(typeof(ViridisCardPool))]
public sealed class FarmerForm : CultivateCard
{
    public FarmerForm()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<FarmerFormPower>(),
    ];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: $"{ModEntry.ResPath}/images/cards/farmer_form_card.ctex");

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<FarmerFormPower>(ctx, play, play.Player.Creature, 1m);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class ScaleInsect : CultivateCard
{
    public ScaleInsect()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Move),
        new PowerVar<VulnerablePower>(2m)
    };

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(WitherKeyword.Value)
    };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CultivateSystem.Wither(play.Player))
            await Apply<VulnerablePower>(ctx, play, play.Target!, DynamicVars["VulnerablePower"].BaseValue);

        await Hit(ctx, play, play.Target!, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class CultivationMastery : CultivateCard
{
    public CultivationMastery()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: $"{ModEntry.ResPath}/images/cards/cultivation_mastery.ctex");

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromKeyword(WitherKeyword.Value)
    };

    protected override CardLocation GetResultLocationForCardPlay()
        => new(Owner, PileType.Exhaust, CardPilePosition.Bottom);

    public override int MaxUpgradeLevel => 999999999;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(7m, ValueProp.Move)
    };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var damage = DynamicVars.Damage.BaseValue;
        var planting = await CultivateSystem.SelectPlanting(ctx, play.Player, "VIRIDIS_CARD_SELECT_CULTIVATE_PROMPT");
        if (planting is not null && CultivateSystem.Wither(play.Player, planting))
        {
            var upgrades = planting.SourceRarity == CardRarity.Ancient ? 2 : 1;
            for (var i = 0; i < upgrades; i++)
                UpgradeSelf();
        }

        await Hit(ctx, play, play.Target!, damage);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m + CurrentUpgradeLevel);
    }

    private void UpgradeSelf()
    {
        var deck = DeckVersion;
        if (deck is not null && deck.IsUpgradable)
            CardCmd.Upgrade(deck, CardPreviewStyle.None);

        if (!ReferenceEquals(deck, this) && IsUpgradable)
            CardCmd.Upgrade(this, CardPreviewStyle.None);
    }
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class KaoziSlayer : CultivateCard, IHypeCard
{
    public KaoziSlayer()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
        KaoziKeyword.Value
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(20m, ValueProp.Move)
    };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var kaozi = play.Player.Creature.GetPower<KaoziPower>();
        await PowerCmd.Remove(kaozi);

        if (CombatState is not { } combat || combat.HittableEnemies.Count == 0)
            return;

        var targets = combat.HittableEnemies.ToList();
        var results = new List<DamageResult>();
        await using var attack = await AttackContext.CreateAsync(combat, ctx, play);
        await CreatureCmd.TriggerAnim(play.Player.Creature, "Attack", play.Player.Character.AttackAnimDelay);
        foreach (var enemy in targets)
        {
            var triggersKaoziBonus = enemy.Monster?.IntendsToAttack != true;
            var damage = DynamicVars.Damage.BaseValue * (triggersKaoziBonus ? 3m : 1m);
            results.AddRange(await CreatureCmd.Damage(ctx, enemy, damage, DamageProps.card, this, play));

            if (triggersKaoziBonus && enemy.IsAlive)
                await CreatureCmd.Stun(enemy);
        }
        attack.AddHit(results);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}
