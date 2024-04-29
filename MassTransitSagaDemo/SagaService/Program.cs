using SagaService;
using Serilog;

namespace TicketsService;

public class Program
{
    public static void Main(string[] args)
    {
        IHostBuilder hostBuilder = CreateHostBuilder(args);
        hostBuilder.Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("appsettings.json", optional: false);
                builder.AddJsonFile("appsettings.Development.json", optional: true);
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .UseCustomSerilog();
}