using System.Text;
using Newtonsoft.Json;

namespace Connections;

public class TestMessageSerialization : IMessageSerialization<TestMessage>
{
    public byte[] Serialize(TestMessage message)
    {
        // TODO: replace with bynary serializer or proto serializer

        return Encoding.UTF8.GetBytes(
            JsonConvert.SerializeObject(message));
    }

    public TestMessage Deserialize(byte[] message)
    {
        // TODO: replace with bynary serializer or proto serializer
        return JsonConvert.DeserializeObject<TestMessage>(Encoding.UTF8.GetString(message));
    }
}