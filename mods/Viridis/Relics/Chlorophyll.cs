using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Viridis.Characters;
using Viridis.Hype;
using Viridis.Pools;

namespace Viridis.Relics;

/// <summary>
/// 枼绿素：初始计数遗物，用于跨战斗传递入脑/反踩层数。
/// 正数为入脑，负数为反踩；每场战斗结束覆盖计数，每场战斗开始赋予对应层数（第一回合即可触发）。
/// </summary>
[RegisterRelic(typeof(ViridisRelicPool))]
[RegisterCharacterStarterRelic(typeof(ViridisCharacter))]
public sealed class Chlorophyll : ModRelicTemplate
{
    private int _balance;

    [SavedProperty]
    public int Balance
    {
        get => _balance;
        set
        {
            int clamped = Math.Clamp(value, -HypeSystem.MaxPolarityStacks, HypeSystem.MaxPolarityStacks);
            if (_balance == clamped)
                return;
            _balance = clamped;
            InvokeDisplayAmountChanged();
        }
    }

    public override LocString Title => new("relics", "VIRIDIS_RELIC_CHLOROPHYLL.title");

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override bool ShowCounter => true;

    public override int DisplayAmount => Balance;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{ModEntry.ResPath}/images/relics/chlorophyll.ctex",
        IconOutlinePath: $"{ModEntry.ResPath}/images/relics/chlorophyll_outline.ctex",
        BigIconPath: $"{ModEntry.ResPath}/images/relics/chlorophyll_big.ctex");

    public override async Task BeforeCombatStart()
    {
        if (Owner is null)
            return;

        HypeSystem.Ensure(Owner);

        // 跨局存档里取出上一场留下的余额，作为本场开局层数
        Balance = PolaritySave.Load();

        if (Balance > 0)
            await HypeSystem.GainEarworm(new BlockingPlayerChoiceContext(), Owner, Balance);
        else if (Balance < 0)
            await HypeSystem.GainBacklash(new BlockingPlayerChoiceContext(), Owner, -Balance);

        PolarityBarUI.Refresh(Owner);
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        if (Owner is null)
            return Task.CompletedTask;

        // 用本场剩余层数覆盖计数，并落盘到 Profile 存档，实现跨局传递
        Balance = HypeSystem.EarwormStacks(Owner) - HypeSystem.BacklashStacks(Owner);
        PolaritySave.Store(Balance);
        return Task.CompletedTask;
    }
}
