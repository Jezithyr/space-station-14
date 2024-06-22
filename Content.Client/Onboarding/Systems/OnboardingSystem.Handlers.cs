using Robust.Shared.GameTelemetry;

namespace Content.Client.Onboarding.Systems;

public sealed partial class OnboardingSystem
{
    private void InitTelemetryHandlers()
    {
        SubscribeAllTelemetryHandlers<OnboardingTriggerEvent>(HandleOnboardingTrigger,
            categoryFilter: TelemetryCategory);
    }


    private void HandleOnboardingTrigger(GameTelemetryId id, ref OnboardingTriggerEvent ev)
    {
        Log.Warning($"{id} FIRED! For player {ToPrettyString(ev.Origin)}");
    }
}
