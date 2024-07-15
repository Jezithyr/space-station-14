﻿using Robust.Shared.NamedEvents;
using Robust.Shared.NamedEvents.Systems;

namespace Content.Client.Onboarding.Systems;
public sealed partial class OnboardingSystem : NamedEventSystem
{
    public const string NamedEventCategory = "Onboarding";
    public static NamedEventId MoveInputId = ("MoveInput", NamedEventCategory);
    protected override void DefineNamedEventIds(bool isServer)
    {
        CreateTriggerId(MoveInputId);
    }

    public override void Initialize()
    {
        InitTelemetryHandlers();
        InitTelemetryTriggers();
    }
}
