using Robust.Shared.Prototypes;

namespace Content.Shared.Onboarding.Prototypes;

[Prototype]
public sealed partial class OnboardingConfigPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; set; } = "";

    [DataField(required: true)]
    public List<ProtoId<OnboardingFlowPrototype>> PrimaryOptions = new ();
}
