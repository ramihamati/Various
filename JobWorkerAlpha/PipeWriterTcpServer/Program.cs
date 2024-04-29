// See https://aka.ms/new-console-template for more information

using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;

var server = new TcpServer();
await server.StartServerAsync(8888);

class TcpServer
{
    public async Task StartServerAsync(int port)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        Console.WriteLine($"Server listening on port {port}");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = ProcessClientAsync(client);
        }
    }

    private async Task ProcessClientAsync(TcpClient client)
    {
        var stream = client.GetStream();
        var pipeReader = PipeReader.Create(stream);

        while (true)
        {
            var result = await pipeReader.ReadAsync();
            var buffer = result.Buffer;

            foreach (var segment in buffer)
            {
                var messageBytes = segment.ToArray();
                var messageString = Encoding.UTF8.GetString(messageBytes);
                Console.WriteLine($"Received message from client: {messageString}");

                // Here you can deserialize the messageBytes to a Message DTO if needed

                // Respond to the client (echo back)
                await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
            }

            pipeReader.AdvanceTo(buffer.End);

            if (result.IsCompleted)
                break;
        }

        pipeReader.Complete();
    }
}
