using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class ReactingComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextUpdate;

    public static TimeSpan UpdateRate = TimeSpan.FromSeconds(0.1);
}
