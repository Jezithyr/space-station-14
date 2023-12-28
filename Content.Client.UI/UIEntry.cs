using System.Diagnostics;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.UI;

// ReSharper disable once InconsistentNaming
public class UIEntry : UIShared
{
    public override void Init()
    {
        Debug.WriteLine("=== HELLO WORLD ===");
    }

    public override void PostInit()
    {
    }

    public override void PreInit()
    {
    }

    public override void UIUpdate(FrameEventArgs frameEventArgs)
    {
    }
}
