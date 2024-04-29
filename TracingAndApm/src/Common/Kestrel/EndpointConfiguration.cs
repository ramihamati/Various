namespace Common;

public class EndpointConfiguration
{
    public string? OS { get; set; }

    public string? Host { get; set; }

    public int? Port { get; set; }

    public string? Scheme { get; set; }

    public string? CertSource { get; set; }

    public string? CertStoreName { get; set; }

    public string? CertStoreLocation { get; set; }

    public string? CertFilePath { get; set; }

    public string? CertPasswordSource { get; set; }

    public string? CertPassword { get; set; }

    public string? CertPasswordEnvKey { get; set; }

    public string? Thumbprint { get; internal set; }

    public string[]? ListenProtocols { get; set; }

    public string WinOSLocalHostFamilyRange { get; set; } = "192";

}
