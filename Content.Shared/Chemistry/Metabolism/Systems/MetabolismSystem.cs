using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Metabolism.Components;
using Content.Shared.Chemistry.Reaction;
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
    [Dependency] private readonly ChemicalReactionSystem _reactionSystem = default!;
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
        var enumerator = EntityQueryEnumerator<MetabolismComponent, ReactionMixerComponent>();
        while (enumerator.MoveNext(out var owner, out var metabolism, out var mixer))
        {
            if (metabolism.Reactions.Count == 0)
                continue; //if we have no reactions, don't even bother to check!
            metabolism.AccumulatedFrameTime += frameTime;
            if (metabolism.AccumulatedFrameTime  < _metabolismUpdateRate)
                continue;
            metabolism.AccumulatedFrameTime -= _metabolismUpdateRate;
            UpdateMetabolism(owner, metabolism, mixer);
        }
    }

    private void UpdateMetabolism(EntityUid owner, MetabolismComponent metabolism, ReactionMixerComponent mixer)
    {
        //fire the metabolize event to fetch our target entity and solutions
        var metabolizeEvent = new DoMetabolizeEvent
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
        _metabolismUpdateRate = 1/_config.GetCVar(CCVars.ChemMetabolismTickrate);
        _metabolismGlobalMultiplier = 1/_config.GetCVar(CCVars.ChemMetabolismGlobalMultiplier);
        _config.OnValueChanged(CCVars.ChemMetabolismTickrate,
            newRate => { _metabolismUpdateRate = 1 / newRate;});
        _config.OnValueChanged(CCVars.ChemMetabolismGlobalMultiplier,
            newMult => { _metabolismGlobalMultiplier = newMult;});
    }
}
