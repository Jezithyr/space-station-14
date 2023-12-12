using Content.Shared.FixedPoint;
using Content.Shared.Medical.Wounds.Components;

namespace Content.Shared.Medical.Organs.Systems;

public partial class OrganUpdateSystem
{
    public void InitializeDamage()
    {
        SubscribeLocalEvent<WoundableComponent, OrganUpdateEvent>(OnWoundableOrganUpdate);
    }

    private void OnWoundableOrganUpdate(EntityUid uid, WoundableComponent woundable, ref OrganUpdateEvent args)
    {
        /*TODO: handling necrosis separate from hitpoints is a bit icky, maybe rewrite this to use hitpoints?
         But then again hitpoints are for physical trauma...
        */
        if (woundable.HitPoints <= woundable.HitPointCap*_organDestroyedThreshold)
        {
            args.Organ.Necrosis += _organDecayMultiplier * args.Organ.DecayRate;
            args.Organ.Necrosis = FixedPoint2.Clamp(args.Organ.Necrosis, _destroyedOrganMinNecro, 1.0);
            args.Organ.Efficiency = 0; //if the organ is below the destroyed threshold it is no longer working!
            Dirty(uid, args.Organ);
        }
    }
}
