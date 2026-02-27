using Debt.ConsoleClient.Extensions;
using Debt.ConsoleClient.Features.Chat;
using Debt.PublicApi.Constants;
using Debt.ServiceDefaults;
using Microsoft.Extensions.Logging;
using MinimalStepifiedSystem.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.ConfigureContainer(new DefaultServiceProviderFactory(new ServiceProviderOptions
{
    ValidateScopes = true,
    ValidateOnBuild = true
}));
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.ConfigureEmbeddedConfiguration();

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

builder.Services
    .AddStorageServices()
    .AddCoreConsoleServices()
    .AddTelemetryServices()
    .AddVersioningServices()
    .AddApplicationLaunchServices()
    .AddScreens()
    .AddInfrastructureServices()
    .AddLlmClient();

builder.Services.AddHttpClient(ProjectConstants.Projects_Debt_MCPServer,
    static client => client.BaseAddress = new($"{Uri.UriSchemeHttps}://{ProjectConstants.Projects_Debt_MCPServer}"));

builder.Services.Configure<KnownScreenOptions>(static ks =>
{
    ks.KnownScreenTypes.Add(Resource.DefaultScreenRouteName, ks.RootScreenType = typeof(ChatScreen));
});

builder.Services.Configure<LlmSessionOptions>(static options =>
{
    options.Model = Resource.GithubCopilotModelName;
});

var host = builder.Build();
host.UseStepifiedSystem();
await host.RunAsync();