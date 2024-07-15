using Content.Shared.Onboarding;

namespace Content.Client.Onboarding.Systems;

public sealed partial class OnboardingSystem
{
    protected override void InitTelemetryHandlers()
    {
        SubscribeAllNamedEventHandlers<OnboardingTriggerEvent>(HandleOnboardingTrigger,
            categoryFilter: NamedEventCategory);
    }
}
