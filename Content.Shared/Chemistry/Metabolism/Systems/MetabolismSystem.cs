using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Metabolism.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chemistry.Metabolism.Systems;

public abstract class MetabolismSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionSystem = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly ChemicalReactionSystem _reactionSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;

    private static float _metabolismGlobalMultiplier;

    public override void Initialize()
    {
        SetupCVars();
        SubscribeLocalEvent<MetabolismComponent, SolutionChangedEvent>(OnMetabolismReaction);
        SubscribeLocalEvent<MetabolismComponent, ReactionAttemptEvent>(OnMetabolizeAttempt);
        SubscribeLocalEvent<MetabolismComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMetabolizeAttempt(EntityUid uid, MetabolismComponent metabolism, ref ReactionAttemptEvent args)
    {
        args.Multiplier = _metabolismGlobalMultiplier * metabolism.Efficiency;
    }

    private void OnMetabolismReaction(EntityUid uid, MetabolismComponent metabolism, ref SolutionChangedEvent args)
    {
        //If we don't have a target to transfer to or the target is ourself don't run transfer logic
        if (metabolism.OutputSolutionEntity == null || metabolism.OutputSolutionEntity == uid)
            return;

        var targetSolution = new Entity<SolutionComponent>(metabolism.OutputSolutionEntity.Value,
            Comp<SolutionComponent>(metabolism.OutputSolutionEntity.Value));
        foreach (var (reagentName, amount) in args.Products)
        {

            //this is fucking aids but it's the only way to actually get the reagent data
            ReagentData? reagentData = null;
            foreach (var reagentQuant in args.Solution.Comp.Solution.Contents)
            {
                if (reagentQuant.Reagent.Prototype == reagentName)
                {
                    reagentData = reagentQuant.Reagent.Data;
                    break;
                }
            }

            //Let's try to transfer the reagent to our target solution holder
            if (_solutionSystem.TryAddReagent(targetSolution, reagentName, amount,
                    targetSolution.Comp.Solution.Temperature, reagentData))
            {
                if (_solutionSystem.RemoveReagent(args.Solution, reagentName, amount, reagentData))
                {
                    Log.Error($"Could not remove reagent: {reagentName} from {args.Solution}");
                }
            }
            else
            {
                Log.Error($"Could not transfer reagent: {reagentName} to {targetSolution}");
            }
        }
    }

    public void SetOutputSolution(EntityUid owner, EntityUid? outputSolutionEntity, MetabolismComponent? metabolism = null,
        SolutionContainerManagerComponent? solutionManager = null)
    {
        if (!Resolve(owner, ref metabolism))
            return;
        if (outputSolutionEntity != null && !HasComp<SolutionContainerManagerComponent>(outputSolutionEntity))
            return;
        metabolism.OutputSolutionEntity = outputSolutionEntity;
        Dirty(owner, metabolism);
    }

    public void UpdateMetabolism(EntityUid owner, MetabolismComponent? metabolism = null, ReactionMixerComponent? mixer = null)
    {
        if (!Resolve(owner, ref metabolism))
            return;
        var solution = new Entity<SolutionComponent>(metabolism.SourceSolutionEntity,Comp<SolutionComponent>(metabolism.SourceSolutionEntity));
        _solutionSystem.UpdateChemicals(solution, true, mixer);
    }

    private void SetupCVars()
    {
        _metabolismGlobalMultiplier = 1/_config.GetCVar(CCVars.ChemMetabolismGlobalMultiplier);
        _config.OnValueChanged(CCVars.ChemMetabolismGlobalMultiplier,
            newMult => { _metabolismGlobalMultiplier = newMult;});
    }
}
