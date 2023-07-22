using System.Collections;

using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Operators;
using BadScript2.Reader;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Utility.Linq;

/// <summary>
///     Implements Common Functionality to parse Linq Queries in BadScript2
/// </summary>
internal static class BadLinqCommon
{
    public static readonly BadExecutionContextOptions PredicateContextOptions = new BadExecutionContextOptions();

    public static (string varName, string query) ParsePredicate(string query)
    {
        string[] parts = query.Split(
            new[]
            {
                "=>",
            },
            StringSplitOptions.RemoveEmptyEntries
        );
        string varName = parts[0].Trim();
        string queryStr = parts[1].Trim();

        return (varName, queryStr);
    }

    public static bool InnerWhere(string varName, BadExpression query, object? o)
    {
        BadExecutionContext ctx = PredicateContextOptions.Build();

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

        if (r is not IBadBoolean b)
        {
            throw new Exception($"Error in LINQ Where: {varName} => {query} : {r.ToSafeString()} is not a boolean");
        }

        return b.Value;
    }

    public static object?[] ToArray(this IEnumerable enumerable)
    {
        List<object?> list = new List<object?>();

        foreach (object? o in enumerable)
        {
            list.Add(o);
        }

        return list.ToArray();
    }

    public static IEnumerable<BadExpression> Parse(string src)
    {
        return new BadSourceParser(new BadSourceReader("<nofile>", src + ';'), BadOperatorTable.Instance).Parse();
    }
}