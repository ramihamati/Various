namespace MiddlewarePipelinePattern;

public interface IMiddleware
{
    Task HandleAsync(Context context, MiddlewareDelegate next);
}
