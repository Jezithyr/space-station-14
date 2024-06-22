using Robust.Shared.GameTelemetry;

namespace Content.Shared.Onboarding;

public sealed class OnboardingHandlerSystem : GameTelemetryHandlerSystem
{
    public override void Initialize()
    {
        SubscribeAllTelemetryHandlers<OnboardingTriggerEvent>(HandleOnboardingTrigger,
            categoryFilter: OnboardingSystem.TelemetryCategory);
    }

    private void HandleOnboardingTrigger(GameTelemetryId id, ref OnboardingTriggerEvent ev)
    {
        Log.Warning($"{id} FIRED! For player {ToPrettyString(ev.Origin)}");
    }
}
