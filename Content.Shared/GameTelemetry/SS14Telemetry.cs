using Robust.Shared.GameTelemetry;

namespace Content.Shared.GameTelemetry;

public sealed class SS14TelemetryCfg : GameTelemetryConfig
{
    public static GameTelemetryId PlayerBuckled = ("buckled", "player-interaction");
    protected override void RunConfig()
    {
        RegId<GameTelemetryArgs>(PlayerBuckled);
    }
}
