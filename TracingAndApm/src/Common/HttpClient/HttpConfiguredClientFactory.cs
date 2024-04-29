using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public class HttpConfiguredClientFactory : IHttpConfiguredClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly List<HttpClientConfiguration> _configurations;
    private readonly Dictionary<string, Func<HttpMessageHandler>> _messageHandlerFactory;

    public HttpConfiguredClientFactory(
        IHttpClientFactory httpClientFactory,
        List<HttpClientConfiguration> configurations,
        Dictionary<string, Func<HttpMessageHandler>> messageHandlerFactory)
    {
        _httpClientFactory = httpClientFactory;
        _configurations = configurations;
        _messageHandlerFactory = messageHandlerFactory;
    }

    /// <summary>
    /// Using this method instead of <see cref="IHttpClientFactory.CreateClient(string)"/> for configured 
    /// clients is better because we can throw an exception if the name was not found. The factory by default does not warn.
    /// </summary>
    /// <param name="name"></param>
    public HttpClient CreateClient(string name)
    {
        if (!_configurations.Any(x => x.Name == name))
        {
            throw new Exception($"Could not find a named HttpClient with the name \'{name}\'");
        }

        return _httpClientFactory.CreateClient(name);
    }

    public HttpMessageHandler CreateMessageHandler(string name)
    {
        if (!_configurations.Any(x => x.Name == name))
        {
            throw new Exception($"Could not find a named HttpClient with the name \'{name}\'");
        }

        return _messageHandlerFactory[name]();
    }

    public static HttpMessageHandler CreateMessageHandler(string name, IConfiguration configuration)
    {
        List<HttpClientConfiguration> httpClientConfigurations = new List<HttpClientConfiguration>();

        configuration.GetSection("HttpClients").Bind(httpClientConfigurations);

        if (httpClientConfigurations is null)
        {
            throw new Exception("Could not bind section HttpClients from appconfig.json. Did you defined it?");
        }

        var clientConfiguration = httpClientConfigurations.Find(x => x.Name == name);

        if (clientConfiguration is null)
        {
            throw new Exception($"Could not find client configuration with the name \'{name}\'");
        }

        return HttpClientConfigurationHelper.GetHttpMessageHandler(clientConfiguration);
    }

    public static HttpMessageHandler CreateMessageHandler(HttpClientConfiguration configuration)
    {
        return HttpClientConfigurationHelper.GetHttpMessageHandler(configuration);
    }

    /// <summary>
    /// The configuration may contain information about <see cref="HttpClient.DefaultRequestVersion"/>
    /// </summary>
    public static HttpClient CreateClient(string name, IConfiguration configuration)
    {
        List<HttpClientConfiguration> httpClientConfigurations = new List<HttpClientConfiguration>();

        configuration.GetSection("HttpClients").Bind(httpClientConfigurations);

        if (httpClientConfigurations is null)
        {
            throw new Exception("Could not bind section HttpClients from appconfig.json. Did you defined it?");
        }

        HttpClientConfiguration? clientConfiguration = httpClientConfigurations
            .Find(x => x.Name == name);

        if (clientConfiguration is null)
        {
            throw new Exception($"Could not find client configuration with the name \'{name}\'");
        }

        HttpMessageHandler messageHandler = HttpClientConfigurationHelper.GetHttpMessageHandler(clientConfiguration);
        HttpClient client = new(messageHandler);

        if (!string.IsNullOrWhiteSpace(clientConfiguration.DefaultRequestVersion))
        {
            client.DefaultRequestVersion = new Version(clientConfiguration.DefaultRequestVersion);
        }

        return client;
    }

    /// <summary>
    /// The configuration may contain information about <see cref="HttpClient.DefaultRequestVersion"/>
    /// </summary>
    public static HttpClient CreateClient(HttpClientConfiguration configuration)
    {
        HttpMessageHandler messageHandler = HttpClientConfigurationHelper.GetHttpMessageHandler(configuration);
        HttpClient client = new HttpClient(messageHandler);

        if (!string.IsNullOrWhiteSpace(configuration.DefaultRequestVersion))
        {
            client.DefaultRequestVersion = new Version(configuration.DefaultRequestVersion);
        }

        return client;
    }
}
