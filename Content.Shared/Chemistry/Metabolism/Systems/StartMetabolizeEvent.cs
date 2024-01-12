using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;

namespace Content.Shared.Chemistry.Metabolism.Systems;

[ByRefEvent]
public record struct StartMetabolizeEvent(
    Entity<SolutionComponent>? TargetSolution = null)
{
    public FixedPoint2 Efficiency = 1.0;
    public EntityUid TargetEntity = EntityUid.Invalid;
    public bool IsValid => TargetSolution != null && TargetEntity.Valid;
}
