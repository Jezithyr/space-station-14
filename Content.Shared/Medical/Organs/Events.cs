using Content.Shared.Body.Organ;
using Content.Shared.FixedPoint;
using Content.Shared.Medical.Organs.Components;

namespace Content.Shared.Medical.Organs;


[ByRefEvent]
public record struct HeartbeatEvent (EntityUid Target, HeartComponent Heart, OrganComponent Organ, float Multiplier);

[ByRefEvent]
public record struct OrganUpdateEvent (EntityUid Target, OrganComponent Organ, float Multiplier);
