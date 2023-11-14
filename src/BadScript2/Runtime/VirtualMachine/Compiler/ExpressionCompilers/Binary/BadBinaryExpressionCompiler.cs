using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <summary>
/// Defines a Compiler for a specific <see cref="BadBinaryExpression"/>.
/// </summary>
/// <typeparam name="T">The Binary Expresion that this Compiler can Compile</typeparam>
public abstract class BadBinaryExpressionCompiler<T> : BadExpressionCompiler<T>
	where T : BadBinaryExpression
{
	protected virtual bool IsLeftAssociative => true;

	protected virtual bool EmitLeft => true;

	protected virtual bool EmitRight => true;

    /// <summary>
    ///     Compiles a Binary Expression
    /// </summary>
    /// <param name="compiler">The Compiler Instance</param>
    /// <param name="expression">The Expression to compile</param>
    /// <returns></returns>
    public abstract IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, T expression);

	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, T expression)
	{
		if (IsLeftAssociative)
		{
			if (EmitLeft)
			{
				foreach (BadInstruction instruction in compiler.Compile(expression.Left))
				{
					yield return instruction;
				}
			}

			if (EmitRight)
			{
				foreach (BadInstruction instruction in compiler.Compile(expression.Right))
				{
					yield return instruction;
				}
			}
		}
		else
		{
			if (EmitRight)
			{
				foreach (BadInstruction instruction in compiler.Compile(expression.Right))
				{
					yield return instruction;
				}
			}

			if (EmitLeft)
			{
				foreach (BadInstruction instruction in compiler.Compile(expression.Left))
				{
					yield return instruction;
				}
			}
		}

		foreach (BadInstruction instruction in CompileBinary(compiler, expression))
		{
			yield return instruction;
		}
	}
}
