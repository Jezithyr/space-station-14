using Robust.Shared.NamedEvents;

namespace Content.Shared.Onboarding.Systems;

public abstract partial class SharedOnboardingSystem
{
    protected void CreateTriggerId(string id, string category = NamedEventCategory)
    {
        CreateTriggerId((id, category));
    }

    protected void CreateTriggerId(NamedEventId telemetryId)
    {
        RegisterId<OnboardingTriggerEvent>(telemetryId);
    }

    public void RaisePlayerTrigger(NamedEventId telemetryId, EntityUid uid, bool oneShot = false)
    {
        var ev = new OnboardingTriggerEvent(uid);
        RaiseLocalPlayerNamedEvent(telemetryId, uid ,ref ev, oneShot);
    }

    public void ResetOneShotPlayerTrigger(NamedEventId id)
    {
        ResetOneShotNamedEvent<OnboardingTriggerEvent>(id);
    }
}
