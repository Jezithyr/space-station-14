using Content.Shared.Onboarding;
using Robust.Shared.NamedEvents;
using Serilog;

namespace Content.Shared.Onboarding.Systems;

public abstract partial class SharedOnboardingSystem
{
    private void InitTelemetryHandlers()
    {
        SubscribeAllNamedEventHandlers<OnboardingTriggerEvent>(HandleOnboardingTrigger,
            categoryFilter: NamedEventCategory);
    }


    private void HandleOnboardingTrigger(NamedEventId id, ref OnboardingTriggerEvent ev)
    {
        Log.Warning($"{id} FIRED! For player {ToPrettyString(ev.Origin)}");
    }
}
