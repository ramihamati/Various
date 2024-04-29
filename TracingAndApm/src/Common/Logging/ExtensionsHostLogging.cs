using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace Common;

public static class ExtensionsHostLogging
{
    public static IHostBuilder UseCustomSerilog(
        this IHostBuilder hostBuilder,
        Action<LoggerConfiguration>? configurator = null)
    {
        hostBuilder.UseSerilog((hostingContext, loggerConfiguration) => ConfigureLogger(
                hostingContext,
                loggerConfiguration,
                configurator));

        return hostBuilder;
    }

    public static void ConfigureLogger(
        HostBuilderContext hostingContext,
        LoggerConfiguration loggerConfiguration,
        Action<LoggerConfiguration>? configurator = null)
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
            .WriteTo.Console(theme: SystemConsoleTheme.Literate);

        if (configurator is not null)
        {
            configurator(loggerConfiguration);
        }

        if (hostingContext.Configuration.GetValue<bool?>("Logging:Elastic:Enabled") ?? false)
        {
            string elasticUrl = hostingContext.Configuration.GetValue<string>("Logging:Elastic:Url");

            ElasticsearchSinkOptions elasticOptions = new(new Uri(elasticUrl))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                MinimumLogEventLevel = LogEventLevel.Debug
            };

            hostingContext.Configuration.GetSection("Logging:Elastic")
                .Bind(elasticOptions);

            LoggingElasticSearchAuthType esAuthType = hostingContext.Configuration.GetValue<LoggingElasticSearchAuthType>("Logging:Elastic:AuthType");

            if (esAuthType == LoggingElasticSearchAuthType.ApiKey)
            {
                string elasticAuthId = hostingContext.Configuration.GetValue<string>("Logging:Elastic:Id");
                string elasticAuthApiKey = hostingContext.Configuration.GetValue<string>("Logging:Elastic:ApiKey");

                if (!string.IsNullOrEmpty(elasticUrl))
                {
                    elasticOptions.ModifyConnectionSettings = x => x.ApiKeyAuthentication(elasticAuthId, elasticAuthApiKey);
                }
            }

            loggerConfiguration.WriteTo.Elasticsearch(elasticOptions);
        }
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    }
}
