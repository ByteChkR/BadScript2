using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Types
{
    public class BadNewExpression : BadExpression
    {
        public readonly BadInvocationExpression Right;

        public BadNewExpression(BadInvocationExpression right, BadSourcePosition position) : base(false, false, position)
        {
            Right = right;
        }

        public override void Optimize()
        {
            Right.Optimize();
        }

        public static IEnumerable<BadObject> CreateObject(
            BadClassPrototype proto,
            BadExecutionContext context,
            BadObject[] args,
            BadSourcePosition pos)
        {
            BadObject obj = BadObject.Null;

            if (proto is BadNativeClassPrototype nativeType)
            {
                foreach (BadObject o in nativeType.CreateInstance(context, args))
                {
                    yield return o;
                }

                yield break;
            }


            foreach (BadObject o in proto.CreateInstance(context))
            {
                obj = o;

                yield return o;
            }

            obj = obj.Dereference();

            if (obj is not BadClass cls)
            {
                throw new BadRuntimeException("Cannot create object from non-class type", pos);
            }

            //Call Constructor if exists

            if (cls.HasProperty(cls.Name))
            {
                BadObject ctor = cls.GetProperty(cls.Name).Dereference();
                if (ctor is not BadFunction func)
                {
                    throw new BadRuntimeException("Cannot create object from non-function type", pos);
                }

                foreach (BadObject o in func.Invoke(args, context))
                {
                    yield return o;
                }
            }

            yield return cls;
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadObject obj = BadObject.Null;

            //Get Type from Right
            foreach (BadObject o in Right.Left.Execute(context))
            {
                obj = o;

                yield return o;
            }

            obj = obj.Dereference();

            if (obj is not BadClassPrototype ptype)
            {
                throw new BadRuntimeException("Cannot create object from non-class type", Position);
            }

            List<BadObject> args = new List<BadObject>();
            foreach (BadObject o in Right.GetArgs(context, args))
            {
                yield return o;
            }

            foreach (BadObject o in CreateObject(ptype, context, args.ToArray(), Position))
            {
                yield return o;
            }
        }
    }
}