using App.Metrics;
using App.Metrics.Counter;
using Grpc.Core;

namespace PlusUltra.AppMetrics.GrpcServer.Interpectors
{
    public static class MetricsExtensions
    {
        /// <summary>
        /// Increments <see cref="RequestCounter"/>
        /// </summary>
        /// <param name="method">Information about the call</param>
        /// <param name="inc">Indicates by how much counter should be incremented. By default it's set to 1</param>
        public static void RequestCounterInc(this IMetricsRoot metrics, GrpcMethodInfo method, double inc = 1d)
        {
            metrics.Measure.Counter.Increment(MetricsRegistry.RequestCounter, new MetricTags(new string[] { "grpc_type", "grpc_service", "grpc_method" }, new string[] { method.MethodType, method.ServiceName, method.Name }));
        }

        /// <summary>
        /// Increments <see cref="ResponseCounter"/>
        /// </summary>
        /// <param name="method">Information about the call</param>
        /// <param name="code">Response status code</param>
        /// <param name="inc">Indicates by how much counter should be incremented. By default it's set to 1</param>
        public static void ResponseCounterInc(this IMetricsRoot metrics, GrpcMethodInfo method, StatusCode code, double inc = 1d)
        {
            metrics.Measure.Counter.Increment(MetricsRegistry.ResponseCounter, new MetricTags(new string[] { "grpc_type", "grpc_service", "grpc_method", "grpc_code" }, new string[] { method.MethodType, method.ServiceName, method.Name, code.ToString() }));
        }

        /// <summary>
        /// Increments <see cref="StreamReceivedCounter"/>
        /// </summary>
        /// <param name="method">Information about the call</param>
        /// <param name="inc">Indicates by how much counter should be incremented. By default it's set to 1</param>
        public static void StreamReceivedCounterInc(this IMetricsRoot metrics, GrpcMethodInfo method, double inc = 1d)
        {
            metrics.Measure.Counter.Increment(MetricsRegistry.StreamReceivedCounter,
                                    new MetricTags(new string[]
                                    {
                                         "grpc_type", "grpc_service", "grpc_method"
                                    }, new string[]
                                     {
                                          method.MethodType, method.ServiceName, method.Name
                                    }));
        }

        /// <summary>
        /// Increments <see cref="StreamSentCounter"/>
        /// </summary>
        /// <param name="method">Information about the call</param>
        /// <param name="inc">Indicates by how much counter should be incremented. By default it's set to 1</param>
        public static void StreamSentCounterInc(this IMetricsRoot metrics, GrpcMethodInfo method, double inc = 1d)
        {
            metrics.Measure.Counter.Increment(MetricsRegistry.StreamSentCounter, new MetricTags(new string[] { "grpc_type", "grpc_service", "grpc_method" }, new string[] { method.MethodType, method.ServiceName, method.Name }));
        }

        /// <summary>
        /// Records latency recorded during the call
        /// </summary>
        /// <param name="method">Infromation about the call</param>
        /// <param name="value">Value that should be recorded</param>
        public static void RecordLatency(this IMetricsRoot metrics, GrpcMethodInfo method, double value)
        {
            metrics.Measure.Histogram.Update(MetricsRegistry.LatencyHistogram, new MetricTags(new string[] { "grpc_type", "grpc_service", "grpc_method" }, new string[] { method.MethodType, method.ServiceName, method.Name }), (long)value);
        }
    }
}