using MassTransit;
using Messages;

namespace TicketsService.Consumers.BuyTicker;

[ChannelName("consumer:tickets-service:reserve-tickets")]
public class ConsumerTicketsCommandReserveTicket : IConsumer<ITicketsCommandReserveTicket>
{
    private bool reserve = true;

    public async Task Consume(ConsumeContext<ITicketsCommandReserveTicket> context)
    {
        if (reserve)
        {
            await context.RespondAsync<ITicketsMessageTicketReserved>(
                new TicketsMessageTicketReserved(
                    context.Message.CorrelationId));
        }
        else
        {
            await context.RespondAsync<ITicketsMessageTicketNotReserved>(
                new TicketsMessageTicketNotReserved(
                    context.Message.CorrelationId,
                    "Tickets sold out"));
        }
    }
}