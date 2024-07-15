using Robust.Shared.NamedEvents;

namespace Content.Client.Onboarding.Systems;

public sealed partial class OnboardingSystem
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
        RaiseLocalPlayerNamedEvent(telemetryId, uid ,ref ev, oneShot);
    }

    public void ResetOneShotPlayerTrigger(NamedEventId id)
    {
        ResetOneShotNamedEvent<OnboardingTriggerEvent>(id);
    }
}
