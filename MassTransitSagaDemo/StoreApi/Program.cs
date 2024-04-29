using Serilog;

namespace WebApplication2;

public class Program
{
    public static void Main(string[] args)
    {
        IHostBuilder hostBuilder = CreateHostBuilder(args);
        hostBuilder.Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseCustomSerilog();
}
