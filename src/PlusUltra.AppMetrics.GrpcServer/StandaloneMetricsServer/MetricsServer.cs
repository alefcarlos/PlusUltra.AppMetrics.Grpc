using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PlusUltra.AppMetrics.GrpcServer.StandaloneMetricsServer
{
    public class MetricsServer : IHostedService
    {
        private IHost _host;
        private readonly ILoggerFactory _loggerFactory;
        private readonly MetricsServerOptions _options;

        public MetricsServer(ILoggerFactory loggerFactory, IOptions<MetricsServerOptions> options)
        {
            _loggerFactory = loggerFactory;
            _options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _host = new HostBuilder()
                    .ConfigureServices(services =>
                    {
                        // Add the logger factory so that logs are configured by the main host
                        services.AddSingleton(_loggerFactory);
                    })
                    .ConfigureWebHost(webHostBuilder =>
                    {
                        webHostBuilder.UseKestrel(options =>
                        {
                            // options.ConfigureEndpointDefaults(defaults => {

                            // });

                            _options.ConfigureServerOptions(options);
                        });

                        webHostBuilder.ConfigureServices(services =>
                        {
                            services.AddRouting();
                            services.AddMetricsReportingHostedService();
                            services.AddMetricsEndpoints(options =>
                            {
                                options.MetricsEndpointOutputFormatter = MetricsInitializing.Metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                            });

                            services.AddMetrics(MetricsInitializing.Metrics);
                        });

                        webHostBuilder.Configure(app =>
                        {
                            app.UseMetricsAllMiddleware();

                            app.UseRouting();

                            app.UseMetricsAllEndpoints();
                        });
                    })
                    .Build();

            return _host.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_host == null)
                await Task.CompletedTask;

            using (_host)
            {
                await _host.StopAsync(cancellationToken);
            }
        }
    }
}