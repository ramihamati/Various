using System.Collections.Concurrent;

public interface IWorker
{
    Task StartAsync(WorkerContext context);
    Task StopAsync();
}

public class Worker : IWorker
{
    private readonly IWorkerJobRelay _workerJobRelay;
    private readonly CancellationToken _cancellationToken;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public Worker(
        IWorkerJobRelay workerJobRelay)
    {
        _workerJobRelay = workerJobRelay;
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
    }

    public async Task StartAsync(WorkerContext context)
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            IWorkerJob job = await _workerJobRelay.GetJobAsync();
            await job.ProcessAsync(context);
            Thread.Sleep(1);
        }
    }

    public Task StopAsync()
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}

public interface IWorkerNode
{
    string Id { get; }
    Task<string> Ping();
    void Start();
    void Stop();
}


public class WorkerNode : IWorkerNode
{
    public string Id { get; }
    private readonly IWorker _worker;
    private readonly WorkerContext context;

    public WorkerNode(
        string id,
        IWorker worker)
    {
        Id = id;
        _worker = worker;
        context = new WorkerContext(id);
    }


    public Task<string> Ping()
    {
        return Task.FromResult("pong");
    }

    public void Start()
    {
        _worker.StartAsync(context);
    }

    public void Stop()
    {
        _worker.StopAsync();
    }
}

public interface IWorkerNetwork
{
    Task RegisterAsync(IWorkerNode workerNode);
}

public class WorkerNetwork : IWorkerNetwork
{
    private readonly HashSet<IWorkerNode> _nodes
        = new(new WorkerPeerEqualityComparer());

    public IReadOnlySet<IWorkerNode> Nodes => _nodes;

    public async Task RegisterAsync(IWorkerNode workerNode)
    {
        workerNode.Start();
        _nodes.Add(workerNode);
    }

    public async Task UnregisterAsync(IWorkerNode workerNode)
    {
        var reference = _nodes.FirstOrDefault(x => x.Id == workerNode.Id);

        if (reference is not null)
        {
            reference.Stop();
            _nodes.Remove(reference);
        }
    }
}

public interface IWorkerJob
{
    Task ProcessAsync(WorkerContext context);
}

public interface IWorkerContext
{
    string Id { get; }
}


public record WorkerContext(string Id) : IWorkerContext;

public interface IWorkerJobRelay
{
    Task<IWorkerJob> GetJobAsync();
}

public class WorkerJobRelay : IWorkerJobRelay
{
    private readonly ConcurrentQueue<IWorkerJob> _jobs = new();

    public Task<IWorkerJob> GetJobAsync()
    {
        return Task.Factory.StartNew(() =>
        {
            while (true)
            {
                if (_jobs.TryDequeue(out IWorkerJob? job))
                {
                    return job;
                }

                SpinWait.SpinUntil(() => _jobs.Any(), 10);
            }
        });
    }

    public Task RegisterJobAsync(IWorkerJob workerJob)
    {
        _jobs.Enqueue(workerJob);
        return Task.CompletedTask;
    }
}