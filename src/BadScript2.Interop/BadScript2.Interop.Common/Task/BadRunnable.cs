using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.Common.Task;

public abstract class BadRunnable
{
    public abstract IEnumerator<BadObject> Enumerator { get; }

    public abstract BadObject GetReturn();


    public static BadRunnable Create(IEnumerable<BadObject> e)
    {
        return Create(e.GetEnumerator());
    }

    public static BadRunnable Create(IEnumerator<BadObject> e)
    {
        return new BadRunnableImpl(e);
    }

    public static BadRunnable Create(BadFunction func, BadExecutionContext ctx, params BadObject[] args)
    {
        return new BadFunctionRunnable(func, ctx, args);
    }


    private class BadRunnableImpl : BadRunnable
    {
        public BadRunnableImpl(IEnumerator<BadObject> enumerator)
        {
            Enumerator = enumerator;
        }

        public override IEnumerator<BadObject> Enumerator { get; }

        public override BadObject GetReturn()
        {
            return BadObject.Null;
        }
    }

    private class BadFunctionRunnable : BadRunnable
    {
        private BadObject m_ReturnValue = BadObject.Null;

        public BadFunctionRunnable(BadFunction func, BadExecutionContext ctx, params BadObject[] args)
        {
            Enumerator = RunnableFunction(func, ctx, args).GetEnumerator();
        }

        public override IEnumerator<BadObject> Enumerator { get; }


        private IEnumerable<BadObject> RunnableFunction(
            BadFunction func,
            BadExecutionContext ctx,
            params BadObject[] args)
        {
            BadObject obj = BadObject.Null;
            foreach (BadObject o in func.Invoke(args, ctx))
            {
                obj = o;

                yield return o;
            }

            m_ReturnValue = obj.Dereference();
        }

        public override BadObject GetReturn()
        {
            return m_ReturnValue;
        }
    }
}