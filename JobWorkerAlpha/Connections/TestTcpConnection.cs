using System.Net;
using System.Net.Sockets;

namespace Connections;

public class TestTcpConnection<TMessage>(
    IPAddress ipAddress,
    int port,
    IMessageSerialization<TMessage> serialization)
{
    public TestTcpSever<TMessage> CreateServer()
    {
        return new TestTcpSever<TMessage>(
            ipAddress,
            port,
            serialization);
    }

    public async Task<TestTcpClient<TMessage>> ConnectAsync()
    {
        TcpClient client = new TcpClient();
        CancellationTokenSource cts = new();

        await client.ConnectAsync(
            ipAddress,
            port, 
            cts.Token);

        return new TestTcpClient<TMessage>(client, serialization);
    }
}