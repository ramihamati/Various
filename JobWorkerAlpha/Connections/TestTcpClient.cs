using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;

namespace Connections;

public class TestTcpClient<TMessage> : IDisposable, IAsyncDisposable
{
    private readonly NetworkStream _stream;
    private readonly TcpClient _client;
    private readonly IMessageSerialization<TMessage> _messageSerialization;

    internal TestTcpClient(TcpClient client,
        IMessageSerialization<TMessage> messageSerialization)
    {
        _client = client;
        _messageSerialization = messageSerialization;
        _stream = client.GetStream();
    }

    public void Dispose()
    {
        _stream.Dispose();
        _client.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _stream.DisposeAsync();
    }

    public async Task SendMessageAsync(
        TMessage message)
    {
        PipeWriter pipeWriter = PipeWriter.Create(_stream);

        byte[] messageBytes = _messageSerialization.Serialize(message);
        await pipeWriter.WriteAsync(messageBytes);
        await pipeWriter.FlushAsync();
    }


    public async Task ProcessIncoming(CancellationToken cancellationToken)
    {
        PipeReader pipeReader = PipeReader.Create(_stream);

        while (!cancellationToken.IsCancellationRequested)
        {
            ReadResult result = await pipeReader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            foreach (var segment in buffer)
            {
                byte[] messageBytes = segment.ToArray();
                string messageString = Encoding.UTF8.GetString(messageBytes);
                Console.WriteLine($"Received message from server: {messageString}");
            }

            pipeReader.AdvanceTo(buffer.End);

            // this should happen when client closes connection
            // or when there is a response indicating that the no more
            // data will be sent
            if (result.IsCompleted)
                break;
        }
    }
}