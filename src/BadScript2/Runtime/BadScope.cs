using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

/// <summary>
///     Implements the Scope for the Script Engine
/// </summary>
public class BadScope : BadObject
{
    private readonly BadInteropExtensionProvider m_Provider;

    /// <summary>
    ///     The Scope Variables
    /// </summary>
    private readonly BadTable m_ScopeVariables = new BadTable();

    private readonly Dictionary<Type, object> m_SingletonCache = new Dictionary<Type, object>();
    private readonly bool m_UseVisibility;

    /// <summary>
    ///     The Caller of the Current Scope
    /// </summary>
    private BadScope? m_Caller;

    /// <summary>
    ///     Creates a new Scope
    /// </summary>
    /// <param name="name">The Name of the Scope</param>
    /// <param name="provider">The Extension Provider of the Runtime</param>
    /// <param name="caller">The Caller of the Scope</param>
    /// <param name="flags">The Flags of the Scope</param>
    public BadScope(
        string name,
        BadInteropExtensionProvider provider,
        BadScope? caller = null,
        BadScopeFlags flags = BadScopeFlags.RootScope)
    {
        Name = name;
        Flags = flags;
        m_Caller = caller;
        m_Provider = provider;
    }

    /// <summary>
    ///     Creates a new Scope
    /// </summary>
    /// <param name="name">The Name of the Scope</param>
    /// <param name="provider">The Extension Provider of the Runtime</param>
    /// <param name="locals">The Local Variables</param>
    /// <param name="caller">The Caller of the Scope</param>
    /// <param name="flags">The Flags of the Scope</param>
    public BadScope(
        string name,
        BadInteropExtensionProvider provider,
        BadTable locals,
        BadScope? caller = null,
        BadScopeFlags flags = BadScopeFlags.RootScope)
    {
        Name = name;
        Flags = flags;
        m_Caller = caller;
        m_ScopeVariables = locals;
        m_Provider = provider;
    }

    /// <summary>
    ///     Creates a new Scope
    /// </summary>
    /// <param name="parent">The Parent Scope</param>
    /// <param name="caller">The Caller of the Scope</param>
    /// <param name="name">The Name of the Scope</param>
    /// <param name="flags">The Flags of the Scope</param>
    /// <param name="useVisibility">Does the Scope use the Visibility Subsystem</param>
    private BadScope(
        BadScope parent,
        BadScope? caller,
        string name,
        BadScopeFlags flags = BadScopeFlags.RootScope,
        bool useVisibility = false) : this(
        name,
        parent.Provider,
        caller,
        ClearCaptures(parent.Flags) | flags
    )
    {
        m_UseVisibility = useVisibility;
        Parent = parent;
    }

    public BadInteropExtensionProvider Provider => Parent != null ? Parent.Provider : m_Provider;

    public BadClass? ClassObject { get; internal set; }


    /// <summary>
    ///     The Parent Scope
    /// </summary>
    public BadScope? Parent { get; }

    /// <summary>
    ///     The Name of the Scope (for Debugging)
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     The Scope Flags
    /// </summary>
    public BadScopeFlags Flags { get; private set; }

    /// <summary>
    ///     Indicates if the Scope should count towards the Stack Trace
    /// </summary>
    private bool CountInStackTrace => (Flags & BadScopeFlags.CaptureReturn) != 0;

    /// <summary>
    ///     Is true if the Break Keyword was set
    /// </summary>
    public bool IsBreak { get; private set; }

    /// <summary>
    ///     Is true if the Continue Keyword was set
    /// </summary>
    public bool IsContinue { get; private set; }

    /// <summary>
    ///     Is true if the Scope encountered an error
    /// </summary>
    public bool IsError { get; private set; }

    /// <summary>
    ///     The Return value of the scope
    /// </summary>
    public BadObject? ReturnValue { get; private set; }

    /// <summary>
    ///     The Runtime Error that occured in the Scope
    /// </summary>
    public BadRuntimeError? Error { get; private set; }

    /// <summary>
    ///     A Class Prototype for the Scope
    /// </summary>
    public static BadClassPrototype Prototype { get; } = new BadNativeClassPrototype<BadScope>(
        "Scope",
        (c, args) =>
        {
            switch (args.Length)
            {
                case 1:
                {
                    if (args[0] is not IBadString name)
                    {
                        throw new BadRuntimeException("Expected Name in Scope Constructor");
                    }

                    return CreateScope(c, name.Value);
                }
                case 2:
                {
                    if (args[0] is not IBadString name)
                    {
                        throw new BadRuntimeException("Expected Name in Scope Constructor");
                    }

                    if (args[1] is not BadTable locals)
                    {
                        throw new BadRuntimeException("Expected Locals Table in Scope Constructor");
                    }

                    return CreateScope(c, name.Value, locals);
                }
                default:
                    throw new BadRuntimeException("Expected 1 or 2 Arguments in Scope Constructor");
            }
        }
    );

    public void SetCaller(BadScope? caller)
    {
        m_Caller = caller;
    }

    public BadScope GetRootScope()
    {
        return Parent?.GetRootScope() ?? this;
    }

    public void AddSingleton<T>(T instance) where T : class
    {
        if (instance == null)
        {
            throw new BadRuntimeException("Cannot add null as singleton");
        }

        m_SingletonCache.Add(typeof(T), instance);
    }

    public T GetSingleton<T>()
    {
        if (Parent != null)
        {
            return Parent.GetSingleton<T>();
        }

        return (T)m_SingletonCache[typeof(T)];
    }

    public T GetSingleton<T>(bool createNew) where T : new()
    {
        if (Parent != null)
        {
            return Parent.GetSingleton<T>(createNew);
        }

        if (m_SingletonCache.ContainsKey(typeof(T)))
        {
            return (T)m_SingletonCache[typeof(T)];
        }

        if (!createNew)
        {
            throw new Exception("Singleton not found");
        }

        T v = new T();
        m_SingletonCache[typeof(T)] = v;

        return v;

    }

    /// <summary>
    ///     Returns the Class Prototype for the Scope
    /// </summary>
    /// <returns>BadClassPrototype</returns>
    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    /// <summary>
    ///     Creates a Root Scope with the given name
    /// </summary>
    /// <param name="ctx">The Calling Context</param>
    /// <param name="name">Scope Name</param>
    /// <param name="locals">The Local Variables of the new Scope</param>
    /// <returns>New Scope Instance</returns>
    private static BadScope CreateScope(BadExecutionContext ctx, string name, BadTable? locals = null)
    {
        BadScope s = locals != null ? new BadScope(name, ctx.Scope.Provider, locals) : new BadScope(name, ctx.Scope.Provider);

        foreach (KeyValuePair<Type, object> kvp in ctx.Scope.GetRootScope().m_SingletonCache)
        {
            s.m_SingletonCache.Add(kvp.Key, kvp.Value);
        }

        return s;
    }

    /// <summary>
    ///     Sets the Scope Flags
    /// </summary>
    /// <param name="flags">Scope Flags</param>
    public void SetFlags(BadScopeFlags flags)
    {
        Flags = flags;
    }

    /// <summary>
    ///     Unsets the Error if it was set
    /// </summary>
    public void UnsetError()
    {
        IsError = false;
        Error = null;

        Parent?.UnsetError();
    }

    /// <summary>
    ///     Returns the Stack Trace of the Current scope
    /// </summary>
    /// <returns>Stack Trace</returns>
    public string GetStackTrace()
    {
        return GetStackTrace(this);
    }

    /// <summary>
    ///     Returns the Stack Trace of the given Scope
    /// </summary>
    /// <param name="scope">The Scope</param>
    /// <returns>Stack Trace</returns>
    private static string GetStackTrace(BadScope scope)
    {
        BadScope? current = scope;
        List<BadScope> stack = new List<BadScope>();

        while (current != null)
        {
            if (current.CountInStackTrace)
            {
                stack.Add(current);
            }

            current = current.m_Caller ?? current.Parent;
        }

        return string.Join("\n", stack.Select(s => s.Name));
    }

    /// <summary>
    ///     Clears all Capture Flags from the given Flags
    /// </summary>
    /// <param name="flags">The Flags to be cleared</param>
    /// <returns>Cleared Flags</returns>
    private static BadScopeFlags ClearCaptures(BadScopeFlags flags)
    {
        return flags &
               ~(BadScopeFlags.CaptureReturn |
                 BadScopeFlags.CaptureBreak |
                 BadScopeFlags.CaptureContinue |
                 BadScopeFlags.CaptureThrow);
    }

    /// <summary>
    ///     Sets the break keyword inside this scope
    /// </summary>
    /// <exception cref="BadRuntimeException">Gets raised if the current scope does not allow the Break Keyword</exception>
    public void SetBreak()
    {
        if ((Flags & BadScopeFlags.AllowBreak) == 0)
        {
            throw new BadRuntimeException("Break not allowed in this scope");
        }

        IsBreak = true;

        if ((Flags & BadScopeFlags.CaptureBreak) == 0)
        {
            Parent?.SetBreak();
        }
    }

    /// <summary>
    ///     Sets the continue keyword inside this scope
    /// </summary>
    /// <exception cref="BadRuntimeException">Gets raised if the current scope does not allow the continue Keyword</exception>
    public void SetContinue()
    {
        if ((Flags & BadScopeFlags.AllowContinue) == 0)
        {
            throw new BadRuntimeException("Continue not allowed in this scope");
        }

        IsContinue = true;

        if ((Flags & BadScopeFlags.CaptureContinue) == 0)
        {
            Parent?.SetContinue();
        }
    }

    /// <summary>
    ///     Sets an error object inside this scope
    /// </summary>
    /// <param name="error">The Error</param>
    public void SetErrorObject(BadRuntimeError error)
    {
        Error = error;
        IsError = true;

        if ((Flags & BadScopeFlags.CaptureThrow) == 0)
        {
            Parent?.SetErrorObject(error);
        }
    }

    /// <summary>
    ///     Sets an error object inside this scope
    /// </summary>
    /// <param name="obj">The Error</param>
    /// <param name="inner">The Inner Error</param>
    /// <exception cref="BadRuntimeException">Gets Raised if an error can not be set in this scope</exception>
    public void SetError(BadObject obj, BadRuntimeError? inner)
    {
        if ((Flags & BadScopeFlags.AllowThrow) == 0)
        {
            throw new BadRuntimeException("Throw not allowed in this scope");
        }

        SetErrorObject(new BadRuntimeError(inner, obj, GetStackTrace()));
    }

    /// <summary>
    ///     Sets the Return value of this scope
    /// </summary>
    /// <param name="value">The Return Value</param>
    /// <exception cref="BadRuntimeException">Gets Raised if the Scope does not allow returning</exception>
    public void SetReturnValue(BadObject? value)
    {
        if ((Flags & BadScopeFlags.AllowReturn) == 0)
        {
            throw new BadRuntimeException("Return not allowed in this scope");
        }

        ReturnValue = value;

        if ((Flags & BadScopeFlags.CaptureReturn) == 0)
        {
            Parent?.SetReturnValue(value);
        }
    }

    /// <summary>
    ///     Returns the Variable Table of the current scope
    /// </summary>
    /// <returns>BadTable with all local variables</returns>
    public BadTable GetTable()
    {
        return m_ScopeVariables;
    }


    /// <summary>
    ///     Creates a subscope of the current scope
    /// </summary>
    /// <param name="name">Scope Name</param>
    /// <param name="caller">The Caller</param>
    /// <param name="useVisibility">Specifies if the scope is part of a class structure(if visibility flags are used)</param>
    /// <param name="flags">Scope Flags</param>
    /// <returns>New BadScope Instance</returns>
    public BadScope CreateChild(
        string name,
        BadScope? caller,
        bool? useVisibility,
        BadScopeFlags flags = BadScopeFlags.RootScope)
    {
        BadScope sc = new BadScope(this, caller, name, flags, useVisibility ?? m_UseVisibility)
        {
            ClassObject = ClassObject,
        };

        return sc;
    }


    /// <summary>
    ///     Defines a new Variable in the current scope
    /// </summary>
    /// <param name="name">Variable Name</param>
    /// <param name="value">Variable Value</param>
    /// <param name="caller">The Caller of the Scope</param>
    /// <param name="info">Variable Info</param>
    /// <exception cref="BadRuntimeException">Gets raised if the specified variable is already defined.</exception>
    public void DefineVariable(BadObject name, BadObject value, BadScope? caller = null, BadPropertyInfo? info = null)
    {
        if (HasLocal(name, caller ?? this, false))
        {
            throw new BadRuntimeException($"Variable {name} is already defined");
        }

        m_ScopeVariables.GetProperty(name, false, caller ?? this).Set(value, info);
    }

    /// <summary>
    ///     Returns the variable info of the specified variable
    /// </summary>
    /// <param name="name">Variable Name</param>
    /// <returns>Variable Info</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the variable can not be found</exception>
    public BadPropertyInfo GetVariableInfo(BadObject name)
    {
        if (HasLocal(name, this))
        {
            return m_ScopeVariables.GetPropertyInfo(name);
        }

        if (Parent == null)
        {
            throw new BadRuntimeException($"Variable '{name}' is not defined");
        }

        return Parent!.GetVariableInfo(name);
    }

    public static BadPropertyVisibility GetPropertyVisibility(BadObject propName)
    {
        return propName is not IBadString s || !s.Value.StartsWith("_") ? BadPropertyVisibility.Public :
            s.Value.StartsWith("__") ? BadPropertyVisibility.Private : BadPropertyVisibility.Protected;
    }

    public bool IsVisibleParentOf(BadScope scope)
    {
        if (scope == this)
        {
            return true;
        }

        BadScope? current = scope.Parent;

        while (current != null)
        {
            if (!current.m_UseVisibility)
            {
                return false;
            }

            if (current == this)
            {
                return true;
            }


            current = current.Parent;
        }

        return false;
    }

    public BadObjectReference GetVariable(BadObject name, BadScope caller)
    {
        if (m_UseVisibility)
        {
            BadPropertyVisibility vis = IsVisibleParentOf(caller) ? BadPropertyVisibility.All : BadPropertyVisibility.Public;

            if ((GetPropertyVisibility(name) & vis) == 0)
            {
                throw BadRuntimeException.Create(caller, $"Variable '{name}' is not visible to {caller}");
            }
        }

        if (HasLocal(name, caller))
        {
            return m_ScopeVariables.GetProperty(name, caller);
        }

        if (Parent == null)
        {
            throw BadRuntimeException.Create(caller, $"Variable '{name}' is not defined");
        }

        return Parent!.GetVariable(name, caller);
    }

    /// <summary>
    ///     Returns a variable reference of the specified variable
    /// </summary>
    /// <param name="name">Variable Name</param>
    /// <returns>Variable Reference</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the variable can not be found</exception>
    public BadObjectReference GetVariable(BadObject name)
    {
        return GetVariable(name, this);
    }

    /// <summary>
    ///     Sets a variable with the specified name to the specified value
    /// </summary>
    /// <param name="name">The Name</param>
    /// <param name="value">The Value</param>
    /// <param name="caller">The Calling Scope</param>
    /// <exception cref="BadRuntimeException">Gets raised if the variable can not be found</exception>
    public void SetVariable(BadObject name, BadObject value, BadScope? caller = null)
    {
        if (HasLocal(name, caller ?? this))
        {
            m_ScopeVariables.GetProperty(name, caller).Set(value);
        }
        else
        {
            if (Parent == null)
            {
                throw new BadRuntimeException($"Variable '{name}' is not defined");
            }

            Parent!.SetVariable(name, value);
        }
    }

    /// <summary>
    ///     returns true if the specified variable is defined in the current scope
    /// </summary>
    /// <param name="name">The Name</param>
    /// <param name="caller">The Caller of the Scope</param>
    /// <param name="useExtensions">Should the Extension Subsystem be searched for the property</param>
    /// <returns>true if the variable is defined</returns>
    public bool HasLocal(BadObject name, BadScope caller, bool useExtensions = true)
    {
        return !useExtensions ? m_ScopeVariables.InnerTable.ContainsKey(name) : m_ScopeVariables.HasProperty(name, caller);
    }

    /// <summary>
    ///     returns true if the specified variable is defined in the current scope or any parent scope
    /// </summary>
    /// <param name="name">The Name</param>
    /// <param name="caller">The Caller of the Scope</param>
    /// <returns>true if the variable is defined</returns>
    public bool HasVariable(BadObject name, BadScope caller)
    {
        return HasLocal(name, caller) || Parent != null && Parent.HasVariable(name, caller);
    }


    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        return HasVariable(propName, caller ?? this) ? GetVariable(propName, caller ?? this) : base.GetProperty(propName, caller);
    }

    public override bool HasProperty(BadObject propName, BadScope? caller = null)
    {
        return HasVariable(propName, caller ?? this) || base.HasProperty(propName, caller);
    }


    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);

        return m_ScopeVariables.ToSafeString(done);
    }
}