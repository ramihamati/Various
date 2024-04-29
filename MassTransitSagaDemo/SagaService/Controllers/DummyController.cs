using Microsoft.AspNetCore.Mvc;

namespace TicketsService.Controllers;


[ApiController]
[Route("api")]
public class DummyController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("tickets service");
    }
}