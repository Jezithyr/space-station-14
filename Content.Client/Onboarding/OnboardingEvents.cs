namespace Content.Client.Onboarding;

[ByRefEvent]
public record struct OnboardingTriggerEvent(EntityUid? Origin);
