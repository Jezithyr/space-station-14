using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Chemistry.Components;
using Content.Shared.GameTicking;
using Content.Shared.Prototypes;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

using ReagentEntity = Robust.Shared.GameObjects.Entity<
    Content.Shared.Chemistry.Components.ReagentDefinitionComponent,
    Content.Shared.Chemistry.Components.ReagentMetamorphicSpriteComponent?>;

using ReactionEntity = Robust.Shared.GameObjects.Entity<
    Content.Shared.Chemistry.Components.ReactionDefinitionComponent,
    Content.Shared.Chemistry.Components.RequiresReactionMixingComponent?,
    Content.Shared.Chemistry.Components.RequiresReactionTemperatureComponent?>;

namespace Content.Shared.Chemistry.Systems;

public sealed partial class ChemicalRegistrySystem : EntitySystem
{
    [Dependency] private readonly INetManager _netManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly IComponentFactory _compFactory = default!;
    [Dependency] private readonly MetaDataSystem _metaSystem = default!;

    private FrozenDictionary<string, ReagentEntity> _reagents =
        FrozenDictionary<string, ReagentEntity>.Empty;
    private FrozenDictionary<string, ReactionEntity> _reactions =
        FrozenDictionary<string, ReactionEntity>.Empty;
    //TODO: Fix Saveload by saving proto<->entityId mapping as part of the map


    public override void Initialize()
    {
        _protoManager.PrototypesReloaded += ProtoManagerOnPrototypesReloaded;
        if (_netManager.IsClient)
        {
            SubscribeNetworkEvent<RoundRestartCleanupEvent>(OnRoundRestart);
        }
        else
        {
            SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);
        }
    }

    private void OnRoundRestart(RoundRestartCleanupEvent ev)
    {
        ClearReagents();
        LoadData();
    }

    public Entity<ReagentDefinitionComponent> GetReagent(string id)
    {
        if (!_reagents.TryGetValue(id, out var data))
            throw new ArgumentException($"Reagent with ID: {id} is not registered!");
        return new Entity<ReagentDefinitionComponent>(data.Owner, data.Comp1);
    }

    public bool HasReagent(string id) => _reagents.ContainsKey(id);

    public bool TryGetReagent(string id,
        [NotNullWhen(true)]out Entity<ReagentDefinitionComponent, ReagentMetamorphicSpriteComponent?>? reagentData)
    {
        if ( _reagents.TryGetValue(id, out var posreagentData))
        {
            reagentData = posreagentData;
            return true;
        }
        reagentData = null;
        return false;
    }

    public Entity<ReactionDefinitionComponent> GetReaction(string id)
    {
        if (!_reactions.TryGetValue(id, out var data))
            throw new ArgumentException($"Reagent with ID: {id} is not registered!");
        return data;
    }

    public bool HasReaction(string id) => _reactions.ContainsKey(id);

    public bool TryGetReaction(string id,
        [NotNullWhen(true)]out Entity<ReactionDefinitionComponent>? reactionData)
    {
        if ( _reactions.TryGetValue(id, out var posreagentData))
        {
            reactionData = posreagentData;
            return true;
        }
        reactionData = null;
        return false;
    }


    private void ClearReagents()
    {
        _reagents = FrozenDictionary<string, Entity<ReagentDefinitionComponent, ReagentMetamorphicSpriteComponent?>>.Empty;
    }

    private void LoadData()
    {
        if (_reagents.Count > 0)
        {
            Log.Error("Tried to load reagent definitions without clearing first!");
            return;
        }


        Dictionary<string, ReagentEntity> pendingReagents = new();
        ConvertLegacyReagentPrototypes(ref pendingReagents);
        Dictionary<string, ReactionEntity> pendingReactions = new();
        ConvertLegacyReactionPrototypes(ref pendingReactions);
        foreach (var entProto in _protoManager.EnumeratePrototypes<EntityPrototype>())
        {
            if (LoadReagentData(entProto, ref pendingReagents))
                continue;
            LoadReactionData(entProto, ref pendingReactions);
        }

        foreach (var (_, reaction) in pendingReactions)
        {
            foreach (var (reactantId, data) in reaction.Comp1.Reactants)
            {
                reaction.Comp1.ReactantEntities.Add(pendingReagents[reactantId].Owner, data);
            }
            foreach (var (reactantId, amount) in reaction.Comp1.Products)
            {
                reaction.Comp1.ProductEntities.Add(pendingReagents[reactantId].Owner, amount);
            }
        }

        _reagents = pendingReagents.ToFrozenDictionary();
        _reactions = pendingReactions.ToFrozenDictionary();
    }

    private bool LoadReagentData(
        EntityPrototype entProto,
        ref Dictionary<string, ReagentEntity> pendingReagents)
    {
        if (!entProto.HasComponent<ReagentDefinitionComponent>(_compFactory))
            return false;
        var newEnt = Spawn(entProto.ID);
        var reagentDef = Comp<ReagentDefinitionComponent>(newEnt);
        pendingReagents.Add(
            entProto.ID,
            (newEnt,
                reagentDef,
                CompOrNull<ReagentMetamorphicSpriteComponent>(newEnt)));
        _metaSystem.SetEntityDescription(newEnt, reagentDef.LocalizedDescription);
        return true;
    }

    private bool LoadReactionData(
        EntityPrototype entProto,
        ref Dictionary<string, ReactionEntity> pendingReactions)
    {
        if (!entProto.HasComponent<ReactionDefinitionComponent>(_compFactory))
            return false;
        var newEnt = Spawn(entProto.ID);
        var reactionDef = Comp<ReactionDefinitionComponent>(newEnt);
        pendingReactions.Add(
            entProto.ID,
            (newEnt,
                reactionDef,
                CompOrNull<RequiresReactionMixingComponent>(newEnt),
                CompOrNull<RequiresReactionTemperatureComponent>(newEnt)
                ));
        return true;
    }

    private void ProtoManagerOnPrototypesReloaded(PrototypesReloadedEventArgs obj)
    {
        ClearReagents();
        LoadData();
    }

    public override void Shutdown()
    {
        ClearReagents();
    }
}
