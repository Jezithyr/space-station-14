using Content.Shared.Administration;
using Robust.Shared.GameTelemetry;

namespace Content.Shared.SS14Telemetry;

public interface IPlayerTelemetryType
{
    public PlayerInfo PlayerInfo { get; }
}

public record struct PlayerTelemetryArgs(
    EntityUid? Origin,
    ICollection<EntityUid>? AffectedEntities,
    PlayerInfo PlayerInfo) : IGameTelemetryType, IPlayerTelemetryType
{
    public GameTelemetryId TelemetryId { get; set; } = default;
}
