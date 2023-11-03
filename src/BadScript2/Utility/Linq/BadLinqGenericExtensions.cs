using System.Collections;

using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;

namespace BadScript2.Utility.Linq;

/// <summary>
///     Implements Generic LINQ Extensions
/// </summary>
public static class BadLinqGenericExtensions
{
	private static TOut InnerSelect<T, TOut>(string varName, BadExpression query, T o)
	{
		BadExecutionContext ctx = BadLinqCommon.PredicateContextOptions.Build();

		if (o is BadObject bo)
		{
			ctx.Scope.DefineVariable(varName, bo);
		}
		else
		{
			if (BadObject.CanWrap(o))
			{
				ctx.Scope.DefineVariable(varName, BadObject.Wrap(o));
			}
			else
			{
				ctx.Scope.DefineVariable(varName, new BadReflectedObject(o!));
			}
		}

		BadObject r = BadObject.Null;

		foreach (BadObject o1 in query.Execute(ctx))
		{
			if (ctx.Scope.IsError)
			{
				throw new Exception($"Error in LINQ Where: {varName} => {query} : {ctx.Scope.Error!.ToSafeString()}");
			}

			r = o1;
		}

		r = r.Dereference();

		if (r is TOut t)
		{
			return t;
		}

		if (r.CanUnwrap())
		{
			return r.Unwrap<TOut>();
		}

		if (r is BadReflectedObject ro && ro.Type.IsAssignableFrom(typeof(TOut)))
		{
			return (TOut)ro.Instance;
		}

		throw new Exception(
			$"Error in LINQ Where: {varName} => {query} : {r.ToSafeString()} is not a {typeof(TOut).Name}");
	}


	public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.FirstOrDefault(o => BadLinqCommon.InnerWhere(varName, query, o));
	}

	public static T First<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.First(o => BadLinqCommon.InnerWhere(varName, query, o));
	}

	public static T LastOrDefault<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.LastOrDefault(o => BadLinqCommon.InnerWhere(varName, query, o));
	}

	public static T Last<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.Last(o => BadLinqCommon.InnerWhere(varName, query, o));
	}

	public static IEnumerable<TOut> Select<T, TOut>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.Select(o => InnerSelect<T, TOut>(varName, query, o));
	}

	public static IEnumerable<TOut> SelectMany<T, TOut>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		foreach (T o in enumerable)
		{
			IEnumerable e = InnerSelect<T, IEnumerable>(varName, query, o);

			foreach (object o1 in e)
			{
				if (o1 is not TOut to)
				{
					throw new Exception(
						$"Error in LINQ SelectMany: {varName} => {query} : {o1} is not a {typeof(TOut).Name}");
				}

				yield return to;
			}
		}
	}

	public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.OrderBy(o => InnerSelect<T, IComparable>(varName, query, o));
	}

	public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.OrderByDescending(o => InnerSelect<T, IComparable>(varName, query, o));
	}

	public static IEnumerable<T> SkipWhile<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.SkipWhile(o => BadLinqCommon.InnerWhere(varName, query, o));
	}

	public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.TakeWhile(o => BadLinqCommon.InnerWhere(varName, query, o));
	}

	public static bool All<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.All(o => BadLinqCommon.InnerWhere(varName, query, o));
	}

	public static bool Any<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.Any(o => BadLinqCommon.InnerWhere(varName, query, o));
	}

	public static IEnumerable<T> Where<T>(this IEnumerable<T> enumerable, string predicate)
	{
		(string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
		BadExpression query = BadLinqCommon.Parse(queryStr).First();

		return enumerable.Where(o => BadLinqCommon.InnerWhere(varName, query, o));
	}
}
