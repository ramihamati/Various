using Microsoft.Extensions.Hosting;

namespace Messages;

[ChannelName("basket-service:command:create-basket")]
public interface IBasketCommandCreateBasket : IMessage
{
    public string UserId { get;  }
}

public class BasketCommandCreateBasket : IBasketCommandCreateBasket
{
    public BasketCommandCreateBasket(
        Guid correlationId,
        string userId)
    {
        CorrelationId = correlationId;
        UserId = userId;
    }

    public string UserId { get;  }
    public Guid CorrelationId { get;  }
}
