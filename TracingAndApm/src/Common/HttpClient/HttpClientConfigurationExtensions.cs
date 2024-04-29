using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public interface IService
{

}
public class MyService : IService
{

}

public static class HttpClientConfigurationExtensions
{
    public static void AddConfiguredHttpClient<TClient>(
        this IServiceCollection services,
        string name) where TClient : class
    {
        services.AddHttpClient<TClient>()
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                return sp.GetRequiredService<IHttpConfiguredClientFactory>()
                    .CreateMessageHandler(name);
            });
    }

    public static void AddConfiguredHttpClient<TClient, TImplementation>(
        this IServiceCollection services,
        string name)
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddHttpClient<TClient, TImplementation>()
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                return sp.GetRequiredService<IHttpConfiguredClientFactory>()
                    .CreateMessageHandler(name);
            });
    }

    public static void AddHttpClientsFromConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //get the settings
        List<HttpClientConfiguration> httpClientConfigurations = new();

        configuration.GetSection("HttpClients").Bind(httpClientConfigurations);

        if (httpClientConfigurations is null)
        {
            throw new Exception("Could not bind section HttpClients from appconfig.json. Did you defined it?");
        }

        //create the message handlers
        Dictionary<string, Func<HttpMessageHandler>> messageHandlers = new Dictionary<string, Func<HttpMessageHandler>>();

        for (int i = 0; i < httpClientConfigurations.Count; i++)
        {
            HttpClientConfiguration conf = httpClientConfigurations[i];

            if (conf is null)
            {
                throw new Exception($"The http client configuration at index[{i}] is null");
            }
            if (string.IsNullOrEmpty(conf.Name))
            {
                throw new Exception($"The http client configuration at index[{i}] does not define a name");
            }

            messageHandlers.Add(
              conf.Name,
              () => HttpClientConfigurationHelper.GetHttpMessageHandler(conf));
        }

        //add the service we will use to generate the client
        services.AddTransient<IHttpConfiguredClientFactory, HttpConfiguredClientFactory>(sp =>
        {
            return new HttpConfiguredClientFactory(
                sp.GetRequiredService<IHttpClientFactory>(),
                httpClientConfigurations,
                messageHandlers);
        });

        //register http clients
        httpClientConfigurations.ForEach(conf =>
        {
            if (string.Equals(conf.Scheme, HttpClientConfigurationOptions.CONF_SCHEME_HTTP, StringComparison.OrdinalIgnoreCase))
            {
                var clientBuilder = services.AddHttpClient(conf.Name);

                if (!string.IsNullOrWhiteSpace(conf.DefaultRequestVersion))
                {
                    clientBuilder.ConfigureHttpClient(client =>
                    {
                        client.DefaultRequestVersion = new Version(conf.DefaultRequestVersion);
                    });
                }
            }
            else if (string.Equals(conf.Scheme, HttpClientConfigurationOptions.CONF_SCHEME_HTTPS, StringComparison.OrdinalIgnoreCase))
            {
                var clientBuilder = services.AddHttpClient(conf.Name);
                clientBuilder.ConfigurePrimaryHttpMessageHandler(messageHandlers[conf.Name!]);

                if (!string.IsNullOrWhiteSpace(conf.DefaultRequestVersion))
                {
                    clientBuilder.ConfigureHttpClient(client =>
                    {
                        client.DefaultRequestVersion = new Version(conf.DefaultRequestVersion);
                    });
                }
            }
            else
            {
                throw new Exception("Invalid http scheme. Allowed only http or https");
            }
        });
    }
}
