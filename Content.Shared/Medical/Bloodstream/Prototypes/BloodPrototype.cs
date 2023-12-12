using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.TypeParsers;

namespace Content.Shared.Medical.Bloodstream.Prototypes;

[Prototype("blood")]
public sealed partial class BloodPrototype : IPrototype
{
    [IdDataField] public string ID { get; init; } = string.Empty;

    [DataField("bloodPlasma", required: true)]
    public Prototype<ReagentPrototype> BloodPlasmaType;

    [DataField("allowedPlasmaTypes", required: true)]
    public AntiBodyType AllowedPlasmaTypes;

    [DataField("bloodCells", required: true)]
    public Prototype<ReagentPrototype> BloodCellsType;

    [DataField("allowedCellTypes", required: true)]
    public AntiBodyType AllowedCellTypes;
}
