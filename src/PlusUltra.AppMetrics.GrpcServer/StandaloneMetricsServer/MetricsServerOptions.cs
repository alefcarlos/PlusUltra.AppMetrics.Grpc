using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace  PlusUltra.AppMetrics.GrpcServer.StandaloneMetricsServer
{
    public class MetricsServerOptions
    {
        public Action<KestrelServerOptions> ConfigureServerOptions { get; set; } = o => { };
    }
}