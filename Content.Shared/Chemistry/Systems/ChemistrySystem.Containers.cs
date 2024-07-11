using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Types;
using Robust.Shared.Containers;

namespace Content.Shared.Chemistry.Systems;

public sealed partial class ChemistrySystem
{
    public const string SolutionPrefix = "@Solution_";


    public Entity<SolutionComponent> GetSolution(
        Entity<ContainerManagerComponent?> solutionOwnerEntity,
        string solutionId)
    {
        ApplyPrefix(ref solutionId);
        if (!_containerSystem.TryGetContainer(solutionOwnerEntity,
                solutionId,
                out var solCon,
                solutionOwnerEntity.Comp))
        {
            throw new ArgumentException(
                $"Solution with ID: {solutionId} was not found on Entity: {ToPrettyString(solutionOwnerEntity)}");
        }
        return (solCon.ContainedEntities[0], Comp<SolutionComponent>(solCon.ContainedEntities[0]));
    }

    public Entity<SolutionComponent> CreateSolution(
        Entity<ContainerManagerComponent?> solutionOwnerEntity,
        string solutionId,
        bool warn = true,
        params ReagentQuantity[] reagents)
    {
        ApplyPrefix(ref solutionId);
        BaseContainer? solCon = null;
        if (_containerSystem.TryGetContainer(solutionOwnerEntity,
                solutionId,
                out solCon,
                solutionOwnerEntity.Comp))
        {
            if (warn)
                Log.Warning($"Solution with ID: {solutionId} already exists on Entity: {ToPrettyString(solutionOwnerEntity)}");
            return (solCon.ContainedEntities[0], Comp<SolutionComponent>(solCon.ContainedEntities[0]));
        }
        else
        {
            solCon = _containerSystem.EnsureContainer<ContainerSlot>(solutionOwnerEntity, solutionId);
        }

        if (!TrySpawnInContainer(null, solCon.Owner, solutionId, out var newSolEnt, solutionOwnerEntity.Comp))
        {
            throw new ArgumentException(
                $"Solution with ID: {solutionId} could not be created on Entity: {ToPrettyString(solutionOwnerEntity)}");
        }

        var solutionComp = new SolutionComponent();
        foreach (var reagentQuant in reagents)
        {
            solutionComp.ReagentQuantities.Add(reagentQuant);
        }
        AddComp(newSolEnt.Value, solutionComp);
        return (newSolEnt.Value, solutionComp);
    }


    private void ApplyPrefix(ref string id)
    {
        id = SolutionPrefix + id;
    }
}
