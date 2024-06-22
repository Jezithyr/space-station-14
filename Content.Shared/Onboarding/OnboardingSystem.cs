using Robust.Shared.GameTelemetry;
using Robust.Shared.Player;

namespace Content.Shared.Onboarding;

// ReSharper disable once InconsistentNaming
public sealed partial class OnboardingSystem : GameTelemetrySystem
{

    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public const string TelemetryCategory = "Onboarding";
    public static GameTelemetryId MoveInputId = ("MoveInput", TelemetryCategory);
    protected override void DefineIds(bool isServer)
    {
        if (isServer)
        {
        }
        else
        {
            RegTriggerId(MoveInputId);
        }
    }

    public override void Initialize()
    {
        InitTriggers();
    }
}
