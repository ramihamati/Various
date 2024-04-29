using Common.Configurations;
using MassTransit;
using Messages;
using Newtonsoft.Json.Converters;

namespace TicketsService;

public class Startup
{
    private IConfiguration Configuration { get; }
    
    private static bool IsDevelopment
    {
        get
        {
            string envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;

            return envVariable == "Development"
                   || envVariable.StartsWith("Dev-")
                   || envVariable == "LinuxDevelopment";
        }
    }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();
        services
            .AddControllers()
            .AddNewtonsoftJson(opt => { opt.SerializerSettings.Converters.Add(new StringEnumConverter()); });
        
        services.AddMassTransit(cfg =>
        {
            ConfigurationRabbitMQ configurationRabbitMq = new(Configuration);

            cfg.AddConsumers(typeof(Startup).Assembly);
            
            cfg.UsingRabbitMq((context, r) =>
            {
                r.Host(configurationRabbitMq.Host, (ushort)configurationRabbitMq.Port, "/", h =>
                {
                    h.Username(configurationRabbitMq.UserName);
                    h.Password(configurationRabbitMq.Password);
                });

                r.MessageTopology.SetEntityNameFormatter(
                    new MassTransitEntityNameFormatter(r.MessageTopology.EntityNameFormatter));
                r.ConfigureEndpoints(context, new MassTransitNameFormatter());
                //r.ConfigureEndpoints(context);
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}