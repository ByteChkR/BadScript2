namespace BadScript2;
//
// public static class BadLambdaCompiler
// {
// 	private static MemberInfo FindMember(Type t, string name)
// 	{
// 		MemberInfo[] infos = t.GetMembers(BindingFlags.Public | BindingFlags.Instance);
//
// 		return infos.First(x => x.Name == name);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadMemberAccessExpression access)
// 	{
// 		Expression left = Compile(vars, access.Left);
//
// 		return Expression.MakeMemberAccess(left, FindMember(left.Type, access.Right.Text));
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadArrayAccessExpression access)
// 	{
// 		return Expression.ArrayAccess(Compile(vars, access.Left), access.Arguments.Select(x => Compile(vars, x)));
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadFormattedStringExpression str)
// 	{
// 		MethodInfo? method = typeof(string).GetMethod("Format",
// 			new[]
// 			{
// 				typeof(string),
// 				typeof(object[])
// 			});
//
// 		return Expression.Call(Expression.Constant(str.Value),
// 			method,
// 			Expression.NewArrayInit(typeof(object), str.Expressions.Select(x => Compile(vars, x))));
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadVariableExpression var)
// 	{
// 		return vars[var.Name];
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadAddExpression add)
// 	{
// 		Expression left = Compile(vars, add.Left);
// 		Expression right = Compile(vars, add.Right);
//
// 		return Expression.Add(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadSubtractExpression sub)
// 	{
// 		Expression left = Compile(vars, sub.Left);
// 		Expression right = Compile(vars, sub.Right);
//
// 		return Expression.Subtract(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadMultiplyExpression mul)
// 	{
// 		Expression left = Compile(vars, mul.Left);
// 		Expression right = Compile(vars, mul.Right);
//
// 		return Expression.Multiply(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadDivideExpression div)
// 	{
// 		Expression left = Compile(vars, div.Left);
// 		Expression right = Compile(vars, div.Right);
//
// 		return Expression.Divide(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadModulusExpression mod)
// 	{
// 		Expression left = Compile(vars, mod.Left);
// 		Expression right = Compile(vars, mod.Right);
//
// 		return Expression.Modulo(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadAddAssignExpression add)
// 	{
// 		Expression left = Compile(vars, add.Left);
// 		Expression right = Compile(vars, add.Right);
//
// 		return Expression.AddAssign(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadSubtractAssignExpression sub)
// 	{
// 		Expression left = Compile(vars, sub.Left);
// 		Expression right = Compile(vars, sub.Right);
//
// 		return Expression.SubtractAssign(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadMultiplyAssignExpression mul)
// 	{
// 		Expression left = Compile(vars, mul.Left);
// 		Expression right = Compile(vars, mul.Right);
//
// 		return Expression.MultiplyAssign(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadDivideAssignExpression div)
// 	{
// 		Expression left = Compile(vars, div.Left);
// 		Expression right = Compile(vars, div.Right);
//
// 		return Expression.DivideAssign(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadModulusAssignExpression mod)
// 	{
// 		Expression left = Compile(vars, mod.Left);
// 		Expression right = Compile(vars, mod.Right);
//
// 		return Expression.ModuloAssign(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadNullCoalescingExpression nc)
// 	{
// 		Expression left = Compile(vars, nc.Left);
// 		Expression right = Compile(vars, nc.Right);
//
// 		return Expression.Coalesce(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadTernaryExpression ternary)
// 	{
// 		Expression test = Compile(vars, ternary.Left);
// 		Expression ifTrue = Compile(vars, ternary.TrueRet);
// 		Expression ifFalse = Compile(vars, ternary.FalseRet);
//
// 		return Expression.Condition(test, ifTrue, ifFalse);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadLogicAndExpression and)
// 	{
// 		Expression left = Compile(vars, and.Left);
// 		Expression right = Compile(vars, and.Right);
//
// 		return Expression.And(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadLogicOrExpression or)
// 	{
// 		Expression left = Compile(vars, or.Left);
// 		Expression right = Compile(vars, or.Right);
//
// 		return Expression.Or(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadLogicXOrExpression xor)
// 	{
// 		Expression left = Compile(vars, xor.Left);
// 		Expression right = Compile(vars, xor.Right);
//
// 		return Expression.ExclusiveOr(left, right);
// 	}
//
//
// 	private static Expression Compile(BadLambdaVariables vars, BadLogicAssignAndExpression and)
// 	{
// 		Expression left = Compile(vars, and.Left);
// 		Expression right = Compile(vars, and.Right);
//
// 		return Expression.AndAssign(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadLogicAssignOrExpression or)
// 	{
// 		Expression left = Compile(vars, or.Left);
// 		Expression right = Compile(vars, or.Right);
//
// 		return Expression.OrAssign(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadLogicAssignXOrExpression xor)
// 	{
// 		Expression left = Compile(vars, xor.Left);
// 		Expression right = Compile(vars, xor.Right);
//
// 		return Expression.ExclusiveOrAssign(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadLogicNotExpression not)
// 	{
// 		Expression left = Compile(vars, not.Right);
//
// 		return Expression.Not(left);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadEqualityExpression eq)
// 	{
// 		Expression left = Compile(vars, eq.Left);
// 		Expression right = Compile(vars, eq.Right);
//
// 		return Expression.Equal(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadInequalityExpression eq)
// 	{
// 		Expression left = Compile(vars, eq.Left);
// 		Expression right = Compile(vars, eq.Right);
//
// 		return Expression.NotEqual(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadGreaterThanExpression gt)
// 	{
// 		Expression left = Compile(vars, gt.Left);
// 		Expression right = Compile(vars, gt.Right);
//
// 		return Expression.GreaterThan(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadGreaterOrEqualExpression ge)
// 	{
// 		Expression left = Compile(vars, ge.Left);
// 		Expression right = Compile(vars, ge.Right);
//
// 		return Expression.GreaterThanOrEqual(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadLessThanExpression lt)
// 	{
// 		Expression left = Compile(vars, lt.Left);
// 		Expression right = Compile(vars, lt.Right);
//
// 		return Expression.LessThan(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadLessOrEqualExpression le)
// 	{
// 		Expression left = Compile(vars, le.Left);
// 		Expression right = Compile(vars, le.Right);
//
// 		return Expression.LessThanOrEqual(left, right);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadPostIncrementExpression postInc)
// 	{
// 		return Expression.PostIncrementAssign(Compile(vars, postInc.Left));
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadPostDecrementExpression postDec)
// 	{
// 		return Expression.PostDecrementAssign(Compile(vars, postDec.Left));
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadPreIncrementExpression preInc)
// 	{
// 		return Expression.PreIncrementAssign(Compile(vars, preInc.Right));
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadPreDecrementExpression preDec)
// 	{
// 		return Expression.PreDecrementAssign(Compile(vars, preDec.Right));
// 	}
//
// 	private static Expression Compile(BadBooleanExpression b)
// 	{
// 		return Expression.Constant(b.Value);
// 	}
//
// 	private static Expression Compile(BadStringExpression s)
// 	{
// 		return Expression.Constant(s.Value);
// 	}
//
// 	private static Expression Compile(BadNullExpression n)
// 	{
// 		return Expression.Constant(null);
// 	}
//
// 	private static Expression Compile(BadNumberExpression n)
// 	{
// 		return Expression.Constant(n.Value);
// 	}
//
//
// 	private static Expression Compile(BadLambdaVariables vars, BadReturnExpression ret)
// 	{
// 		if (ret.Right != null)
// 		{
// 			return Compile(vars, ret.Right);
// 		}
//
// 		return Expression.Constant(null, vars.ReturnType);
// 	}
//
// 	private static Expression Compile(BadLambdaVariables vars, BadExpression expr)
// 	{
// 		switch (expr)
// 		{
// 			case BadMemberAccessExpression access:
// 				return Compile(vars, access);
// 			case BadArrayAccessExpression access:
// 				return Compile(vars, access);
// 			case BadFormattedStringExpression str:
// 				return Compile(vars, str);
// 			case BadVariableExpression var:
// 				return Compile(vars, var);
// 			case BadAddExpression add:
// 				return Compile(vars, add);
// 			case BadSubtractExpression sub:
// 				return Compile(vars, sub);
// 			case BadMultiplyExpression mul:
// 				return Compile(vars, mul);
// 			case BadDivideExpression div:
// 				return Compile(vars, div);
// 			case BadModulusExpression mod:
// 				return Compile(vars, mod);
// 			case BadAddAssignExpression add:
// 				return Compile(vars, add);
// 			case BadSubtractAssignExpression sub:
// 				return Compile(vars, sub);
// 			case BadMultiplyAssignExpression mul:
// 				return Compile(vars, mul);
// 			case BadDivideAssignExpression div:
// 				return Compile(vars, div);
// 			case BadModulusAssignExpression mod:
// 				return Compile(vars, mod);
// 			case BadNullCoalescingExpression nc:
// 				return Compile(vars, nc);
// 			case BadTernaryExpression ternary:
// 				return Compile(vars, ternary);
// 			case BadLogicAndExpression and:
// 				return Compile(vars, and);
// 			case BadLogicOrExpression or:
// 				return Compile(vars, or);
// 			case BadLogicXOrExpression xor:
// 				return Compile(vars, xor);
// 			case BadLogicAssignAndExpression and:
// 				return Compile(vars, and);
// 			case BadLogicAssignOrExpression or:
// 				return Compile(vars, or);
// 			case BadLogicAssignXOrExpression xor:
// 				return Compile(vars, xor);
// 			case BadLogicNotExpression not:
// 				return Compile(vars, not);
// 			case BadEqualityExpression eq:
// 				return Compile(vars, eq);
// 			case BadInequalityExpression eq:
// 				return Compile(vars, eq);
// 			case BadGreaterThanExpression gt:
// 				return Compile(vars, gt);
// 			case BadGreaterOrEqualExpression ge:
// 				return Compile(vars, ge);
// 			case BadLessThanExpression lt:
// 				return Compile(vars, lt);
// 			case BadLessOrEqualExpression le:
// 				return Compile(vars, le);
// 			case BadPostIncrementExpression postInc:
// 				return Compile(vars, postInc);
// 			case BadPostDecrementExpression postDec:
// 				return Compile(vars, postDec);
// 			case BadPreIncrementExpression preInc:
// 				return Compile(vars, preInc);
// 			case BadPreDecrementExpression preDec:
// 				return Compile(vars, preDec);
// 			case BadBooleanExpression b:
// 				return Compile(b);
// 			case BadStringExpression s:
// 				return Compile(s);
// 			case BadNullExpression n:
// 				return Compile(n);
// 			case BadNumberExpression n:
// 				return Compile(n);
// 			case BadReturnExpression r:
// 				return Compile(vars, r);
// 			default:
// 				throw new NotImplementedException();
// 		}
// 	}
//
// 	public static Expression<Func<TIn0, TOut>> Compile<TIn0, TOut>(this BadRuntime runtime, string expr)
// 	{
// 		BadExpression[] result = runtime.Parse(expr).ToArray();
//
// 		if (result.Length != 1 || result[0] is not BadFunctionExpression f)
// 		{
// 			throw new ArgumentException("Lambda must be a function");
// 		}
//
// 		return Compile<TIn0, TOut>(f);
// 	}
//
// 	public static Expression<Func<TIn0, TOut>> Compile<TIn0, TOut>(BadFunctionExpression expr)
// 	{
// 		BadFunctionParameter[] parameterExpressions = expr.Parameters.ToArray();
//
// 		if (parameterExpressions.Length != 1)
// 		{
// 			throw new ArgumentException("Lambda must have a single parameter");
// 		}
//
// 		BadExpression[] body = expr.Body.ToArray();
//
// 		if (body.Length != 1)
// 		{
// 			throw new ArgumentException("Lambda body must be a single expression");
// 		}
//
// 		List<Type> types = new List<Type>();
// 		types.Add(typeof(TIn0));
// 		types.Add(typeof(TOut));
// 		types.Add(typeof(string));
// 		Dictionary<string, ParameterExpression> parameters = new Dictionary<string, ParameterExpression>();
// 		parameters.Add(parameterExpressions[0].Name, Expression.Parameter(typeof(TIn0), parameterExpressions[0].Name));
// 		BadLambdaVariables vars = new BadLambdaVariables(typeof(TOut), parameters, types);
//
// 		return Expression.Lambda<Func<TIn0, TOut>>(Compile(vars, body[0]), parameters.Values);
// 	}
//
// 	private class BadLambdaVariables
// 	{
// 		private readonly List<Type> m_Types;
// 		private readonly Dictionary<string, ParameterExpression> m_Variables;
// 		public readonly Type ReturnType;
//
// 		public BadLambdaVariables(
// 			Type returnType,
// 			Dictionary<string, ParameterExpression> mVariables,
// 			List<Type> mTypes)
// 		{
// 			m_Variables = mVariables;
// 			m_Types = mTypes;
// 			ReturnType = returnType;
// 		}
//
// 		public ParameterExpression this[string name] => m_Variables[name];
//
// 		public Type GetType(string name)
// 		{
// 			return m_Types.First(x => x.Name == name);
// 		}
// 	}
// }
