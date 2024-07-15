using Robust.Shared.Prototypes;

namespace Content.Client.Onboarding.Prototypes;

[Prototype]
public sealed partial class OnboardingStepPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; set; } = "";

    /// <summary>
    /// How difficult is this step. Steps with difficulty *less than* a player's knowledge level will be skipped.
    /// </summary>
    [DataField]
    public byte Difficulty = 0;


}

