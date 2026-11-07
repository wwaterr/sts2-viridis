using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Cultivate;
using Viridis.Pools;

namespace Viridis.Cards;

// ---------- 先古牌 ----------

/// <summary>
/// 小松绿：永久栽培。获得 3（5）格挡，随机造成 3（5）伤害，然后这两个数字翻倍。
/// 在栽培区时每个回合结束再触发一次。
/// </summary>
[RegisterCard(typeof(ViridisCardPool))]
public sealed class SmallPine : CultivateCard
{
    public SmallPine() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.RandomEnemy) { }

    public override bool GainsBlock => true;

    public override string Title => "小松绿";

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: $"{ModEntry.ResPath}/images/cards/small_pine.ctex");

    protected override bool ShowsCultivateKeyword => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { PermanentCultivateKeyword.Value };

    public override bool CultivatesPermanently => true;

    public override decimal PermanentBlockValue => DynamicVars.Block.BaseValue;

    public override decimal PermanentDamageValue => DynamicVars.Damage.BaseValue;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new BlockVar(3m, ValueProp.Move),
            new DamageVar(3m, ValueProp.Move),
        };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
        => await CultivateSystem.CultivatePermanent(
            ctx,
            play.Player,
            this,
            DynamicVars.Block.BaseValue,
            DynamicVars.Damage.BaseValue,
            triggerNow: true);

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
