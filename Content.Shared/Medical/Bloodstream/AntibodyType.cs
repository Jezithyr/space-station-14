namespace Content.Shared.Medical.Bloodstream;

[Flags, Serializable]
public enum AntiBodyType
{
    //== only these are used by humans ==
    None = 0,
    A = 1,
    B = 1<<1,
    Rh = 1<<2,
    //== human bloodgroups ==
    ONeg = None,
    OPos = Rh,
    ANeg = A,
    APos = A | Rh,
    BNeg = B,
    BPos = B | Rh,
    ABNeg = A | B,
    ABPos = A | B | Rh,
}
