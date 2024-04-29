using Common;
using Elastic.Apm.AspNetCore;
using ThirdGrpc;

namespace SecondApi;

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
        services.AddControllers().AddNewtonsoftJson();
        services.AddCors();
        services.AddHealthChecks();
        services.AddHttpClientsFromConfiguration(Configuration);

        services.AddGrpcClient<GRPCHubService.GRPCHubServiceClient>(options =>
        {
            options.Address = new System.Uri(Configuration["EndPoints:ThirdGrpc"]);
        }).ConfigurePrimaryHttpMessageHandler(() => HttpConfiguredClientFactory.CreateMessageHandler("GenericHttps", Configuration));
        
        services.AddOpenTelemetry(new ConfigurationOpenTelemetry(Configuration));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseElasticApm();
        app.UseRouting();
        app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}