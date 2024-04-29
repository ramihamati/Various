using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace TracingAndApm.Controllers;

public class ClientSecondApi : IClientSecondApi
{
    private readonly HttpClient _client;

    public ClientSecondApi(
        HttpClient client)
    {
        _client = client;
    }

    public async Task<ResponseCreateHub> CreateHub(RequestCreateHub request)
    {
        StringContent content = new(JsonConvert.SerializeObject(request));
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri("api/second", UriKind.Relative));
        httpRequestMessage.Content = content;
        httpRequestMessage.Version = HttpVersion.Version20;
        var response = await _client.SendAsync(httpRequestMessage);
        return await response.Content.ReadAsAsync<ResponseCreateHub>();
    }
}