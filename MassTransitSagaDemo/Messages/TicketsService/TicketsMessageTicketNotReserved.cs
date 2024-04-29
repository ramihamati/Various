using Microsoft.Extensions.Hosting;

namespace Messages;


[ChannelName("tickets-service:message:ticket-not-reserved")]
public interface ITicketsMessageTicketNotReserved : IMessage
{
    string Reason { get;  }
}

public class TicketsMessageTicketNotReserved : ITicketsMessageTicketNotReserved
{
    public string Reason { get;  }
    public Guid CorrelationId { get;  }
    
    public TicketsMessageTicketNotReserved(
        Guid correlationId,
        string reason)
    {
        CorrelationId = correlationId;
        Reason = reason;
    }
}