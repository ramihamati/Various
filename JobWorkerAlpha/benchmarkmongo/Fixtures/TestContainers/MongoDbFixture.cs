using DotNet.Testcontainers.Builders;
using MongoDB.Driver;
using DotNet.Testcontainers.Containers;

namespace alpha.benchmarks;

public sealed class MongoDbFixture
{
    public IContainer RawContainer { get; set; }
    public IMongoClient Connection { get; set; }
    public string ConnectionString { get; set; }

    private int _port = PortGenerator.Next();
    
    public MongoDbFixture()
    {
        RawContainer = new ContainerBuilder()
            .WithImage("mongo")
            .WithExposedPort(_port)
            .WithPortBinding(_port, 27017)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await RawContainer.StartAsync()
            .ConfigureAwait(false);
        ConnectionString = $"mongodb://127.0.0.1:{_port}/";
        Connection = new MongoClient(this.ConnectionString);
    }

    public async ValueTask DisposeAsync()
    {
        if (this.RawContainer is not null)
        {
            await this.RawContainer.DisposeAsync();
        }
    }
}