using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Viridis.Cards;

namespace Viridis.Powers;

/// <summary>Vine Entangle's temporary Strength loss.</summary>
public sealed class ViridisTempStrengthDownPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<VineEntangle>();

    protected override bool IsPositive => false;
}

/// <summary>Thorns' temporary Thorns, removed after the owner's next turn.</summary>
public sealed class ViridisTempThornsPower : TemporaryThornsPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Thorns>();
}
