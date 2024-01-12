using Content.Shared.Chemistry.Metabolism.Prototypes;
using Content.Shared.Chemistry.Reagent;
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
    [DataField(required:true)]
    public List<MetabolicReactionData> Reactions = new();


    /// <summary>
    /// How much volume can this metabolic component process in one update
    /// -1 is unlimited!
    /// </summary>
    [DataField]
    public FixedPoint2 MaxReagentProcessingVolume = -1;

    public float AccumulatedFrameTime = 0f;

}

[DataRecord]
public record struct MetabolicReactionData(FixedPoint2 Multiplier, ProtoId<MetabolicReactionPrototype> Reaction, List<ReagentEffect> Effects);
