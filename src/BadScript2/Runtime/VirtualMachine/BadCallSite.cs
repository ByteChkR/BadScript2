using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Settings;
using System.Runtime.CompilerServices;

namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
/// Base interface for different call site strategies (Phase 5.2).
/// A call site represents a location where a function is invoked,
/// and can be specialized based on compile-time knowledge.
/// </summary>
public interface IBadCallSite
{
    /// <summary>
    /// Invokes the target with the given arguments.
    /// </summary>
    IEnumerable<BadObject> Invoke(BadObject target, BadObject[] args, BadSourcePosition position, BadExecutionContext context);

    /// <summary>
    /// Gets whether this call site can handle the given target.
    /// </summary>
    bool CanHandle(BadObject target);
}

/// <summary>
/// Dynamic call site that can call any callable object.
/// This is the fallback for when compile-time information is not available.
/// </summary>
public sealed class BadDynamicCallSite : IBadCallSite
{
    public IEnumerable<BadObject> Invoke(BadObject target, BadObject[] args, BadSourcePosition position, BadExecutionContext context)
    {
        return BadInvocationExpression.Invoke(target, args, position, context);
    }

    public bool CanHandle(BadObject target) => true; // Can handle anything
}

/// <summary>
/// Call site for compiled functions.
/// Provides a faster path by directly creating frames and executing instructions.
/// </summary>
public sealed class BadCompiledCallSite : IBadCallSite
{
    private readonly bool m_IsStaticInline;

    public BadCompiledCallSite(BadCompiledFunction function, bool isStaticInline = false)
    {
        m_IsStaticInline = isStaticInline;
    }

    public IEnumerable<BadObject> Invoke(BadObject target, BadObject[] args, BadSourcePosition position, BadExecutionContext context)
    {
        if (target is not BadCompiledFunction func)
        {
            throw new BadRuntimeException("Call site target is not a compiled function", position);
        }

        if (m_IsStaticInline && func.IsStatic)
        {
            return func.Invoke(args, context);
        }

        return func.Invoke(args, context);
    }

    public bool CanHandle(BadObject target) => target is BadCompiledFunction;
}

/// <summary>
/// Factory for creating appropriate call sites based on runtime information.
/// </summary>
public sealed class BadCallSiteFactory
{
    private static readonly BadDynamicCallSite s_DynamicCallSite = new();

    /// <summary>
    /// Creates a call site for the given target.
    /// </summary>
    public static IBadCallSite CreateCallSite(BadObject? target)
    {
        if (target is BadCompiledFunction compiledFunc)
        {
            // For compiled functions, we could use a faster path
            bool canUseInline = BadNativeOptimizationSettings.Instance.UseStaticMethodSpecialization &&
                                compiledFunc.IsStatic;
            return new BadCompiledCallSite(compiledFunc, canUseInline);
        }

        // For all other callables, use dynamic dispatch
        return s_DynamicCallSite;
    }

    /// <summary>
    /// Creates a cached call site that can be reused across multiple invocations.
    /// Useful for high-frequency call patterns.
    /// </summary>
    public static BadCachedCallSite CreateCachedCallSite(BadObject? firstTarget)
    {
        return new BadCachedCallSite(firstTarget);
    }
}

/// <summary>
/// Cached call site that remembers the last target type and can specialize for common cases.
/// This is useful for polymorphic call sites that are called frequently.
/// </summary>
public sealed class BadCachedCallSite
{
    private BadObject? m_LastTarget;
    private IBadCallSite? m_CachedSite;

    public BadCachedCallSite(BadObject? initialTarget = null)
    {
        if (initialTarget != null)
        {
            m_LastTarget = initialTarget;
            m_CachedSite = BadCallSiteFactory.CreateCallSite(initialTarget);
        }
    }

    /// <summary>
    /// Invokes the target, using the cached call site if the target matches.
    /// Falls back to creating a new call site if the target type changes.
    /// </summary>
    public IEnumerable<BadObject> Invoke(BadObject target, BadObject[] args, BadSourcePosition position, BadExecutionContext context)
    {
        // Fast path: same target type
        if (m_LastTarget != null && m_LastTarget.GetType() == target.GetType())
        {
            if (m_CachedSite?.CanHandle(target) ?? false)
            {
                return m_CachedSite.Invoke(target, args, position, context);
            }
        }

        // Slow path: target type changed, need new call site
        m_LastTarget = target;
        m_CachedSite = BadCallSiteFactory.CreateCallSite(target);
        return m_CachedSite.Invoke(target, args, position, context);
    }
}

/// <summary>
/// Call site for method invocations on objects.
/// Uses a monomorphic inline cache: caches the last resolved method for the last seen target.
/// Optimal for monomorphic call sites (same instance called repeatedly).
/// Falls back to GetProperty lookup on target change.
/// </summary>
public sealed class BadMethodCallSite : IBadCallSite
{
    private readonly string m_MethodName;

    // Monomorphic inline cache: last target → last resolved method
    private BadObject? m_CachedTarget;
    private BadObject? m_CachedMethod;

    public BadMethodCallSite(string methodName)
    {
        m_MethodName = methodName;
    }

    public IEnumerable<BadObject> Invoke(BadObject target, BadObject[] args, BadSourcePosition position, BadExecutionContext context)
    {
        BadObject method;

        if (ReferenceEquals(m_CachedTarget, target) && m_CachedMethod != null)
        {
            method = m_CachedMethod;
        }
        else
        {
            method = target.GetProperty(m_MethodName, context.Scope).Dereference(position);

            if (method == null)
            {
                throw new BadRuntimeException($"Method '{m_MethodName}' resolved to null", position);
            }

            m_CachedTarget = target;
            m_CachedMethod = method;
        }

        return BadInvocationExpression.Invoke(method, args, position, context);
    }

    public bool CanHandle(BadObject target) => true;
}









