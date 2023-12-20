using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <summary>
///     Compiles a BadScript Expression of type T
/// </summary>
/// <typeparam name="T">The BadExpression Type that this Compiler can Compile</typeparam>
public abstract class BadExpressionCompiler<T> : IBadExpressionCompiler
	where T : BadExpression
{
	/// <summary>
	///     Compiles an Expression
	/// </summary>
	/// <param name="compiler">The Compiler Instance</param>
	/// <param name="expression">The Expression to Compile</param>
	/// <returns>Enumeration of Instructions</returns>
	/// <exception cref="BadCompilerException">Gets raised if the expression does not match the typeof T</exception>
	IEnumerable<BadInstruction> IBadExpressionCompiler.Compile(BadCompiler compiler, BadExpression expression)
	{
		if (expression.GetType() != typeof(T))
		{
			throw new BadCompilerException("Invalid Expression Type");
		}

		return Compile(compiler, (T)expression);
	}

	/// <summary>
	///     Compiles an Expression
	/// </summary>
	/// <param name="compiler">The Compiler Instance</param>
	/// <param name="expression">The Expression to Compile</param>
	/// <returns>Enumeration of Instructions</returns>
	public abstract IEnumerable<BadInstruction> Compile(BadCompiler compiler, T expression);
}
