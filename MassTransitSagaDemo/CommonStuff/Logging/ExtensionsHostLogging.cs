using Serilog;
using Serilog.Exceptions;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Microsoft.Extensions.Hosting;

public static class ExtensionsHostLogging
{
    public static IHostBuilder UseCustomSerilog(
        this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog(ConfigureLogger);
        return hostBuilder;
    }

    private static void ConfigureLogger(
        HostBuilderContext hostingContext,
        LoggerConfiguration loggerConfiguration)
    {
        IHostEnvironment env = hostingContext.HostingEnvironment;

        loggerConfiguration
            .MinimumLevel.Debug()
            .Enrich.WithProperty("ApplicationName", env.ApplicationName)
            .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .WriteTo.Console(new JsonFormatter());

        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    }
}