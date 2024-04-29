using Common;
using Common;

namespace ThirdGrpc;

public static class Program
{
    public static void Main(string[] args)
    {
        IHostBuilder hostBuilder = CreateHostBuilder(args);
        hostBuilder.Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.ConfigureKestrel(k =>
                {
                    ServerConfiguration.ConfigureEndpoints(k);
                });
            })
            .UseCustomSerilog();
}
