using System.Diagnostics;
using System.Text.Json;
using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Settings;

namespace BadScript2.PipelinePerf;

internal static class Program
{
    private static readonly JsonSerializerOptions s_JsonOptions = new()
    {
        WriteIndented = true
    };

     private static int Main(string[] args)
     {
         try
         {
             PerfOptions options = PerfOptions.Parse(args);
             List<string> files = ResolveFileInputs(options);

             if (files.Count == 0)
             {
                 throw new ArgumentException("No input files resolved. Pass --file and/or --glob.");
             }

             List<BenchmarkTarget> targets = options.ResolveTargets();
             if (targets.Count == 0)
             {
                 throw new ArgumentException("No benchmark targets resolved. Pass --target or use defaults.");
             }

             string srcRoot = FindSourceRoot();
             string artifactDirectory = Path.Combine(srcRoot, "artifacts", "benchmarks", "pipeline-perf");
             Directory.CreateDirectory(artifactDirectory);

             // The perf harness toggles editable optimization settings. Ensure a root settings store exists.
             if (!BadSettingsProvider.HasRootSettings)
             {
                 BadSettingsProvider.SetRootSettings(new BadSettings("pipeline-perf"));
             }

             // Save original settings
             bool originalSlotValue = BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath;
             bool originalAdaptiveBurst = BadNativeOptimizationSettings.Instance.UseAdaptiveVmBurstSize;
             int originalVmBurstSize = BadNativeOptimizationSettings.Instance.VmBurstSize;
             List<PipelineStageResult> results = new();

             try
             {
                 foreach (string file in files)
                 {
                     Console.WriteLine($"File: {file}");
                     string source = File.ReadAllText(file);

                     foreach (BenchmarkTarget target in targets)
                     {
                         // Reset settings that may leak between targets (adaptive burst, etc.)
                         BadNativeOptimizationSettings.Instance.UseAdaptiveVmBurstSize = false;
                         BadNativeOptimizationSettings.Instance.VmBurstSize = originalVmBurstSize;
                         // Apply target settings
                         target.ConfigureSettings(BadNativeOptimizationSettings.Instance);
                         Console.WriteLine($"  Target: {target.Name}");

                         foreach (PipelineStage stage in options.Stages)
                         {
                             PipelineStageResult result = RunStageSafe(file, source, stage, target, options);
                             results.Add(result);
                             PrintStageSummary(result);
                         }
                     }
                 }
             }
             finally
             {
                 BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath = originalSlotValue;
                 BadNativeOptimizationSettings.Instance.UseAdaptiveVmBurstSize = originalAdaptiveBurst;
                 BadNativeOptimizationSettings.Instance.VmBurstSize = originalVmBurstSize;
             }

             PipelinePerfReport report = new(DateTimeOffset.UtcNow,
                                             Environment.Version.ToString(),
                                             options,
                                             results);

             string reportPath = Path.Combine(artifactDirectory,
                                              $"pipeline-perf-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.json");
             File.WriteAllText(reportPath, JsonSerializer.Serialize(report, s_JsonOptions));
             Console.WriteLine();
             Console.WriteLine($"Report written to {reportPath}");

             return 0;
         }
         catch (Exception e)
         {
             Console.Error.WriteLine(e.Message);
             return 1;
         }
     }

     private static PipelineStageResult RunStage(string filePath,
                                                 string source,
                                                 PipelineStage stage,
                                                 BenchmarkTarget target,
                                                 PerfOptions options)
     {
         return stage switch
         {
             PipelineStage.Parse => RunParseStage(filePath, source, target, options),
             PipelineStage.Compile => RunCompileStage(filePath, source, target, options),
             PipelineStage.Execute => RunExecuteStage(filePath, source, target, options),
             _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, "Unsupported stage")
         };
     }

     private static PipelineStageResult RunStageSafe(string filePath,
                                                     string source,
                                                     PipelineStage stage,
                                                     BenchmarkTarget target,
                                                     PerfOptions options)
     {
         try
         {
             return RunStage(filePath, source, stage, target, options);
         }
         catch (Exception e)
         {
             return new PipelineStageResult(filePath,
                                            stage,
                                            target.Name,
                                            0,
                                            null,
                                            [],
                                            [],
                                            new BenchmarkSummary(0, 0, 0, 0),
                                            null,
                                            null,
                                            e.Message);
         }
     }

     private static PipelineStageResult RunParseStage(string filePath,
                                                      string source,
                                                      BenchmarkTarget target,
                                                      PerfOptions options)
     {
         int expressionCount = 0;
         List<double> warmup = MeasureRuns(options.WarmupRuns,
                                           () =>
                                           {
                                               expressionCount = ConsumeExpressions(BadRuntime.Parse(source, filePath));
                                           });

         List<double> measured = MeasureRuns(options.MeasuredRuns,
                                             () =>
                                             {
                                                 expressionCount = ConsumeExpressions(BadRuntime.Parse(source, filePath));
                                             });

         return new PipelineStageResult(filePath,
                                        PipelineStage.Parse,
                                        target.Name,
                                        expressionCount,
                                        null,
                                        warmup,
                                        measured,
                                        BuildSummary(measured),
                                        null,
                                        null,
                                        null);
     }

     private static PipelineStageResult RunCompileStage(string filePath,
                                                        string source,
                                                        BenchmarkTarget target,
                                                        PerfOptions options)
     {
         BadExpression[] parsed = BadRuntime.Parse(source, filePath).ToArray();
         int instructionCount = 0;

         List<double> warmup = MeasureRuns(options.WarmupRuns,
                                           () =>
                                           {
                                               instructionCount = BadCompiler.Compile(parsed).Count();
                                           });

         List<double> measured = MeasureRuns(options.MeasuredRuns,
                                             () =>
                                             {
                                                 instructionCount = BadCompiler.Compile(parsed).Count();
                                             });

         return new PipelineStageResult(filePath,
                                        PipelineStage.Compile,
                                        target.Name,
                                        parsed.Length,
                                        instructionCount,
                                        warmup,
                                        measured,
                                        BuildSummary(measured),
                                        null,
                                        null,
                                        null);
     }

      private static PipelineStageResult RunExecuteStage(string filePath,
                                                         string source,
                                                         BenchmarkTarget target,
                                                         PerfOptions options)
      {
          BadExpression[] parsed = BadRuntime.Parse(source, filePath).ToArray();
          BadInstruction[] instructions = BadCompiler.Compile(parsed).ToArray();

          using BadRuntime runtime = new BadRuntime();
          using BadExecutionContext setupContext = runtime.CreateContext(Path.GetDirectoryName(filePath) ?? Directory.GetCurrentDirectory());
          BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                 true,
                                                                 setupContext.Scope,
                                                                 BadSourcePosition.FromSource(filePath, 0, source.Length),
                                                                 null,
                                                                 false,
                                                                 false,
                                                                 null,
                                                                 BadAnyPrototype.Instance,
                                                                 false);

          long maxEvalFallbackCount = 0;
          Dictionary<string, long>? optimizationCounters = null;

          // Warmup runs - also gather profiling counters for adaptive burst size
          BadRuntimeVirtualMachine.ResetOptimizationCounters();
          List<double> warmup = MeasureRuns(options.WarmupRuns,
                                            () =>
                                            {
                                                Execute(function, instructions, runtime, filePath);
                                            });

          // Adaptive VmBurstSize: analyze warmup counters to choose optimal burst
          BadNativeOptimizationSettings settings = BadNativeOptimizationSettings.Instance;
          int? adaptiveBurstChosen = null;
          int originalBurstSize = settings.VmBurstSize;  // save for restore after adaptive adjustment
          if (settings.UseAdaptiveVmBurstSize)
          {
              Dictionary<string, long> warmupCounters = BadRuntimeVirtualMachine.GetOptimizationCounterSnapshot();
              int adaptedBurst = ComputeAdaptiveBurstSize(warmupCounters,
                                                          settings.MinAdaptiveVmBurstSize,
                                                          settings.MaxAdaptiveVmBurstSize);
              settings.VmBurstSize = adaptedBurst;
              adaptiveBurstChosen = adaptedBurst;
          }

          List<double> measured = MeasureRuns(options.MeasuredRuns,
                                              () =>
                                              {
                                                  BadRuntimeVirtualMachine.ResetOptimizationCounters();
                                                  Execute(function, instructions, runtime, filePath);
                                                  maxEvalFallbackCount = Math.Max(maxEvalFallbackCount, BadRuntimeVirtualMachine.EvalInstructionCount);
                                                  optimizationCounters = BadRuntimeVirtualMachine.GetOptimizationCounterSnapshot();
                                              });

          // Restore VmBurstSize to what the target configured (not the adaptive override)
          settings.VmBurstSize = originalBurstSize;

          // Inject adaptive burst size into counters if used
          if (adaptiveBurstChosen.HasValue && optimizationCounters != null)
          {
              optimizationCounters["AdaptiveVmBurstSizeChosen"] = adaptiveBurstChosen.Value;
          }

         return new PipelineStageResult(filePath,
                                        PipelineStage.Execute,
                                        target.Name,
                                        parsed.Length,
                                        instructions.Length,
                                        warmup,
                                        measured,
                                        BuildSummary(measured),
                                        maxEvalFallbackCount,
                                        optimizationCounters,
                                        null);
     }

    private static List<double> MeasureRuns(int count, Action run)
    {
        List<double> values = new(count);

        for (int i = 0; i < count; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            run();
            sw.Stop();
            values.Add(sw.Elapsed.TotalMilliseconds);
        }

        return values;
    }

    private static void Execute(BadCompiledFunction function,
                                BadInstruction[] instructions,
                                BadRuntime runtime,
                                string filePath)
    {
        using BadExecutionContext context = runtime.CreateContext(Path.GetDirectoryName(filePath) ?? Directory.GetCurrentDirectory());
        BadRuntimeVirtualMachine vm = new(function, instructions);

        foreach (BadObject _ in vm.Execute(context))
        {
        }
    }

    private static int ConsumeExpressions(IEnumerable<BadExpression> expressions)
    {
        int count = 0;

        foreach (BadExpression _ in expressions)
        {
            count++;
        }

        return count;
    }

    private static BenchmarkSummary BuildSummary(IReadOnlyList<double> values)
    {
        if (values.Count == 0)
        {
            return new BenchmarkSummary(0, 0, 0, 0);
        }

        double[] sorted = values.OrderBy(v => v).ToArray();
        double median = sorted.Length % 2 == 0
                            ? (sorted[(sorted.Length / 2) - 1] + sorted[sorted.Length / 2]) / 2.0
                            : sorted[sorted.Length / 2];

        return new BenchmarkSummary(sorted[0], median, sorted[^1], sorted.Average());
    }

    private static List<string> ResolveFileInputs(PerfOptions options)
    {
        HashSet<string> files = new(StringComparer.Ordinal);

        foreach (string file in options.Files)
        {
            string fullPath = Path.GetFullPath(file);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Input file not found: {fullPath}");
            }

            files.Add(fullPath);
        }

        foreach (string glob in options.Globs)
        {
            string baseDirectory = ResolveGlobBaseDirectory(glob);
            string pattern = ResolveGlobPattern(glob);

            if (!Directory.Exists(baseDirectory))
            {
                continue;
            }

            foreach (string file in Directory.EnumerateFiles(baseDirectory, pattern, SearchOption.AllDirectories))
            {
                files.Add(Path.GetFullPath(file));
            }
        }

        return files.OrderBy(x => x, StringComparer.Ordinal).ToList();
    }

    private static string ResolveGlobBaseDirectory(string glob)
    {
        string normalized = glob.Replace('\\', '/');
        int firstWildcardIndex = normalized.IndexOfAny(['*', '?']);

        if (firstWildcardIndex < 0)
        {
            return Path.GetDirectoryName(Path.GetFullPath(glob)) ?? Directory.GetCurrentDirectory();
        }

        string prefix = normalized[..firstWildcardIndex];
        int slashIndex = prefix.LastIndexOf('/');
        string basePath = slashIndex >= 0 ? prefix[..slashIndex] : ".";
        return Path.GetFullPath(basePath);
    }

    private static string ResolveGlobPattern(string glob)
    {
        string normalized = glob.Replace('\\', '/');
        int firstWildcardIndex = normalized.IndexOfAny(['*', '?']);

        if (firstWildcardIndex < 0)
        {
            return Path.GetFileName(glob);
        }

        string pattern = normalized[firstWildcardIndex..];

        // Basic support for **/*.ext by mapping to *.ext with recursive search.
        int slashIndex = pattern.LastIndexOf('/');
        return slashIndex >= 0 ? pattern[(slashIndex + 1)..] : pattern;
    }

    private static string FindSourceRoot()
    {
        DirectoryInfo? current = new(AppContext.BaseDirectory);

        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "BadScript2.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    /// <summary>
    /// Compute the optimal VmBurstSize based on counter profiling data.
    /// Heuristic:
    ///  - High loop fast-path hits → larger burst (less yield overhead)
    ///  - High invoke fallback hits → smaller burst (more cooperative yielding)
    ///  - Balanced → medium burst
    /// </summary>
    private static int ComputeAdaptiveBurstSize(Dictionary<string, long> counters,
                                                int minBurst,
                                                int maxBurst)
    {
        counters.TryGetValue("LoopMoveNextNativeFastPath", out long loopNativeFast);
        counters.TryGetValue("LoopMoveNextMethodSlotFastPath", out long loopMethodFast);
        counters.TryGetValue("LoopMoveNextSlowPath", out long loopSlow);
        counters.TryGetValue("InvokeFallbackPath", out long invokeFallback);
        counters.TryGetValue("InvokeCompiledFallbackVmPath", out long invokeCompiledFallback);
        counters.TryGetValue("LoadVarSlotPath", out long loadVarSlot);
        counters.TryGetValue("LoadVarScopePath", out long loadVarScope);

        long totalLoop = loopNativeFast + loopMethodFast + loopSlow;
        long totalInvoke = invokeFallback + invokeCompiledFallback;
        long totalLoad = loadVarSlot + loadVarScope;

        // Score: 0.0 (fallback-heavy) to 1.0 (fast-path-heavy)
        double score = 0.5;

        // Loop score: more native fast paths = better
        if (totalLoop > 0)
        {
            double loopFastRatio = (double)(loopNativeFast + loopMethodFast) / totalLoop;
            score = 0.4 * loopFastRatio + 0.6 * score;
        }

        // Invoke score: fewer fallbacks = better
        if (totalLoad > 0)
        {
            double slotRatio = (double)loadVarSlot / totalLoad;
            score = 0.4 * slotRatio + 0.6 * score;
        }

        // More fallback invocations = prefer smaller burst
        if (totalInvoke > 100)
        {
            // Many fallback invocations → reduce burst
            score *= 0.7;
        }

        // Clamp score to [0, 1] and map to burst size range
        score = Math.Clamp(score, 0.0, 1.0);
        int range = maxBurst - minBurst;
        int burst = minBurst + (int)(score * range);

        // Round to nearest power-of-2 for cache friendliness
        burst = RoundToPowerOfTwo(burst);
        return Math.Clamp(burst, minBurst, maxBurst);
    }

    private static int RoundToPowerOfTwo(int value)
    {
        if (value <= 1) return 1;
        int result = 1;
        while (result < value) result <<= 1;
        // Pick the closer power of two
        return (result - value) < (value - (result >> 1)) ? result : (result >> 1);
    }

    private static void PrintStageSummary(PipelineStageResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.Error))
        {
            Console.WriteLine($"    [{result.Stage}] FAILED: {result.Error}");
            return;
        }

        Console.WriteLine($"    [{result.Stage}] median={result.Summary.MedianMs:F3} ms mean={result.Summary.MeanMs:F3} ms min={result.Summary.MinMs:F3} ms max={result.Summary.MaxMs:F3} ms");

        if (result.Stage == PipelineStage.Execute && result.EvalFallbackCount.HasValue)
        {
            Console.WriteLine($"      eval-fallbacks(max): {result.EvalFallbackCount.Value}");

            if (result.OptimizationCounters is { Count: > 0 })
            {
                foreach (KeyValuePair<string, long> counter in result.OptimizationCounters
                             .Where(c => c.Value > 0)
                             .OrderByDescending(c => c.Value)
                             .ThenBy(c => c.Key, StringComparer.Ordinal))
                {
                    Console.WriteLine($"      {counter.Key}: {counter.Value}");
                }
            }
        }
    }
}

internal enum PipelineStage
{
    Parse,
    Compile,
    Execute
}



internal sealed record PerfOptions(IReadOnlyList<string> Files,
                                   IReadOnlyList<string> Globs,
                                   IReadOnlyList<PipelineStage> Stages,
                                   IReadOnlyList<string> Targets,
                                   int WarmupRuns,
                                   int MeasuredRuns)
{
    public List<BenchmarkTarget> ResolveTargets()
    {
        List<BenchmarkTarget> targets = new();

        if (Targets.Count == 0)
        {
            // Default to all predefined targets
            targets.AddRange(BenchmarkTarget.Predefined.All);
        }
        else
        {
            foreach (string targetName in Targets)
            {
                BenchmarkTarget? target = BenchmarkTarget.Predefined.GetByName(targetName);
                if (target == null)
                {
                    throw new ArgumentException($"Unknown benchmark target: {targetName}");
                }
                targets.Add(target);
            }
        }

        return targets;
    }

    public static PerfOptions Parse(string[] args)
    {
        List<string> files = new();
        List<string> globs = new();
        HashSet<PipelineStage> stages = new();
        List<string> targets = new();
        int warmupRuns = 2;
        int measuredRuns = 8;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--file":
                    files.Add(args[++i]);
                    break;
                case "--glob":
                    globs.Add(args[++i]);
                    break;
                case "--stage":
                    stages.Add(ParseStage(args[++i]));
                    break;
                case "--target":
                    targets.Add(args[++i]);
                    break;
                case "--warmup-runs":
                    warmupRuns = int.Parse(args[++i]);
                    break;
                case "--measured-runs":
                    measuredRuns = int.Parse(args[++i]);
                    break;
                case "--help":
                case "-h":
                    PrintHelp();
                    Environment.Exit(0);
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {args[i]}");
            }
        }

        if (stages.Count == 0)
        {
            stages.UnionWith([PipelineStage.Parse, PipelineStage.Compile, PipelineStage.Execute]);
        }

        if (warmupRuns < 0)
        {
            throw new ArgumentException("--warmup-runs must be >= 0");
        }

        if (measuredRuns <= 0)
        {
            throw new ArgumentException("--measured-runs must be > 0");
        }

        return new PerfOptions(files, globs, stages.OrderBy(x => x).ToArray(), targets, warmupRuns, measuredRuns);
    }

    private static PipelineStage ParseStage(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "parse" => PipelineStage.Parse,
            "compile" => PipelineStage.Compile,
            "execute" => PipelineStage.Execute,
            _ => throw new ArgumentException("--stage must be parse, compile or execute")
        };
    }

    private static void PrintHelp()
    {
        Console.WriteLine("BadScript2.PipelinePerf");
        Console.WriteLine("  --file <path>            Adds an input .bs source file (can be used multiple times)");
        Console.WriteLine("  --glob <pattern>         Adds input files via simple recursive glob (e.g. /repo/**/*.bs)");
        Console.WriteLine("  --stage <name>           parse | compile | execute (repeatable; default: all)");
        Console.WriteLine("  --target <name>          Benchmark target name (repeatable; default: all)");
        Console.WriteLine("                           Available targets: Baseline, Compiled, CompiledOptimized, Full");
        Console.WriteLine("  --warmup-runs <count>    Warmup run count per stage (default: 2)");
        Console.WriteLine("  --measured-runs <count>  Measured run count per stage (default: 8)");
    }
}

internal sealed record PipelinePerfReport(DateTimeOffset CreatedAtUtc,
                                          string DotNetVersion,
                                          PerfOptions Options,
                                          List<PipelineStageResult> Results);

internal sealed record PipelineStageResult(string FilePath,
                                           PipelineStage Stage,
                                           string TargetName,
                                           int ExpressionCount,
                                           int? InstructionCount,
                                           List<double> WarmupRunsMs,
                                           List<double> MeasuredRunsMs,
                                           BenchmarkSummary Summary,
                                           long? EvalFallbackCount,
                                           Dictionary<string, long>? OptimizationCounters,
                                           string? Error);

internal sealed record BenchmarkSummary(double MinMs, double MedianMs, double MaxMs, double MeanMs);




