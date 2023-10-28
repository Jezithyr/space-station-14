﻿using Content.Client.UserInterface.Controls;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.UserInterface.Systems.Medical.Controls;

[GenerateTypedNameReferences]
public sealed partial class CommonStatusDisplay : BoxContainer
{
    public bool TargetingSelf { get; private set; }
    public CommonStatusDisplay()
    {
        RobustXamlLoader.Load(this);
    }
    public void UpdateTarget(string name, string species, bool self)
    {
        TargetingSelf = self;
        if (self)
        {
            name = $"{name} (Self)";
        }
        TargetName.Text = name;
        TargetSpecies.Text = species;
    }

}
