using Robust.Shared.GameTelemetry;

namespace Content.Shared.GameplayTelemetry;

[ByRefEvent]
public record struct EntityTelemetry(EntityUid Origin, string Message) : IGameTelemetryArgs;
