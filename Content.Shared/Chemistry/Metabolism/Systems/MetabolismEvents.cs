using Content.Shared.FixedPoint;

namespace Content.Shared.Chemistry.Metabolism.Systems;

[ByRefEvent]
public record struct MetabolizeEvent(Dictionary<string, FixedPoint2> Products, FixedPoint2 UnitReactions);
