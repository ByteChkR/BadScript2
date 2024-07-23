using System.Collections;

using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
namespace BadScript2.Utility.Linq;

/// <summary>
///     Implements LINQ Extensions
/// </summary>
public static class BadLinqExtensions
{
    /// <summary>
    ///     Unpacks the given BadObject.
    /// </summary>
    /// <param name="obj">The Object to unpack</param>
    /// <returns>The unpacked Object</returns>
    private static object Unpack(this BadObject obj)
    {
        if (obj.CanUnwrap())
        {
            return obj.Unwrap();
        }

        if (obj is BadReflectedObject ro)
        {
            return ro.Instance;
        }

        return obj;
    }

    /// <summary>
    ///     The inner select function for the Linq Extensions.
    /// </summary>
    /// <param name="varName">The Variable Name</param>
    /// <param name="query">The Query Expression</param>
    /// <param name="o">The Object to apply the query to</param>
    /// <returns>The Result of the Query</returns>
    /// <exception cref="Exception">Thrown if the query is invalid.</exception>
    private static object InnerSelect(string varName, BadExpression query, object? o)
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

        return r.Dereference().Unpack();
    }

    /// <summary>
    ///     Returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to get the first element from.</param>
    /// <returns>The first element of the sequence, or a default value if the sequence contains no elements.</returns>
    public static object? FirstOrDefault(this IEnumerable enumerable)
    {
        return Enumerable.FirstOrDefault(enumerable.Cast<object>());
    }

    /// <summary>
    ///     Returns the first element of the sequence that satisfies a condition or a default value if no such element is
    ///     found.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to get the first element from.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The first element of the sequence that satisfies a condition or a default value if no such element is found.</returns>
    public static object? FirstOrDefault(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        return enumerable.Cast<object>().FirstOrDefault(o => BadLinqCommon.InnerWhere(varName, query, o));
    }

    /// <summary>
    ///     Returns the first element of the sequence that satisfies a condition.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to get the first element from.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The first element of the sequence that satisfies a condition.</returns>
    /// <exception cref="Exception">Thrown if no matching element is found.</exception>
    public static object First(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        foreach (object o in enumerable)
        {
            if (BadLinqCommon.InnerWhere(varName, query, o))
            {
                return o;
            }
        }

        throw new Exception("No matching element found");
    }

    /// <summary>
    ///     Returns the last element of the sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to get the last element from.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The last element of the sequence that satisfies a condition or a default value if no such element is found.</returns>
    public static object? LastOrDefault(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();
        object? last = null;

        foreach (object o in enumerable)
        {
            if (BadLinqCommon.InnerWhere(varName, query, o))
            {
                last = o;
            }
        }

        return last;
    }

    /// <summary>
    ///     Returns the last element of the sequence that satisfies a condition.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to get the last element from.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The last element of the sequence that satisfies a condition.</returns>
    public static object Last(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        object? last = null;

        foreach (object o in enumerable)
        {
            if (BadLinqCommon.InnerWhere(varName, query, o))
            {
                last = o;
            }
        }

        return last ?? throw new Exception("No matching element found");
    }

    /// <summary>
    ///     Takes the first count elements from the given IEnumerable.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to take the elements from.</param>
    /// <param name="count">The number of elements to take.</param>
    /// <returns>The first count elements from the given IEnumerable.</returns>
    public static IEnumerable Take(this IEnumerable enumerable, int count)
    {
        int i = 0;

        foreach (object o in enumerable)
        {
            if (i >= count)
            {
                yield break;
            }

            yield return o;

            i++;
        }
    }

    /// <summary>
    ///     Skips the first count elements from the given IEnumerable.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to skip the elements from.</param>
    /// <param name="count">The number of elements to skip.</param>
    /// <returns>The IEnumerable with the first count elements skipped.</returns>
    public static IEnumerable Skip(this IEnumerable enumerable, int count)
    {
        int i = 0;

        foreach (object o in enumerable)
        {
            if (i >= count)
            {
                yield return o;
            }

            i++;
        }
    }

    /// <summary>
    ///     Takes the last count elements from the given IEnumerable.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to take the elements from.</param>
    /// <param name="count">The number of elements to take.</param>
    /// <returns>The last count elements from the given IEnumerable.</returns>
    public static IEnumerable TakeLast(this IEnumerable enumerable, int count)
    {
        List<object> l = enumerable.Cast<object>().ToList();

        for (int i = l.Count - count; i < l.Count; i++)
        {
            yield return l[i];
        }
    }

    /// <summary>
    ///     Skips the last count elements from the given IEnumerable.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to skip the elements from.</param>
    /// <param name="count">The number of elements to skip.</param>
    /// <returns>The IEnumerable with the last count elements skipped.</returns>
    public static IEnumerable SkipLast(this IEnumerable enumerable, int count)
    {
        List<object> l = enumerable.Cast<object>().ToList();

        for (int i = 0; i < l.Count - count; i++)
        {
            yield return l[i];
        }
    }

    /// <summary>
    ///     Selects the given elements from the given IEnumerable.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to select the elements from.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The selected elements from the given IEnumerable.</returns>
    /// <exception cref="Exception">Thrown if the query is invalid.</exception>
    public static IEnumerable SelectMany(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        foreach (object o in enumerable)
        {
            object e = InnerSelect(varName, query, o);

            if (e is not IEnumerable en)
            {
                throw new Exception("SelectMany must return an IEnumerable");
            }

            foreach (object o1 in en)
            {
                yield return o1;
            }
        }
    }

    /// <summary>
    ///     Selects the given elements from the given IEnumerable.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to select the elements from.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The selected elements from the given IEnumerable.</returns>
    public static IEnumerable Select(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        foreach (object o in enumerable)
        {
            yield return InnerSelect(varName, query, o);
        }
    }

    /// <summary>
    ///     Orders the given IEnumerable by the given predicate.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to order.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The ordered IEnumerable.</returns>
    public static IEnumerable OrderBy(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        IEnumerable<object> e = enumerable.Cast<object>();

        return e.OrderBy(o => InnerSelect(varName, query, o));
    }

    /// <summary>
    ///     Orders the given IEnumerable by the given predicate in descending order.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to order.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The ordered IEnumerable.</returns>
    public static IEnumerable OrderByDescending(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        IEnumerable<object> e = enumerable.Cast<object>();

        return e.OrderByDescending(o => InnerSelect(varName, query, o));
    }


    /// <summary>
    ///     Skips elements from the given IEnumerable while the given predicate is true.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to skip the elements from.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The IEnumerable with the skipped elements.</returns>
    public static IEnumerable SkipWhile(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();
        bool skip = true;

        foreach (object o in enumerable)
        {
            if (skip && BadLinqCommon.InnerWhere(varName, query, o))
            {
                continue;
            }

            skip = false;

            yield return o;
        }
    }

    /// <summary>
    ///     Takes elements from the given IEnumerable while the given predicate is true.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to take the elements from.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The IEnumerable with the taken elements.</returns>
    public static IEnumerable TakeWhile(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        foreach (object o in enumerable)
        {
            if (BadLinqCommon.InnerWhere(varName, query, o))
            {
                yield return o;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    ///     Returns true if all elements of the given IEnumerable satisfy the given predicate.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to check.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>True if all elements of the given IEnumerable satisfy the given predicate.</returns>
    public static bool All(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        foreach (object o in enumerable)
        {
            if (!BadLinqCommon.InnerWhere(varName, query, o))
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    ///     Returns true if there is any element in the given IEnumerable
    /// </summary>
    /// <param name="enumerable">The IEnumerable to check.</param>
    /// <returns>True if there is any element in the given IEnumerable</returns>
    public static bool Any(this IEnumerable enumerable)
    {
        return Enumerable.Any(enumerable.Cast<object>());
    }

    /// <summary>
    ///     Returns true if any element of the given IEnumerable satisfies the given predicate.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to check.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>True if any element of the given IEnumerable satisfies the given predicate.</returns>
    public static bool Any(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        return enumerable.Cast<object>().Any(o => BadLinqCommon.InnerWhere(varName, query, o));
    }

    /// <summary>
    ///     Filters the given IEnumerable by the given predicate.
    /// </summary>
    /// <param name="enumerable">The IEnumerable to filter.</param>
    /// <param name="predicate">The predicate to apply to the elements.</param>
    /// <returns>The filtered IEnumerable.</returns>
    public static IEnumerable Where(this IEnumerable enumerable, string predicate)
    {
        (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
        BadExpression query = BadLinqCommon.Parse(queryStr).First();

        foreach (object o in enumerable)
        {
            if (BadLinqCommon.InnerWhere(varName, query, o))
            {
                yield return o;
            }
        }
    }
}