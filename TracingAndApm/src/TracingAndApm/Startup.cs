using Common;
using Elastic.Apm.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TracingAndApm.Controllers;

namespace TracingAndApm;

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

        // create services.AddHttpClientsFromConfiguration<Service>(Configuration)
        services.AddHttpClientsFromConfiguration(Configuration);
        services.AddHttpClient<IClientSecondApi, ClientSecondApi>() // add as example
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                return sp.GetRequiredService<IHttpConfiguredClientFactory>().CreateMessageHandler("GenericHttps"); // added to interface
            }).ConfigureHttpClient(client => client.BaseAddress = new Uri(Configuration.GetValue<string>("EndPoints:SecondApi")));

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