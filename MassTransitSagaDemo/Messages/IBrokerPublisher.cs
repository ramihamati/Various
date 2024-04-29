namespace Messages;

public interface IBrokerPublisher
{
    Task CommandBuyTicketAsync(Guid correlationId, string userId, string classId);
}