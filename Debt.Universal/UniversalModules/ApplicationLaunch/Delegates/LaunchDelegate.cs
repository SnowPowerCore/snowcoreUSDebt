using Debt.ApplicationLaunch.Context;
using Debt.ApplicationLaunch.Models;

namespace Debt.ApplicationLaunch.Delegates;

public delegate Task<ApplicationLaunchResult> LaunchDelegate(LaunchContext context, CancellationToken token = default);