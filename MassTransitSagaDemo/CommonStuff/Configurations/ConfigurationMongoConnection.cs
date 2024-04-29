using Microsoft.Extensions.Configuration;

namespace Common.Configurations;

public class ConfigurationMongoConnection
{
    public string ConnectionString { get; }

    public ConfigurationMongoConnection(
        IConfiguration configuration) 
    {
        this.ConnectionString = configuration.GetValue<string>("MongoDb:ConnectionString")
            ?? throw new Exception("Could not read the configuration for the mongo connection string");
    }
}