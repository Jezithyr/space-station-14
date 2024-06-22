using Content.Shared.Movement.Events;
using Content.Shared.Onboarding.Components;

namespace Content.Shared.Onboarding;

public sealed partial class OnboardingSystem
{
    private void InitTriggers()
    {
        SubscribeLocalEvent<OnboardingTriggerComponent, MoveInputEvent>(OnMoveInputChanged);
    }

    private void OnMoveInputChanged(Entity<OnboardingTriggerComponent> ent, ref MoveInputEvent args)
    {
        RaisePlayerTrigger(MoveInputId, ent, true);
    }
}
