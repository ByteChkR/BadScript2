using BadScript2.Settings;

namespace BadScript2.Runtime.Settings
{
    public class BadNativeOptimizationSettings : BadSettingsProvider<BadNativeOptimizationSettings>
    {
        public BadNativeOptimizationSettings() : base("Runtime.NativeOptimizations") { }


        private BadSettings? m_UseStringCachingObj;

        private BadSettings? UseStringCachingObj =>
            m_UseStringCachingObj ??= Settings?.GetProperty(nameof(UseStringCaching));
        public bool UseStringCaching => UseStringCachingObj?.GetValue<bool>() ?? false;

    
        private BadSettings? m_UseConstantExpressionOptimizationObj;
    
        private BadSettings? UseConstantExpressionOptimizationObj => m_UseConstantExpressionOptimizationObj ??= Settings?.GetProperty(nameof(UseConstantExpressionOptimization));
        public bool UseConstantExpressionOptimization =>
            UseConstantExpressionOptimizationObj?.GetValue<bool>() ?? false;
    
        private BadSettings? m_UseStaticExtensionCachingObj;
    
        private BadSettings? UseStaticExtensionCachingObj => m_UseStaticExtensionCachingObj ??= Settings?.GetProperty(nameof(UseStaticExtensionCaching));
        public bool UseStaticExtensionCaching =>
            UseStaticExtensionCachingObj?.GetValue<bool>() ?? false;
    }
}