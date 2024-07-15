

namespace Content.Shared.Onboarding.Systems;
public abstract partial class SharedOnboardingSystem : EntitySystem
{
    public const string NamedEventCategory = "Onboarding";
    protected override void RegisterNamedEventIds(bool isServer)
    {
    }

    public override void Initialize()
    {
        InitTelemetryHandlers();
        InitTelemetryTriggers();
    }
}
