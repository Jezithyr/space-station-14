using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared.Medical.Organs.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HeartComponent : Component
{
    [DataField("pumpingPressure"), AutoNetworkedField]
    public FixedPoint2 OptimalPumpingPressure = 40;
}
