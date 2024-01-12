using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Metabolism.Prototypes;

[Prototype("metabolicReaction")]
public sealed partial class MetabolicReactionPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = "";

    [DataField]
    public bool AllowPartialReactions = false;

    [DataField(required: true)]
    private LocId Name { get; set; }

    [ViewVariables(VVAccess.ReadOnly)]
    public string LocalizedName => Loc.GetString(Name);

    [DataField(required:true)]
    public Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> Reactants = new();

    [DataField(required:true)]
    public Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> Catalysts = new();

    [DataField(required:true)]
    public Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> Products = new();

    public FixedPoint2 ReactantVolume = FixedPoint2.Zero;
    public FixedPoint2 ProductVolume = FixedPoint2.Zero;

    public MetabolicReactionPrototype()
    {
        foreach (var (_, volume) in Reactants)
        {
            ReactantVolume += volume;
        }
        foreach (var (_, volume) in Catalysts)
        {
            ReactantVolume += volume;
            ProductVolume += volume;
        }
        foreach (var (_, volume) in Products)
        {
            ProductVolume += volume;
        }
    }
}
