using MegaCrit.Sts2.Core.Logging;
using STS2RitsuLib;
using STS2RitsuLib.Utils.Persistence;

namespace Viridis.Hype;

/// <summary>跨局存档：只存一个有符号的极性余额（正=入脑，负=反踩）。</summary>
public sealed class PolarityRecord
{
    public int Balance { get; set; }
}

/// <summary>
/// 枼绿素的计数落盘处。用 Profile 作用域，让它在不同局之间保留。
/// </summary>
public static class PolaritySave
{
    private const string StoreKey = "polarity";
    private static bool _registered;

    public static void EnsureRegistered()
    {
        if (_registered)
            return;

        try
        {
            using (RitsuLibFramework.BeginModDataRegistration(ModEntry.ModId))
            {
                var store = RitsuLibFramework.GetDataStore(ModEntry.ModId);
                store.Register(
                    key: StoreKey,
                    fileName: "polarity.json",
                    scope: SaveScope.Profile,
                    defaultFactory: () => new PolarityRecord(),
                    autoCreateIfMissing: true);
            }

            _registered = true;
        }
        catch (Exception e)
        {
            Log.Error($"[Viridis] PolaritySave registration failed: {e}");
        }
    }

    public static int Load()
    {
        EnsureRegistered();
        try
        {
            var store = RitsuLibFramework.GetDataStore(ModEntry.ModId);
            var record = store.Get<PolarityRecord>(StoreKey);
            return Math.Clamp(record?.Balance ?? 0, -HypeSystem.MaxPolarityStacks, HypeSystem.MaxPolarityStacks);
        }
        catch (Exception e)
        {
            Log.Error($"[Viridis] PolaritySave.Load failed: {e}");
            return 0;
        }
    }

    public static void Store(int balance)
    {
        EnsureRegistered();
        try
        {
            var store = RitsuLibFramework.GetDataStore(ModEntry.ModId);
            store.Modify<PolarityRecord>(StoreKey, data =>
                data.Balance = Math.Clamp(balance, -HypeSystem.MaxPolarityStacks, HypeSystem.MaxPolarityStacks));
            store.Save(StoreKey);
        }
        catch (Exception e)
        {
            Log.Error($"[Viridis] PolaritySave.Store failed: {e}");
        }
    }
}
