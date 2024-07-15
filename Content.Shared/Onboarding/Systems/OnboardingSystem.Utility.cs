using Robust.Shared.NamedEvents;

namespace Content.Shared.Onboarding.Systems;

public abstract partial class SharedOnboardingSystem
{
    private void CreateTriggerId(string id, string category = NamedEventCategory)
    {
        CreateTriggerId((id, category));
    }

    private void CreateTriggerId(NamedEventId telemetryId)
    {
        RegisterId<OnboardingTriggerEvent>(telemetryId);
    }

    public void RaisePlayerTrigger(NamedEventId telemetryId, EntityUid uid, bool oneShot = false)
    {
        var ev = new OnboardingTriggerEvent(uid);
        RaisePlayerNamedEvent(telemetryId, uid ,ref ev, oneShot);
    }

    public void ResetOneShotPlayerTrigger(NamedEventId id)
    {
        ResetOneShotNamedEvent<OnboardingTriggerEvent>(id);
    }
}
