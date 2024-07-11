using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using ReagentEntity = Robust.Shared.GameObjects.Entity<
    Content.Shared.Chemistry.Components.ReagentDefinitionComponent,
    Content.Shared.Chemistry.Components.ReagentMetamorphicSpriteComponent?>;

using ReactionEntity = Robust.Shared.GameObjects.Entity<
    Content.Shared.Chemistry.Components.ReactionDefinitionComponent,
    Content.Shared.Chemistry.Components.RequiresReactionMixingComponent?,
    Content.Shared.Chemistry.Components.RequiresReactionTemperatureComponent?>;

namespace Content.Shared.Chemistry.Systems;

public sealed partial class ChemicalRegistrySystem
{
    private readonly FixedPoint4 _molarMass = 18;
    private void ConvertLegacyReagentPrototypes(ref Dictionary<string, ReagentEntity> pendingReagents)
    {
        foreach (var reagentProto in _protoManager.EnumeratePrototypes<ReagentPrototype>())
        {
            var newEnt = Spawn();
            var reagentDef = AddComp<ReagentDefinitionComponent>(newEnt);
            ReagentMetamorphicSpriteComponent? reagentMetaMorph = null;

            reagentDef.NameLocId = reagentProto.NameLocId;
            reagentDef.MolarMass = _molarMass;
            reagentDef.Recognizable = reagentProto.Recognizable;
            reagentDef.PricePerUnit = reagentProto.PricePerUnit;
            reagentDef.Flavor = reagentProto.Flavor;
            reagentDef.DescriptionLocId = reagentProto.DescriptionLocId;
            reagentDef.PhysicalDescriptionLocId = reagentProto.PhysicalDescriptionLocId;
            reagentDef.FlavorMinimum = reagentProto.FlavorMinimum;
            reagentDef.SubstanceColor = reagentProto.SubstanceColor;
            reagentDef.SpecificHeat = reagentProto.SpecificHeat;
            reagentDef.BoilingPoint = reagentProto.BoilingPoint;
            reagentDef.MeltingPoint = reagentProto.MeltingPoint;
            reagentDef.Slippery = reagentProto.Slippery;
            reagentDef.Fizziness = reagentProto.Fizziness;
            reagentDef.Viscosity = reagentProto.Viscosity;
            reagentDef.FootstepSound = reagentProto.FootstepSound;
            reagentDef.WorksOnTheDead = reagentProto.WorksOnTheDead;
            reagentDef.Metabolisms = reagentProto.Metabolisms;
            reagentDef.ReactiveEffects = reagentProto.ReactiveEffects;
            reagentDef.TileReactions = reagentProto.TileReactions;
            reagentDef.PlantMetabolisms = reagentProto.PlantMetabolisms;
            reagentDef.LegacyId = reagentProto.ID;

            if (reagentProto.MetamorphicSprite != null)
            {
                reagentMetaMorph = AddComp<ReagentMetamorphicSpriteComponent>(newEnt);
                reagentMetaMorph.MetamorphicSprite = reagentProto.MetamorphicSprite;
                reagentMetaMorph.MetamorphicMaxFillLevels = reagentProto.MetamorphicMaxFillLevels;
                reagentMetaMorph.MetamorphicFillBaseName = reagentProto.MetamorphicFillBaseName;
                reagentMetaMorph.MetamorphicChangeColor = reagentProto.MetamorphicChangeColor;
            }
            _metaSystem.SetEntityName(newEnt, reagentProto.ID);
            _metaSystem.SetEntityDescription(newEnt, reagentProto.LocalizedDescription);
            pendingReagents.Add(reagentProto.ID, (newEnt, reagentDef, reagentMetaMorph));
        }
    }

    private void ConvertLegacyReactionPrototypes(ref Dictionary<string, ReactionEntity> pendingReactions)
    {
        foreach (var reactionProto in _protoManager.EnumeratePrototypes<ReactionPrototype>())
        {
            var newEnt = Spawn();
            var reactionDef = AddComp<ReactionDefinitionComponent>(newEnt);
            var tempReq = AddComp<RequiresReactionTemperatureComponent>(newEnt);
            RequiresReactionMixingComponent? mixingReq = null;
            tempReq.MinimumTemperature = reactionProto.MinimumTemperature;
            tempReq.MaximumTemperature = reactionProto.MaximumTemperature;
            reactionDef.ConserveEnergy = reactionProto.ConserveEnergy;
            reactionDef.Effects = reactionProto.Effects;
            reactionDef.Impact = reactionProto.Impact;
            reactionDef.Sound = reactionProto.Sound;
            reactionDef.Quantized = reactionProto.Quantized;
            reactionDef.Priority = reactionProto.Priority;
            reactionDef.LegacyId = reactionProto.ID;

            reactionDef.Reactants = new();
            foreach (var (reagentId, data) in reactionProto.Reactants)
            {
                reactionDef.Reactants.Add(reagentId, new ReactantData(data.Amount, data.Catalyst));
            }

            if (reactionProto.MixingCategories != null)
            {
                mixingReq = AddComp<RequiresReactionMixingComponent>(newEnt);
                mixingReq.MixingCategories = reactionProto.MixingCategories;
            }

            _metaSystem.SetEntityName(newEnt, reactionProto.Name);
            pendingReactions.Add(reactionProto.ID, (newEnt, reactionDef, mixingReq, tempReq));
        }
    }
}
