using BadScript2.Parser.Expressions.Binary;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <summary>
///     Defines a Compiler for a specific <see cref="BadBinaryExpression" />.
/// </summary>
/// <typeparam name="T">The Binary Expresion that this Compiler can Compile</typeparam>
public abstract class BadBinaryExpressionCompiler<T> : BadExpressionCompiler<T>
    where T : BadBinaryExpression
{
    /// <summary>
    ///     Indicates if the Expression is Left Associative
    /// </summary>
    protected virtual bool IsLeftAssociative => true;

    /// <summary>
    ///     Indicates if the Left Expression should be emitted
    /// </summary>
    protected virtual bool EmitLeft => true;

    /// <summary>
    ///     Indicates if the Right Expression should be emitted
    /// </summary>
    protected virtual bool EmitRight => true;

    /// <summary>
    ///     Compiles a Binary Expression
    /// </summary>
    /// <param name="compiler">The Compiler Instance</param>
    /// <param name="expression">The Expression to compile</param>
    /// <returns>List of Instructions</returns>
    public abstract void CompileBinary(BadExpressionCompileContext context, T expression);

    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, T expression)
    {
        if (IsLeftAssociative)
        {
            if (EmitLeft)
            {
                context.Compile(expression.Left);
            }

            if (EmitRight)
            {
                context.Compile(expression.Right);
            }
        }
        else
        {
            if (EmitRight)
            {
                context.Compile(expression.Right);
            }

            if (EmitLeft)
            {
                context.Compile(expression.Left);
            }
        }
        CompileBinary(context, expression);
    }
}