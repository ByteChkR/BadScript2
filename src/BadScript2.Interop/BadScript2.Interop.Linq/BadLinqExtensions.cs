using System;
using System.Linq;

using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Utility;

namespace BadScript2.Interop.Linq;

/// <summary>
///     Implements Linq Extensions
/// </summary>
public class BadLinqExtensions : BadInteropExtension
{
	/// <summary>
	///     Invokes the given function with the given arguments and unpacks the result to Type T
	/// </summary>
	/// <param name="func">Function</param>
	/// <param name="ctx">Execution Context</param>
	/// <param name="args">Arguments</param>
	/// <typeparam name="T">The Type to unpack to.</typeparam>
	/// <returns>Result of the Invocation</returns>
	private static T Invoke<T>(BadFunction func, BadExecutionContext ctx, params BadObject[] args)
    {
        BadObject o = BadObject.Null;

        foreach (BadObject obj in func.Invoke(args, ctx))
        {
            o = obj;
        }

        return o.Dereference(null)
                .Unwrap<T>();
    }

	/// <summary>
	///     Invokes the given function with the given arguments
	/// </summary>
	/// <param name="func">Function</param>
	/// <param name="ctx">Execution Context</param>
	/// <param name="args">Arguments</param>
	/// <returns>Result of the Invocation</returns>
	private static BadObject Invoke(BadFunction func, BadExecutionContext ctx, params BadObject[] args)
    {
        BadObject o = BadObject.Null;

        foreach (BadObject obj in func.Invoke(args, ctx))
        {
            o = obj;
        }

        return o.Dereference(null);
    }
	
	/// <summary>
	/// Computes the sum of the sequence of Decimal values.
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="selector">The Selector Function or null</param>
	/// <returns>The Sum of the Elements</returns>
	private static BadObject Sum(BadExecutionContext ctx, IBadEnumerable e, BadObject selector)
	{
		if (selector is BadFunction f)
		{
			return e.Sum((x) => Invoke<decimal>(f, ctx, x));
		}
		return e.Sum(x => x.Unwrap<decimal>());
	}

	/// <summary>
	///     Selects elements of the given enumerable with the given selector function
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="selector">The Filter Function</param>
	/// <returns>Enumeration</returns>
	private static BadObject Select(BadExecutionContext ctx, IBadEnumerable e, BadFunction selector)
    {
        return new BadInteropEnumerable(e.Select(x => Invoke(selector, ctx, x)));
    }
	
	/// <summary>
	/// Selects elements of the given enumerable with the given selector function
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="selector">The Filter Function</param>
	/// <returns>Enumeration</returns>
	private static BadObject SelectMany(BadExecutionContext ctx, IBadEnumerable e, BadFunction selector)
	{
		return new BadInteropEnumerable(e.SelectMany(x => Invoke<IBadEnumerable>(selector, ctx, x)));
	}
	
	/// <summary>
	/// Groups the elements of the given enumerable by the given key selector
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="keySelector">The Key Selector Function</param>
	/// <returns>Grouped Enumeration</returns>
	private static BadObject GroupBy(BadExecutionContext ctx, IBadEnumerable e, BadFunction keySelector)
	{
		return new BadInteropEnumerable(e
			.GroupBy(x => Invoke(keySelector, ctx, x))
			.Select(x => new BadInteropGroup(x)));
	}

	/// <summary>
	///     Filters the given enumerable with the given filter function
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="filter">The Filter Function</param>
	/// <returns>Filtered Enumeration</returns>
	private static BadObject Where(BadExecutionContext ctx, IBadEnumerable e, BadFunction filter)
    {
        return new BadInteropEnumerable(e.Where(x => Invoke<bool>(filter, ctx, x)));
    }

	/// <summary>
	///     Gets the first element in the enumerable that satisfies the given filter
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="o">The (optional) Filter Function</param>
	/// <returns>First element</returns>
	/// <exception cref="BadRuntimeException">Gets raised if the predicate is not null but also not a function</exception>
	private static BadObject First(BadExecutionContext ctx, IBadEnumerable e, BadObject o)
    {
        if (o == BadObject.Null)
        {
            return e.First();
        }

        if (o is BadFunction filter)
        {
            return e.First(x => Invoke<bool>(filter, ctx, x));
        }

        throw new BadRuntimeException("Invalid Filter");
    }

	/// <summary>
	///     Gets the first element in the enumerable that satisfies the given filter
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="o">The (optional) Filter Function</param>
	/// <returns>First element or null</returns>
	/// <exception cref="BadRuntimeException">Gets raised if the predicate is not null but also not a function</exception>
	private static BadObject FirstOrDefault(BadExecutionContext ctx, IBadEnumerable e, BadObject o)
    {
        if (o == BadObject.Null)
        {
            return e.FirstOrDefault() ?? BadObject.Null;
        }

        if (o is BadFunction filter)
        {
            return e.FirstOrDefault(x => Invoke<bool>(filter, ctx, x)) ?? BadObject.Null;
        }

        throw new BadRuntimeException("Invalid Filter");
    }

	/// <summary>
	///     Gets the last element in the enumerable that satisfies the given filter
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="o">The (optional) Filter Function</param>
	/// <returns>Last element</returns>
	/// <exception cref="BadRuntimeException">Gets raised if the predicate is not null but also not a function</exception>
	private static BadObject Last(BadExecutionContext ctx, IBadEnumerable e, BadObject o)
    {
        if (o == BadObject.Null)
        {
            return e.Last();
        }

        if (o is BadFunction filter)
        {
            return e.Last(x => Invoke<bool>(filter, ctx, x));
        }

        throw new BadRuntimeException("Invalid Filter");
    }

	/// <summary>
	///     Gets the last element in the enumerable that satisfies the given filter
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="o">The (optional) Filter Function</param>
	/// <returns>Last element or null</returns>
	/// <exception cref="BadRuntimeException">Gets raised if the predicate is not null but also not a function</exception>
	private static BadObject LastOrDefault(BadExecutionContext ctx, IBadEnumerable e, BadObject o)
    {
        if (o == BadObject.Null)
        {
            return e.LastOrDefault() ?? BadObject.Null;
        }

        if (o is BadFunction filter)
        {
            return e.LastOrDefault(x => Invoke<bool>(filter, ctx, x)) ?? BadObject.Null;
        }

        throw new BadRuntimeException("Invalid Filter");
    }

	/// <summary>
	///     Checks if any element matches the given filter
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="o">The (optional) Filter Function</param>
	/// <returns>True if any element satisfies the filter</returns>
	/// <exception cref="BadRuntimeException"></exception>
	private static BadObject Any(BadExecutionContext ctx, IBadEnumerable e, BadObject o)
    {
        if (o == BadObject.Null)
        {
            return e.Any();
        }

        if (o is BadFunction filter)
        {
            return e.Any(x => Invoke<bool>(filter, ctx, x));
        }

        throw new BadRuntimeException("Invalid Filter");
    }

	/// <summary>
	///     Appends an Object to the Enumerable
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="obj">The Object to Append</param>
	/// <returns>New Enumeration</returns>
	private static BadObject Append(BadExecutionContext ctx, IBadEnumerable e, BadObject obj)
    {
        return new BadInteropEnumerable(e.Append(obj));
    }

	/// <summary>
	///     Concatenates two Enumerables
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e1">The First Enumerable</param>
	/// <param name="e2">The Second Enumerable</param>
	/// <returns>Enumerable</returns>
	private static BadObject Concat(BadExecutionContext ctx, IBadEnumerable e1, IBadEnumerable e2)
    {
        return new BadInteropEnumerable(e1.Concat(e2));
    }

	/// <summary>
	///     Checks if all elements match the given filter
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="filter">The Filter Function</param>
	/// <returns>True if all elements satisfy the filter</returns>
	private static BadObject All(BadExecutionContext ctx, IBadEnumerable e, BadFunction filter)
    {
        return e.All(x => Invoke<bool>(filter, ctx, x));
    }

	/// <summary>
	///     Counts all Elements
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="predicate">
	///     (Optional) Predicate Function. If specified, only counts the elements that this predicate
	///     matches
	/// </param>
	/// <returns>Number of Elements in the Enumerable</returns>
	/// <exception cref="BadRuntimeException">Gets raised if the predicate is not null but also not a function</exception>
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

	/// <summary>
	///     Implementation for 'ElementAt' function.
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="index">Index of the element to return</param>
	/// <returns>The element at the index</returns>
	private static BadObject ElementAt(BadExecutionContext ctx, IBadEnumerable e, decimal index)
    {
        return e.ElementAt((int)index);
    }

	/// <summary>
	///     Implementation for 'ElementAtOrDefault' function.
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="index">Index of the element to return</param>
	/// <returns>The element at the index or BadObject.Null</returns>
	private static BadObject ElementAtOrDefault(BadExecutionContext ctx, IBadEnumerable e, decimal index)
    {
        return e.ElementAtOrDefault((int)index) ?? BadObject.Null;
    }

	/// <summary>
	///     Implementation for 'ToTable' function.
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="keySelector">Key Selector Function</param>
	/// <param name="valueSelector">Value Selector Function</param>
	/// <returns>Bad Table</returns>
	private static BadObject ToTable(BadExecutionContext ctx,
	                                 IBadEnumerable e,
	                                 BadFunction keySelector,
	                                 BadFunction valueSelector)
    {
        return new BadTable(e.ToDictionary(v =>
                                           {
                                               BadObject k = Invoke(keySelector, ctx, v);

                                               if (k is not IBadString s)
                                               {
                                                   throw BadRuntimeException.Create(ctx.Scope, "Invalid Key");
                                               }

                                               return s.Value;
                                           },
                                           v => Invoke(valueSelector, ctx, v)
                                          )
                           );
    }


	/// <summary>
	///     Implementation for 'Skip' function.
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="index">The Amount of items to skip</param>
	/// <returns>Enumeration</returns>
	private static BadObject Skip(BadExecutionContext ctx, IBadEnumerable e, decimal index)
    {
        return new BadInteropEnumerable(e.Skip((int)index));
    }

	/// <summary>
	///     Implementation for 'Take' function.
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="index">The Amount of items to take</param>
	/// <returns>Enumeration</returns>
	private static BadObject Take(BadExecutionContext ctx, IBadEnumerable e, decimal index)
    {
        return new BadInteropEnumerable(e.Take((int)index));
    }

	/// <summary>
	///     Implementation for 'SkipLast' function.
	/// </summary>
	/// <param name="ctx">The Execution Context</param>
	/// <param name="e">The Enumerable</param>
	/// <param name="index">The Amount of items to skip</param>
	/// <returns>Enumeration</returns>
	private static BadObject SkipLast(BadExecutionContext ctx, IBadEnumerable e, decimal index)
    {
        return new BadInteropEnumerable(e.SkipLast((int)index));
    }

	/// <summary>
	///     Implementation for 'ToArray' function.
	/// </summary>
	/// <param name="e">The Enumerable</param>
	/// <returns>BadArray</returns>
	private static BadObject ToArray(IBadEnumerable e)
    {
        return new BadArray(e.ToList());
    }

	/// <summary>
	///     Implementation for 'Reverse' function.
	/// </summary>
	/// <param name="e">The Enumerable</param>
	/// <returns>Enumeration</returns>
	private static BadObject Reverse(IBadEnumerable e)
    {
        return new BadInteropEnumerable(e.Reverse());
    }

    /// <inheritdoc />
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        Register(provider, "ToArray", (_, e) => ToArray(e), BadNativeClassBuilder.GetNative("Array"));
        Register(provider, "Reverse", (_, e) => Reverse(e), BadAnyPrototype.Instance);
        Register<BadFunction>(provider, "Select", Select, BadNativeClassBuilder.Enumerable,
	        new BadFunctionParameter("selector", false, true, false, null, BadFunction.Prototype));
        Register<BadFunction>(provider, "Where", Where, BadNativeClassBuilder.Enumerable,
	        new BadFunctionParameter("selector", false, true, false, null, BadFunction.Prototype));
        Register<BadFunction>(provider, "All", All, BadNativeClassBuilder.GetNative("bool"),
	        new BadFunctionParameter("selector", false, true, false, null, BadFunction.Prototype));
        Register<BadObject>(provider, "Append", Append, BadNativeClassBuilder.Enumerable,
	        new BadFunctionParameter("element", false, true, false, null, BadAnyPrototype.Instance));
        Register<IBadEnumerable>(provider, "Concat", Concat, BadNativeClassBuilder.Enumerable,
	        new BadFunctionParameter("selector", false, true, false, null, BadNativeClassBuilder.Enumerable));
        Register<decimal>(provider, "ElementAt", ElementAt, BadAnyPrototype.Instance, 
	        new BadFunctionParameter("index", false, true, false, null, BadNativeClassBuilder.GetNative("num")));
        Register<decimal>(provider, "ElementAtOrDefault", ElementAtOrDefault, BadAnyPrototype.Instance, 
	        new BadFunctionParameter("index", false, true, false, null, BadNativeClassBuilder.GetNative("num")));
        Register<decimal>(provider, "Skip", Skip, BadNativeClassBuilder.Enumerable, 
	        new BadFunctionParameter("count", false, true, false, null, BadNativeClassBuilder.GetNative("num")));
        Register<decimal>(provider, "SkipLast", SkipLast, BadNativeClassBuilder.Enumerable, 
	        new BadFunctionParameter("count", false, true, false, null, BadNativeClassBuilder.GetNative("num")));
        Register<decimal>(provider, "Take", Take, BadNativeClassBuilder.Enumerable, 
	        new BadFunctionParameter("count", false, true, false, null, BadNativeClassBuilder.GetNative("num")));
        Register<BadFunction>(provider, "OrderBy", OrderBy, BadNativeClassBuilder.Enumerable,
	        new BadFunctionParameter("selector", false, true, false, null, BadNativeClassBuilder.Enumerable));
        provider.RegisterObject<IBadEnumerable>("Sum",
												e => new BadInteropFunction("Sum",
																			(c, args) => Sum(c, e, args.Length == 0 ? BadObject.Null : args[0]),
																			false,
																			BadNativeClassBuilder.GetNative("num"),
																			new BadFunctionParameter("selector",
																				 true,
																				 false,
																				 false,
																				 null
																				)
																		   )
											   );

        provider.RegisterObject<IBadEnumerable>("SelectMany",
												e => new BadDynamicInteropFunction<BadFunction>("SelectMany",
													 (c, ks) => SelectMany(c, e, ks),
													 BadAnyPrototype.Instance,
													 new BadFunctionParameter("selector", false, true, false, null, BadFunction.Prototype)
													)
											   );
        provider.RegisterObject<IBadEnumerable>("GroupBy",
												e => new BadDynamicInteropFunction<BadFunction>("GroupBy",
													 (c, ks) => GroupBy(c, e, ks),
													 BadAnyPrototype.Instance,
													 new BadFunctionParameter("selector", false, true, false, null, BadFunction.Prototype)
													)
											   );
        provider.RegisterObject<IBadEnumerable>("First",
                                                e => new BadInteropFunction("First",
                                                                            (c, args) => First(c, e, args[0]),
                                                                            false,
                                                                            BadAnyPrototype.Instance,
                                                                            new BadFunctionParameter("selector",
                                                                                 true,
                                                                                 false,
                                                                                 false,
                                                                                 null
                                                                                )
                                                                           )
                                               );

        provider.RegisterObject<IBadEnumerable>("FirstOrDefault",
                                                e => new BadInteropFunction("FirstOrDefault",
                                                                            (c, args) => FirstOrDefault(c, e, args[0]),
                                                                            false,
                                                                            BadAnyPrototype.Instance,
                                                                            new BadFunctionParameter("selector",
                                                                                 true,
                                                                                 false,
                                                                                 false,
                                                                                 null
                                                                                )
                                                                           )
                                               );

        provider.RegisterObject<IBadEnumerable>("Last",
                                                e => new BadInteropFunction("Last",
                                                                            (c, args) => Last(c, e, args[0]),
                                                                            false,
                                                                            BadAnyPrototype.Instance,
                                                                            new BadFunctionParameter("selector",
                                                                                 true,
                                                                                 false,
                                                                                 false,
                                                                                 null
                                                                                )
                                                                           )
                                               );

        provider.RegisterObject<IBadEnumerable>("LastOrDefault",
                                                e => new BadInteropFunction("LastOrDefault",
                                                                            (c, args) => LastOrDefault(c, e, args[0]),
                                                                            false,
                                                                            BadAnyPrototype.Instance,
                                                                            new BadFunctionParameter("selector",
                                                                                 true,
                                                                                 false,
                                                                                 false,
                                                                                 null
                                                                                )
                                                                           )
                                               );

        provider.RegisterObject<IBadEnumerable>("Any",
                                                e => new BadInteropFunction("Any",
                                                                            (c, args) => Any(c, e, args[0]),
                                                                            false,
                                                                            BadNativeClassBuilder.GetNative("bool"),
                                                                            new BadFunctionParameter("filter",
                                                                                 true,
                                                                                 false,
                                                                                 false,
                                                                                 null
                                                                                )
                                                                           )
                                               );

        provider.RegisterObject<IBadEnumerable>("Count",
                                                e => new BadInteropFunction("Count",
                                                                            (c, args) => Count(c, e, args.Length == 0 ? BadObject.Null : args[0]),
                                                                            false,
                                                                            BadNativeClassBuilder.GetNative("num"),
                                                                            new BadFunctionParameter("predicate",
                                                                                 true,
                                                                                 false,
                                                                                 false,
                                                                                 null
                                                                                )
                                                                           )
                                               );

        provider.RegisterObject<IBadEnumerable>("ToTable",
                                                e => new BadDynamicInteropFunction<BadFunction, BadFunction>("ToTable",
                                                     (c, ks, vs) => ToTable(c, e, ks, vs),
                                                     BadNativeClassBuilder.GetNative("Table"),
                                                     new BadFunctionParameter("keySelector", false, true, false, null, BadFunction.Prototype),
                                                     new BadFunctionParameter("valueSelector", false, true, false, null, BadFunction.Prototype)
                                                    )
                                               );
    }

    /// <summary>
    ///     Implementation for 'OrderBy' function.
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="e">The Enumerable</param>
    /// <param name="function">The Selector Function</param>
    /// <returns>Enumeration</returns>
    private static BadObject OrderBy(BadExecutionContext ctx, IBadEnumerable e, BadFunction function)
    {
        return new BadInteropEnumerable(e.OrderBy(o =>
                                                  {
                                                      BadObject? r = BadObject.Null;

                                                      foreach (BadObject o1 in function.Invoke(new[] { o },
                                                                    ctx
                                                                   ))
                                                      {
                                                          r = o1;
                                                      }

                                                      return r.Dereference(null);
                                                  }
                                                 )
                                       );
    }

    /// <summary>
    ///     Shorthand Wrapper for RegisterObject.
    ///     Registers a Linq Function that takes a IBadEnumerable object and returns a BadObject.
    /// </summary>
    /// <param name="provider">The Extension Provider of the Runtime</param>
    /// <param name="name">Name of the Function</param>
    /// <param name="func">Function</param>
    /// <param name="returnType">The Return Type of the Function</param>
    private static void Register(BadInteropExtensionProvider provider,
                                 string name,
                                 Func<BadExecutionContext, IBadEnumerable, BadObject> func,
                                 BadClassPrototype returnType)
    {
        provider.RegisterObject<IBadEnumerable>(name,
                                                o => new BadDynamicInteropFunction(name, c => func(c, o), returnType)
                                               );
    }

    /// <summary>
    ///     Shorthand Wrapper for RegisterObject.
    ///     Registers a Linq Function that takes a IBadEnumerable object, an arbitrary object and returns a BadObject.
    /// </summary>
    /// <param name="provider">The Extension Provider of the Runtime</param>
    /// <param name="name">Name of the Function</param>
    /// <param name="func">Function</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="param">The Parameter Definition of the Function</param>
    private static void Register<T>(BadInteropExtensionProvider provider,
                                    string name,
                                    Func<BadExecutionContext, IBadEnumerable, T, BadObject> func,
                                    BadClassPrototype returnType, BadFunctionParameter param)
    {
        provider.RegisterObject<IBadEnumerable>(name,
                                                o => new BadDynamicInteropFunction<T>(name,
                                                     (c, a) => func(c, o, a),
                                                     returnType,param
                                                    )
                                               );
    }
}