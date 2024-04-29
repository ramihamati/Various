using Microsoft.Extensions.Configuration;

namespace Common.Configurations;

public class ConfigurationRabbitMQ
{
    public string Host { get; }
    public int Port { get; }
    public string UserName { get; }
    public string Password { get; }
    public int ConnectionTimeoutSec { get; }

    public ConfigurationRabbitMQ(IConfiguration configuration)
    {
        this.Host =
            configuration.GetValue<string>("MQHost:Host")?.Trim()
            ?? throw new Exception("Could not read the configuration for the rabbit mq host");
        this.Port =
            configuration.GetValue<int?>("MQHost:Port")
            ?? throw new Exception("Could not read the configuration for the rabbit mq port");
        this.UserName =
            configuration.GetValue<string>("MQHost:User")?.Trim()
            ?? throw new Exception("Could not read the configuration for the rabbit mq username");
        this.Password =
            configuration.GetValue<string>("MQHost:Password")?.Trim()
            ?? throw new Exception("Could not read the configuration for the rabbit mq password");
        this.ConnectionTimeoutSec =
            configuration.GetValue<int?>("MQHost:ConnectionTimeoutSec")
            ?? throw new Exception("Could not read the configuration for the rabbit mq timeout");
    }
}