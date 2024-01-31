using Content.Shared.Chemistry.Reaction;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chemistry.Metabolism.Components;


//TODO: Rename to Metabolizer when there is no longer a name conflict!
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MetabolismComponent : Component
{
    /// <summary>
    /// The metabolic reactions that this component performs in order of priority. Reactions closer to the beginning of the list
    /// have priority!
    /// </summary>
    [DataField(required:true), AutoNetworkedField]
    public List<MetabolicReactionData> Reactions = new();

    /// <summary>
    /// The efficiency of the metabolic reactions
    /// </summary>
    [DataField, AutoNetworkedField]
    public FixedPoint2 Efficiency = 1.0f;

    [AutoNetworkedField, ViewVariables(VVAccess.ReadOnly)]
    public EntityUid SourceSolutionEntity;

    [DataField, AutoNetworkedField]
    public EntityUid? OutputSolutionEntity;
}

[DataRecord]
public record struct MetabolicReactionData(FixedPoint2 Multiplier, ProtoId<ReactionPrototype> Reaction);
