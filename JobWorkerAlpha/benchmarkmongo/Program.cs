using System.Diagnostics;
using BenchmarkDotNet.Running;
using benchmarks;

// BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);


WorkerComms comms = new();
Stopwatch stopwatch = new();
stopwatch.Start();
await comms.Setup();
await comms.AsyncInserts();

stopwatch.Stop();
Console.WriteLine(stopwatch.Elapsed.Seconds);