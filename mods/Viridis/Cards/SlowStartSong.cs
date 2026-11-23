using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Pools;
using Viridis.Powers;

namespace Viridis.Cards;

/// <summary>
/// 慢启动歌回：将 10 张感染加入弃牌堆；每个回合开始把若干状态牌变化为歌回124，
/// 变化数量从 1 开始逐回合 +1。升级后获得保留。
/// </summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class SlowStartSong : CultivateCard
{
    public SlowStartSong() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var state = play.Player.Creature.CombatState;
        if (state is null)
            return;

        var infections = new List<CardModel>();
        for (var i = 0; i < 10; i++)
            infections.Add(state.CreateCard<Infection>(play.Player));

        await CardPileCmd.AddGeneratedCardsToCombat(infections, PileType.Discard, play.Player);
        await Apply<SlowStartSongPower>(ctx, play, play.Player.Creature, 1m);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
