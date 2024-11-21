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
    /// <summary>
    ///     Select Lambda Function
    /// </summary>
    /// <param name="varName">The Variable Name</param>
    /// <param name="query">The Query Expression</param>
    /// <param name="o">The Object to apply the query to</param>
    /// <typeparam name="T">The Input Type</typeparam>
    /// <typeparam name="TOut">The Output Type</typeparam>
    /// <returns>The Result of the Query</returns>
    /// <exception cref="Exception">Thrown if the query is invalid.</exception>
    private static TOut InnerSelect<T, TOut>(string varName, BadExpression query, T o)
    {
        BadExecutionContext ctx = BadLinqCommon.PredicateContextOptions.Build();

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
            r = o1;
        }

        r = r.Dereference(query.Position);

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

        throw new
            Exception($"Error in LINQ Where: {varName} => {query} : {r.ToSafeString()} is not a {typeof(TOut).Name}");
    }


    /// <summary>
    ///     Returns the first element in the enumerable that matches the given predicate or the default value if no element
    ///     matches the predicate.
    /// </summary>
    /// <param name="enumerable">The enumerable to search.</param>
    /// <param name="predicate">The predicate to match.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>
    ///     The first element in the enumerable that matches the given predicate or the default value if no element
    ///     matches the predicate.
    /// </returns>
    public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.FirstOrDefault(o => BadLinqCommon.InnerWhere(varName, query, o));
    }

    /// <summary>
    ///     Returns the first element in the enumerable that matches the given predicate or throws an exception if no element
    ///     matches the predicate.
    /// </summary>
    /// <param name="enumerable">The enumerable to search.</param>
    /// <param name="predicate">The predicate to match.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>
    ///     The first element in the enumerable that matches the given predicate or throws an exception if no element
    ///     matches the predicate.
    /// </returns>
    public static T First<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.First(o => BadLinqCommon.InnerWhere(varName, query, o));
    }

    /// <summary>
    ///     Returns the last element in the enumerable that matches the given predicate or the default value if no element
    ///     matches the predicate.
    /// </summary>
    /// <param name="enumerable">The enumerable to search.</param>
    /// <param name="predicate">The predicate to match.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>
    ///     The last element in the enumerable that matches the given predicate or the default value if no element matches
    ///     the predicate.
    /// </returns>
    public static T LastOrDefault<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.LastOrDefault(o => BadLinqCommon.InnerWhere(varName, query, o));
    }

    /// <summary>
    ///     Returns the last element in the enumerable that matches the given predicate or throws an exception if no element
    ///     matches the predicate.
    /// </summary>
    /// <param name="enumerable">The enumerable to search.</param>
    /// <param name="predicate">The predicate to match.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>
    ///     The last element in the enumerable that matches the given predicate or throws an exception if no element
    ///     matches the predicate.
    /// </returns>
    public static T Last<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.Last(o => BadLinqCommon.InnerWhere(varName, query, o));
    }

    /// <summary>
    ///     Selects a property from the given enumerables elements.
    /// </summary>
    /// <param name="enumerable">The enumerable to select from.</param>
    /// <param name="predicate">The predicate to select the property.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <typeparam name="TOut">The property type.</typeparam>
    /// <returns>The selected property.</returns>
    public static IEnumerable<TOut> Select<T, TOut>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.Select(o => InnerSelect<T, TOut>(varName, query, o));
    }

    /// <summary>
    ///     Selects a property from the given enumerables elements and flattens the result.
    /// </summary>
    /// <param name="enumerable">The enumerable to select from.</param>
    /// <param name="predicate">The predicate to select the property.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <typeparam name="TOut">The property type.</typeparam>
    /// <returns>The selected property.</returns>
    /// <exception cref="Exception">Thrown if the query is invalid.</exception>
    public static IEnumerable<TOut> SelectMany<T, TOut>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        foreach (T o in enumerable)
        {
            IEnumerable e = InnerSelect<T, IEnumerable>(varName, query, o);

            foreach (object o1 in e)
            {
                if (o1 is not TOut to)
                {
                    throw new
                        Exception($"Error in LINQ SelectMany: {varName} => {query} : {o1} is not a {typeof(TOut).Name}"
                                 );
                }

                yield return to;
            }
        }
    }

    /// <summary>
    ///     Orders the given enumerable by the given predicate.
    /// </summary>
    /// <param name="enumerable">The enumerable to order.</param>
    /// <param name="predicate">The predicate to order by.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>The ordered enumerable.</returns>
    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.OrderBy(o => InnerSelect<T, IComparable>(varName, query, o));
    }

    /// <summary>
    ///     Orders the given enumerable by the given predicate in descending order.
    /// </summary>
    /// <param name="enumerable">The enumerable to order.</param>
    /// <param name="predicate">The predicate to order by.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>The ordered enumerable.</returns>
    public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.OrderByDescending(o => InnerSelect<T, IComparable>(varName, query, o));
    }

    /// <summary>
    ///     Skips the elements while the given predicate is true.
    /// </summary>
    /// <param name="enumerable">The enumerable to skip.</param>
    /// <param name="predicate">The predicate to skip by.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>The skipped enumerable.</returns>
    public static IEnumerable<T> SkipWhile<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.SkipWhile(o => BadLinqCommon.InnerWhere(varName, query, o));
    }

    /// <summary>
    ///     Takes the elements while the given predicate is true.
    /// </summary>
    /// <param name="enumerable">The enumerable to take.</param>
    /// <param name="predicate">The predicate to take by.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>The taken enumerable.</returns>
    public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.TakeWhile(o => BadLinqCommon.InnerWhere(varName, query, o));
    }

    /// <summary>
    ///     Returns true if all elements in the enumerable match the given predicate.
    /// </summary>
    /// <param name="enumerable">The enumerable to check.</param>
    /// <param name="predicate">The predicate to check.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>True if all elements in the enumerable match the given predicate.</returns>
    public static bool All<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.All(o => BadLinqCommon.InnerWhere(varName, query, o));
    }

    /// <summary>
    ///     Returns true if any element in the enumerable matches the given predicate.
    /// </summary>
    /// <param name="enumerable">The enumerable to check.</param>
    /// <param name="predicate">The predicate to check.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>True if any element in the enumerable matches the given predicate.</returns>
    public static bool Any<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.Any(o => BadLinqCommon.InnerWhere(varName, query, o));
    }

    /// <summary>
    ///     Filters the given enumerable by the given predicate.
    /// </summary>
    /// <param name="enumerable">The enumerable to filter.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>The filtered enumerable.</returns>
    public static IEnumerable<T> Where<T>(this IEnumerable<T> enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);

        BadExpression query = BadLinqCommon.Parse(queryStr)
                                           .First();

        return enumerable.Where(o => BadLinqCommon.InnerWhere(varName, query, o));
    }
}