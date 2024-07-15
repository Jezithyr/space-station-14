using Content.Shared.Onboarding.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Client.Onboarding.Systems;

public sealed partial class OnboardingSystem
{
    public bool StartFlow (ProtoId<OnboardingFlowPrototype> newFlow)
    {
        foreach (var flow in _activeFlows)
        {
            if (flow.ID != newFlow)
                continue;
            Log.Warning($"Tried to enable flow: {newFlow} while it was already active!");
            return false;
        }
        _activeFlows.Add(ProtoManager.Index(newFlow));
        return true;
    }

    public bool StopFlow(ProtoId<OnboardingFlowPrototype> flow)
    {
        return _activeFlows.Remove(ProtoManager.Index(flow));
    }

    public void StopAllFlows()
    {
        _activeFlows.Clear();
    }
}
