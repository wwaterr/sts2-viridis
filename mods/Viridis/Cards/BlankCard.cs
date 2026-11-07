using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Characters;
using Viridis.Pools;

namespace Viridis.Cards;

/// <summary>
/// 空白卡：1 费技能，不做任何事。
/// 初始牌组由 [RegisterCharacterStarterCard(..., 10)] 直接给 10 张。
/// 本地化键：VIRIDIS_CARD_BLANK_CARD.title / .description
/// </summary>
[RegisterCard(typeof(ViridisCardPool))]
[RegisterCharacterStarterCard(typeof(ViridisCharacter), 10)]
public sealed class BlankCard : ModCardTemplate
{
    public BlankCard()
        : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        => Task.CompletedTask;
}