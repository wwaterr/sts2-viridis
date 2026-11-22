using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Viridis.Cards;

namespace Viridis.Powers;

/// <summary>可爱动静：打出娘笑/娘摇/娘养时抽 1 张牌。</summary>
public sealed class CuteCommotionPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override LocString Title => new("powers", "VIRIDIS_POWER_CUTE_COMMOTION_POWER.title");

    public override LocString Description => new("powers", "VIRIDIS_POWER_CUTE_COMMOTION_POWER.description");

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Player is null || cardPlay.Card.Owner != Owner.Player)
            return;

        if (cardPlay.Card is NiangSmile or NiangWave or NiangCare)
        {
            for (var i = 0; i < Amount; i++)
                await CardPileCmd.Draw(choiceContext, 1m, Owner.Player);
        }
    }
}
