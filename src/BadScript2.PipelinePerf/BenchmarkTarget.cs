using BadScript2.Runtime.Settings;

namespace BadScript2.PipelinePerf;

/// <summary>
/// Represents a set of runtime settings to benchmark.
/// </summary>
internal sealed record BenchmarkTarget(string Name, Action<BadNativeOptimizationSettings> ConfigureSettings)
{
    /// <summary>
    /// Predefined benchmark targets.
    /// </summary>
    public static class Predefined
    {
        /// <summary>
        /// Baseline: all optimizations disabled.
        /// </summary>
        public static readonly BenchmarkTarget Baseline = new(
            "Baseline",
            settings =>
            {
                settings.UseDefaultCompilation = false;
                settings.UseLambdaDefaultCompilation = false;
                settings.UseSlotLocalFastPath = false;
                settings.UseConstantFoldingOptimization = false;
                settings.UseConstantSubstitutionOptimization = false;
                settings.UseStaticExtensionCaching = false;
                settings.UseLoopFastPath = false;
                settings.UseStringCaching = false;
                settings.VmBurstSize = 1;
                settings.UseStaticMethodSpecialization = false;
                settings.UseConstantFunctionCaching = false;
            });

        /// <summary>
        /// Compiled only.
        /// </summary>
        public static readonly BenchmarkTarget Compiled = new(
            "Compiled",
            settings =>
            {
                Baseline.ConfigureSettings(settings);
                settings.UseDefaultCompilation = true;
                settings.UseLambdaDefaultCompilation = true;
            });


        /// <summary>
        /// Full optimizations enabled.
        /// </summary>
        public static readonly BenchmarkTarget Full = new(
            "Optimized",
            settings => { 
                Compiled.ConfigureSettings(settings);
                settings.UseStaticMethodSpecialization = true;
                settings.UseSlotLocalFastPath = true;
                settings.UseLoopFastPath = true;
                settings.VmBurstSize = 128;
                settings.UseBinaryOperatorSpecialization = true;
                settings.UseUnaryOperatorSpecialization = true;
                settings.UseComparisonSpecialization = true;
                settings.UseLoopConditionSpecialization = true;
                settings.UseAdaptiveVmBurstSize = true;
                settings.MinAdaptiveVmBurstSize = 4;
                settings.MaxAdaptiveVmBurstSize = 256;
                settings.UseInlineCaching = true;
                settings.InlineCacheSize = 4;
                settings.UseNullCheckInlineCache = true;
                settings.UseEscapeAnalysis = true;
                settings.UseConstantFoldingOptimization = true;
                settings.UseConstantSubstitutionOptimization = true;
                settings.UseStaticExtensionCaching = true;
                settings.UseStringCaching = true;
                settings.UseConstantFunctionCaching = true;
            });

        /// <summary>
        /// Get all predefined targets.
        /// </summary>
        public static IReadOnlyList<BenchmarkTarget> All { get; } = 
        [
            Baseline,
            Compiled,
            Full,
        ];

        /// <summary>
        /// Get a target by name.
        /// </summary>
        public static BenchmarkTarget? GetByName(string name)
        {
            return All.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Create a custom benchmark target with a specific configuration.
    /// </summary>
    /// <param name="name">Target name</param>
    /// <param name="slotLocalFastPath">Enable slot-backed local fast path</param>
    /// <returns>Custom benchmark target</returns>
    public static BenchmarkTarget Create(string name, bool slotLocalFastPath)
    {
        return new BenchmarkTarget(
            name,
            settings => { settings.UseSlotLocalFastPath = slotLocalFastPath; });
    }
}


