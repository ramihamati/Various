using MassTransit;

namespace Messages;

public class BrokerPublisher : IBrokerPublisher
{
    private readonly IPublishEndpoint _busControl;

    public BrokerPublisher(IPublishEndpoint busControl)
    {
        _busControl = busControl;
    }

    public Task CommandBuyTicketAsync(Guid correlationId, string userId, string classId)
    {
        return _busControl.Publish<ISagaCommandBuyTicket>(new SagaCommandBuyTicket(
            correlationId,
            userId,
            classId));
    }
}