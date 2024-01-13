using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Metabolism.Components;
using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chemistry.Metabolism.Systems;

public sealed class MetabolismSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionSystem = default!;
    [Dependency] private readonly ChemicalReactionSystem _reactionSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;

    private static float _metabolismGlobalMultiplier;

    public override void Initialize()
    {
        SetupCVars();
        SubscribeLocalEvent<MetabolismComponent, SolutionChangedEvent>(OnMetabolismReaction);
        SubscribeLocalEvent<MetabolismComponent, ReactionAttemptEvent>(OnMetabolizeAttempt);
    }

    private void OnMetabolizeAttempt(EntityUid uid, MetabolismComponent component, ref ReactionAttemptEvent args)
    {
        args.Multiplier = _metabolismGlobalMultiplier * component.Efficiency;

    }

    private void OnMetabolismReaction(EntityUid uid, MetabolismComponent component, ref SolutionChangedEvent args)
    {
    }

    public void UpdateMetabolism(EntityUid owner, MetabolismComponent? metabolism = null, ReactionMixerComponent? mixer = null)
    {
        //fire the metabolize event to fetch our target entity and solutions
        var metabolizeEvent = new StartMetabolizeEvent
        {
            Efficiency = 1.0f
        };
        RaiseLocalEvent(owner, ref metabolizeEvent);
        if (metabolizeEvent.Efficiency <= 0)
            return;
        if (!metabolizeEvent.IsValid)
        {
            Log.Error($"Metabolism event for {ToPrettyString(owner)} is missing input or output solution");
            return;
        }
        _solutionSystem.UpdateChemicals(metabolizeEvent.TargetSolution!.Value, true, mixer);
    }



    private void SetupCVars()
    {
        _metabolismGlobalMultiplier = 1/_config.GetCVar(CCVars.ChemMetabolismGlobalMultiplier);
        _config.OnValueChanged(CCVars.ChemMetabolismGlobalMultiplier,
            newMult => { _metabolismGlobalMultiplier = newMult;});
    }
}
