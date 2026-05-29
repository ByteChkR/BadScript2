using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
/// Contains Function Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Function;

/// <summary>
///     Compiles the <see cref="BadFunctionExpression" />.
/// </summary>
public class BadFunctionExpressionCompiler : BadExpressionCompiler<BadFunctionExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadFunctionExpression expression)
    {
        BadInstruction[]? compiledInstructions = null;
        bool? useOverrides = null;
        BadSymbolTable? symbolTable = null;
        bool requiresClosureScopeMaterialization = false;

        if (expression.CompileLevel == BadFunctionCompileLevel.Compiled)
        {
            if (BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath)
            {
                symbolTable = BadFunctionSymbolTableBuilder.Build(expression);
                requiresClosureScopeMaterialization =
                    symbolTable != null && BadFunctionSymbolTableBuilder.HasNestedFunctionLikeConstruct(expression);
            }
            compiledInstructions = BadCompiler.Compile(expression.Body).ToArray();
            useOverrides = true;
        }
        else if (expression.CompileLevel == BadFunctionCompileLevel.CompiledFast)
        {
            if (BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath)
            {
                symbolTable = BadFunctionSymbolTableBuilder.Build(expression);
                requiresClosureScopeMaterialization =
                    symbolTable != null && BadFunctionSymbolTableBuilder.HasNestedFunctionLikeConstruct(expression);
            }
            compiledInstructions = BadCompiler.Compile(expression.Body).ToArray();
            useOverrides = false;
        }

        BadCompiledFunctionTemplate template = new BadCompiledFunctionTemplate(expression,
                                                                               compiledInstructions,
                                                                               useOverrides,
                                                                               symbolTable,
                                                                               requiresClosureScopeMaterialization
                                                                              );
        context.Emit(BadOpCode.CreateFunction, expression.Position, template);
    }
}
