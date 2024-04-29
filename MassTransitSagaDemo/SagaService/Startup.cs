using Common.Configurations;
using MassTransit;
using Messages;
using Newtonsoft.Json.Converters;

namespace SagaService;

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
        
        services.AddScoped<IBrokerPublisher, BrokerPublisher>();

        AddCustomMassTransit(services, Configuration);
    }
    
    public static IServiceCollection AddCustomMassTransit(
        IServiceCollection services,
        IConfiguration configuration)
    {
        ConfigurationRabbitMQ settingsRabbitMQ = new ConfigurationRabbitMQ(configuration);
        ConfigurationMongoConnection settingsMongoConnection = new ConfigurationMongoConnection(configuration);

        services.AddMassTransit(cfg =>
        {
            // cfg.AddConsumer<BuyTicketFaultReserve>();

            cfg.AddSagaStateMachine<BuyTicketSaga, BuyTicketSagaState>()
                .MongoDbRepository(r =>
                {
                    r.Connection = settingsMongoConnection.ConnectionString;
                    r.DatabaseName = "SagaState";
                });

            cfg.UsingRabbitMq((context, r) =>
            {
                r.Host(settingsRabbitMQ.Host, (ushort)settingsRabbitMQ.Port, "/", h =>
                {
                    h.Username(settingsRabbitMQ.UserName);
                    h.Password(settingsRabbitMQ.Password);
                });
                
                r.MessageTopology.SetEntityNameFormatter(
                    new MassTransitEntityNameFormatter(r.MessageTopology.EntityNameFormatter));
                r.ConfigureEndpoints(context, new MassTransitNameFormatter());
            });
        });


        return services;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}