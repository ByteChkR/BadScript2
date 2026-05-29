using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Common;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Function;

/// <summary>
///     Compiles the <see cref="BadInvocationExpression" />.
/// </summary>
public class BadInvocationExpressionCompiler : BadExpressionCompiler<BadInvocationExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadInvocationExpression expression)
    {
        if (expression.Left is BadVariableExpression variableExpression &&
            variableExpression.Name == BadStaticKeys.BASE_KEY)
        {
            context.Compile(expression.Arguments, false);
            context.Compile(expression.Left);
            context.Emit(BadOpCode.LoadMember, expression.Position, BadStaticKeys.CONSTRUCTOR_NAME);
            context.Emit(BadOpCode.Invoke, expression.Position, expression.ArgumentCount);

            return;
        }

        if (expression.Left is BadMemberAccessExpression memberAccess && memberAccess.GenericArguments.Count == 0)
        {
            context.Compile(expression.Arguments, false);
            context.Compile(memberAccess.Left);
            context.Emit(BadOpCode.InvokeMember,
                         expression.Position,
                         expression.ArgumentCount,
                         memberAccess.Right.Text,
                         memberAccess.NullChecked
                        );

            return;
        }

        context.Compile(expression.Arguments, false);
        context.Compile(expression.Left);

        if (expression.Left is BadFunctionExpression fn &&
            (fn.CompileLevel == BadFunctionCompileLevel.Compiled || fn.CompileLevel == BadFunctionCompileLevel.CompiledFast))
        {
            context.Emit(BadOpCode.InvokeCompiled, expression.Position, expression.ArgumentCount);
        }
        else
        {
            context.Emit(BadOpCode.Invoke, expression.Position, expression.ArgumentCount);
        }
    }
}