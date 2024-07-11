using Content.Shared.Atmos;
using Content.Shared.FixedPoint;

namespace Content.Shared.Chemistry.Systems;

public sealed partial class ChemistrySystem
{
    public static FixedPoint4 MolsToVolume(
        float expansionConstant,
        FixedPoint4 moles,
        float pressureKpa = Atmospherics.OneAtmosphere,
        float tempKelvin = Atmospherics.T20C)
    {
        return moles * (tempKelvin * expansionConstant/pressureKpa);
    }

    public static FixedPoint4 VolumeToMols(
        float expansionConstant,
        FixedPoint4 volume,
        float pressureKpa = Atmospherics.OneAtmosphere,
        float tempKelvin = Atmospherics.T20C)
    {
        return volume / (tempKelvin * expansionConstant/pressureKpa);
    }
}
