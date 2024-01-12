using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Metabolism.Components;
using Content.Shared.Chemistry.Metabolism.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Chemistry.Metabolism.Systems;

public sealed class MetabolismSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly SharedContainerSystem _containers = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private static float _metabolismUpdateRate;
    private static float _metabolismGlobalMultiplier;

    public override void Initialize()
    {
        SetupCVars();
    }

    public override void Update(float frameTime)
    {
        var metabolisms = EntityQueryEnumerator<MetabolismComponent>();
        while (metabolisms.MoveNext(out var owner, out var metabolism))
        {
            if (metabolism.Reactions.Count == 0)
                continue; //if we have no reactions, don't even bother to check!
            metabolism.AccumulatedFrameTime += frameTime;
            if (metabolism.AccumulatedFrameTime  < _metabolismUpdateRate)
                continue;
            metabolism.AccumulatedFrameTime -= _metabolismUpdateRate;
            UpdateMetabolism(owner, metabolism);
        }
    }

    private void UpdateMetabolism(EntityUid owner, MetabolismComponent metabolism)
    {

        var metabolizeEvent = new DoMetabolizeEvent();
        RaiseLocalEvent(owner, ref metabolizeEvent);
        if (metabolism.Reactions.Count == 0 || metabolizeEvent.Efficiency <= 0)
            return;
        if (!metabolizeEvent.IsValid)
        {
            Log.Error($"Metabolism event for {ToPrettyString(owner)} is missing input or output solution");
            return;
        }
        FixedPoint2 volumeProcessed = 0;
        foreach (var (rawMultiplier,reactionProto,effects) in metabolism.Reactions)
        {
            var multiplier = rawMultiplier* _metabolismGlobalMultiplier*metabolizeEvent.Efficiency;
            FixedPoint2 reactionPercentage = 1.0f;
            var metabolicReaction = _prototypeManager.Index(reactionProto);
            if (metabolism.MaxReagentProcessingVolume < 0)
                metabolism.MaxReagentProcessingVolume = FixedPoint2.MaxValue;

            var maxProdVolume = metabolicReaction.ProductVolume*multiplier;
            var maxReactVolume = metabolicReaction.ReactantVolume * multiplier;
            var maxVolume = maxProdVolume < maxReactVolume ? maxProdVolume : maxReactVolume;

            reactionPercentage = FixedPoint2.Abs(metabolicReaction.ReactantVolume*multiplier / maxVolume);

            if (!metabolicReaction.AllowPartialReactions && reactionPercentage < 1.0f)
                continue;
            if (volumeProcessed >= metabolism.MaxReagentProcessingVolume)
                break;
            if (!CalculatePercentReacted(owner, metabolism, multiplier, metabolicReaction,
                    metabolizeEvent.InputSolution!, ref reactionPercentage))
                continue;
            DoMetabolicReactions(owner, metabolizeEvent.TargetEntity, metabolism, multiplier, metabolicReaction,effects, metabolizeEvent.InputSolution!,
                metabolizeEvent.OutputSolution!, ref reactionPercentage);
        }
    }

    private void DoMetabolicReactions(EntityUid owner, EntityUid target ,MetabolismComponent metabolism, FixedPoint2 multiplier,
        MetabolicReactionPrototype reactionProto, List<ReagentEffect> effects, SolutionComponent inputSolution, SolutionComponent outputSolution
        ,ref FixedPoint2 reactionPercentage)
    {
        foreach (var (reagentId, volume) in reactionProto.Reactants)
        {
            inputSolution.Solution.RemoveReagent(new ReagentId(reagentId, null), volume*reactionPercentage*multiplier);
            //TODO: DEBUG - check if the amount removed matches the amount we calculated
        }
        foreach (var (reagentId, volume) in reactionProto.Products)
        {
            outputSolution.Solution.AddReagent(new ReagentId(reagentId, null), volume*reactionPercentage*multiplier);
            //TODO: DEBUG - check if the amount added matches the amount we calculated
        }
        //TODO: Catalyst flushing, aka should the catalyst also be moved into the output solution?
        var args = new ReagentEffectArgs(target, owner, inputSolution.Solution, proto, 0,
            EntityManager, null, scale);

        foreach (var effect in effects)
        {
            if (!effect.ShouldApply(args, _random))
                continue;

            if (effect.ShouldLog)
            {
                _adminLogger.Add(LogType.ReagentEffect, effect.LogImpact,
                    $"Metabolism effect {effect.GetType().Name:effect} of reaction {reactionProto.LocalizedName} applied on entity {target:entity} at " +
                    $"{Transform(target).Coordinates:coordinates}");
            }

            effect.Effect(args);
        }
    }

    private bool CalculatePercentReacted(EntityUid owner, MetabolismComponent metabolism, FixedPoint2 multiplier,
        MetabolicReactionPrototype reactionProto, SolutionComponent inputSolution//TODO: Implement catalyst flushing!
        ,ref FixedPoint2 reactionPercentage)
    {
        var percentReacted = reactionPercentage;
        foreach (var (reactantId, volume) in reactionProto.Catalysts)
        {
            var reactantPercent = inputSolution.Solution.GetReagentQuantity(new ReagentId(reactantId, null))/volume*multiplier;
            if (reactantPercent < percentReacted) //percentage of reactant used is bound first by the catalylst then second by the reactant itself
                percentReacted = reactantPercent;
        }

        foreach (var (reactantId, volume) in reactionProto.Reactants)
        {
            var reactantPercent = inputSolution.Solution.GetReagentQuantity(new ReagentId(reactantId, null))/volume*multiplier;
            if (reactantPercent < percentReacted) //percentage of reactant used is bound first by the reactant itself
                percentReacted = reactantPercent;
        }

        if (reactionProto.AllowPartialReactions)
            return true;
        return percentReacted >= 1;
    }




    private void SetupCVars()
    {
        _metabolismUpdateRate = 1/_config.GetCVar(CCVars.ChemMetabolismTickrate);
        _metabolismGlobalMultiplier = 1/_config.GetCVar(CCVars.ChemMetabolismGlobalMultiplier);
        _config.OnValueChanged(CCVars.ChemMetabolismTickrate,
            newRate => { _metabolismUpdateRate = 1 / newRate;});
        _config.OnValueChanged(CCVars.ChemMetabolismGlobalMultiplier,
            newMult => { _metabolismGlobalMultiplier = newMult;});
    }
}
