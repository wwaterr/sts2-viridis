using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Viridis.Cards;

/// <summary>林黛绿：将两张厕纸（升级后为厕纸+）放入抽牌堆；自身获得 2 层虚弱。</summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class LinDaiGreen : CultivateCard
{
    public LinDaiGreen() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<PaperTowel>(),
        HoverTipFactory.FromPower<WeakPower>(2),
    };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var state = play.Player.Creature.CombatState;
        if (state is not null)
        {
            var towels = new List<MegaCrit.Sts2.Core.Models.CardModel>();
            for (int i = 0; i < 2; i++)
            {
                var towel = state.CreateCard<PaperTowel>(play.Player);
                if (IsUpgraded)
                    CardCmd.Upgrade(towel, CardPreviewStyle.None);
                towels.Add(towel);
            }

            await CardPileCmd.AddGeneratedCardsToCombat(towels, PileType.Draw, play.Player, CardPilePosition.Top);
        }

        await Apply<WeakPower>(ctx, play, play.Player.Creature, 2m);
    }
}
