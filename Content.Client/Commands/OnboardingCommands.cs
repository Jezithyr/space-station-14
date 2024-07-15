using Content.Client.Onboarding.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Client.Commands;

[AnyCommand]
public sealed class StartOnboardingCommand : LocalizedCommands
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    public override string Command => "startonboarding";

    public override string Help => $"Usage: {Command} <OnboardingFlowPrototypeId>";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player == null)
        {
            shell.WriteLine("Only a player can run this command");
            return;
        }

        if (args.Length != 1)
        {
            shell.WriteLine(Help);
            return;
        }
        if (_entityManager.System<OnboardingSystem>().StartFlow(args[0]))
            shell.WriteLine($"Started onboarding flow: {args[0]}");
    }
}

[AnyCommand]
public sealed class StopOnboardingCommand : LocalizedCommands
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    public override string Command => "stoponboarding";

    public override string Help => $"Usage: {Command} <OnboardingFlowPrototypeId>";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player == null)
        {
            shell.WriteLine("Only a player can run this command");
            return;
        }

        if (args.Length != 1)
        {
            shell.WriteLine(Help);
            return;
        }
        if (_entityManager.System<OnboardingSystem>().StopFlow(args[0]))
            shell.WriteLine($"Stopped onboarding flow: {args[0]}");
    }
}

[AnyCommand]
public sealed class StopAllOnboardingCommand : LocalizedCommands
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    public override string Command => "stopallonboarding";

    public override string Help => $"Usage: {Command} <OnboardingFlowPrototypeId>";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player == null)
        {
            shell.WriteLine("Only a player can run this command");
            return;
        }

        if (args.Length != 1)
        {
            shell.WriteLine(Help);
            return;
        }
        _entityManager.System<OnboardingSystem>().StopAllFlows();
    }
}

[AnyCommand]
public sealed class ListActiveOnboardingCommand : LocalizedCommands
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    public override string Command => "listactiveonboarding";

    public override string Help => $"Usage: {Command}";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player == null)
        {
            shell.WriteLine("Only a player can run this command");
            return;
        }

        if (args.Length != 0)
        {
            shell.WriteLine(Help);
            return;
        }

        var output = "Active Flows: ";
        foreach (var flowProto in _entityManager.System<OnboardingSystem>().ActiveFlows)
        {
            output += $" {flowProto.ID},";
        }
        shell.WriteLine(output);
    }
}
