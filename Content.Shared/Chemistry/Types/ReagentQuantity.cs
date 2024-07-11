using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Types;

[Serializable, NetSerializable]
[DataDefinition]
public partial struct ReagentQuantity
{
    /// <summary>
    /// EntityProtoId of the reagent
    /// (Or ProtoId in the case of legacy reagents)
    /// </summary>
    [DataField(required: true)]
    public string ReagentId { get; private set; }

    /// <summary>
    /// Quantity of the reagent *IN MOLS*
    /// </summary>
    [DataField(required: true)]
    public FixedPoint4 Quantity { get; private set; }

    [DataField]
    public EntityUid ReagentEntId = EntityUid.Invalid;

}
