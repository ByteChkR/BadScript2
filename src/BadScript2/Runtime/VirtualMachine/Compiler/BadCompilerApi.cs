using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
///     Implements an API for the Compiler.
/// </summary>
public class BadCompilerApi : BadInteropApi
{
    /// <summary>
    ///     Creates a new API Instance.
    /// </summary>
    public BadCompilerApi() : base("Compiler") { }

    /// <inheritdoc />
    protected override void LoadApi(BadTable target)
    {
        target.SetFunction<BadExpressionFunction, bool>(
            "Compile",
            CompileFunction,
            BadNativeClassBuilder.GetNative("Function")
        );
    }

    /// <summary>
    ///     Compiles a Function.
    /// </summary>
    /// <param name="func">The Function to compile.</param>
    /// <param name="useOverride">If the compiled function should support operator overrides.</param>
    /// <returns>The compiled function.</returns>
    public static BadCompiledFunction CompileFunction(BadExpressionFunction func, bool useOverride)
    {
        BadInstruction[] instrs = BadCompiler.Compile(func.Body).ToArray();


        return new BadCompiledFunction(
            instrs,
            useOverride,
            func.ParentScope,
            func.Position,
            func.Name,
            func.IsConstant,
            func.IsStatic,
            func.MetaData,
            func.ReturnType,
            func.IsSingleLine,
            func.Parameters
        );
    }
}