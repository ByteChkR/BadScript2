using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Settings;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
/// Structured representation of a method definition used by compiled class templates.
/// </summary>
public sealed class BadCompiledMethodTemplate
{
    public BadCompiledMethodTemplate(BadFunctionExpression expression, bool isStatic)
    {
        Name = expression.Name?.Text ?? "<anonymous>";
        IsStatic = isStatic;
        IsConstructor = expression.Name?.Text == BadStaticKeys.CONSTRUCTOR_NAME;

        BadInstruction[]? compiledInstructions = null;
        bool? useOverrides = null;

        if (expression.CompileLevel == BadFunctionCompileLevel.Compiled)
        {
            compiledInstructions = BadCompiler.Compile(expression.Body).ToArray();
            useOverrides = true;
        }
        else if (expression.CompileLevel == BadFunctionCompileLevel.CompiledFast)
        {
            compiledInstructions = BadCompiler.Compile(expression.Body).ToArray();
            useOverrides = false;
        }

        bool requiresClosureScopeMaterialization =
            BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath &&
            BadFunctionSymbolTableBuilder.HasNestedFunctionLikeConstruct(expression);

        Function = new BadCompiledFunctionTemplate(expression,
                                                   compiledInstructions,
                                                   useOverrides,
                                                   BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath
                                                       ? BadFunctionSymbolTableBuilder.Build(expression)
                                                       : null,
                                                   requiresClosureScopeMaterialization);
    }

    /// <summary>
    /// Method name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Indicates whether the method belongs to the static class body.
    /// </summary>
    public bool IsStatic { get; }

    /// <summary>
    /// Indicates whether the method is the constructor.
    /// </summary>
    public bool IsConstructor { get; }

    /// <summary>
    /// Backing function template.
    /// </summary>
    public BadCompiledFunctionTemplate Function { get; }

    /// <summary>
    /// Materializes the method in the given execution context.
    /// </summary>
    public IEnumerable<BadObject> Instantiate(BadExecutionContext context)
    {
        return Function.Instantiate(context);
    }
}
