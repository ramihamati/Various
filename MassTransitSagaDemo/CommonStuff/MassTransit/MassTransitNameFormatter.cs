using System.Reflection;
using MassTransit;

namespace Microsoft.Extensions.Hosting;

public class MassTransitNameFormatter : IEndpointNameFormatter
{
    private char sepparator = '-';
    public string Separator { get; } = "-";


    public string TemporaryEndpoint(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            tag = "endpoint";

        return $"tmp:{tag}:{Guid.NewGuid()}";
    }

    public string Consumer<T>()
        where T : class, IConsumer
    {
        return FormatName("consumer", typeof(T));
    }

    public string Message<T>()
        where T : class
    {
        return FormatName("message", typeof(T));
    }

    public string Saga<T>()
        where T : class, ISaga
    {
        return FormatName("saga", typeof(T));
    }

    public string ExecuteActivity<T, TArguments>()
        where T : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        return FormatName($"activity:execute", typeof(T));
    }

    public string CompensateActivity<T, TLog>()
        where T : class, ICompensateActivity<TLog>
        where TLog : class
    {
        return FormatName($"activity:compensate", typeof(T));
    }

    public string SanitizeName(string name)
    {
        return name.ToLowerInvariant();
    }

    private string FormatName(string prefix, Type type)
    {
        if (prefix.EndsWith(Separator))
        {
            prefix = prefix[0..^1];
        }

        ChannelNameAttribute? channelNameAttr = type.GetCustomAttribute<ChannelNameAttribute>();
        if (channelNameAttr is null)
        {
            throw new Exception($"The type {type.FullName} is not decorated with {typeof(ChannelNameAttribute).FullName}");
        }

        string name = channelNameAttr.Name;

        name = SanitizeName(name);

        if (!name.StartsWith(prefix))
        {
            return $"demo:mt:{prefix}:{name}";
        }
        else
        {
            return $"demo:mt:{name}";
        }
    }
}