using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.Common.Task;

/// <summary>
///     Implements a Runnable Object
/// </summary>
public abstract class BadRunnable
{
    /// <summary>
    ///     The Runtime Error
    /// </summary>
    public BadRuntimeError? Error { get; private set; }

    /// <summary>
    ///     Empty Runnable
    /// </summary>
    public static BadRunnable Empty { get; } = new BadEmptyRunnable();

    /// <summary>
    ///     The Enumerator of the Runnable
    /// </summary>
    public abstract IEnumerator<BadObject> Enumerator { get; }

    /// <summary>
    ///     Gets the Return Value
    /// </summary>
    /// <returns>The Return Value</returns>
    public abstract BadObject GetReturn();

    /// <summary>
    ///     Sets the Runtime Error
    /// </summary>
    /// <param name="err">The Error</param>
    public void SetError(BadRuntimeError err)
    {
        Error = err;
    }

    /// <summary>
    ///     Creates a Runnable from an Enumeration
    /// </summary>
    /// <param name="e">Enumeration</param>
    /// <returns>Runnable</returns>
    public static BadRunnable Create(IEnumerable<BadObject> e)
    {
        return Create(e.GetEnumerator());
    }

    /// <summary>
    ///     Creates a Runnable from an Enumeration
    /// </summary>
    /// <param name="e">Enumerator</param>
    /// <returns>Runnable</returns>
    public static BadRunnable Create(IEnumerator<BadObject> e)
    {
        return new BadRunnableImpl(e);
    }

    /// <summary>
    ///     Creates a Runnable from a Function
    /// </summary>
    /// <param name="func">Function</param>
    /// <param name="ctx">Execution Context</param>
    /// <param name="args">Function Arguments</param>
    /// <returns>Runnable</returns>
    public static BadRunnable Create(BadFunction func, BadExecutionContext ctx, params BadObject[] args)
    {
        return new BadFunctionRunnable(func, ctx, args);
    }

    /// <summary>
    ///     Implements the 'Empty' Runnable
    /// </summary>
    private class BadEmptyRunnable : BadRunnable
    {
        // ReSharper disable once NotDisposedResourceIsReturnedByProperty
        public override IEnumerator<BadObject> Enumerator => Enumerable.Empty<BadObject>().GetEnumerator();

        public override BadObject GetReturn()
        {
            return BadObject.Null;
        }
    }


    /// <summary>
    ///     Implements the Runnable
    /// </summary>
    private class BadRunnableImpl : BadRunnable
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="enumerator">Enumeration</param>
        public BadRunnableImpl(IEnumerator<BadObject> enumerator)
        {
            Enumerator = enumerator;
        }

        /// <inheritdoc />
        public override IEnumerator<BadObject> Enumerator { get; }

        /// <inheritdoc />
        public override BadObject GetReturn()
        {
            return BadObject.Null;
        }
    }

    /// <summary>
    ///     Implements a Function Runnable
    /// </summary>
    private class BadFunctionRunnable : BadRunnable
    {
        /// <summary>
        ///     The Return Value
        /// </summary>
        private BadObject m_ReturnValue = BadObject.Null;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="func">Function</param>
        /// <param name="ctx">Execution Context</param>
        /// <param name="args">Function Arguments</param>
        public BadFunctionRunnable(BadFunction func, BadExecutionContext ctx, params BadObject[] args)
        {
            Enumerator = RunnableFunction(func, ctx, args).GetEnumerator();
        }

        /// <inheritdoc />
        public override IEnumerator<BadObject> Enumerator { get; }


        /// <summary>
        ///     Creates an Enumeration from a Function
        /// </summary>
        /// <param name="func">Function</param>
        /// <param name="ctx">Execution Context</param>
        /// <param name="args">Function Arguments</param>
        /// <returns>Enumeration</returns>
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

        /// <inheritdoc />
        public override BadObject GetReturn()
        {
            return m_ReturnValue;
        }
    }
}