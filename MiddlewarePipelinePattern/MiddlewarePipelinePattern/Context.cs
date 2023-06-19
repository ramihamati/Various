namespace MiddlewarePipelinePattern;

public class Context
{
    public Dictionary<string, object> Properties { get; private set; }

    public Context()
    {
        Properties = new();
    }
}
