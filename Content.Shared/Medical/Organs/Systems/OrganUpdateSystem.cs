using Content.Shared.Body.Organ;
using Content.Shared.CCVar;
using Content.Shared.FixedPoint;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;

namespace Content.Shared.Medical.Organs.Systems;

public sealed partial class OrganUpdateSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;

    private static float _organUpdateRate;
    private static float _globalOrganMultiplier;
    private static float _organDecayMultiplier;
    private static FixedPoint2 _destroyedOrganMinNecro = 0.5;
    private static FixedPoint2 _organDestroyedThreshold = 0.01;
    public override void Initialize()
    {
        SetupCVars();
        InitializeDamage();
    }

    public override void Update(float frameTime)
    {
        var organs = EntityQueryEnumerator<OrganComponent>();
        while (organs.MoveNext(out var uid, out var organ))
        {
            organ.AccumulatedFrameTime += frameTime;
            //update the next update time before checking if this is allowed to tick to prevent garbage from being
            //constantly ticked by update
            if (organ.AccumulatedFrameTime  < _organUpdateRate)
                continue;
            organ.AccumulatedFrameTime -= _organUpdateRate;
            if (organ.Necrosis >= 1)
            {
                //if necrosis at 100% then this organ is dead and should not be ticked anymore,
                //so don't raise an update event
                organ.CanTick = false;
                Dirty(uid, organ);
                continue;
            }
            if (!organ.CanTick)
                continue;
            var ev = new OrganUpdateEvent(uid, organ, _globalOrganMultiplier);
            RaiseLocalEvent(uid, ref ev);
        }
    }
    private void SetupCVars()
    {
        _organUpdateRate = 1/_config.GetCVar(CCVars.MedicalOrganTickrate);
        _globalOrganMultiplier = _config.GetCVar(CCVars.MedicalOrganMultiplier);
        _organDecayMultiplier = _config.GetCVar(CCVars.MedicalOrganDecayMultiplier);
        _config.OnValueChanged(CCVars.MedicalOrganTickrate,
            newRate => { _organUpdateRate = 1 / newRate;});
        _config.OnValueChanged(CCVars.MedicalOrganMultiplier,
            newMult => { _globalOrganMultiplier = newMult;});
        _config.OnValueChanged(CCVars.MedicalOrganDecayMultiplier,
            newMult => { _organDecayMultiplier = newMult;});
    }
}
