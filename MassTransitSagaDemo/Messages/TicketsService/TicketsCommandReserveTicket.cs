using Microsoft.Extensions.Hosting;

namespace Messages;

[ChannelName("tickets-service:command:reserve-ticket")]
public interface ITicketsCommandReserveTicket : IMessage
{
    public string UserId { get;  }
    public string CourseId { get; }
}

public class TicketsCommandReserveTicket : ITicketsCommandReserveTicket
{
    public TicketsCommandReserveTicket(
        Guid correlationId,
        string userId, 
        string courseId)
    {
        CorrelationId = correlationId;
        UserId = userId;
        CourseId = courseId;
    }

    public string UserId { get;  }
    public Guid CorrelationId { get;  }
    public string CourseId { get; }
}