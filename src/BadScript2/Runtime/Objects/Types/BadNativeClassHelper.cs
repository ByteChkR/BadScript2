using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Objects.Types;

public static class BadNativeClassHelper
{
    private static readonly Dictionary<string, Func<BadObject[], BadObject>> s_NativeConstructors =
        new Dictionary<string, Func<BadObject[], BadObject>>
        {
            {
                "string",
                a =>
                {
                    if (a.Length != 1)
                    {
                        throw new BadRuntimeException("string constructor takes exactly one argument");
                    }

                    return BadObject.Wrap(a[0].ToString());
                }
            },
            {
                "bool",
                a =>
                {
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
                "num",
                a =>
                {
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
                "Function",
                a =>
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
                "Array",
                a =>
                {
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
                "Table",
                a =>
                {
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

    public static Func<BadObject[], BadObject> GetConstructor(string name)
    {
        return s_NativeConstructors[name];
    }
}