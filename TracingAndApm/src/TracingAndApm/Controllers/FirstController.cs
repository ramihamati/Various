using Microsoft.AspNetCore.Mvc;

namespace TracingAndApm.Controllers;

[ApiController]
[Route("api/first")]
public class FirstController : ControllerBase
{
    private readonly ILogger<FirstController> _logger;
    private readonly IClientSecondApi _clientSecondApi;

    public FirstController(
        ILogger<FirstController> logger,
        IClientSecondApi clientSecondApi)
    {
        _logger = logger;
        this._clientSecondApi = clientSecondApi;
    }

    [HttpPost]
    public async Task<IActionResult> CreateHub(RequestCreateHub request)
    {
        var response = await _clientSecondApi.CreateHub(request);
        return Ok(response);
    }
}
