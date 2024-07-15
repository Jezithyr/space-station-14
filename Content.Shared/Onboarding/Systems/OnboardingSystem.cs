using Robust.Shared.NamedEvents;
using Robust.Shared.NamedEvents.Systems;

namespace Content.Shared.Onboarding.Systems;
public abstract partial class SharedOnboardingSystem : NamedEventSystem
{
    public const string NamedEventCategory = "Onboarding";
    public static NamedEventId MoveInputId = ("MoveInput", NamedEventCategory);
    protected override void DefineNamedEventIds(bool isServer)
    {
        if (isServer)
        {
        }
        else
        {
            CreateTriggerId(MoveInputId);
        }
    }

    public override void Initialize()
    {
        InitTelemetryHandlers();
        InitTelemetryTriggers();
    }
}
