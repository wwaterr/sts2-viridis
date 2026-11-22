using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using Viridis.Characters;
using Viridis.Cultivate;
using Viridis.Pools;

namespace Viridis.Cards;

[RegisterDustyTomeCard(typeof(ViridisCharacter))]
[RegisterCard(typeof(ViridisCardPool))]
public sealed class GlasshouseCanopy : PlaceholderArtCard
{
    public GlasshouseCanopy() : base(3, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new BlockVar(12m, ValueProp.Move) };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.GainBlock(play.Player.Creature, DynamicVars.Block, play);
        await CultivateSystem.Grow(ctx, play.Player, 2);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}
