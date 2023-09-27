using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

/// <summary>
///     Defines Settings for Native Optimizations
/// </summary>
public class BadNativeOptimizationSettings : BadSettingsProvider<BadNativeOptimizationSettings>
{
    private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseConstantExpressionOptimization =
        new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseConstantExpressionOptimization");

    private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseConstantFunctionCaching =
        new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseConstantFunctionCaching");

    private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseStaticExtensionCaching =
        new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseStaticExtensionCaching");

    private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseStringCaching =
        new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseStringCaching");

    /// <summary>
    ///     Creates a new instance of the BadNativeOptimizationSettings class
    /// </summary>
    public BadNativeOptimizationSettings() : base("Runtime.NativeOptimizations") { }


    /// <summary>
    ///     Allow the runtime to cache string objects.
    ///     If enabled, the runtime will reuse string objects for the same string value.
    /// </summary>
    public bool UseStringCaching
    {
        get => m_UseStringCaching.GetValue();
        set => m_UseStringCaching.Set(value);
    }


    /// <summary>
    ///     Allow the runtime to optimize constant expressions
    ///     If enabled the runtime will try to optimize constant expressions like 1 + 2 to 3
    /// </summary>
    public bool UseConstantExpressionOptimization
    {
        get => m_UseConstantExpressionOptimization.GetValue();
        set => m_UseConstantExpressionOptimization.Set(value);
    }

    /// <summary>
    ///     Allow the runtime to cache extensions for object types.
    ///     If enabled, the runtime will cache the results of extension lookups for object types.
    /// </summary>
    public bool UseStaticExtensionCaching
    {
        get => m_UseStaticExtensionCaching.GetValue();
        set => m_UseStaticExtensionCaching.Set(value);
    }

    /// <summary>
    ///     Allow the runtime to cache the returns of constant functions
    ///     If enabled the runtime will cache the return value of functions for invocations that have the same parameters
    /// </summary>
    public bool UseConstantFunctionCaching
    {
        get => m_UseConstantFunctionCaching.GetValue();
        set => m_UseConstantFunctionCaching.Set(value);
    }
}