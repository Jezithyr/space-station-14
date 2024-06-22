using Content.Client.Onboarding.Components;
using Content.Shared.Movement.Events;

namespace Content.Client.Onboarding.Systems;

public sealed partial class OnboardingSystem
{
    private void InitTelemetryTriggers()
    {
        SubscribeLocalEvent<OnboardingStateComponent, MoveInputEvent>(OnMoveInputChanged);
    }

    private void OnMoveInputChanged(Entity<OnboardingStateComponent> ent, ref MoveInputEvent args)
    {
        RaisePlayerTrigger(MoveInputId, ent, true);
    }
}
