// See https://aka.ms/new-console-template for more information

using System.Net;
using Connections;

TestTcpConnection<TestMessage> connection = new TestTcpConnection<TestMessage>(
    IPAddress.Loopback,
    8080,
    new TestMessageSerialization());

TestTcpClient<TestMessage> client = await connection.ConnectAsync();

CancellationTokenSource cts = new();

client.ProcessIncoming(cts.Token);

while (!cts.IsCancellationRequested)
{
    Console.WriteLine("Send:");
    var input = Console.ReadLine();

    await client.SendMessageAsync(new TestMessage()
    {
        Content = input ?? "empty",
        Timestamp = DateTime.UtcNow
    });
}


SpinWait.SpinUntil(() => cts.IsCancellationRequested);