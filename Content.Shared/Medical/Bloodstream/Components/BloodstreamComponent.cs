using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Medical.Bloodstream.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Toolshed.TypeParsers;

namespace Content.Shared.Medical.Bloodstream.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BloodstreamNewComponent : Component
{
    //BloodVessel volume used to calculate blood pressure
    [DataField("totalVolume", required:true), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public FixedPoint2 TotalVolume = 1000;

    [DataField("bloodType", required: true)]
    public Prototype<BloodPrototype> BloodType;

    [DataField("randomizeBloodType")]
    public bool RandomizeBloodType = true;

    //The antigen types of the plasma and cell solutions respectively.
    //Both of these combined determine the bloodtype if this species has bloodtypes.
    [DataField("plasmaType"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public AntiBodyType PlasmaType;
    [DataField("cellType"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public AntiBodyType CellType;

    //The actual blood solution, this will contain plasma, cells and any chemicals/meds that are dissolved
    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public Solution BloodSolution = new();

    //Not sure if networking these would cause issues.

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public FixedPoint2 BleedRate = 0;
}
