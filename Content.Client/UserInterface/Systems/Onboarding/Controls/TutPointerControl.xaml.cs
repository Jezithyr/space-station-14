﻿using Content.Client.Stylesheets;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Onboarding.Controls;

[GenerateTypedNameReferences]
public sealed partial class TutPointerControl : Control
{
    public const string Prefix = "TutArrow_";
    private Direction _direction = Direction.Invalid;
    public TutPointerControl()
    {
        RobustXamlLoader.Load(this);
        var sheet = IoCManager.Resolve<IStylesheetManager>().SheetNano;
        Stylesheet = sheet;
        OnboardingPointer.Stylesheet = sheet;
    }

    public void UpdateName(Control target)
    {
        Name = Prefix + target.Name;
    }

    public void UpdateDirection(Direction direction)
    {
        OnboardingPointer.UpdateDirection(direction);
        _direction = direction;
    }
}
