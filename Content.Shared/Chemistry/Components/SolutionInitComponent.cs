using Content.Shared.Chemistry.Prototypes;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class SolutionInitComponent : Component
{
    [DataField, AutoNetworkedField]
    public Dictionary<string, SolutionData> Solutions = new();

    [DataRecord, Serializable, NetSerializable]
    public partial record SolutionData(
        List<ReagentQuantity> ReagentQuantities,
        List<ProtoId<ReactionVesselTypePrototype>>? ReactionVesselTypes = null,
        List<ProtoId<MixingCategoryPrototype>>? MixerTypes = null);
}
