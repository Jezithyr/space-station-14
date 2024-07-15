using Robust.Shared.Prototypes;

namespace Content.Shared.Onboarding.Prototypes;

[Prototype]
public sealed class OnboardingFlowPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; set; } = "";
}
