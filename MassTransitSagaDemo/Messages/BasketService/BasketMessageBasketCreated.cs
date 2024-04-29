using Microsoft.Extensions.Hosting;

namespace Messages;


[ChannelName("basket-service:message:basket-created")]
public interface IBasketMessageBasketCreated : IMessage
{
    public string BasketId { get;  }
}

public class BasketMessageBasketCreated : IBasketMessageBasketCreated
{
    public BasketMessageBasketCreated(
        Guid correlationId,
        string basketId)
    {
        CorrelationId = correlationId;
        BasketId = basketId;
    }

    public string BasketId { get;  }
    public Guid CorrelationId { get;  }
}