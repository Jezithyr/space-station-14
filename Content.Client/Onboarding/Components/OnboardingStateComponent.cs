namespace Content.Client.Onboarding.Components;

[RegisterComponent]
public sealed partial class OnboardingStateComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public byte FlowId;
    [ViewVariables(VVAccess.ReadOnly)]
    public ushort Step;
    [ViewVariables(VVAccess.ReadOnly)]
    public ushort Stage;
}
