using Microsoft.Extensions.Configuration;

namespace Common;

public class ConfigurationOpenTelemetry
    : IConfigurationOpenTelemetry
{
    public string Service { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string JaegerHost { get; set; }
    public int JaegerPort { get; set; }
    public string ZipkinUrl { get; }
    public ConfigurationOpenTelemetry(
        IConfiguration configuration)
    {
        this.Service = configuration.GetValue<string>("OpenTelemetry:Service");
        this.Version = configuration.GetValue<string>("OpenTelemetry:Version"); 
        this.JaegerHost = configuration.GetValue<string>("OpenTelemetry:Jaeger:Host");
        this.JaegerPort = configuration.GetValue<int>("OpenTelemetry:Jaeger:Port");
        this.ZipkinUrl = configuration.GetValue<string>("OpenTelemetry:ZipkinUrl");
    }
}
