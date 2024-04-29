namespace Common;

public class ServerSettings
{
    public int? MaxConcurrentConnections { get; set; }

    public int? MaxConcurrentUpgradedConnections { get; set; }

    public int? MaxRequestBodySize { get; set; }
}
