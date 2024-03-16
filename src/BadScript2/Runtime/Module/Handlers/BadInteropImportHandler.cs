using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Module.Handlers;

public class BadInteropImportHandler : BadImportHandler
{
    private readonly BadFunction m_GetFunction;
    private readonly BadFunction m_GetHashFunction;
    private readonly BadExecutionContext m_HandlerContext;
    private readonly BadFunction m_HasFunction;
    private readonly BadFunction m_IsTransientFunction;

    public BadInteropImportHandler(BadExecutionContext ctx, BadObject scriptHandler)
    {
        m_HandlerContext = ctx;
        BadObject has = scriptHandler.GetProperty("Has").Dereference();
        if (has is not BadFunction hasF)
        {
            throw new BadRuntimeException("Has function not found");
        }

        m_HasFunction = hasF;
        BadObject get = scriptHandler.GetProperty("Get").Dereference();
        if (get is not BadFunction getF)
        {
            throw new BadRuntimeException("Get function not found");
        }

        m_GetFunction = getF;
        BadObject getHash = scriptHandler.GetProperty("GetHash").Dereference();
        if (getHash is not BadFunction getHashF)
        {
            throw new BadRuntimeException("GetHash function not found");
        }

        m_GetHashFunction = getHashF;

        BadObject isTransient = scriptHandler.GetProperty("IsTransient").Dereference();

        if (isTransient is not BadFunction isTransientF)
        {
            throw new BadRuntimeException("IsTransient function not found");
        }

        m_IsTransientFunction = isTransientF;
    }

    private void EnsureSuccess()
    {
        if (m_HandlerContext.Scope.IsError)
        {
            throw new BadRuntimeErrorException(m_HandlerContext.Scope.Error);
        }
    }

    private BadObject RunFunction(BadFunction func, params BadObject[] args)
    {
        IEnumerable<BadObject> e = func.Invoke(args, m_HandlerContext);
        BadObject? result = BadObject.Null;
        foreach (BadObject o in e)
        {
            result = o;
        }

        EnsureSuccess();

        return result.Dereference();
    }

    public override bool IsTransient()
    {
        BadObject result = RunFunction(m_IsTransientFunction);
        if (result is not IBadBoolean b)
        {
            throw new BadRuntimeException("IsTransient function did not return a boolean");
        }

        return b.Value;
    }

    public override bool Has(string path)
    {
        BadObject result = RunFunction(m_HasFunction, path);
        if (result is not IBadBoolean b)
        {
            throw new BadRuntimeException("Has function did not return a boolean");
        }

        return b.Value;
    }

    public override string GetHash(string path)
    {
        BadObject result = RunFunction(m_GetHashFunction, path);
        if (result is not IBadString s)
        {
            throw new BadRuntimeException("GetHash function did not return a string");
        }

        return s.Value;
    }

    public override IEnumerable<BadObject> Get(string path)
    {
        BadObject? result = BadObject.Null;
        foreach (BadObject o in m_GetFunction.Invoke(new BadObject[] { path }, BadExecutionContext.Create(new BadInteropExtensionProvider())))
        {
            result = o;

            yield return o;
        }

        EnsureSuccess();

        yield return result.Dereference();
    }
}