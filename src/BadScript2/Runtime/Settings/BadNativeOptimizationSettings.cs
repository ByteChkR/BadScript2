using System;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Settings;

/// <summary>
/// Contains Runtime Settings Objects
/// </summary>
namespace BadScript2.Runtime.Settings;

/// <summary>
///     Defines Settings for Native Optimizations
/// </summary>
public class BadNativeOptimizationSettings : BadSettingsProvider<BadNativeOptimizationSettings>
{
    // /// <summary>
    // ///     Editable Setting for the Setting UseConstantFoldingOptimization
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseConstantFoldingOptimization =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseConstantFoldingOptimization");
    //
    // /// <summary>
    // ///     Editable Setting for the Setting UseConstantFunctionCaching
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseConstantFunctionCaching =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseConstantFunctionCaching");
    //
    // /// <summary>
    // ///     Editable Setting for the Setting UseConstantSubstitutionOptimization
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseConstantSubstitutionOptimization =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseConstantSubstitutionOptimization");
    //
    // /// <summary>
    // ///     Editable Setting for the Setting UseStaticExtensionCaching
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseStaticExtensionCaching =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseStaticExtensionCaching");
    //
    // /// <summary>
    // ///     Editable Setting for the Setting UseStringCaching
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseStringCaching =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseStringCaching");
    //
    // /// <summary>
    // /// Editable Setting for the Setting UseLambdaDefaultCompilation
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseLambdaDefaultCompilation =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseLambdaDefaultCompilation", false);
    //
    // /// <summary>
    // /// E
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseDefaultCompilation =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseDefaultCompilation", false);
    //
    // /// <summary>
    // ///     Editable setting for static method specialization fast-path.
    // ///     Disabled by default until all call edge-cases are hardened.
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseStaticMethodSpecialization =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseStaticMethodSpecialization", false);
    //
    // /// <summary>
    // ///     Editable setting for the VTable method-slot fast-path in InvokeMember (AP3).
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseMethodSlotFastPath =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseMethodSlotFastPath", true);
    //
    // /// <summary>
    // ///     Editable setting for the property reference cache in LoadMember (AP5).
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UsePropertyReferenceCache =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UsePropertyReferenceCache", true);
    //
    // /// <summary>
    // ///     Editable setting for the IBadEnumerator fast-path in MoveNext/GetCurrent (AP4).
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseLoopFastPath =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseLoopFastPath", true);
    //
    // /// <summary>
    // ///     Editable setting for slot-backed locals and parameters in compiled functions (AP1).
    // ///     Disabled paths fall back to legacy scope-backed variable handling.
    // /// </summary>
    // private readonly BadEditableSetting<BadNativeOptimizationSettings, bool> m_UseSlotLocalFastPath =
    //     new BadEditableSetting<BadNativeOptimizationSettings, bool>("UseSlotLocalFastPath", true);

    public BadFunctionCompileLevel DefaultCompileLevel =>
        UseDefaultCompilation ? BadFunctionCompileLevel.Compiled : BadFunctionCompileLevel.None;
    public BadFunctionCompileLevel DefaultLambdaCompileLevel =>
        UseLambdaDefaultCompilation ? BadFunctionCompileLevel.Compiled : BadFunctionCompileLevel.None;
    
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
        get;
        set;
    } = true;


    /// <summary>
    ///     Allow the runtime to optimize constant expressions
    ///     If enabled the runtime will try to optimize constant expressions like 1 + 2 to 3
    /// </summary>
    public bool UseConstantFoldingOptimization
    {
        get;
        set;
    } = true;


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
        get;
        set;
    } = true;

    /// <summary>
    ///     Allow the runtime to cache extensions for object types.
    ///     If enabled, the runtime will cache the results of extension lookups for object types.
    /// </summary>
    public bool UseStaticExtensionCaching
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allow the runtime to cache the returns of constant functions
    ///     If enabled the runtime will cache the return value of functions for invocations that have the same parameters
    /// </summary>
    public bool UseConstantFunctionCaching
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allow the runtime to Compile Lambda Expression by default.
    ///     If a lambda is used a lot of times, it might benefit from being compiled.
    /// </summary>
    public bool UseLambdaDefaultCompilation
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allows the runtime to Compile ALL Function expressions by default(except Lambdas).
    ///     This is an experimental feature that might improve performance in some cases.
    ///    If enabled, the runtime will compile all function expressions by default.
    /// </summary>
    public bool UseDefaultCompilation
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allows the runtime to use the static method specialization fast-path in the VM.
    /// </summary>
    public bool UseStaticMethodSpecialization
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allows the VM to use the VTable method-slot fast-path for InvokeMember on BadClass instances (AP3).
    /// </summary>
    public bool UseMethodSlotFastPath
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allows the VM LoadMember handler to cache BadObjectReferences for public BadClass members (AP5).
    /// </summary>
    public bool UsePropertyReferenceCache
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allows the VM MoveNext/GetCurrent handlers to use the IBadEnumerator fast-path (AP4).
    /// </summary>
    public bool UseLoopFastPath
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allows compiled functions to use slot-backed locals and parameters instead of legacy scope-backed access (AP1).
    /// </summary>
    public bool UseSlotLocalFastPath
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Number of VM instructions to execute per outer Execute() iteration (burst mode).
    ///     Default 1 (no burst). Higher values reduce IEnumerable state-machine overhead in tight loops.
    /// </summary>
    public int VmBurstSize
    {
        get;
        set => field = Math.Max(1, value);
    } = 4;

    /// <summary>
    ///     Allows the runtime to automatically select optimal VmBurstSize based on workload characteristics.
    ///     If enabled, the runtime will use CounterAnalysis to determine best burst size (Phase A Optimization).
    /// </summary>
    public bool UseAdaptiveVmBurstSize
    {
        get;
        set;
    } = false;

    /// <summary>
    ///     Minimum VmBurstSize when using adaptive mode.
    /// </summary>
    public int MinAdaptiveVmBurstSize
    {
        get;
        set => field = Math.Max(1, value);
    } = 4;

    /// <summary>
    ///     Maximum VmBurstSize when using adaptive mode.
    /// </summary>
    public int MaxAdaptiveVmBurstSize
    {
        get;
        set => field = Math.Max(1, value);
    } = 256;

    /// <summary>
    ///     Allows the runtime to use type-specialized fast-paths for binary operators (+, -, *, /, ==, <, >, etc.).
    ///     If enabled, int+int, string+string, etc. will use optimized native operations (Phase A Optimization).
    /// </summary>
    public bool UseBinaryOperatorSpecialization
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allows the runtime to use specialized fast-paths for unary operators (-, !, ~).
    ///     If enabled, -int, !bool, etc. will use optimized native operations (Phase A Optimization).
    /// </summary>
    public bool UseUnaryOperatorSpecialization
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allows the runtime to use specialized fast-paths for comparison operators (<, >, <=, >=, ==, !=).
    ///     Especially useful in loop conditions where comparisons happen frequently (Phase A Optimization).
    /// </summary>
    public bool UseComparisonSpecialization
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allows the runtime to optimize loop conditions (i < max, i > min, etc.) with specialized fast-paths.
    ///     Works in conjunction with UseLoopFastPath (Phase A Optimization).
    /// </summary>
    public bool UseLoopConditionSpecialization
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Allows the runtime to use inline caching (IC) for member access (obj.property, obj.method).
    ///     IC caches the member lookup result to avoid repeated lookups on the same object type (Phase B Optimization).
    /// </summary>
    public bool UseInlineCaching
    {
        get;
        set;
    } = false;

    /// <summary>
    ///     Number of inline cache entries per call-site when UseInlineCaching is enabled.
    /// </summary>
    public int InlineCacheSize
    {
        get;
        set => field = Math.Max(1, value);
    } = 4;

    /// <summary>
    ///     Allows the runtime to use null-check inline caching for optional member access (?. operator).
    ///     Caches results of null checks to optimize the fast path (Phase B Optimization).
    /// </summary>
    public bool UseNullCheckInlineCache
    {
        get;
        set;
    } = false;

    /// <summary>
    ///     Allows the runtime to use escape analysis for automatic stack allocation of small objects.
    ///     Objects that don't escape the function can be stack-allocated instead of heap-allocated (Phase C Optimization).
    /// </summary>
    public bool UseEscapeAnalysis
    {
        get;
        set;
    } = false;
}
