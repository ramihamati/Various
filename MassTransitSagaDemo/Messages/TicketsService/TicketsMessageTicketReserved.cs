using Microsoft.Extensions.Hosting;

namespace Messages;

[ChannelName("tickets-service:message:ticket-reserved")]
public interface ITicketsMessageTicketReserved : IMessage
{
}

public class TicketsMessageTicketReserved : ITicketsMessageTicketReserved
{
    
    public TicketsMessageTicketReserved(
        Guid correlationId)
    {
        CorrelationId = correlationId;
    }
    public Guid CorrelationId { get;  }
}

