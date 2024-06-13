using Robust.Shared.GameTelemetry;

namespace Content.Shared.GameplayTelemetry;

public sealed class OnboadingTeleHandler : GameTelemetryHandler
{
    [Dependency] private ILogManager _logManager = default!;
    protected override void RegisterHandlers(bool isServer)
    {
    }

    private void Listener(GameTelemetryId id, EntityTelemetry ev)
    {
    }
}
