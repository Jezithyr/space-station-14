using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;

namespace Content.Shared.Chemistry.Metabolism.Systems;

[ByRefEvent]
public record struct DoMetabolizeEvent(
    FixedPoint2 Efficiency,
    EntityUid TargetEntity,
    SolutionComponent? InputSolution = null,
    SolutionComponent? OutputSolution = null)
{
    public bool IsValid => InputSolution != null && OutputSolution != null && TargetEntity.Valid;

};
