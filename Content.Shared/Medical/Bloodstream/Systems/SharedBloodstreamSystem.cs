using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Medical.Bloodstream.Components;

namespace Content.Shared.Medical.Bloodstream.Systems;

public abstract class SharedBloodstreamSystem : EntitySystem
{


    public void ApplyBleed(EntityUid target, FixedPoint2 bleedAmount, BloodstreamComponent? bloodstream)
    {
        if (!Resolve(target, ref bloodstream))
            return;
        bloodstream.BleedRate += bleedAmount;
        if (bloodstream.BleedRate < 0)
        {
            bloodstream.BleedRate = 0;
        }
        Dirty(target, bloodstream);
    }

    public virtual void OnUpdateBloodLevel(EntityUid target, BloodstreamComponent bloodstreamComponent,
        FixedPoint2 oldVolume, FixedPoint2 newVolume)
    {
    }

    public virtual void HandleSpillBlood(EntityUid target, BloodstreamComponent bloodstream, Solution spiltSolution){}

    protected void ApplyBleeds(EntityUid target, BloodstreamComponent bloodstream)
    {
        var oldBloodVolume = bloodstream.BloodSolution.Volume;
        HandleSpillBlood(target, bloodstream, bloodstream.BloodSolution.SplitSolution(bloodstream.BleedRate));
        OnUpdateBloodLevel(target, bloodstream, oldBloodVolume, bloodstream.BloodSolution.Volume);
    }

}
