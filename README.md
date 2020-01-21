[![Build Status](https://alefcarlos.visualstudio.com/PlusUltra/_apis/build/status/alefcarlos.PlusUltra.AppMetrics.Grpc?branchName=master)](https://alefcarlos.visualstudio.com/PlusUltra/_build/latest?definitionId=23&branchName=master)
![Nuget](https://img.shields.io/nuget/v/PlusUltra.AppMetrics.Grpc.Server)

# C# gRPC interceptors for AppMetrics Prometheus monitoring

This project was inspired by [e-conomic/csharp-grpc-prometheus](https://github.com/e-conomic/csharp-grpc-prometheus) 

# Usage

Add AppMetrics standlone server

```sharp
services.AddStandaloneMetricsServer(options => {
    options.ListenAnyIP(1010); //here you can set target port
    options.AllowSynchronousIO = true;
});
```

Configure interceptor globaly

```csharp          
services.AddGrpc(options =>
{
    options.Interceptors.Add<ServerMetricsInterceptor>();
});
```

Open the url https://localhost:1010/metrics