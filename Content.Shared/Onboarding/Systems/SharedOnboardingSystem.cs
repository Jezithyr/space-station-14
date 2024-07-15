using Content.Shared.Onboarding.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared.Onboarding.Systems;
public abstract partial class SharedOnboardingSystem : EntitySystem
{
    [Dependency] protected readonly IPrototypeManager ProtoManager = default!;

    private OnboardingConfigPrototype? _onboardingConfig = null;

    public const string NamedEventCategory = "Onboarding";
    protected override void RegisterNamedEventIds(bool isServer)
    {

    }

    public override void Initialize()
    {
        ProtoManager.PrototypesReloaded += LoadPrimaryOptions;
        LoadPrimaryOptions();

        if (_onboardingConfig == null)
            return; //do not continue init if onboarding is disabled

        InitTelemetryHandlers();
        InitTelemetryTriggers();
    }


    private void LoadPrimaryOptions(PrototypesReloadedEventArgs? args = null)
    {
        _onboardingConfig = null;
        var count = 0;
        foreach (var primaryOnboardingFlow in ProtoManager.EnumeratePrototypes<OnboardingConfigPrototype>())
        {
            if (count > 0)
            {
                Log.Warning($"Multiple Primary Onboarding flows are defined! Only one should be present/enabled!");
                break;
            }

            var onboardingOptions = "";
            #if DEBUG
            onboardingOptions = "with onboarding options: ";
            foreach (var option in primaryOnboardingFlow.PrimaryOptions)
            {
                onboardingOptions += $"${option.Id} ,";
            }
            #endif

            Log.Info($"Found Primary Onboarding Flow: {primaryOnboardingFlow.ID} {onboardingOptions}");
            _onboardingConfig = primaryOnboardingFlow;
            count++;
        }

        if (count == 0)
        {
            Log.Info("No Primary Onboarding Flow found. Onboarding is disabled!");
        }
    }
}
