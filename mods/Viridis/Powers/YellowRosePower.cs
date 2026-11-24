using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace Viridis.Powers;

/// <summary>黄刺玫：每打出一张攻击牌，获得 1（2）点荆棘。</summary>
public sealed class YellowRosePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override LocString Title => new("powers", "VIRIDIS_POWER_YELLOW_ROSE_POWER.title");

    public override LocString Description => new("powers", "VIRIDIS_POWER_YELLOW_ROSE_POWER.description");

    public override PowerAssetProfile AssetProfile =>
        new("res://Viridis/images/powers/yellow_rose.ctex", "res://Viridis/images/powers/yellow_rose.ctex");

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || cardPlay.Card.Type != CardType.Attack)
        {
            return;
        }

        await PowerCmd.Apply<ThornsPower>(choiceContext, Owner, Amount, Owner, cardPlay.Card);
    }
}
