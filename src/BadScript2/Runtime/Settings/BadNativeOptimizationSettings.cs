using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

public class BadNativeOptimizationSettings : BadSettingsProvider<BadNativeOptimizationSettings>
{
    private BadSettings? m_UseConstantExpressionOptimizationObj;

    private BadSettings? m_UseStaticExtensionCachingObj;


    private BadSettings? m_UseStringCachingObj;
    private BadSettings? m_UseConstantFunctionCachingObj;
    public BadNativeOptimizationSettings() : base("Runtime.NativeOptimizations") { }

    private BadSettings? UseStringCachingObj => m_UseStringCachingObj?? (Settings?.HasProperty(nameof(UseStringCaching)) ?? false
        ? m_UseStringCachingObj ??= Settings?.GetProperty(nameof(UseStringCaching)):null);

    private BadSettings? UseConstantFunctionCachingObj =>m_UseConstantFunctionCachingObj??( Settings?.HasProperty(nameof(UseConstantFunctionCaching)) ?? false
        ? m_UseConstantFunctionCachingObj ??= Settings?.GetProperty(nameof(UseConstantFunctionCaching))
        : null);
    private BadSettings? UseConstantExpressionOptimizationObj =>m_UseConstantExpressionOptimizationObj??(Settings?.HasProperty(nameof(UseConstantExpressionOptimization)) ?? false
        ?  m_UseConstantExpressionOptimizationObj ??=
            Settings?.GetProperty(nameof(UseConstantExpressionOptimization)):null);

    private BadSettings? UseStaticExtensionCachingObj =>m_UseStaticExtensionCachingObj??(Settings?.HasProperty(nameof(UseStaticExtensionCaching)) ?? false
        ? m_UseStaticExtensionCachingObj ??= Settings?.GetProperty(nameof(UseStaticExtensionCaching)):null);


    public bool UseStringCaching => UseStringCachingObj?.GetValue<bool>() ?? false;


    public bool UseConstantExpressionOptimization =>
        UseConstantExpressionOptimizationObj?.GetValue<bool>() ?? false;

    public bool UseStaticExtensionCaching =>
        UseStaticExtensionCachingObj?.GetValue<bool>() ?? false;

    public bool UseConstantFunctionCaching => UseConstantFunctionCachingObj?.GetValue<bool>() ?? false;
}