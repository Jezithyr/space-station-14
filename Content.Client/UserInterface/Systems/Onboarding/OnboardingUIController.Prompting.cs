using Content.Client.UserInterface.Systems.Onboarding.Controls;

namespace Content.Client.UserInterface.Systems.Onboarding;

public sealed partial class OnboardingUIController
{

    public void PointToControl(string controlName, Direction direction)
    {
        if (UIManager.ActiveScreen == null
            || OnboardingLayer == null
            ||!UIManager.ActiveScreen.TryFindControlInHierarchy(controlName, out var foundControl, typeof(OnboardingLayerWidget)))
            throw new ArgumentException($"{controlName} could not be found on Screen: {UIManager.ActiveScreen}");

        if (OnboardingLayer.HasTarget(controlName))
            throw new ArgumentException($"{controlName} already has an active onboarding prompt");

        var pointer = new TutPointerControl();
        pointer.UpdateName(foundControl);
        pointer.UpdateDirection(direction.Invert());
        OnboardingLayer.AddPrompt(foundControl, pointer);
        OnboardingLayer.OffsetPrompt(foundControl, pointer, direction);
    }

    public void DismissPointer(string controlName)
    {
        OnboardingLayer?.RemovePrompt(controlName);
    }
}
