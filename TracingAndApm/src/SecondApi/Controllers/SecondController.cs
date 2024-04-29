using Microsoft.AspNetCore.Mvc;
using ThirdGrpc;

namespace SecondApi.Controllers;

[ApiController]
[Route("api/second")]
public class SecondController : ControllerBase
{
    private readonly ILogger<SecondController> _logger;
    private readonly GRPCHubService.GRPCHubServiceClient _gRPCHubService;

    public SecondController(
        ILogger<SecondController> logger,
        GRPCHubService.GRPCHubServiceClient gRPCHubService)
    {
        _logger = logger;
        this._gRPCHubService = gRPCHubService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateHub(RequestCreateHub request)
    {
        var response = await _gRPCHubService.CreateHubAsync(new rpcRequestCreateHub
        {
            HubDescription = request.HubDescription,
            HubName = request.HubName
        });

        return Ok(new ResponseCreateHub
        {
            CreatedAt = response.Hub.HubValueValue.CreatedAt.ToDateTime(),
            Description = response.Hub.HubValueValue.Description,
            Name = response.Hub.HubValueValue.Name,
            Id = response.Hub.HubValueValue.Id
        });
    }
}
