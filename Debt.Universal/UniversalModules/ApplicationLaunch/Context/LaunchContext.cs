using MinimalStepifiedSystem.Base;

namespace Debt.ApplicationLaunch.Context;

public class LaunchContext(Version currentVersion) : BaseGenericContext
{
    public Version CurrentVersion { get; } = currentVersion;
}