using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Medical.Bloodstream.Components;

namespace Content.Shared.Medical.Bloodstream.Systems;

public abstract class SharedBloodstreamSystem : EntitySystem
{
    public void ApplyBleed(EntityUid target, FixedPoint2 bleedAmount, BloodstreamNewComponent? bloodstream)
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

    public virtual void OnUpdateBloodLevel(EntityUid target, BloodstreamNewComponent legacyBloodstreamComponent,
        FixedPoint2 oldVolume, FixedPoint2 newVolume)
    {
    }

    public virtual void HandleSpillBlood(EntityUid target, BloodstreamNewComponent legacyBloodstream, Solution spiltSolution){}

    protected void ApplyBleeds(EntityUid target, BloodstreamNewComponent legacyBloodstream)
    {
        var oldBloodVolume = legacyBloodstream.BloodSolution.Volume;
        HandleSpillBlood(target, legacyBloodstream, legacyBloodstream.BloodSolution.SplitSolution(legacyBloodstream.BleedRate));
        OnUpdateBloodLevel(target, legacyBloodstream, oldBloodVolume, legacyBloodstream.BloodSolution.Volume);
    }
}
