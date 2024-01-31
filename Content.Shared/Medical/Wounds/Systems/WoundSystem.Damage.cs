using System.Linq;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Medical.Wounds.Components;

namespace Content.Shared.Medical.Wounds.Systems;

public partial class WoundSystem
{

    private void InitDamage()
    {
        SubscribeLocalEvent<WoundableComponent, DamageChangedEvent>(OnWoundableDamaged);
        SubscribeLocalEvent<BodyComponent, DamageChangedEvent>(OnBodyDamaged);
    }

    //Damagable handler that relays damage from a humanoid/body entity to the root woundable bodypart entity
    private void OnBodyDamaged(EntityUid target, BodyComponent component, DamageChangedEvent args)
    {
        if (!args.AppliesWounds)
            return;
        if (component.RootContainer.ContainedEntity == null
            || !TryComp<WoundableComponent>(component.RootContainer.ContainedEntity, out var rootWoundable))
            return;
        if (args.DamageDelta == null)
        {
            Log.Warning($"Tried to set damage directly on {target} to {component.RootContainer.ContainedEntity} " +
                      $"{rootWoundable} directly setting damage on woundable entities will not spawn wounds!");
            return;
        }

        //TODO: This only applies wounds to the torso, this makes debugging easier, later swap this for the randomization logic!
        if (_damageable.TryChangeDamage(component.RootContainer.ContainedEntity, args.DamageDelta,
                interruptsDoAfters: args.InterruptsDoAfters, origin: args.Origin) != null)
            return;
        Log.Error($"Failed to relay damage to a woundable entity " +
                  $"{component.RootContainer.ContainedEntity} that does NOT have a damagable component. " +
                  $"This is required for wounds to function!");

        /* TODO: replace the above logic with this when you are done testing to properly select a random bodypart
        so that wounds don't only get applied to the torso */

        // if (!TryGetRandomActiveChildWoundable(target, out var data, rootWoundable))
        // {
        //     Log.Verbose($"Could not find a woundable to relay wounds to in {rootWoundable}. If this expected ignore this warning," +
        //                 $" otherwise double check your woundable hierarchy!");
        //     return;
        // }
        //
        // var childEntity = data.Value.Item1;
        // if (_damageable.TryChangeDamage(childEntity, args.DamageDelta,
        //         interruptsDoAfters: args.InterruptsDoAfters, origin: args.Origin) != null)
        //     return;
        // Log.Error($"Failed to relay damage to a woundable entity {childEntity} that does NOT have a damagable component. " +
        //           $"This is required for wounds to function!");
    }

    //Handler that is triggered when a woundable recieves damage, this is a proxy to the applyDamageAndCreateWounds function
    //which holds all the main logic!
    private void OnWoundableDamaged(EntityUid target, WoundableComponent woundable,  DamageChangedEvent args)
    {
        if (!args.AppliesWounds)
            return;
        if (args.DamageDelta == null)
        {
            Log.Error($"Tried to set damage directly on {target} {woundable} do not set damage directly on entities " +
                      $"that are handled by woundables otherwise everything will explode");
            return;
        }

        if (woundable.AllowWounds)
        {
            ApplyDamageAndCreateWounds(target, woundable, args.DamageDelta);
            return;
        }
    }

    //Main wound application and damage dispatching function
    protected void ApplyDamageAndCreateWounds(EntityUid target, WoundableComponent woundable, DamageSpecifier damage)
    {
        if (_net.IsClient)
            return;
        ApplyWoundableDamage(target, woundable, damage);

        //Create wounds for each woundable
        foreach (var (damageType, rawDamage) in damage.DamageDict)
        {
            var woundPool = GetWoundPoolFromDamageType(target, damageType, woundable);
            if (woundPool == null)
                continue;
            var woundProtoId =
                SelectWoundProtoUsingDamage(woundPool.Value, CalculateWoundDamage(target, rawDamage, woundable));
            if (woundProtoId == null)
                return;
            if (!TrySpawnWound(target, woundProtoId.Value, out var data))
            {
                Log.Error("Wound Creation failed!");
            }
        }
    }

    //Applies damage to woundables and handles part gibbing/destruction
    private void ApplyWoundableDamage(EntityUid target, WoundableComponent woundable, DamageSpecifier damage)
    {
        var totalAdjDmg = woundable.DamageScaling * damage.GetTotal();

        if (totalAdjDmg < 0)
            return;//TODO: write esoteric healing logic. For now this will have no effect!

        woundable.HitPoints -= totalAdjDmg; //TODO factor in resistances when they get implemented
        if (woundable.HitPoints < 0)
        {
            woundable.Integrity += woundable.HitPoints;
            woundable.HitPoints = 0;
            if (woundable.Integrity < 0)
            {
                var overflow = -woundable.Integrity;
                woundable.Integrity = 0;
                Dirty(target, woundable);
                DestroyWoundable(target, woundable, damage, overflow / totalAdjDmg);
                return;
            }
        }
        Dirty(target, woundable);
        return;
    }


    //Part gibbing logic, this is separate from body gibbing and specifically deals with gibbing single parts and their organs
    private void DestroyWoundable(EntityUid woundableEntity, WoundableComponent woundable, DamageSpecifier originalDamage, FixedPoint2 percentOverflow)
    {
        if (TryComp<BodyPartComponent>(woundableEntity, out var bodyPart))
        {
            if (bodyPart.Body != null && TryComp<BodyComponent>(bodyPart.Body.Value, out var body)
                                      && _body.IsPartRoot(bodyPart.Body.Value, woundableEntity, body, bodyPart))
            {
                _body.GibBody(bodyPart.Body.Value, true, body);
            }
        }
    }

    /// <summary>
    /// Helper function to calculate how much damage a woundable will receive after scaling is applied
    /// </summary>
    /// <param name="target">Owner of the woundable</param>
    /// <param name="damage">Unmodified damage</param>
    /// <param name="woundable">Woundable component</param>
    /// <returns>The damage that would be applied to a woundable after modifiers</returns>
    public FixedPoint2 CalculateWoundDamage(EntityUid target, FixedPoint2 damage, WoundableComponent? woundable)
    {
        if (!Resolve(target, ref woundable))
            return 0;
        return woundable.DamageScaling * damage;
    }
}
