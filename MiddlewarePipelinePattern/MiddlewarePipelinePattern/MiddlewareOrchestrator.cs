namespace MiddlewarePipelinePattern;


public class MiddlewareOrchestrator : IMiddlewareOrchestrator
{
    private readonly IMiddleware[] _middlewares;

    public MiddlewareOrchestrator(
        IEnumerable<IMiddleware> middlewares)
    {
        _middlewares = middlewares.ToArray();
    }
    public async Task HandleAsync(Context context)
    {
        MiddlewareDelegate next = ctx => InvokeNextAsync(0, ctx);
        await next(context);
    }

    private async Task InvokeNextAsync(
        int current,
        Context context)
    {
        if (current >= _middlewares.Length)
        {
            // if it reaches here, then the final middleware has called next()
            // we just return true
            return;
        }

        var nextMiddleware = _middlewares[current];

        await nextMiddleware.HandleAsync(context, ctx => InvokeNextAsync(
            ++current,
            ctx));
    }
}
