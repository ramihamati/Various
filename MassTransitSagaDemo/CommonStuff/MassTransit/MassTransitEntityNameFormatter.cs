using System.Reflection;
using MassTransit;

namespace Microsoft.Extensions.Hosting;

public class MassTransitEntityNameFormatter : IEntityNameFormatter
{
    private readonly MethodInfo formatMethod;
    private readonly IEntityNameFormatter _formatter;

    public MassTransitEntityNameFormatter(IEntityNameFormatter formatter)
    {
        formatMethod = typeof(MassTransitEntityNameFormatter).GetMethod(nameof(FormatEntityName));
        _formatter = formatter;
    }

    public string FormatEntityName<T>()
    {
        Type type = typeof(T);


        if (type.IsAssignableTo(typeof(Fault)))
        {
            if (type.IsGenericType)
            {
                List<string> names = new List<string>() { "demo:fault" };

                foreach (var genericType in type.GetGenericArguments())
                {
                    names.Add(
                        (string)formatMethod.MakeGenericMethod(genericType).Invoke(this, Array.Empty<object>())
                    );
                }

                return string.Join(":", names);
            }
            else
            {
                return _formatter.FormatEntityName<T>();
            }
        }

        ChannelNameAttribute channelNameAttr = type.GetCustomAttribute<ChannelNameAttribute>();
        if (channelNameAttr is null)
        {
            throw new Exception($"The type {type.FullName} is not decorated with {typeof(ChannelNameAttribute).FullName}");
        }

        string name = type.GetCustomAttribute<ChannelNameAttribute>().Name;

        return $"demo:mt:{name}";
    }
}