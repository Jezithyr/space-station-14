using Robust.Shared.Prototypes;

namespace Content.Shared.Chemistry.Prototypes;

[Prototype]
public sealed class ReactionVesselTypePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;
}
