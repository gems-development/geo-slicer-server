using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Benchmarks.Benchmarks;

namespace Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        ManualConfig config = new ManualConfig()
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddValidator(JitOptimizationsValidator.DontFailOnError)
            .AddLogger(ConsoleLogger.Default)
            .AddColumnProvider(DefaultColumnProviders.Instance)
            .AddColumn(StatisticColumn.Max);
        //BenchmarkRunner.Run<GeometryUnifierBench>(config);
        //BenchmarkRunner.Run<GeometryInnerClickBench>(config);
        //BenchmarkRunner.Run<GeometryOuterClickBench>(config);
        //BenchmarkRunner.Run<SearchGeometryIntersectsScreenBench>(config);
            //BenchmarkRunner.Run<GetGeometryIntersectionScreenBench>(config);
            BenchmarkRunner.Run<GetGeometryIntersectionDisplayWithoutLoading>(config);
            //BenchmarkRunner.Run<GetGeometryIntersectionDisplay>(config);
    }
}