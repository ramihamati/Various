namespace Common;

public interface IConfigurationOpenTelemetry
{
    string Service { get; }
    string Version { get; }
    string JaegerHost { get; }
    int JaegerPort { get; }
    string ZipkinUrl { get; }
}
