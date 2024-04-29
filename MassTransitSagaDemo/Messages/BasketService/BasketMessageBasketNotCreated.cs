using Microsoft.Extensions.Hosting;

namespace Messages;

[ChannelName("basket-service:message:basket-not-created")]
public interface IBasketMessageBasketNotCreated : IMessage
{
    public string Reason { get;  }
}

public class BasketMessageBasketNotCreated : IBasketMessageBasketNotCreated
{
    public BasketMessageBasketNotCreated(
        Guid correlationId,
        string reason)
    {
        CorrelationId = correlationId;
        Reason = reason;
    }

    public string Reason { get; }
    public Guid CorrelationId { get;  }
}