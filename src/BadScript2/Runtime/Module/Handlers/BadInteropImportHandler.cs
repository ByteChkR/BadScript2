using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Module.Handlers;

/// <summary>
/// Implements an Import Handler for Implementing an Import Handler within BadScript2
/// </summary>
public class BadInteropImportHandler : BadImportHandler
{
    /// <summary>
    /// Holds the function that is used to resolve a module
    /// </summary>
    private readonly BadFunction m_GetFunction;
    /// <summary>
    /// Holds the function that is used to resolve a hash for a module
    /// </summary>
    private readonly BadFunction m_GetHashFunction;
    /// <summary>
    /// The Execution Context of the Handler
    /// </summary>
    private readonly BadExecutionContext m_HandlerContext;
    /// <summary>
    /// Holds the function that is used to check if a module exists in this handler
    /// </summary>
    private readonly BadFunction m_HasFunction;
    /// <summary>
    /// Holds the function that is used to check if a module is transient
    /// </summary>
    private readonly BadFunction m_IsTransientFunction;

    /// <summary>
    /// Creates a new BadInteropImportHandler instance
    /// </summary>
    /// <param name="ctx">The Execution Context of the Handler</param>
    /// <param name="scriptHandler">The Script Handler that implements the import handler</param>
    /// <exception cref="BadRuntimeException">Gets thrown if the script handler does not implement the required functions</exception>
    public BadInteropImportHandler(BadExecutionContext ctx, BadObject scriptHandler)
    {
        m_HandlerContext = ctx;

        BadObject has = scriptHandler.GetProperty("Has")
                                     .Dereference(null);

        if (has is not BadFunction hasF)
        {
            throw new BadRuntimeException("Has function not found");
        }

        m_HasFunction = hasF;

        BadObject get = scriptHandler.GetProperty("Get")
                                     .Dereference(null);

        if (get is not BadFunction getF)
        {
            throw new BadRuntimeException("Get function not found");
        }

        m_GetFunction = getF;

        BadObject getHash = scriptHandler.GetProperty("GetHash")
                                         .Dereference(null);

        if (getHash is not BadFunction getHashF)
        {
            throw new BadRuntimeException("GetHash function not found");
        }

        m_GetHashFunction = getHashF;

        BadObject isTransient = scriptHandler.GetProperty("IsTransient")
                                             .Dereference(null);

        if (isTransient is not BadFunction isTransientF)
        {
            throw new BadRuntimeException("IsTransient function not found");
        }

        m_IsTransientFunction = isTransientF;
    }

    /// <summary>
    /// Runs the specified function with the given arguments
    /// </summary>
    /// <param name="func">The function to run</param>
    /// <param name="args">The arguments to pass to the function</param>
    /// <returns>The result of the function</returns>
    private BadObject RunFunction(BadFunction func, params BadObject[] args)
    {
        IEnumerable<BadObject> e = func.Invoke(args, m_HandlerContext);
        BadObject? result = BadObject.Null;

        foreach (BadObject o in e)
        {
            result = o;
        }

        return result.Dereference(null);
    }

    
    /// <inheritdoc />
    public override bool IsTransient()
    {
        BadObject result = RunFunction(m_IsTransientFunction);

        if (result is not IBadBoolean b)
        {
            throw new BadRuntimeException("IsTransient function did not return a boolean");
        }

        return b.Value;
    }

    /// <inheritdoc />
    public override bool Has(string path)
    {
        BadObject result = RunFunction(m_HasFunction, path);

        if (result is not IBadBoolean b)
        {
            throw new BadRuntimeException("Has function did not return a boolean");
        }

        return b.Value;
    }

    /// <inheritdoc />
    public override string GetHash(string path)
    {
        BadObject result = RunFunction(m_GetHashFunction, path);

        if (result is not IBadString s)
        {
            throw new BadRuntimeException("GetHash function did not return a string");
        }

        return s.Value;
    }

    /// <inheritdoc />
    public override IEnumerable<BadObject> Get(string path)
    {
        BadObject? result = BadObject.Null;

        foreach (BadObject o in m_GetFunction.Invoke(new BadObject[] { path },
                                                     BadExecutionContext.Create(new BadInteropExtensionProvider())
                                                    ))
        {
            result = o;

            yield return o;
        }

        yield return result.Dereference(null);
    }
}