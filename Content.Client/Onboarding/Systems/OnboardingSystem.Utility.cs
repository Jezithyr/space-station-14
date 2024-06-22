using Robust.Shared.GameTelemetry;

namespace Content.Client.Onboarding.Systems;

public sealed partial class OnboardingSystem
{
    private void CreateTriggerId(string id, string category = TelemetryCategory)
    {
        CreateTriggerId((id, category));
    }

    private void CreateTriggerId(GameTelemetryId telemetryId)
    {
        RegisterId<OnboardingTriggerEvent>(telemetryId);
    }

    public void RaisePlayerTrigger(GameTelemetryId telemetryId, EntityUid uid, bool oneShot = false)
    {
        var ev = new OnboardingTriggerEvent(uid);
        RaisePlayerTelemetryEvent(telemetryId, uid ,ref ev, oneShot);
    }

    public void ResetOneShotPlayerTrigger(GameTelemetryId id)
    {
        ResetOneShotTelemetryEvent<OnboardingTriggerEvent>(id);
    }
}
