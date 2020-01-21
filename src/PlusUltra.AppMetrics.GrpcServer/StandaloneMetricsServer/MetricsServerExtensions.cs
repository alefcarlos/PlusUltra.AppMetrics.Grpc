using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace PlusUltra.AppMetrics.GrpcServer.StandaloneMetricsServer
{
    public static class MetricsServerExtensions
    {
        public static IServiceCollection AddStandaloneMetricsServer(this IServiceCollection services,
                                                                 Action<KestrelServerOptions> configureServer = null)
        {
            services.AddHostedService<MetricsServer>();

            services.Configure<MetricsServerOptions>(options =>
            {
                if (configureServer != null)
                {
                    options.ConfigureServerOptions += configureServer;
                }
            });
            return services;
        }
    }
}