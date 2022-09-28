using BadScript2.Settings;

namespace BadScript2.Runtime.Settings
{
    /// <summary>
    ///     Defines Settings for Native Optimizations
    /// </summary>
    public class BadNativeOptimizationSettings : BadSettingsProvider<BadNativeOptimizationSettings>
    {
        /// <summary>
        ///     Allow the runtime to optimize constant expressions
        ///     If enabled the runtime will try to optimize constant expressions like 1 + 2 to 3
        /// </summary>
        private BadSettings? m_UseConstantExpressionOptimizationObj;

        /// <summary>
        ///     Allow the runtime to cache the returns of constant functions
        ///     If enabled the runtime will cache the return value of functions for invocations that have the same parameters
        /// </summary>
        private BadSettings? m_UseConstantFunctionCachingObj;

        /// <summary>
        ///     Allow the runtime to cache extensions for object types.
        ///     If enabled, the runtime will cache the results of extension lookups for object types.
        /// </summary>
        private BadSettings? m_UseStaticExtensionCachingObj;


        /// <summary>
        ///     Allow the runtime to cache string objects.
        ///     If enabled, the runtime will reuse string objects for the same string value.
        /// </summary>
        private BadSettings? m_UseStringCachingObj;

        /// <summary>
        ///     Creates a new instance of the BadNativeOptimizationSettings class
        /// </summary>
        public BadNativeOptimizationSettings() : base("Runtime.NativeOptimizations") { }


        /// <summary>
        ///     Allow the runtime to cache string objects.
        ///     If enabled, the runtime will reuse string objects for the same string value.
        /// </summary>
        private BadSettings? UseStringCachingObj => m_UseStringCachingObj ??
                                                    (Settings?.HasProperty(nameof(UseStringCaching)) ?? false
                                                        ? m_UseStringCachingObj ??= Settings?.GetProperty(nameof(UseStringCaching))
                                                        : null);

        /// <summary>
        ///     Allow the runtime to cache the returns of constant functions
        ///     If enabled the runtime will cache the return value of functions for invocations that have the same parameters
        /// </summary>
        private BadSettings? UseConstantFunctionCachingObj => m_UseConstantFunctionCachingObj ??
                                                              (Settings?.HasProperty(nameof(UseConstantFunctionCaching)) ?? false
                                                                  ? m_UseConstantFunctionCachingObj ??= Settings?.GetProperty(nameof(UseConstantFunctionCaching))
                                                                  : null);

        /// <summary>
        ///     Allow the runtime to optimize constant expressions
        ///     If enabled the runtime will try to optimize constant expressions like 1 + 2 to 3
        /// </summary>
        private BadSettings? UseConstantExpressionOptimizationObj => m_UseConstantExpressionOptimizationObj ??
                                                                     (Settings?.HasProperty(nameof(UseConstantExpressionOptimization)) ?? false
                                                                         ? m_UseConstantExpressionOptimizationObj ??=
                                                                             Settings?.GetProperty(nameof(UseConstantExpressionOptimization))
                                                                         : null);

        /// <summary>
        ///     Allow the runtime to cache extensions for object types.
        ///     If enabled, the runtime will cache the results of extension lookups for object types.
        /// </summary>
        private BadSettings? UseStaticExtensionCachingObj => m_UseStaticExtensionCachingObj ??
                                                             (Settings?.HasProperty(nameof(UseStaticExtensionCaching)) ?? false
                                                                 ? m_UseStaticExtensionCachingObj ??= Settings?.GetProperty(nameof(UseStaticExtensionCaching))
                                                                 : null);


        /// <summary>
        ///     Allow the runtime to cache string objects.
        ///     If enabled, the runtime will reuse string objects for the same string value.
        /// </summary>
        public bool UseStringCaching => UseStringCachingObj?.GetValue<bool>() ?? false;


        /// <summary>
        ///     Allow the runtime to optimize constant expressions
        ///     If enabled the runtime will try to optimize constant expressions like 1 + 2 to 3
        /// </summary>
        public bool UseConstantExpressionOptimization =>
            UseConstantExpressionOptimizationObj?.GetValue<bool>() ?? false;

        /// <summary>
        ///     Allow the runtime to cache extensions for object types.
        ///     If enabled, the runtime will cache the results of extension lookups for object types.
        /// </summary>
        public bool UseStaticExtensionCaching =>
            UseStaticExtensionCachingObj?.GetValue<bool>() ?? false;

        /// <summary>
        ///     Allow the runtime to cache the returns of constant functions
        ///     If enabled the runtime will cache the return value of functions for invocations that have the same parameters
        /// </summary>
        public bool UseConstantFunctionCaching => UseConstantFunctionCachingObj?.GetValue<bool>() ?? false;
    }
}