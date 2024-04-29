using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

[MemoryDiagnoser]
[RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column, AllStatisticsColumn]
// [HtmlExporter]
public class PoolBenchmark
{
    [Params(100, 1000, 10000, 1000000)]
    public int Count { get; set; }

    [Benchmark]
    public void ListTest()
    {
        PopulateList(Count, 100);
    }

    [Benchmark]
    public void PoolTest()
    {
        PopulateSharedArray(Count, 100);
    }

    public void PopulateSharedArray(int size, int numberOfTimes)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            int[] array = ArrayPool<int>.Shared.Rent(size);
            int counter = 0; // always needed because the pool is not exact size and to avoid clearing
            for (int j = 0; j < size; j++)
            {
                array[j] = j;
                counter++;
            }
            
            ArrayPool<int>.Shared.Return(array, false);
        }
    }
    
    public void PopulateList(int size, int numberOfTimes)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            List<int> list = new(size);
            for (int j = 0; j < size; j++)
            {
                list.Add(j);
            }
        }
    }
}