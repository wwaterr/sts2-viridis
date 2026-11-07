using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Cultivate;
using Viridis.Pools;

namespace Viridis.Cards;

// ---------- 先古牌 ----------

[RegisterCard(typeof(ViridisCardPool))]
public sealed class WorldTree : CultivateCard
{
    public WorldTree() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self) { }

    protected override bool ShowsCultivateKeyword => true;
    public override string Title => "世界树";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<WorldTreePower>(ctx, play, play.Player.Creature, 1m);
}

[RegisterCard(typeof(ViridisCardPool))]
public sealed class PrimordialSpore : CultivateCard
{
    public PrimordialSpore() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self) { }
    public override string Title => "始祖孢子";

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await Apply<PrimordialSporePower>(ctx, play, play.Player.Creature, 1m);
}