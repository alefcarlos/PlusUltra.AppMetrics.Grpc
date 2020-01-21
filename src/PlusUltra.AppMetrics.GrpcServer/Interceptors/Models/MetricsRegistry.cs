using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Histogram;
using App.Metrics.ReservoirSampling.Uniform;

namespace PlusUltra.AppMetrics.GrpcServer.Interpectors
{
    public static class MetricsRegistry
    {
        /// <summary>
        /// Counter for requests sent from the client side or requests received on the server side
        /// </summary>
        public static CounterOptions RequestCounter => new CounterOptions
        {
            Name = "grpc server started total",
            MeasurementUnit = Unit.Calls,

        };

        /// <summary>
        /// Counter for responses received on the client side or responses sent from the server side
        /// </summary>
        public static CounterOptions ResponseCounter => new CounterOptions
        {
            Name = "grpc server handled total",
            MeasurementUnit = Unit.Calls,
        };

        /// <summary>
        /// Counter for counting total number of messages received from stream - used on both client and server side
        /// </summary>
        public static CounterOptions StreamReceivedCounter = new CounterOptions
        {
            Name = "grpc server msg received total",
            MeasurementUnit = Unit.Calls,
        };

        /// <summary>
        /// Counter for counting total number of messages sent through stream - used on both client and server side
        /// </summary>
        public static CounterOptions StreamSentCounter = new CounterOptions
        {
            Name = "grpc server msg sent total",
            MeasurementUnit = Unit.Calls,
        };

        /// <summary>
        /// Histogram for recording latency on both server and client side.
        /// By default it is disabled and can be enabled with <see cref="EnableLatencyMetrics"/>
        /// </summary>
        public static HistogramOptions LatencyHistogram = new HistogramOptions
        {
            Name = "grpc server handling seconds",
            MeasurementUnit = Unit.Calls,
            Reservoir = () => new DefaultAlgorithmRReservoir(),
        };
    }
}