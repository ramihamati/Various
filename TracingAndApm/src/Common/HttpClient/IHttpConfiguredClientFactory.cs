namespace Microsoft.Extensions.DependencyInjection;

public interface IHttpConfiguredClientFactory
{
    HttpClient CreateClient(string name);
    HttpMessageHandler CreateMessageHandler(string name);
}
