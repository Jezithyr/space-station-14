using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Medical.Treatments.Prototypes;

[Prototype("treatment")]
public sealed class TreatmentPrototype : IPrototype
{
    [IdDataField] public string ID { get; init; } = string.Empty;

    [DataField("repeatable")] public bool Repeatable = false;

    [DataField("severityChange", required: true)]
    public FixedPoint2 SeverityDelta;

    [DataField("healingSpeedChange", required: true)]
    public FixedPoint2 HealingSpeedDelta;

    //TODO: raise treatment event

    [DataField("allowedWounds", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public HashSet<EntityPrototype> AllowedWounds = new();
}
