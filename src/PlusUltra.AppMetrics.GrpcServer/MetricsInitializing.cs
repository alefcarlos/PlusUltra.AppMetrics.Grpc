using App.Metrics;

namespace PlusUltra.AppMetrics.GrpcServer
{
    internal static class MetricsInitializing
    {
        public static IMetricsRoot Metrics = App.Metrics.AppMetrics.CreateDefaultBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .OutputMetrics.AsPrometheusProtobuf()
                .Build();
    }
}