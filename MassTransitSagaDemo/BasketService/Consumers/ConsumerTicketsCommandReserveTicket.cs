using MassTransit;
using Messages;

namespace BasketService.Consumers;


[ChannelName("consumer:basket-service:create-basket")]
public class ConsumerCommandCreateBasket : IConsumer<IBasketCommandCreateBasket>
{
    private bool create = true;
    
    public async Task Consume(ConsumeContext<IBasketCommandCreateBasket> context)
    {
        if (create)
        {
            await context.RespondAsync<IBasketMessageBasketCreated>(new BasketMessageBasketCreated(
                context.Message.CorrelationId,
                $"basket-{context.Message.CorrelationId}"));    
        }
        else
        {
            await context.RespondAsync<IBasketMessageBasketNotCreated>(new BasketMessageBasketNotCreated(
                context.Message.CorrelationId,
                "Basket error"));    
        }
    }
}