using Content.Shared.Onboarding.Systems;
using Robust.Shared.NamedEvents;

namespace Content.Client.Onboarding.Systems;
public sealed partial class OnboardingSystem : SharedOnboardingSystem
{
    public static NamedEventId MoveInputId = ("MoveInput", NamedEventCategory);
    protected override void RegisterNamedEventIds(bool isServer)
    {
        CreateTriggerId(MoveInputId);
    }
}
