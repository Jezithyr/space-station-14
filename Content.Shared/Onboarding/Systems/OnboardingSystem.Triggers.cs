using Content.Client.Onboarding.Components;
using Content.Shared.Movement.Events;

namespace Content.Shared.Onboarding.Systems;

public abstract partial class SharedOnboardingSystem
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
