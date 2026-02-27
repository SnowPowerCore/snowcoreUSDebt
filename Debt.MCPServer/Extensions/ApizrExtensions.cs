using Apizr;
using Apizr.Extending.Configuring.Common;
using Microsoft.Extensions.Http.Resilience;

namespace Debt.MCPServer.Extensions;

public static class ApizrExtensions
{
    /// <summary>
        /// Register all your Apizr managed apis with common shared options.
        /// You may call WithConfiguration option to adjust settings to your need.
        /// </summary>
        /// <param name="optionsBuilder">Adjust common shared options</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureTreasuryDebtApizrManagers(
            this IServiceCollection services,
            Action<IApizrExtendedCommonOptionsBuilder> optionsBuilder)
        {
            optionsBuilder ??= _ => { }; // Default empty options if null
            optionsBuilder += options => options
                .WithRefitSettings(new RefitSettings
                {
                    ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
                    {
                        TypeInfoResolver = DebtMcpJsonSerializerContext.Default
                    })
                })
                .ConfigureHttpClientBuilder(builder => builder
                    .AddStandardResilienceHandler(config =>
                    {
                        var timeSpan = TimeSpan.FromMinutes(1);
                        config.AttemptTimeout.Timeout = timeSpan;
                        config.CircuitBreaker.SamplingDuration = timeSpan * 2;
                        config.TotalRequestTimeout.Timeout = timeSpan * 3;
                        config.Retry = new HttpRetryStrategyOptions
                        {
                            UseJitter = true,
                            MaxRetryAttempts = 3,
                            Delay = TimeSpan.FromSeconds(0.5)
                        };
                    }))
                .WithPriority();
            
            return services.AddApizr(
                registry => registry.AddManagerFor<ITreasuryDebtApi>(),
                optionsBuilder);

        }
}