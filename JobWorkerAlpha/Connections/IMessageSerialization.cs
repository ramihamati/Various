using System.Text.Json.Serialization;

namespace Connections;

public interface IMessageSerialization<T>
{
    public byte[] Serialize(T message);
    public T Deserialize(byte[] message);
}