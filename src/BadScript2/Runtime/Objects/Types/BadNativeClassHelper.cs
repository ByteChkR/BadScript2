using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Helper Class for Creating Native Class Prototypes
/// </summary>
public static class BadNativeClassHelper
{
    /// <summary>
    ///     The Constructors for the Native Types
    /// </summary>
    private static readonly Dictionary<string, Func<BadObject[], BadObject>> s_NativeConstructors =
        new Dictionary<string, Func<BadObject[], BadObject>>
        {
            {
                "string", a =>
                {
                    if (a.Length == 0) return string.Empty;
                    if (a.Length != 1)
                    {
                        throw new BadRuntimeException("string constructor takes exactly one argument");
                    }

                    return BadObject.Wrap(a[0]
                                              .ToString()
                                         );
                }
            },
            {
                "bool", a =>
                {
                    if (a.Length == 0) return BadObject.False;
                    if (a.Length != 1)
                    {
                        throw new BadRuntimeException("boolean constructor takes exactly one argument");
                    }

                    if (a[0] is BadBoolean boolVal)
                    {
                        return boolVal;
                    }

                    throw new BadRuntimeException("boolean constructor takes a boolean");
                }
            },
            {
                "num", a =>
                {
                    if (a.Length == 0) return 0;
                    if (a.Length != 1)
                    {
                        throw new BadRuntimeException("number constructor takes exactly one argument");
                    }

                    if (a[0] is BadNumber num)
                    {
                        return num;
                    }

                    throw new BadRuntimeException("number constructor takes a number");
                }
            },
            {
                "Function", a =>
                {
                    if (a.Length != 1)
                    {
                        throw new BadRuntimeException("function constructor takes exactly one argument");
                    }

                    if (a[0] is BadFunction func)
                    {
                        return func;
                    }

                    throw new BadRuntimeException("function constructor takes a function");
                }
            },
            {
                "Array", a =>
                {
                    if (a.Length == 0) return new BadArray();
                    if (a.Length != 1)
                    {
                        throw new BadRuntimeException("array constructor takes exactly one argument");
                    }

                    if (a[0] is BadArray arr)
                    {
                        return arr;
                    }

                    throw new BadRuntimeException("array constructor takes an array");
                }
            },
            {
                "Table", a =>
                {
                    if (a.Length == 0) return new BadTable();
                    if (a.Length != 1)
                    {
                        throw new BadRuntimeException("table constructor takes exactly one argument");
                    }

                    if (a[0] is BadTable table)
                    {
                        return table;
                    }

                    throw new BadRuntimeException("table constructor takes a table");
                }
            },
        };

    /// <summary>
    ///     Executes the Enumerator
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="enumerator">The Enumerator</param>
    /// <returns>The Enumeration</returns>
    /// <exception cref="BadRuntimeException">Thrown if the Enumerator is invalid</exception>
    public static IEnumerable<BadObject> ExecuteEnumerator(BadExecutionContext ctx, BadObject enumerator)
    {
        BadSourcePosition runtimePos = BadSourcePosition.Create("<runtime>", "", 0, 0);
        BadObject moveNext = enumerator.GetProperty("MoveNext")
                                       .Dereference(runtimePos);

        BadObject getCurrent = enumerator.GetProperty("GetCurrent")
                                         .Dereference(runtimePos);
        BadObject? result = BadObject.False;

        foreach (BadObject o in BadInvocationExpression.Invoke(moveNext, Array.Empty<BadObject>(), runtimePos, ctx))
        {
            result = o;
        }

        while (result.Dereference(runtimePos) == BadObject.True)
        {
            BadObject? obj = null;

            foreach (BadObject o in BadInvocationExpression.Invoke(getCurrent,
                                                                   Array.Empty<BadObject>(),
                                                                   runtimePos,
                                                                   ctx
                                                                  ))
            {
                obj = o;
            }

            if (obj == null)
            {
                throw new BadRuntimeException("Enumerator.GetCurrent() returned NULL");
            }

            yield return obj.Dereference(runtimePos);

            foreach (BadObject o in BadInvocationExpression.Invoke(moveNext, Array.Empty<BadObject>(), runtimePos, ctx))
            {
                result = o;
            }
        }
    }

    /// <summary>
    ///     Executes the Enumerable
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="enumerable">The Enumerable</param>
    /// <returns>The Enumeration</returns>
    /// <exception cref="BadRuntimeException">Thrown if the Enumerable is invalid</exception>
    public static IEnumerable<BadObject> ExecuteEnumerate(BadExecutionContext ctx, BadObject enumerable)
    {
        BadSourcePosition runtimePos = BadSourcePosition.Create("<runtime>", "", 0, 0);
        BadObject enumerator = enumerable.GetProperty("GetEnumerator")
                                         .Dereference(runtimePos);
        BadObject result = BadObject.Null;

        foreach (BadObject o in BadInvocationExpression.Invoke(enumerator, Array.Empty<BadObject>(), runtimePos, ctx))
        {
            result = o;
        }

        if (result == BadObject.Null)
        {
            throw BadRuntimeException.Create(ctx.Scope, "GetEnumerator() returned NULL");
        }

        return ExecuteEnumerator(ctx, result);
    }

    /// <summary>
    ///     Returns a Constructor for the given Native Type
    /// </summary>
    /// <param name="name">Name of the Type</param>
    /// <returns>Class Constructor</returns>
    public static Func<BadObject[], BadObject> GetConstructor(string name)
    {
        return s_NativeConstructors[name];
    }
}