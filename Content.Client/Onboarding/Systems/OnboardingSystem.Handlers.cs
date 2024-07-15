
using Robust.Shared.NamedEvents;

namespace Content.Client.Onboarding.Systems;

public sealed partial class OnboardingSystem
{
    protected override void InitTelemetryHandlers()
    {
        SubscribeAllNamedEventHandlers<OnboardingTriggerEvent>(HandleOnboardingTrigger,
            categoryFilter: NamedEventCategory);
    }


    private void HandleOnboardingTrigger(NamedEventId id, ref OnboardingTriggerEvent ev)
    {
        Log.Warning($"{id} FIRED! For player {ToPrettyString(ev.Origin)}");
    }
}
