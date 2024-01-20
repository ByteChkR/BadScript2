using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

/// <summary>
///     Defines Settings for Native Optimizations
/// </summary>
public class BadNativeOptimizationSettings : BadSettingsProvider<BadNativeOptimizationSettings>
{
    /// <summary>
    /// Editable Setting for the Setting UseConstantFoldingOptimization
    /// </summary>
    private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseConstantFoldingOptimization =
        new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseConstantFoldingOptimization");

    /// <summary>
    /// Editable Setting for the Setting UseConstantFunctionCaching
    /// </summary>
    private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseConstantFunctionCaching =
        new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseConstantFunctionCaching");

    /// <summary>
    /// Editable Setting for the Setting UseConstantSubstitutionOptimization
    /// </summary>
    private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseConstantSubstitutionOptimization =
        new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseConstantSubstitutionOptimization");

    /// <summary>
    /// Editable Setting for the Setting UseStaticExtensionCaching
    /// </summary>
    private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseStaticExtensionCaching =
        new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseStaticExtensionCaching");

    /// <summary>
    /// Editable Setting for the Setting UseStringCaching
    /// </summary>
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
    public bool UseConstantFoldingOptimization
    {
        get => m_UseConstantFoldingOptimization.GetValue();
        set => m_UseConstantFoldingOptimization.Set(value);
    }

    /// <summary>
    ///     Allow the runtime to optimize constant expressions to a higher degree than constant folding.
    ///     If enabled, the runtime try to optimize constant expressions that reference constants and variables that are marked
    ///     as constant.
    ///     Example Input:
    ///     const a = 1;
    ///     const b = 2;
    ///     const c = a + b;
    ///     let d = c + 1;
    ///     Example Output:
    ///     const a = 1;
    ///     const b = 2;
    ///     const c = 3;
    ///     let d = 4;
    /// </summary>
    public bool UseConstantSubstitutionOptimization
    {
        get => m_UseConstantSubstitutionOptimization.GetValue();
        set => m_UseConstantSubstitutionOptimization.Set(value);
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