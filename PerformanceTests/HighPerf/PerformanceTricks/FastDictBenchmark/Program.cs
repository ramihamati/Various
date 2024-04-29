using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;
using Implementation;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

public struct StringComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        if (x is not null && y is not null)
        {
            return x == y;
        }

        if (x is null && y is null)
        {
            return true;
        }

        return false;
    }

    public int GetHashCode(string obj)
    {
        return obj.GetHashCode();
    }
}

public struct StringSpanComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        if (x is null && y is null)
            return true;
        if (x is null || y is null)
            return true;

        ReadOnlySpan<char> spanx = x.AsSpan();
        ReadOnlySpan<char> spany = y.AsSpan();

        if (spanx.Length != spany.Length)
        {
            return false;
        }

        for (int i = 0; i < spanx.Length; i++)
        {
            if (spanx[i] != spany[i])
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(string obj)
    {
        return obj.GetHashCode();
    }
}


// [InProcess()]
[MemoryDiagnoser]
[RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column, AllStatisticsColumn]
// [HtmlExporter]
public class FibonacciBenchmark
{
    [Params(100, 1000, 10000, 1000000)]
    public int Count { get; set; }

    public static Dictionary<string, string> NormalDictionary { get; set; } = [];

    public static FastDictionary<string, string, StringComparer> FastDictionary { get; set; }
        = new(new StringComparer());

    public static FastDictionary<string, string, StringSpanComparer> FastSpanDictionary { get; set; }
        = new(new StringSpanComparer());

    [GlobalSetup]
    public void Setup()
    {
        foreach (int i in Enumerable.Range(0, Count))
        {
            NormalDictionary.Add(i.ToString(), i.ToString());
            FastDictionary.Add(i.ToString(), i.ToString());
            FastSpanDictionary.Add(i.ToString(), i.ToString());
        }
    }

    [Benchmark]
    public void FastDict()
    {
        var getValue = FastDictionary.TryGetValue("57", out string? value);
    }


    [Benchmark]
    public void FastSpanDict()
    {
        var getValue = FastSpanDictionary.TryGetValue("57", out string? value);
    }

    [Benchmark]
    public void NormalDict()
    {
        var getValue = NormalDictionary.TryGetValue("57", out string? value);
    }
}