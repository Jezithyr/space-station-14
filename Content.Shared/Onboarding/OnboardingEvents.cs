namespace Content.Shared.Onboarding;

[ByRefEvent]
public record struct OnboardingTriggerEvent(EntityUid? Origin);
