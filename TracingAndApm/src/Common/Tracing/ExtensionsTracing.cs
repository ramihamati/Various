using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common;

public static class ExtensionsTracing
{
    public static IServiceCollection AddOpenTelemetry<T>(this IServiceCollection services, T configuration)
        where T : IConfigurationOpenTelemetry
    {
        services.AddOpenTelemetryTracing(builder =>
        {
            builder
                .AddSource(configuration.Service)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                    .AddService(serviceName: configuration.Service, serviceVersion: configuration.Version))
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddJaegerExporter(op =>
            {
                op.AgentHost = configuration.JaegerHost;
                op.AgentPort = configuration.JaegerPort;
            })
            .AddZipkinExporter(o =>
            {
                o.Endpoint = new Uri(configuration.ZipkinUrl);
            });
        });

        return services;
    }
}
