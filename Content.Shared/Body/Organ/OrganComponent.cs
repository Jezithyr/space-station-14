using Content.Shared.Body.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Medical.Organs.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;

namespace Content.Shared.Body.Organ;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedBodySystem), typeof(OrganUpdateSystem))]
public sealed partial class OrganComponent : Component
{
    /// <summary>
    /// Relevant body this organ is attached to.
    /// </summary>
    [DataField("body"), AutoNetworkedField]
    public EntityUid? Body;

    //does this organ run tick updates?
    [DataField("canTick"), AutoNetworkedField]
    public bool CanTick = true;

    /// <summary>
    /// Organ Efficency
    /// </summary>
    [DataField("efficiency"), AutoNetworkedField]
    public FixedPoint2 Efficiency = 1.00;

    [DataField("necrosis"), AutoNetworkedField]
    public FixedPoint2 Necrosis = 0.0;

    /// <summary>
    /// Decay rate per organ tick when this organ is undergoing necrosis
    /// </summary>
    [DataField("decayRate"), AutoNetworkedField]
    public FixedPoint2 DecayRate = 0.0;

    public float AccumulatedFrameTime;
}
