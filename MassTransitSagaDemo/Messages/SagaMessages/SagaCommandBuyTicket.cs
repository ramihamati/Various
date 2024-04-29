using Microsoft.Extensions.Hosting;

namespace Messages;

[ChannelName("saga:command-buy-ticket")]
public interface ISagaCommandBuyTicket : IMessage
{
    public string UserId { get;  }
    public string CourseId { get; }
}

public class SagaCommandBuyTicket : ISagaCommandBuyTicket
{
    public SagaCommandBuyTicket(
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
