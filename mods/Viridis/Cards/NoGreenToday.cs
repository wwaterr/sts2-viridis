using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

/// <summary>今日无绿：造成伤害，将抽牌堆中的 1 张牌变为厕纸（升级后为厕纸+），然后结束你的回合。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class NoGreenToday : CultivateCard
{
    public NoGreenToday() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<PaperTowel>(),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DamageVar(9m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not null)
            await Hit(ctx, play, play.Target, DynamicVars.Damage.BaseValue);

        var state = play.Player.Creature.CombatState;
        var drawPile = PileType.Draw.GetPile(play.Player).Cards;
        if (state is not null && drawPile.Count > 0)
        {
            var replacement = state.CreateCard<PaperTowel>(play.Player);
            if (IsUpgraded)
                CardCmd.Upgrade(replacement, CardPreviewStyle.None);

            await CardCmd.Transform(drawPile[0], replacement, CardPreviewStyle.None);
        }

        PlayerCmd.EndTurn(play.Player, canBackOut: false);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
