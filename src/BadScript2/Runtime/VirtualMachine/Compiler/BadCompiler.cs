using BadScript2.Common;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.Parser.Expressions.Binary.Logic.Assign;
using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.Parser.Expressions.Binary.Math.Assign;
using BadScript2.Parser.Expressions.Binary.Math.Atomic;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Function;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Types;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Variables;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

public class BadCompiler
{
	public static readonly BadCompiler Instance = new BadCompiler();

	public readonly bool AllowEval;

	private readonly Dictionary<Type, IBadExpressionCompiler> m_Compilers = new Dictionary<Type, IBadExpressionCompiler>
	{
		{
			typeof(BadVariableExpression), new BadVariableExpressionCompiler()
		},
		{
			typeof(BadVariableDefinitionExpression), new BadVariableDefinitionExpressionCompiler()
		},
		{
			typeof(BadMemberAccessExpression), new BadMemberAccessExpressionCompiler()
		},
		{
			typeof(BadEqualityExpression), new BadEqualityExpressionCompiler()
		},
		{
			typeof(BadInequalityExpression), new BadInequalityExpressionCompiler()
		},
		{
			typeof(BadLessThanExpression), new BadLessExpressionCompiler()
		},
		{
			typeof(BadLessOrEqualExpression), new BadLessOrEqualExpressionCompiler()
		},
		{
			typeof(BadGreaterOrEqualExpression), new BadGreaterOrEqualExpressionCompiler()
		},
		{
			typeof(BadGreaterThanExpression), new BadGreaterExpressionCompiler()
		},
		{
			typeof(BadLogicAndExpression), new BadLogicAndExpressionCompiler()
		},
		{
			typeof(BadLogicOrExpression), new BadLogicOrExpressionCompiler()
		},
		{
			typeof(BadLogicXOrExpression), new BadLogicXOrExpressionCompiler()
		},
		{
			typeof(BadLogicNotExpression), new BadLogicNotExpressionCompiler()
		},
		{
			typeof(BadLogicAssignAndExpression), new BadLogicAssignAndExpressionCompiler()
		},
		{
			typeof(BadLogicAssignOrExpression), new BadLogicAssignOrExpressionCompiler()
		},
		{
			typeof(BadLogicAssignXOrExpression), new BadLogicAssignXOrExpressionCompiler()
		},
		{
			typeof(BadAddExpression), new BadAddExpressionCompiler()
		},
		{
			typeof(BadSubtractExpression), new BadSubtractExpressionCompiler()
		},
		{
			typeof(BadMultiplyExpression), new BadMultiplyExpressionCompiler()
		},
		{
			typeof(BadExponentiationExpression), new BadExponentiationExpressionCompiler()
		},
		{
			typeof(BadDeleteExpression), new BadDeleteExpressionCompiler()
		},
		{
			typeof(BadInstanceOfExpression), new BadInstanceOfExpressionCompiler()
		},
		{
			typeof(BadTypeOfExpression), new BadTypeOfExpressionCompiler()
		},
		{
			typeof(BadDivideExpression), new BadDivideExpressionCompiler()
		},
		{
			typeof(BadModulusExpression), new BadModulusExpressionCompiler()
		},
		{
			typeof(BadAddAssignExpression), new BadAddAssignExpressionCompiler()
		},
		{
			typeof(BadSubtractAssignExpression), new BadSubtractAssignExpressionCompiler()
		},
		{
			typeof(BadMultiplyAssignExpression), new BadMultiplyAssignExpressionCompiler()
		},
		{
			typeof(BadDivideAssignExpression), new BadDivideAssignExpressionCompiler()
		},
		{
			typeof(BadModulusAssignExpression), new BadModulusAssignExpressionCompiler()
		},
		{
			typeof(BadPostDecrementExpression), new BadPostDecrementExpressionCompiler()
		},
		{
			typeof(BadPostIncrementExpression), new BadPostIncrementExpressionCompiler()
		},
		{
			typeof(BadPreDecrementExpression), new BadPreDecrementExpressionCompiler()
		},
		{
			typeof(BadPreIncrementExpression), new BadPreIncrementExpressionCompiler()
		},
		{
			typeof(BadAssignExpression), new BadAssignExpressionCompiler()
		},
		{
			typeof(BadBooleanExpression), new BadBooleanExpressionCompiler()
		},
		{
			typeof(BadNumberExpression), new BadNumberExpressionCompiler()
		},
		{
			typeof(BadStringExpression), new BadStringExpressionCompiler()
		},
		{
			typeof(BadNullExpression), new BadNullExpressionCompiler()
		},
		{
			typeof(BadIfExpression), new BadIfExpressionCompiler()
		},
		{
			typeof(BadReturnExpression), new BadReturnExpressionCompiler()
		},
		{
			typeof(BadContinueExpression), new BadContinueExpressionCompiler()
		},
		{
			typeof(BadBreakExpression), new BadBreakExpressionCompiler()
		},
		{
			typeof(BadThrowExpression), new BadThrowExpressionCompiler()
		},
		{
			typeof(BadWhileExpression), new BadWhileExpressionCompiler()
		},
		{
			typeof(BadForExpression), new BadForExpressionCompiler()
		},
		{
			typeof(BadFormattedStringExpression), new BadFormattedStringExpressionCompiler()
		},
		{
			typeof(BadConstantExpression), new BadConstantExpressionCompiler()
		},
		{
			typeof(BadArrayAccessExpression), new BadArrayAccessExpressionCompiler()
		},
		{
			typeof(BadArrayAccessReverseExpression), new BadArrayAccessReverseExpressionCompiler()
		},
		{
			typeof(BadTernaryExpression), new BadTernaryExpressionCompiler()
		},
		{
			typeof(BadNullCoalescingExpression), new BadNullCoalescingExpressionCompiler()
		},
		{
			typeof(BadNullCoalescingAssignExpression), new BadNullCoalescingAssignExpressionCompiler()
		},
		{
			typeof(BadLockExpression), new BadLockExpressionCompiler()
		},
		{
			typeof(BadArrayExpression), new BadArrayExpressionCompiler()
		},
		{
			typeof(BadTableExpression), new BadTableExpressionCompiler()
		},
		{
			typeof(BadInvocationExpression), new BadInvocationExpressionCompiler()
		},
		{
			typeof(BadForEachExpression), new BadForEachExpressionCompiler()
		},
		{
			typeof(BadNewExpression), new BadNewExpressionCompiler()
		},
		{
			typeof(BadRangeExpression), new BadRangeExpressionCompiler()
		},
		{
			typeof(BadTryCatchExpression), new BadTryCatchExpressionCompiler()
		},
		{
			typeof(BadInExpression), new BadInExpressionCompiler()
		}
	};

	public BadCompiler(bool allowEval = true)
	{
		AllowEval = allowEval;
	}

	public IEnumerable<BadInstruction> Compile(BadExpression expression)
	{
		Type t = expression.GetType();

		if (!m_Compilers.TryGetValue(t, out IBadExpressionCompiler compiler))
		{
			if (AllowEval)
			{
				return new[]
				{
					new BadInstruction(BadOpCode.Eval, expression.Position, expression)
				};
			}

			throw new BadCompilerException("No Compiler for Expression Type " + expression.GetType().Name);
		}

		return compiler.Compile(this, expression);
	}

	public IEnumerable<BadInstruction> Compile(IEnumerable<BadExpression> expressions, bool clearStack = true)
	{
		foreach (BadExpression expression in expressions)
		{
			BadSourcePosition? position = null;

			foreach (BadInstruction instruction in Compile(expression))
			{
				position = instruction.Position;

				yield return instruction;
			}

			if (clearStack && position != null)
			{
				yield return new BadInstruction(BadOpCode.ClearStack, position);
			}
		}
	}

	public static IEnumerable<BadInstruction> Compile(string src)
	{
		BadSourceParser parser = BadSourceParser.Create("<nofile>", src);

		return Instance.Compile(parser.Parse());
	}
}
