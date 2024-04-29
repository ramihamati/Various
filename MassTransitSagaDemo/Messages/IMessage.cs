using Microsoft.Extensions.Hosting;

namespace Messages;

[ChannelName("message")]
public interface IMessage
{
    Guid CorrelationId { get; }
}