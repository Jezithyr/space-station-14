using Robust.Shared.GameTelemetry;

namespace Content.Shared.SS14Telemetry.Handlers;

public sealed class OnboardingTelemetryHandler: GameTelemetryHandler,
    IGameTelemetryHandler<GameTelemetryArgs>,
    IGameTelemetryHandler<PlayerTelemetryArgs>
{
    public void HandleTelemetryArgs(GameTelemetryArgs args)
    {
        Logger.Debug($"YIPPPPPPPEEEEEEEEEEE: {args.TelemetryId}");
    }

    public void HandleTelemetryArgs(PlayerTelemetryArgs args)
    {
        Logger.Debug($"YIPPPPPPPEEEEEEEEEEE: Player: {args.TelemetryId}");
    }
}
