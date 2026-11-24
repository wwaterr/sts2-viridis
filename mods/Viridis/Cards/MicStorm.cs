using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Hype;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

/// <summary>麦克风暴：对所有敌人造成 9（12）点伤害 2 次；手牌中每有一张状态牌，额外攻击一次并入脑 2。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class MicStorm : CultivateCard
{
    public MicStorm() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(EarwormKeyword.Value),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(9m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int statuses = PileType.Hand.GetPile(play.Player).Cards.Count(c => c.Type == CardType.Status);
        int hits = 2 + statuses;

        for (int i = 0; i < hits; i++)
            await HitAll(ctx, play, DynamicVars.Damage.BaseValue);

        if (statuses > 0)
            await HypeSystem.GainEarworm(ctx, play.Player, statuses * 2);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
