using Content.Shared.Chemistry.Types;
using Robust.Shared.GameStates;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SolutionComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<ReagentQuantity> ReagentQuantities = new();
}
