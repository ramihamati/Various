namespace Microsoft.Extensions.Hosting;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public class ChannelNameAttribute : Attribute
{
    public string Name { get; }

    public ChannelNameAttribute(string name)
    {
        Name = name;
    }
}