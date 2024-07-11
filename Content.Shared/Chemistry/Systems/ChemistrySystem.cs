using System.Runtime.InteropServices;
using Content.Shared.Chemistry.Components;
using Robust.Shared.Containers;
using Robust.Shared.Network;

namespace Content.Shared.Chemistry.Systems;

public sealed partial class ChemistrySystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly NetManager _netManager = default!;
    [Dependency] private readonly ChemicalRegistrySystem _chemRegistry = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<SolutionComponent, MapInitEvent>(OnSolutionMapInit);
    }

    private void OnSolutionMapInit(Entity<SolutionComponent> ent, ref MapInitEvent args)
    {
        if (_netManager.IsClient)
            return;//This prevents issues with sandbox menu

        foreach (ref var quantity in CollectionsMarshal.AsSpan(ent.Comp.ReagentQuantities))
        {
            quantity.ReagentEntId = _chemRegistry.GetReagent(quantity.ReagentId);
        }
        Dirty(ent);
    }
}
