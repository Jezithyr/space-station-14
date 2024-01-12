using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;

namespace Content.Shared.Chemistry.Metabolism.Systems;

[ByRefEvent]
public record struct DoMetabolizeEvent(
    FixedPoint2 Efficiency,
    EntityUid TargetEntity,
    Entity<SolutionComponent>? TargetSolution = null)
{
    public bool IsValid => TargetSolution != null && TargetEntity.Valid;

};
