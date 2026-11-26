using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using Viridis.Pools;

namespace Viridis.Cards;

/// <summary>起承转考：造成伤害，获得同接与考子。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class GoLive : CultivateCard, IHypeCard
{
    public GoLive() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(8m, ValueProp.Move),
            new IntVar("ViewerGain", 3m),
        };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { ViewerCountKeyword.Value, KaoziKeyword.Value };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);
        HypeSystem.Add(play.Player, (int)DynamicVars["ViewerGain"].BaseValue);
        await Apply<KaoziPower>(ctx, play, play.Player.Creature, 1m);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["ViewerGain"].UpgradeValueBy(1m);
    }
}

/// <summary>xsl开播了兄弟们撤：本回合已打出过牌则无法打出；高伤害并给同接/考子。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class SurpriseStream : CultivateCard, IHypeCard
{
    public SurpriseStream() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(8m, ValueProp.Move) };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { ViewerCountKeyword.Value };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is null)
            return;

        var state = play.Player.Creature.CombatState;
        if (state is null)
            return;

        var history = CombatManager.Instance.History;
        var dealtDamageBefore = history.Entries
            .OfType<DamageReceivedEntry>()
            .Any(e => e.HappenedThisTurn(state) && e.Dealer == play.Player.Creature);

        await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue, hits: dealtDamageBefore ? 1 : 2);

        var damageDealt = history.Entries
            .OfType<DamageReceivedEntry>()
            .Where(e => e.HappenedThisTurn(state) && e.Dealer == play.Player.Creature && e.CardSource == this)
            .Sum(e => e.Result.UnblockedDamage);

        if (damageDealt > 0)
            HypeSystem.Add(play.Player, damageDealt);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>倒下：本回合已打出过牌则无法打出；给反踩/串子，回复 30% 最大生命并结束回合。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class SleepStream : CultivateCard, IHypeCard
{
    public SleepStream() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust, AntiStompKeyword.Value };

    protected override bool IsPlayable => !HasPlayedCardThisTurn;

    protected override bool ShouldGlowRedInternal => !IsPlayable;

    private bool HasPlayedCardThisTurn =>
        CombatState is { } state
        && CombatManager.Instance.History.CardPlaysStarted.Any(e =>
            e.HappenedThisTurn(state) && e.CardPlay.Player == Owner);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<AntiStompPower>(ctx, play, play.Player.Creature, 3m);
        await CreatureCmd.Heal(play.Player.Creature, Math.Floor(play.Player.Creature.MaxHp * 0.3m));
        PlayerCmd.EndTurn(play.Player, canBackOut: false);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
