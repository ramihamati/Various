namespace MiddlewarePipelinePattern;

public class MiddlewareTwo : IMiddleware
{
    public async Task HandleAsync(Context context, MiddlewareDelegate next)
    {
        context.Properties.Add("MiddlewareTwo", "Reached");
        await next(context);
    }
}
