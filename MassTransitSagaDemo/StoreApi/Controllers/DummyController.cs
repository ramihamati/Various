using MassTransit.RabbitMqTransport;
using Messages;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers;

[ApiController]
[Route("api")]
public class DummyController : ControllerBase
{
    private readonly IBrokerPublisher _publisher;

    public DummyController(
        IBrokerPublisher publisher)
    {
        _publisher = publisher;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("hello");
    }

    [HttpPost("tickets/buy")]
    public async Task<IActionResult> BuyTickets(
        [FromBody] ApiModelBuyTickets model)
    {
        try
        {
            Guid correlationId = Guid.NewGuid();

            await _publisher.CommandBuyTicketAsync(
                correlationId,
                model.UserId,
                model.ClassId);
        
            return Ok(correlationId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}