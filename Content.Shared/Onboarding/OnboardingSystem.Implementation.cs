using Content.Shared.Movement.Events;
using Robust.Shared.GameTelemetry;
using Robust.Shared.Player;

namespace Content.Shared.Onboarding;

public sealed partial class OnboardingSystem
{
    private void RegTriggerId(string id, string category = TelemetryCategory)
    {
        RegTriggerId((id, category));
    }

    private void RegTriggerId(GameTelemetryId telemetryId)
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
