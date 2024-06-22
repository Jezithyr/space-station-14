using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar;
using Content.Client.UserInterface.Systems.Onboarding.Controls;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Onboarding;

public sealed partial class OnboardingUIController : UIController , IOnStateChanged<GameplayState>
{
    [Dependency] private GameTopMenuBarUIController _topMenuBarUI = default!;
    private MenuButton? OnboardingButton => UIManager.GetActiveUIWidgetOrNull<MenuBar.Widgets.GameTopMenuBar>()?.OnboardingButton;
    private OnboardingLayerWidget? OnboardingLayer => UIManager.GetActiveUIWidgetOrNull<OnboardingLayerWidget>();
    public override void Initialize()
    {
        base.Initialize();
    }
    public void OnStateEntered(GameplayState state)
    {

    }

    public void OnStateExited(GameplayState state)
    {
        CleanElements();
    }


    private bool test = true;
    private void OnboardingButtonOnPressed(BaseButton.ButtonEventArgs obj)
    {
        if (test)
        {
            PointToControl("SlotButton_body_part_slot_left hand", Direction.North);
        }
        else
        {
            DismissPointer("SlotButton_body_part_slot_left hand");
        }
        test = !test;
    }

    public void UnloadButton()
    {
        if (OnboardingButton == null)
        {
            return;
        }

        OnboardingButton.Pressed = false;
        OnboardingButton.OnPressed -= OnboardingButtonOnPressed;
    }

    public void LoadButton()
    {
        if (OnboardingButton == null)
        {
            return;
        }

        OnboardingButton.OnPressed += OnboardingButtonOnPressed;
    }

    private void CleanElements()
    {
        OnboardingLayer?.ClearPrompts();
    }
}
