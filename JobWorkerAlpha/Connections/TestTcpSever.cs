using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Connections;

public class TestTcpSever<TMessage>
{
    private readonly IPAddress _ipAddress;
    private readonly int _port;
    private readonly IMessageSerialization<TMessage> _messageSerialization;

    internal TestTcpSever(IPAddress ipAddress,
        int port,
        IMessageSerialization<TMessage> messageSerialization)
    {
        _ipAddress = ipAddress;
        _port = port;
        _messageSerialization = messageSerialization;
    }

    public async Task StartAsync()
    {
        var listener = new TcpListener(_ipAddress, _port);
        listener.Start();

        Console.WriteLine($"Server listening on port {_port}");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            var remote = client.Client.RemoteEndPoint.Serialize().ToString();
            var handle = client.Client.Handle.ToInt64();
            Console.WriteLine($"new connection from client {remote} with handle {handle}");
            _ = ProcessClientAsync(client);
        }
    }

    private async Task ProcessClientAsync(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        PipeReader pipeReader = PipeReader.Create(stream);

        while (true)
        {
            ReadResult result = await pipeReader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            foreach (ReadOnlyMemory<byte> segment in buffer)
            {
                byte[] messageBytes = segment.ToArray();

                TMessage messageReceived = _messageSerialization
                    .Deserialize(messageBytes);
                
                Console.WriteLine(
                    $"Client: {client.Client.Handle.ToInt64()} " + $"Message: {messageReceived}");
            }

            pipeReader.AdvanceTo(buffer.End);

            if (result.IsCompleted)
                break;

            string received = $"received at {DateTime.UtcNow.ToLongDateString()}";
            await stream.WriteAsync(
                Encoding.UTF8.GetBytes(received),
                0,
                Encoding.UTF8.GetBytes(received).Length);
        }

        pipeReader.Complete();
    }
}