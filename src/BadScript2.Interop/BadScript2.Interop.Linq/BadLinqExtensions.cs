using System;
using System.Linq;

using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Utility;

namespace BadScript2.Interop.Linq;

public class BadLinqExtensions : BadInteropExtension
{
	private static T Invoke<T>(BadFunction func, BadExecutionContext ctx, params BadObject[] args)
	{
		BadObject o = BadObject.Null;

		foreach (BadObject obj in func.Invoke(args, ctx))
		{
			o = obj;
		}

		return o.Dereference().Unwrap<T>();
	}

	private static BadObject Invoke(BadFunction func, BadExecutionContext ctx, params BadObject[] args)
	{
		BadObject o = BadObject.Null;

		foreach (BadObject obj in func.Invoke(args, ctx))
		{
			o = obj;
		}

		return o.Dereference();
	}

	private static BadObject Select(BadExecutionContext context, IBadEnumerable e, BadFunction selector)
	{
		return new BadInteropEnumerable(e.Select(x => Invoke(selector, context, x)));
	}

	private static BadObject Where(BadExecutionContext context, IBadEnumerable e, BadFunction filter)
	{
		return new BadInteropEnumerable(e.Where(x => Invoke<bool>(filter, context, x)));
	}

	private static BadObject First(BadExecutionContext context, IBadEnumerable e, BadObject o)
	{
		if (o == BadObject.Null)
		{
			return e.First();
		}

		if (o is BadFunction filter)
		{
			return e.First(x => Invoke<bool>(filter, context, x));
		}

		throw new BadRuntimeException("Invalid Filter");
	}

	private static BadObject FirstOrDefault(BadExecutionContext context, IBadEnumerable e, BadObject o)
	{
		if (o == BadObject.Null)
		{
			return e.FirstOrDefault() ?? BadObject.Null;
		}

		if (o is BadFunction filter)
		{
			return e.FirstOrDefault(x => Invoke<bool>(filter, context, x)) ?? BadObject.Null;
		}

		throw new BadRuntimeException("Invalid Filter");
	}

	private static BadObject Last(BadExecutionContext context, IBadEnumerable e, BadObject o)
	{
		if (o == BadObject.Null)
		{
			return e.Last();
		}

		if (o is BadFunction filter)
		{
			return e.Last(x => Invoke<bool>(filter, context, x));
		}

		throw new BadRuntimeException("Invalid Filter");
	}

	private static BadObject LastOrDefault(BadExecutionContext context, IBadEnumerable e, BadObject o)
	{
		if (o == BadObject.Null)
		{
			return e.LastOrDefault() ?? BadObject.Null;
		}

		if (o is BadFunction filter)
		{
			return e.LastOrDefault(x => Invoke<bool>(filter, context, x)) ?? BadObject.Null;
		}

		throw new BadRuntimeException("Invalid Filter");
	}

	private static BadObject Any(BadExecutionContext context, IBadEnumerable e, BadObject o)
	{
		if (o == BadObject.Null)
		{
			return e.Any();
		}

		if (o is BadFunction filter)
		{
			return e.Any(x => Invoke<bool>(filter, context, x));
		}

		throw new BadRuntimeException("Invalid Filter");
	}

	private static BadObject Append(BadExecutionContext context, IBadEnumerable e, BadObject obj)
	{
		return new BadInteropEnumerable(e.Append(obj));
	}

	private static BadObject Concat(BadExecutionContext context, IBadEnumerable e1, IBadEnumerable e2)
	{
		return new BadInteropEnumerable(e1.Concat(e2));
	}

	private static BadObject All(BadExecutionContext context, IBadEnumerable e, BadFunction filter)
	{
		return e.All(x => Invoke<bool>(filter, context, x));
	}

	private static BadObject Count(BadExecutionContext ctx, IBadEnumerable e, BadObject predicate)
	{
		if (predicate == BadObject.Null)
		{
			return e.Count();
		}

		if (predicate is BadFunction func)
		{
			return e.Count(x => Invoke<bool>(func, ctx, x));
		}

		throw new BadRuntimeException("Invalid Predicate");
	}

	private static BadObject ElementAt(BadExecutionContext ctx, IBadEnumerable e, decimal index)
	{
		return e.ElementAt((int)index);
	}

	private static BadObject ElementAtOrDefault(BadExecutionContext ctx, IBadEnumerable e, decimal index)
	{
		return e.ElementAtOrDefault((int)index) ?? BadObject.Null;
	}

	private static BadObject ToTable(
		BadExecutionContext ctx,
		IBadEnumerable e,
		BadFunction keySelector,
		BadFunction valueSelector)
	{
		return new BadTable(e.ToDictionary(v => Invoke(keySelector, ctx, v), v => Invoke(valueSelector, ctx, v)));
	}

	private static BadObject Skip(BadExecutionContext ctx, IBadEnumerable e, decimal index)
	{
		return new BadInteropEnumerable(e.Skip((int)index));
	}

	private static BadObject Take(BadExecutionContext ctx, IBadEnumerable e, decimal index)
	{
		return new BadInteropEnumerable(e.Take((int)index));
	}

	private static BadObject SkipLast(BadExecutionContext ctx, IBadEnumerable e, decimal index)
	{
		return new BadInteropEnumerable(e.SkipLast((int)index));
	}

	private static BadObject ToArray(IBadEnumerable e)
	{
		return new BadArray(e.ToList());
	}

	private static BadObject Reverse(IBadEnumerable e)
	{
		return new BadInteropEnumerable(e.Reverse());
	}


	protected override void AddExtensions()
	{
		Register("ToArray", (_, e) => ToArray(e));
		Register("Reverse", (_, e) => Reverse(e));
		Register<BadFunction>("Select", Select);
		Register<BadFunction>("Where", Where);
		Register<BadFunction>("All", All);
		Register<BadObject>("Append", Append);
		Register<IBadEnumerable>("Concat", Concat);
		Register<decimal>("ElementAt", ElementAt);
		Register<decimal>("ElementAtOrDefault", ElementAtOrDefault);
		Register<decimal>("Skip", Skip);
		Register<decimal>("SkipLast", SkipLast);
		Register<decimal>("Take", Take);
		RegisterObject<IBadEnumerable>("First",
			e => new BadInteropFunction("First",
				(c, args) => First(c, e, args[0]),
				new BadFunctionParameter("selector", true, false, false, null)));

		RegisterObject<IBadEnumerable>("FirstOrDefault",
			e => new BadInteropFunction("FirstOrDefault",
				(c, args) => FirstOrDefault(c, e, args[0]),
				new BadFunctionParameter("selector", true, false, false, null)));

		RegisterObject<IBadEnumerable>("Last",
			e => new BadInteropFunction("Last",
				(c, args) => Last(c, e, args[0]),
				new BadFunctionParameter("selector", true, false, false, null)));

		RegisterObject<IBadEnumerable>("LastOrDefault",
			e => new BadInteropFunction("LastOrDefault",
				(c, args) => LastOrDefault(c, e, args[0]),
				new BadFunctionParameter("selector", true, false, false, null)));
		RegisterObject<IBadEnumerable>("Any",
			e => new BadInteropFunction("Any",
				(c, args) => FirstOrDefault(c, e, args[0]),
				new BadFunctionParameter("filter", true, false, false, null)));
		RegisterObject<IBadEnumerable>("Count",
			e => new BadInteropFunction("Count",
				(c, args) => Count(c, e, args[0]),
				new BadFunctionParameter("predicate", true, false, false, null)));

		RegisterObject<IBadEnumerable>("ToTable",
			e => new BadDynamicInteropFunction<BadFunction, BadFunction>("ToTable",
				(c, ks, vs) => ToTable(c, e, ks, vs)));
	}

	private static void Register(string name, Func<BadExecutionContext, IBadEnumerable, BadObject> func)
	{
		RegisterObject<IBadEnumerable>(name, o => new BadDynamicInteropFunction(name, c => func(c, o)));
	}

	private static void Register<T>(string name, Func<BadExecutionContext, IBadEnumerable, T, BadObject> func)
	{
		RegisterObject<IBadEnumerable>(name, o => new BadDynamicInteropFunction<T>(name, (c, a) => func(c, o, a)));
	}
}
