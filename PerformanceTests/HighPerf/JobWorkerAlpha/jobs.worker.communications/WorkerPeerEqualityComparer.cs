public class WorkerPeerEqualityComparer : IEqualityComparer<IWorkerNode>
{
    public bool Equals(IWorkerNode? x, IWorkerNode? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id;
    }

    public int GetHashCode(IWorkerNode obj)
    {
        return obj.Id.GetHashCode();
    }
}