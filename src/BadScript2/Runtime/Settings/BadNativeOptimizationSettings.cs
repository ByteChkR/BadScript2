using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

public class BadNativeOptimizationSettings : BadSettingsProvider<BadNativeOptimizationSettings>
{
    private BadSettings? m_UseConstantExpressionOptimizationObj;

    private BadSettings? m_UseStaticExtensionCachingObj;


    private BadSettings? m_UseStringCachingObj;
    public BadNativeOptimizationSettings() : base("Runtime.NativeOptimizations") { }

    private BadSettings? UseStringCachingObj =>
        m_UseStringCachingObj ??= Settings?.GetProperty(nameof(UseStringCaching));

    public bool UseStringCaching => UseStringCachingObj?.GetValue<bool>() ?? false;

    private BadSettings? UseConstantExpressionOptimizationObj => m_UseConstantExpressionOptimizationObj ??=
        Settings?.GetProperty(nameof(UseConstantExpressionOptimization));

    public bool UseConstantExpressionOptimization =>
        UseConstantExpressionOptimizationObj?.GetValue<bool>() ?? false;

    private BadSettings? UseStaticExtensionCachingObj =>
        m_UseStaticExtensionCachingObj ??= Settings?.GetProperty(nameof(UseStaticExtensionCaching));

    public bool UseStaticExtensionCaching =>
        UseStaticExtensionCachingObj?.GetValue<bool>() ?? false;
}