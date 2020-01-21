using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace PlusUltra.AppMetrics.GrpcServer.Interpectors
{
    /// <summary>
    /// Interceptor for intercepting calls on server side 
    /// </summary>
    public class ServerMetricsInterceptor : Interceptor
    {
        /// <summary>
        /// Constructor for server side interceptor
        /// </summary>
        /// <param name="enableLatencyMetrics">Set if latency metrics is enabled</param>
        public ServerMetricsInterceptor(bool enableLatencyMetrics = false)
        {
            this.enableLatencyMetrics = enableLatencyMetrics;
        }
        private readonly bool enableLatencyMetrics;


        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            GrpcMethodInfo method = new GrpcMethodInfo(context.Method, MethodType.Unary);

            MetricsInitializing.Metrics.RequestCounterInc(method);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                TResponse result = await continuation(request, context);
                MetricsInitializing.Metrics.ResponseCounterInc(method, context.Status.StatusCode);
                return result;
            }
            catch (RpcException e)
            {
                MetricsInitializing.Metrics.ResponseCounterInc(method, e.Status.StatusCode);
                throw;
            }
            finally
            {
                watch.Stop();
                if (enableLatencyMetrics)
                    MetricsInitializing.Metrics.RecordLatency(method, watch.Elapsed.TotalSeconds);
            }
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            GrpcMethodInfo method = new GrpcMethodInfo(context.Method, MethodType.ServerStreaming);

            MetricsInitializing.Metrics.RequestCounterInc(method);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Task result;

            try
            {
                result = continuation(request,
                    new WrapperServerStreamWriter<TResponse>(responseStream,
                        () => { MetricsInitializing.Metrics.StreamSentCounterInc(method); }),
                    context);

                MetricsInitializing.Metrics.ResponseCounterInc(method, StatusCode.OK);
            }
            catch (RpcException e)
            {
                MetricsInitializing.Metrics.ResponseCounterInc(method, e.Status.StatusCode);
                throw;
            }
            finally
            {
                watch.Stop();

                if (enableLatencyMetrics)
                    MetricsInitializing.Metrics.RecordLatency(method, watch.Elapsed.TotalSeconds);
            }

            return result;
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
            ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            GrpcMethodInfo method = new GrpcMethodInfo(context.Method, MethodType.ClientStreaming);

            MetricsInitializing.Metrics.RequestCounterInc(method);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Task<TResponse> result;

            try
            {
                result = continuation(
                    new WrapperStreamReader<TRequest>(requestStream,
                        () => { MetricsInitializing.Metrics.StreamReceivedCounterInc(method); }), context);

                MetricsInitializing.Metrics.ResponseCounterInc(method, StatusCode.OK);
            }
            catch (RpcException e)
            {
                MetricsInitializing.Metrics.ResponseCounterInc(method, e.Status.StatusCode);
                throw;
            }
            finally
            {
                watch.Stop();
                if (enableLatencyMetrics)
                    MetricsInitializing.Metrics.RecordLatency(method, watch.Elapsed.TotalSeconds);
            }

            return result;
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream, ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            GrpcMethodInfo method = new GrpcMethodInfo(context.Method, MethodType.DuplexStreaming);

            MetricsInitializing.Metrics.RequestCounterInc(method);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Task result;

            try
            {
                result = continuation(
                    new WrapperStreamReader<TRequest>(requestStream,
                        () => { MetricsInitializing.Metrics.StreamReceivedCounterInc(method); }),
                    new WrapperServerStreamWriter<TResponse>(responseStream,
                        () => { MetricsInitializing.Metrics.StreamSentCounterInc(method); }), context);

                MetricsInitializing.Metrics.ResponseCounterInc(method, StatusCode.OK);

            }
            catch (RpcException e)
            {
                MetricsInitializing.Metrics.ResponseCounterInc(method, e.Status.StatusCode);

                throw;
            }
            finally
            {
                watch.Stop();
                if (enableLatencyMetrics)
                    MetricsInitializing.Metrics.RecordLatency(method, watch.Elapsed.TotalSeconds);
            }

            return result;
        }
    }
}