using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <summary>
///     Compiles a BadScript Expression of type T
/// </summary>
/// <typeparam name="T">The BadExpression Type that this Compiler can Compile</typeparam>
public abstract class BadExpressionCompiler<T> : IBadExpressionCompiler
    where T : BadExpression
{
#region IBadExpressionCompiler Members

	/// <summary>
	///     Compiles an Expression
	/// </summary>
	/// <param name="context">The Context of the Compilation</param>
	/// <param name="expression">The Expression to Compile</param>
	/// <returns>Enumeration of Instructions</returns>
	/// <exception cref="BadCompilerException">Gets raised if the expression does not match the typeof T</exception>
	void IBadExpressionCompiler.Compile(BadExpressionCompileContext context, BadExpression expression)
    {
        if (expression.GetType() != typeof(T))
        {
            throw new BadCompilerException("Invalid Expression Type");
        }

        Compile(context, (T)expression);
    }

#endregion

	/// <summary>
	///     Compiles an Expression
	/// </summary>
	/// <param name="context">The Context of the Compilation</param>
	/// <param name="expression">The Expression to Compile</param>
	/// <returns>Enumeration of Instructions</returns>
	public abstract void Compile(BadExpressionCompileContext context, T expression);
}