namespace Connections;

public record TestMessage
{
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }

    public override string ToString()
    {
        return $"{Content} - {Timestamp}";
    }
}