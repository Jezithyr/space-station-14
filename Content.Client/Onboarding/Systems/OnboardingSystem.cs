using Robust.Shared.GameTelemetry;
using Robust.Shared.GameTelemetry.Systems;

namespace Content.Client.Onboarding.Systems;
public sealed partial class OnboardingSystem : GameTelemetrySystem
{
    public const string TelemetryCategory = "Onboarding";
    public static GameTelemetryId MoveInputId = ("MoveInput", TelemetryCategory);
    protected override void DefineTelemetryIds(bool isServer)
    {
        if (isServer)
        {
        }
        else
        {
            CreateTriggerId(MoveInputId);
        }
    }

    public override void Initialize()
    {
        InitTelemetryHandlers();
        InitTelemetryTriggers();
    }
}
