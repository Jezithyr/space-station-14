using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chemistry.Reaction;

[RegisterComponent]
public sealed partial class ReactionMixerComponent : Component
{
    /// <summary>
    ///     A list of IDs for categories of reactions that can be mixed (i.e. HOLY for a bible, DRINK for a spoon)
    /// </summary>
    [ViewVariables]
    [DataField]
    public List<ProtoId<MixingCategoryPrototype>> ReactionTypes = default!;

    [DataField]
    public FixedPoint2 ReactantVolumeCapacity = -1;

    [DataField]
    public MixerFullEffect MixerOverVolumeEffect = MixerFullEffect.Overflow;

    /// <summary>
    ///     A string which identifies the string to be sent when successfully mixing a solution
    /// </summary>
    [ViewVariables]
    [DataField]
    public LocId MixMessage = "default-mixing-success";
}

[ByRefEvent]
public record struct MixingAttemptEvent(EntityUid Mixed, bool Cancelled = false);

public readonly record struct AfterMixingEvent(EntityUid Mixed, EntityUid Mixer);

[ByRefEvent]
public record struct GetMixableSolutionAttemptEvent(EntityUid Mixed, Entity<SolutionComponent>? MixedSolution = null);

//this is not a boolean because in the future I will be adding bursting for organ metabolism.
//(Don't swallow spesscoke and spentos at the same time kids)
[Serializable]
public enum MixerFullEffect
{
    Prevent,
    Overflow,
}
