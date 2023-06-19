using Microsoft.Extensions.DependencyInjection;

namespace MiddlewarePipelinePattern;

internal class Program
{
    static async Task Main(string[] args)
    {
        ServiceCollection services = new();
        services.AddScoped<IMiddleware, MiddlewareOne>();
        services.AddScoped<IMiddleware, MiddlewareTwo>();
        services.AddScoped<IMiddlewareOrchestrator, MiddlewareOrchestrator>();
        ServiceProvider provider = services.BuildServiceProvider();

        IMiddlewareOrchestrator orchestrator
            = provider.GetRequiredService<IMiddlewareOrchestrator>();

        var context = new Context();
        await orchestrator.HandleAsync(context);

        Console.WriteLine("Hello, World!");
    }
}