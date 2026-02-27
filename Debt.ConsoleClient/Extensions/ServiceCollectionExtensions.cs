using Debt.ApplicationLaunch.Implementations.BackgroundServices;
using Debt.ApplicationLaunch.Interfaces;
using Debt.ConsoleClient.Features.AppLaunch;
using Debt.ConsoleClient.Features.AppLaunch.EveryTime;
using Debt.ConsoleClient.Features.Chat;
using Debt.ConsoleClient.Features.LLM.Copilot;
using Debt.ConsoleClient.Services;
using Debt.ConsoleHandling.Implementations.Services;
using Debt.LocalStorage.Implementations.Services;
using Debt.LocalStorage.Interfaces;
using Debt.TelemetryHandling.Implementations.Services;
using Debt.VersionTracking.Implementations.Services;
using Hanssens.Net;
using Microsoft.ApplicationInsights;

namespace Debt.ConsoleClient.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageServices(this IServiceCollection services)
    {
        services.AddSingleton<ILocalStorage>(static sp =>
            new Hanssens.Net.LocalStorage(new LocalStorageConfiguration { AutoLoad = true, AutoSave = true }));
        services.AddSingleton<ILocalStorageService>(static sp =>
            new SingleFileLocalStorageService(sp.GetRequiredService<ILocalStorage>()));

        return services;
    }

    public static IServiceCollection AddCoreConsoleServices(this IServiceCollection services)
    {
        services.AddSingleton<IConsoleService>(static sp => new StandardConsoleService());
        services.AddSingleton<IConsoleNavigationService>(static sp =>
            new ConsoleNavigationService(sp.GetRequiredService<IOptions<KnownScreenOptions>>(),
                sp.GetRequiredService<IServiceProvider>()));
        services.AddSingleton<IConsoleApplicationInfrastructureService>(static sp =>
            new ConsoleApplicationInfrastructureService(sp.GetRequiredService<IConsoleNavigationService>(),
                sp.GetRequiredService<IConsoleService>()));
        services.AddSingleton<IConsoleApplicationService>(static sp =>
            new ConsoleApplicationService(sp.GetRequiredService<IHostApplicationLifetime>(),
                sp.GetRequiredService<IConsoleApplicationInfrastructureService>(), sp.GetRequiredService<ITelemetryService>(),
                sp.GetRequiredService<IVersionTrackingService>()));

        return services;
    }

    public static IServiceCollection AddTelemetryServices(this IServiceCollection services)
    {
        services.AddSingleton<TelemetryClient>();
        services.AddSingleton<ITelemetryService>(static sp =>
            new ApplicationInsightsTelemetryService(sp.GetRequiredService<TelemetryClient>()));

        return services;
    }

    public static IServiceCollection AddVersioningServices(this IServiceCollection services)
    {
        services.AddSingleton<IVersionTrackingService>(static sp =>
            new LocalVersionTrackingService(sp.GetRequiredService<ILocalStorageService>()));

        return services;
    }

    public static IServiceCollection AddApplicationLaunchServices(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationLaunchService>(static sp =>
            new DebtConsoleAppLaunchService(sp.GetRequiredService<IVersionTrackingService>()));
        services.AddSingleton(static sp =>
            new HandleLaunchErrorsStep(sp.GetRequiredService<IConsoleApplicationService>()));
        services.AddSingleton(static sp =>
            new NavigateToRootScreenStep(sp.GetRequiredService<IConsoleNavigationService>()));

        return services;
    }

    public static IServiceCollection AddScreens(this IServiceCollection services)
    {
        services.AddSingleton(static sp => new ScreenBase(sp.GetRequiredService<IConsoleApplicationService>()));
        services.AddSingleton(static sp => new ChatScreen(sp.GetRequiredService<IConsoleApplicationService>(),
            sp.GetRequiredService<IHttpClientFactory>(),
            sp.GetRequiredService<ILlmClient>()));

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddHostedService(static sp =>
            new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
                sp.GetRequiredService<IApplicationLaunchService>()));

        return services;
    }

    public static IServiceCollection AddLlmClient(this IServiceCollection services)
    {
        services.AddSingleton<ILlmClient>(static sp => new CopilotLlmClient(sp.GetRequiredService<IOptions<LlmSessionOptions>>()));

        return services;
    }
}