using Content.Shared.Onboarding.Prototypes;
using Content.Shared.Onboarding.Systems;
using Robust.Shared.NamedEvents;

namespace Content.Client.Onboarding.Systems;
public sealed partial class OnboardingSystem : SharedOnboardingSystem
{
    private List<OnboardingFlowPrototype> _activeFlows = new();
    public IReadOnlyList<OnboardingFlowPrototype> ActiveFlows => _activeFlows;
    public bool OnboardingActive => _activeFlows.Count > 0;

    public static NamedEventId MoveInputId = ("Tut_Move", NamedEventCategory);
    protected override void RegisterNamedEventIds(bool isServer)
    {
        CreateTriggerId(MoveInputId);
    }
}
