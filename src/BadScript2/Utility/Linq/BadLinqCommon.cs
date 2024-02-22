using System.Collections;

using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Operators;
using BadScript2.Reader;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

/// <summary>
/// Contains the Linq Implementation and Extensions
/// </summary>
namespace BadScript2.Utility.Linq;

/// <summary>
///     Implements Common Functionality to parse Linq Queries in BadScript2
/// </summary>
internal static class BadLinqCommon
{
    /// <summary>
    ///     The Predicate Context Options for the Linq Extensions.
    /// </summary>
    public static readonly BadExecutionContextOptions PredicateContextOptions = new BadExecutionContextOptions();

    /// <summary>
    ///     Parses a Predicate Query into a Variable Name and a Query Expression.
    /// </summary>
    /// <param name="query">The Query to parse</param>
    /// <returns>(Variable Name, Query Expression)</returns>
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

    /// <summary>
    ///     The Where Lamda Function for the Linq Extensions.
    /// </summary>
    /// <param name="varName">The Variable Name</param>
    /// <param name="query">The Query Expression</param>
    /// <param name="o">The Object to apply the query to</param>
    /// <returns>True if the Object matches the Query</returns>
    /// <exception cref="Exception">Thrown if the query is invalid.</exception>
    public static bool InnerWhere(string varName, BadExpression query, object? o)
    {
        BadExecutionContext ctx = PredicateContextOptions.Build();

        if (o is BadObject bo)
        {
            ctx.Scope.DefineVariable(varName, bo);
        }
        else
        {
            ctx.Scope.DefineVariable(varName, BadObject.CanWrap(o) ? BadObject.Wrap(o) : new BadReflectedObject(o!));
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

    /// <summary>
    ///     Materializes the given IEnumerable into an Array.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to materialize.</param>
    /// <returns>The Materialized Array.</returns>
    public static object?[] ToArray(this IEnumerable enumerable)
    {
        return Enumerable.ToArray(enumerable.Cast<object?>());
    }

    /// <summary>
    ///     Parses the given source into an IEnumerable of BadExpressions.
    /// </summary>
    /// <param name="src">The Source to parse.</param>
    /// <returns>The IEnumerable of BadExpressions.</returns>
    public static IEnumerable<BadExpression> Parse(string src)
    {
        return new BadSourceParser(new BadSourceReader("<nofile>", src + ';'), BadOperatorTable.Instance).Parse();
    }
}