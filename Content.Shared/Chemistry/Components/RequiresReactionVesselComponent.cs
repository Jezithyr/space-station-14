using Content.Shared.Chemistry.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
public sealed partial class RequiresReactionVesselComponent : Component
{
    /// <summary>
    ///     The required reaction vessel type for this reaction to occur
    /// </summary>
    [DataField(required: true)]
    public List<ProtoId<ReactionVesselTypePrototype>> ReactionVesselType = default!;
}
