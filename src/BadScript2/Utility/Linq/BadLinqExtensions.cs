using System.Collections;

using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;

namespace BadScript2.Utility.Linq
{
    public static class BadLinqExtensions
    {
        private static object? Unpack(this BadObject obj)
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

        private static object? InnerSelect(string varName, BadExpression query, object? o)
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
                    throw new Exception($"Error in LINQ Select: {varName} => {query} : {ctx.Scope.Error!.ToSafeString()}");
                }

                r = o1;
            }

            return r.Dereference().Unpack();
        }

        public static object? FirstOrDefault(this IEnumerable enumerable)
        {
            foreach (object o in enumerable)
            {
                return o;
            }

            return null;
        }

        public static object? FirstOrDefault(this IEnumerable enumerable, string predicate)
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

            return null;
        }

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

        public static IEnumerable TakeLast(this IEnumerable enumerable, int count)
        {
            List<object> l = new List<object>();
            foreach (object o in enumerable)
            {
                l.Add(o);
            }

            for (int i = l.Count - count; i < l.Count; i++)
            {
                yield return l[i];
            }
        }

        public static IEnumerable SkipLast(this IEnumerable enumerable, int count)
        {
            List<object> l = new List<object>();
            foreach (object o in enumerable)
            {
                l.Add(o);
            }

            for (int i = 0; i < l.Count - count; i++)
            {
                yield return l[i];
            }
        }

        public static IEnumerable SelectMany(this IEnumerable enumerable, string predicate)
        {
            (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
            BadExpression query = BadLinqCommon.Parse(queryStr).First();

            foreach (object o in enumerable)
            {
                object? e = InnerSelect(varName, query, o);
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

        public static IEnumerable Select(this IEnumerable enumerable, string predicate)
        {
            (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
            BadExpression query = BadLinqCommon.Parse(queryStr).First();

            foreach (object o in enumerable)
            {
                yield return InnerSelect(varName, query, o);
            }
        }

        public static IEnumerable OrderBy(this IEnumerable enumerable, string predicate)
        {
            (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
            BadExpression query = BadLinqCommon.Parse(queryStr).First();

            IEnumerable<object> e = enumerable.Cast<object>();

            return e.OrderBy(o => InnerSelect(varName, query, o));
        }

        public static IEnumerable OrderByDescending(this IEnumerable enumerable, string predicate)
        {
            (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
            BadExpression query = BadLinqCommon.Parse(queryStr).First();

            IEnumerable<object> e = enumerable.Cast<object>();

            return e.OrderByDescending(o => InnerSelect(varName, query, o));
        }


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

        public static bool Any(this IEnumerable enumerable)
        {
            foreach (object o in enumerable)
            {
                return true;
            }

            return false;
        }

        public static bool Any(this IEnumerable enumerable, string predicate)
        {
            (string varName, string queryStr) = BadLinqCommon.ParsePredicate(predicate);
            BadExpression query = BadLinqCommon.Parse(queryStr).First();

            foreach (object o in enumerable)
            {
                if (BadLinqCommon.InnerWhere(varName, query, o))
                {
                    return true;
                }
            }

            return false;
        }

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
}