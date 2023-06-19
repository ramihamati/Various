namespace MiddlewarePipelinePattern;

public interface IMiddlewareOrchestrator
{
    Task HandleAsync(Context context);
}
