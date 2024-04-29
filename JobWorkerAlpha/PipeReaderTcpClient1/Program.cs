// See https://aka.ms/new-console-template for more information

using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;

var client = new TcpNetworkConnection();
var message = new Message { Content = "Hello, Server 1!", Timestamp = DateTime.Now };
await client.SendMessageAsync("127.0.0.1", 8888, message);

public class Message
{
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}

class TcpNetworkConnection
{
    // private readonly int _port;
    //
    // public TcpNetworkConnection(
    //     int port)
    // {
    //     _port = port;
    // }
    //
    //
    
    public async Task ReadAsync(NetworkStream stream)
    {
        var pipeReader = PipeReader.Create(stream);

        while (true)
        {
            var result = await pipeReader.ReadAsync();
            var buffer = result.Buffer;

            foreach (var segment in buffer)
            {
                var messageBytes = segment.ToArray();
                var messageString = Encoding.UTF8.GetString(messageBytes);
                Console.WriteLine($"Received message from server: {messageString}");

                // Here you can deserialize the messageBytes to a Message DTO if needed

                // Respond to the client (echo back)
                await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
            }

            pipeReader.AdvanceTo(buffer.End);

            // this should happen when client closes connection
            // or when there is a response indicating that the no more
            // data will be sent
            if (result.IsCompleted)
                break;
        }
    }

    public async Task SendMessageAsync(string serverIp, int port, Message message)
    {
        var client = new TcpClient();
        await client.ConnectAsync(serverIp, port);

        var stream = client.GetStream();
        

        var pipeWriter = PipeWriter.Create(stream);

        var messageString = $"{message.Content} - {message.Timestamp}";
        var messageBytes = Encoding.UTF8.GetBytes(messageString);

        await pipeWriter.WriteAsync(messageBytes);
        await pipeWriter.FlushAsync();
        await ReadAsync(stream);
    }
}