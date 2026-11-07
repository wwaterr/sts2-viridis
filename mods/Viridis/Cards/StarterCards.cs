using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Cultivate;
using Viridis.Characters;

namespace Viridis.Cards;

/// <summary>打击：1 费 6（9）伤害。</summary>
[RegisterCard(typeof(ViridisCardPool))]
[RegisterCharacterStarterCard(typeof(ViridisCharacter), 4)]
public sealed class PineNeedle : CultivateCard
{
    public PineNeedle() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    public override string Title => "打击";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(6m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>防御：1 费 5（8）格挡。</summary>
[RegisterCard(typeof(ViridisCardPool))]
[RegisterCharacterStarterCard(typeof(ViridisCharacter), 4)]
public sealed class RootGuard : CultivateCard
{
    public RootGuard() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    public override bool GainsBlock => true;

    public override string Title => "防御";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new BlockVar(5m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Block(ctx, play, DynamicVars.Block.BaseValue);

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

/// <summary>种植：1（0）费，栽培 1 张手牌并抽 1 张牌。</summary>
[RegisterCard(typeof(ViridisCardPool))]
[RegisterCharacterStarterCard(typeof(ViridisCharacter), 1)]
public sealed class Plant : CultivateCard
{
    public Plant() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;

    public override string Title => "种植";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CultivateSystem.CultivateFromHand(ctx, play.Player, this);
        await CardPileCmd.Draw(ctx, 1m, play.Player);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

/// <summary>微小种子：1 费，获得 3（5）格挡并随机造成 3（5）伤害；被栽培时也触发。</summary>
[RegisterCard(typeof(ViridisCardPool))]
[RegisterCharacterStarterCard(typeof(ViridisCharacter), 1)]
public sealed class TinySeed : CultivateCard
{
    public TinySeed() : base(1, CardType.Attack, CardRarity.Basic, TargetType.RandomEnemy) { }

    public override bool GainsBlock => true;

    public override string Title => "微小种子";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new BlockVar(3m, ValueProp.Move),
            new DamageVar(3m, ValueProp.Move),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Trigger(ctx, play.Player, play);

    public override async Task OnCultivated(PlayerChoiceContext ctx, Player player)
        => await Trigger(ctx, player, null);

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.Damage.UpgradeValueBy(2m);
    }

    private async Task Trigger(PlayerChoiceContext ctx, Player player, CardPlay? play)
    {
        await CreatureCmd.GainBlock(player.Creature, DynamicVars.Block.BaseValue, BlockProps.card, play);

        var state = player.Creature.CombatState;
        if (state is null)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingRandomOpponents(state)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }
}
