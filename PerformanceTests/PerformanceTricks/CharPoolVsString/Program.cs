using System.Buffers;
using System.IO.Pipelines;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

// BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
var config = ManualConfig.Create(DefaultConfig.Instance);
BenchmarkRunner.Run<PoolBenchmark>(config);

public class AntiVirusFriendlyConfig : ManualConfig
{
    public AntiVirusFriendlyConfig()
    {
        AddJob(Job.MediumRun
            .WithToolchain(InProcessEmitToolchain.Instance));
    }
}

// var b = new PoolBenchmark();
// b.Setup();
// await b.StreamReadTest();
[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
[RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column, AllStatisticsColumn]
public class PoolBenchmark
{
    public Stream Stream { get; set; } = null!;


    [GlobalSetup]
    public void Setup()
    {
        // var text = File.ReadAllText("D:\\git-shared\\highperf\\HighPerf\\PerformanceTricks\\CharPoolVsString\\LargeText.txt");
        // using (MemoryStream memoryStream = new MemoryStream())
        // Stream = new MemoryStream();
        // var writer = new StreamWriter(Stream);
        // writer.Write(text);
        // writer.Flush();
        // Stream.Position = 0;
        
        // we can't emit IL (antivirus) so we can read from assembly
         this.Stream = Assembly.GetExecutingAssembly()
             .GetManifestResourceStream("CharPoolVsString.LargeText.txt")!;
    }


    [Benchmark]
    public async Task StreamReadTest()
    {
        await StreamRead();
    }

    [Benchmark]
    public async Task PipReadTest()
    {
        await PipeRead();
    }

    public async Task PipeRead()
    {
        PipeReader reader = PipeReader.Create(Stream);
        CancellationTokenSource cts = new();
        string? line;
        do
        {
            ReadResult result = await reader.ReadAsync();

            line = Encoding.UTF8.GetString(result.Buffer.ToArray());
            reader.AdvanceTo(result.Buffer.End);

            if (result.IsCompleted)
            {
                cts.Cancel();
            }
        } while (!cts.Token.IsCancellationRequested);
    }

    public async Task StreamRead()
    {
        using var reader = new StreamReader(Stream);
        CancellationTokenSource cts = new();
        string? line;
        int readSize = 0;
        using IMemoryOwner<char> buffer = MemoryPool<char>.Shared.Rent(4096);

        do
        {
            readSize = await reader.ReadAsync(buffer.Memory);
            if (readSize == 0)
            {
                cts.Cancel();
            }
            else
            {
                line = buffer.Memory.Slice(0, readSize).ToString();
            }
        } while (!cts.Token.IsCancellationRequested);
    }
}