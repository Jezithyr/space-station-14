using Content.Shared.Humanoid.Markings;
using Content.Shared.Localizations;
using Robust.Shared.GameTelemetry;

namespace Content.Shared.IoC
{
    public static class SharedContentIoC
    {
        public static void Register()
        {
            IoCManager.Register<MarkingManager, MarkingManager>();
            IoCManager.Register<ContentLocalizationManager, ContentLocalizationManager>();
        }
    }
}
