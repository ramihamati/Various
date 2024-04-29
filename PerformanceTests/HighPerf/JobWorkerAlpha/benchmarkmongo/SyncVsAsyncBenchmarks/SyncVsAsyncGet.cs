using alpha.benchmarks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MongoDB.Bson;

namespace benchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
// [SimpleJob(RuntimeMoniker.NativeAot80)]
public class SyncVsAsyncGet
{
    [Params(100)]
    public int N;

    private static MongoDbFixture _fixture = new();

    [GlobalSetup]
    public async Task Setup()
    {
        await _fixture.InitializeAsync();
    }

    [Benchmark]
    public async Task AsyncInserts()
    {
        var database = _fixture.Connection.GetDatabase(Guid.NewGuid().ToString());
        var collection = database.GetCollection<Person>("persons");

        for (int i = 0; i < N; i++)
        {
            await collection.InsertOneAsync(CreatePerson());
        }
    }

    [Benchmark]
    public void SyncInserts()
    {
        var database = _fixture.Connection.GetDatabase(Guid.NewGuid().ToString());
        var collection = database.GetCollection<Person>("persons");

        for (int i = 0; i < N; i++)
        {
            collection.InsertOne(CreatePerson());
        }
    }

    private Person CreatePerson()
    {
        return new Person()
        {
            Id = ObjectId.GenerateNewId(),
            FirstName = "First",
            LastName = "Last"
        };
    }
}