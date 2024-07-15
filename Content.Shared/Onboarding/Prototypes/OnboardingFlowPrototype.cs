using Robust.Shared.Prototypes;

namespace Content.Client.Onboarding.Prototypes;

[Prototype]
public sealed class OnboardingFlowPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; set; } = "";
}
