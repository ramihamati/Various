// See https://aka.ms/new-console-template for more information
using System.Net;
using Connections;

TestTcpConnection<TestMessage> connection = new TestTcpConnection<TestMessage>(
    IPAddress.Any,
    8080,
    new TestMessageSerialization());

TestTcpSever<TestMessage> server = connection.CreateServer();

await server.StartAsync();


