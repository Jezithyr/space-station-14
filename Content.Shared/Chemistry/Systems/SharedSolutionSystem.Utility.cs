﻿using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;

namespace Content.Shared.Chemistry.Systems;

public partial class SharedSolutionSystem
{
    private static SolutionComponent.ReagentData _invalidReagentData = new();
    private static SolutionComponent.VariantData _invalidVariantData = new();

/// <summary>
    /// Get the possible overflow from changing the volume of this solution.
    /// </summary>
    /// <param name="solution">Target Solution</param>
    /// <param name="delta">Potential volume change</param>
    /// <returns>Overflow (positive)</returns>
    [PublicAPI]
    public FixedPoint2 GetPossibleOverflow(Entity<SolutionComponent> solution,
        FixedPoint2 delta)
    {
        if (delta <= 0)
            return 0;
        var overflow = delta - solution.Comp.AvailableVolume;
        return overflow < 0 ? 0 : overflow;
    }

    private FixedPoint2 FixUnderflow(FixedPoint2 value, ref FixedPoint2 delta)
    {
        if (delta >= 0)
            return 0;
        var underflow = delta - value;
        if (underflow < 0)
            return 0;
        delta += underflow;
        return underflow;
    }



    [PublicAPI]
    public FixedPoint2 GetMissingAmount(Entity<SolutionComponent> solution, FixedPoint2 delta)
    {
        if (delta >= 0)
            return 0;
        var underflow = delta - solution.Comp.Volume;
        return underflow <= 0 ? 0 : underflow;
    }

    /// <summary>
    /// Check if this solution will overflow if it's volume gets changed by the delta amount.
    /// OverCapacityAmount can be negative to represent an underflow.
    /// </summary>
    /// <param name="solution"></param>
    /// <param name="delta"></param>
    /// <param name="overCapacityAmount"></param>
    /// <returns></returns>
    [PublicAPI]
    public bool WillOverflow(Entity<SolutionComponent> solution,
        FixedPoint2 delta,
        out FixedPoint2 overCapacityAmount)
    {
        overCapacityAmount = GetPossibleOverflow(solution, delta);
        return solution.Comp.CanOverflow && overCapacityAmount > 0;
    }


    /// <summary>
    /// Optimize memory usage of reagent lists
    /// </summary>
    /// <param name="solution"></param>
    protected void TrimAllocs(Entity<SolutionComponent> solution)
    {
        //This is for memory optimization, there is no point to dirtying this since we aren't in a rush to sync it.
        solution.Comp.Contents.TrimExcess();
        solution.Comp.Contents.EnsureCapacity(ReagentAlloc);
        foreach (ref var reagentData in solution.Comp.ContentsSpan)
        {
            TrimAlloc(ref reagentData);
        }
    }

    /// <summary>
    /// Optimize variant memory usage for a given reagent
    /// </summary>
    /// <param name="reagentData"></param>
    protected void TrimAlloc(ref SolutionComponent.ReagentData reagentData)
    {
        if (reagentData.Variants?.Count == 0)
        {
            reagentData.Variants = null;
            return;
        }
        reagentData.Variants?.TrimExcess();
        reagentData.Variants?.EnsureCapacity(VariantAlloc);
    }

    /// <summary>
    /// Updates the primary reagent
    /// </summary>
    /// <param name="solution"></param>
    protected void UpdatePrimaryReagent(Entity<SolutionComponent> solution)
    {
        if (CheckIfEmpty(solution))
            return;
        var contents = solution.Comp.ContentsSpan;
        UpdatePrimaryReagent(solution, contents, GetPrimaryReagentQuantity(solution, contents));
    }

    /// <summary>
    /// Updates the primary reagent
    /// </summary>
    /// <param name="solution"></param>
    /// <param name="reagentData"></param>
    /// <param name="delta"></param>
    protected void UpdatePrimaryReagent(Entity<SolutionComponent> solution,
        ref SolutionComponent.ReagentData reagentData,
        FixedPoint2 delta)
    {
        if (!reagentData.IsValid || CheckIfEmpty(solution))
            return;
        var contentsSpan = solution.Comp.ContentsSpan;
        var primaryQuantity = GetPrimaryReagentQuantity(solution, contentsSpan);
        if (solution.Comp.PrimaryReagentIndex == reagentData.Index
            && FixedPoint2.Sign(delta) >= 0)
            return;
        if (reagentData.TotalQuantity >= primaryQuantity)
        {
            solution.Comp.PrimaryReagentIndex = reagentData.Index;
        }
        UpdatePrimaryReagent(solution, contentsSpan, primaryQuantity);
    }

    /// <summary>
    /// Updates the primary reagent
    /// </summary>
    /// <param name="solution"></param>
    /// <param name="contents"></param>
    /// <param name="primaryReagentQuantity"></param>

    private void UpdatePrimaryReagent(
        Entity<SolutionComponent> solution,
        Span<SolutionComponent.ReagentData> contents,
        FixedPoint2 primaryReagentQuantity)
    {
        var newIndex = 0;
        foreach (ref var reagentData in contents)
        {
            if (reagentData.TotalQuantity > primaryReagentQuantity)
                newIndex = reagentData.Index;
        }

        if (newIndex == solution.Comp.PrimaryReagentIndex)
            return;
        solution.Comp.PrimaryReagentIndex = newIndex;
        UpdateAppearance((solution, solution, null));
    }

    /// <summary>
    /// Check if the solution is empty and update primaryReagent
    /// </summary>
    /// <param name="solution"></param>
    /// <returns></returns>
    protected bool CheckIfEmpty(Entity<SolutionComponent> solution)
    {
        if (solution.Comp.Volume != 0)
            return false;
        if (solution.Comp.PrimaryReagentIndex < 0)
            return true;
        solution.Comp.PrimaryReagentIndex = -1;
        return true;
    }

    /// <summary>
    /// Shifts Contents Indices after a reagent was removed
    /// </summary>
    /// <param name="solution"></param>
    /// <param name="startingIndex"></param>
    protected void ShiftContentIndices(Entity<SolutionComponent> solution,
        int startingIndex)
    {
        if (startingIndex >= solution.Comp.Contents.Count)
            return;
        var contents = solution.Comp.ContentsSpan;
        for (var i = startingIndex; i < contents.Length; i++)
        {
            ref var reagentData = ref contents[i];
            reagentData.Index--;
            if (reagentData.Variants == null)
                continue;
            foreach (ref var variantData in reagentData.VariantsSpan)
            {
                variantData.ParentIndex = i;
            }
        }
    }

    private int[] CreateRandomIndexer(int length)
    {
        var indices = new int[length];
        for (var i = 0; i < length; i++)
        {
            indices[i] = i;
        }
        Random.GetRandom().Shuffle(indices);
        return indices;
    }

    private FixedPoint2 ClampToEpsilon(float input)
    {
        return input < FixedPoint2.Epsilon ? FixedPoint2.Epsilon : input;
    }


    public bool ResolveReagent(ref ReagentDef reagent, bool logMissing = true)
    {
        return ChemistryRegistry.ResolveReagentDef(ref reagent, logMissing);
    }

    public bool ResolveReagent(ref ReagentQuantity reagent, bool logMissing = true)
    {
        return ChemistryRegistry.ResolveReagentDef(ref reagent, logMissing);
    }

}
