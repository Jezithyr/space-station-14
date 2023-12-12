using Content.Shared.Medical.Organs.Components;

namespace Content.Shared.Medical.Organs.Systems;

public sealed class HeartSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HeartComponent,OrganUpdateEvent>(OnHeartTicked);
    }

    private void OnHeartTicked(EntityUid uid, HeartComponent heart, ref OrganUpdateEvent args)
    {
        //heartbeat/pulse stuff goes here
        var ev = new HeartbeatEvent(uid, heart, args.Organ, args.Multiplier);
        RaiseLocalEvent(uid, ref ev);
    }
}
