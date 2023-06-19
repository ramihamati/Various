namespace MiddlewarePipelinePattern;

public class MiddlewareOne : IMiddleware
{
    public async Task HandleAsync(Context context, MiddlewareDelegate next)
    {
        context.Properties.Add("MiddlewareOne", "Reached");
        await next(context);
    }
}
