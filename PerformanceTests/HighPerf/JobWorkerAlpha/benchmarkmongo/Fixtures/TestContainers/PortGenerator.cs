namespace alpha.benchmarks;

public static class PortGenerator
{
    private static int _port = 12000;

    public static int Next()
    {
        _port += Random.Shared.Next(1, 1000) ;
        return _port;
    }
}