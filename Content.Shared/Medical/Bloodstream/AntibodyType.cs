namespace Content.Shared.Medical.Bloodstream;

[Flags, Serializable]
public enum AntiBodyType
{
    //== only these are used by humans ==
    None = 0,
    TypeA = 1,
    TypeB = 1<<1,
    //== below are fictional/aliens ==
    TypeC = 1<<2,
    TypeD = 1<<3
}
