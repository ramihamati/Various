using alpha.benchmarks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MongoDB.Bson;

namespace benchmarks;

public class WorkerPersonInsert : IWorkerJob
{
    private readonly Person _person;
    private readonly MongoDbFixture _fixture;

    public WorkerPersonInsert(
        Person person,
        MongoDbFixture _fixture)
    {
        _person = person;
        this._fixture = _fixture;
    }

    public async Task ProcessAsync(WorkerContext context)
    {
        Console.WriteLine(context.Id);
        var database = _fixture.Connection.GetDatabase(Guid.NewGuid().ToString());
        var collection = database.GetCollection<Person>("persons");
        await collection.InsertOneAsync(_person);
    }
}

[SimpleJob(RuntimeMoniker.Net80)]
// [SimpleJob(RuntimeMoniker.NativeAot80)]
public class WorkerComms
{
    private static MongoDbFixture _fixture = new();
    private static WorkerJobRelay _jobRelay = new();
    private static WorkerNetwork _workerNetwork = new();

    public async Task Setup()
    {
        await _fixture.InitializeAsync();
    }

    public async Task AsyncInserts()
    {
        for (int i = 0; i < 20; i++)
        {
            Console.WriteLine("registering worker");
            
            await _workerNetwork.RegisterAsync(new WorkerNode(
                Guid.NewGuid().ToString(),
                new Worker(_jobRelay)));
        }

        for (int i = 0; i < 1000; i++)
        {
            await _jobRelay.RegisterJobAsync(new WorkerPersonInsert(
                new Person()
                {
                    Id = ObjectId.GenerateNewId(),
                    FirstName = "",
                    LastName = ""
                }, _fixture));
        }
    }
}