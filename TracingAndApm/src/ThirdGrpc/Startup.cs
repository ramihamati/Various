using Common;
using Elastic.Apm.AspNetCore;
using ThirdGrpc.Services;

namespace ThirdGrpc;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks();
        services.AddGrpc();
        services.AddCors();
        services.AddOpenTelemetry(new ConfigurationOpenTelemetry(Configuration));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseElasticApm();
        app.UseRouting();
        app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapGrpcService<HubService>();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    }
}