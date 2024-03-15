using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

/// <summary>
///     Implements the Scope for the Script Engine
/// </summary>
public class BadScope : BadObject, IDisposable
{
    /// <summary>
    ///     The Finalizer List of the Scope
    /// </summary>
    private readonly List<Action> m_Finalizers = new List<Action>();

    /// <summary>
    ///     The Extension Provider
    /// </summary>
    private readonly BadInteropExtensionProvider m_Provider;

    /// <summary>
    ///     A List of Registered APIs
    /// </summary>
    private readonly List<string> m_RegisteredApis = new List<string>();

    /// <summary>
    ///     The Scope Variables
    /// </summary>
    private readonly BadTable m_ScopeVariables = new BadTable();

    /// <summary>
    ///     The Singleton Cache
    /// </summary>
    private readonly Dictionary<Type, object> m_SingletonCache = new Dictionary<Type, object>();

    /// <summary>
    ///     Indicates if the Scope uses the visibility subsystem
    /// </summary>
    private readonly bool m_UseVisibility;

    /// <summary>
    ///     The Caller of the Current Scope
    /// </summary>
    private BadScope? m_Caller;

    /// <summary>
    ///     Contains the exported variables of the scope
    /// </summary>
    private BadObject? m_Exports;

    private bool m_IsDisposed;

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

    /// <summary>
    ///     A List of Registered APIs
    /// </summary>
    public IReadOnlyCollection<string> RegisteredApis => Parent?.RegisteredApis ?? m_RegisteredApis;

    /// <summary>
    ///     The Extension Provider
    /// </summary>
    public BadInteropExtensionProvider Provider => Parent != null ? Parent.Provider : m_Provider;

    /// <summary>
    ///     The Class Object of the Scope
    /// </summary>
    public BadClass? ClassObject { get; internal set; }

    /// <summary>
    ///     The Function Object of the Scope
    /// </summary>
    public BadFunction? FunctionObject { get; internal set; }


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

    /// <summary>
    ///     Disposes the Scope and calls all finalizers
    /// </summary>
    public void Dispose()
    {
        if (m_IsDisposed)
        {
            return;
        }

        m_IsDisposed = true;
        foreach (Action finalizer in m_Finalizers)
        {
            finalizer();
        }

        m_Finalizers.Clear();
    }

    /// <summary>
    ///     Registers an API
    /// </summary>
    /// <param name="api"></param>
    internal void SetRegisteredApi(string api)
    {
        if (Parent != null)
        {
            Parent.SetRegisteredApi(api);

            return;
        }

        if (!m_RegisteredApis.Contains(api))
        {
            m_RegisteredApis.Add(api);
        }
    }

    /// <summary>
    ///     Adds a Finalizer to the Scope
    /// </summary>
    /// <param name="finalizer">The Finalizer</param>
    /// <exception cref="BadRuntimeException">Gets raised if the Scope is already disposed</exception>
    public void AddFinalizer(Action finalizer)
    {
        if (m_IsDisposed)
        {
            throw BadRuntimeException.Create(this, "Scope is already disposed");
        }

        m_Finalizers.Add(finalizer);
    }

    /// <summary>
    ///     Sets the Caller of the Scope
    /// </summary>
    /// <param name="caller">The Caller</param>
    public void SetCaller(BadScope? caller)
    {
        m_Caller = caller;
    }

    /// <summary>
    ///     Returns the Root Scope of the Scope
    /// </summary>
    /// <returns>The Root Scope</returns>
    public BadScope GetRootScope()
    {
        return Parent?.GetRootScope() ?? this;
    }

    /// <summary>
    ///     Adds a Singleton to the Scope
    /// </summary>
    /// <param name="instance"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="BadRuntimeException"></exception>
    public void AddSingleton<T>(T instance) where T : class
    {
        if (instance == null)
        {
            throw new BadRuntimeException("Cannot add null as singleton");
        }

        m_SingletonCache.Add(typeof(T), instance);
    }

    /// <summary>
    ///     Gets a Singleton from the Scope
    /// </summary>
    /// <typeparam name="T">Type of the Singleton</typeparam>
    /// <returns>The Singleton</returns>
    public T? GetSingleton<T>()
    {
        if (Parent != null)
        {
            return Parent.GetSingleton<T>();
        }

        if (m_SingletonCache.TryGetValue(typeof(T), out object? value))
        {
            return (T)value;
        }

        return default;
    }

    /// <summary>
    ///     Gets a Singleton from the Scope
    /// </summary>
    /// <param name="createNew">Should a new instance be created if the singleton does not exist</param>
    /// <typeparam name="T">Type of the Singleton</typeparam>
    /// <returns>The Singleton</returns>
    /// <exception cref="Exception">Gets raised if the singleton does not exist and createNew is false</exception>
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
    ///     Returns the exported key value pairs of the scope
    /// </summary>
    /// <returns>BadTable with all exported variables</returns>
    public BadObject GetExports()
    {
        return m_Exports ?? Null;
    }

    public void SetExports(BadExecutionContext ctx, BadObject exports)
    {
        if (m_Exports != null)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Exports are already set");
        }

        m_Exports = exports;
    }

    /// <summary>
    ///     Sets an exported key value pair in the scope
    /// </summary>
    /// <param name="key">The Key</param>
    /// <param name="value">The Value</param>
    public void AddExport(string key, BadObject value)
    {
        if (Parent != null)
        {
            Parent.AddExport(key, value);
        }
        else
        {
            if (m_Exports == null)
            {
                m_Exports = new BadTable();
            }

            m_Exports.SetProperty(key, value);
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
            FunctionObject = FunctionObject,
        };

        return sc;
    }

    public void DefineProperty(string name, BadClassPrototype type, BadExpression getAccessor, BadExpression? setAccessor, BadExecutionContext caller)
    {
        if (HasLocal(name, caller.Scope, false))
        {
            throw new BadRuntimeException($"Property {name} is already defined");
        }
        Action<BadObject, BadPropertyInfo?>? setter = null;
        if (setAccessor != null)
        {
            setter = (value, pi) =>
            {
                var setCtx = new BadExecutionContext(caller.Scope.CreateChild($"set {name}", caller.Scope, null));
                setCtx.Scope.DefineVariable("value", value, setCtx.Scope, new BadPropertyInfo(BadAnyPrototype.Instance, true));
                foreach (BadObject o in setCtx.Execute(setAccessor))
                {
                    //Execute
                }
            };
        }
        m_ScopeVariables.PropertyInfos.Add(name, new BadPropertyInfo(type, setter == null));
        m_ScopeVariables.InnerTable.Add(name, BadObjectReference.Make($"property {name}",
            () =>
            {
                var getCtx = new BadExecutionContext(caller.Scope.CreateChild($"get {name}", caller.Scope, null));
                var get = Null;
                foreach (BadObject o in getCtx.Execute(getAccessor))
                {
                    get = o;
                }
                return get.Dereference();
            },
            setter));
    }

    /// <summary>
    ///     Defines a new Variable in the current scope
    /// </summary>
    /// <param name="name">Variable Name</param>
    /// <param name="value">Variable Value</param>
    /// <param name="caller">The Caller of the Scope</param>
    /// <param name="info">Variable Info</param>
    /// <exception cref="BadRuntimeException">Gets raised if the specified variable is already defined.</exception>
    public void DefineVariable(string name, BadObject value, BadScope? caller = null, BadPropertyInfo? info = null)
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
    public BadPropertyInfo GetVariableInfo(string name)
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

    /// <summary>
    ///     Returns the visibility of the specified property
    /// </summary>
    /// <param name="propName">The Property Name</param>
    /// <returns>The Visibility of the Property</returns>
    public static BadPropertyVisibility GetPropertyVisibility(string propName)
    {
        char first = propName[0];
        char second = propName.Length > 1 ? propName[1] : '\0';

        return second switch
        {
            '_' => first == '_' ? BadPropertyVisibility.Private : BadPropertyVisibility.Public,
            _ => first == '_' ? BadPropertyVisibility.Protected : BadPropertyVisibility.Public,
        };
    }

    /// <summary>
    ///     Returns true if the specified scope is visible to the current scope
    /// </summary>
    /// <param name="scope">The Scope</param>
    /// <returns>true if the scope is visible</returns>
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

    /// <summary>
    ///     Returns the variable reference of the specified variable
    /// </summary>
    /// <param name="name">Variable Name</param>
    /// <param name="caller">The Calling Scope</param>
    /// <returns>Variable Reference</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the variable can not be found or is not visible</exception>
    public BadObjectReference GetVariable(string name, BadScope caller)
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
    public BadObjectReference GetVariable(string name)
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
    public void SetVariable(string name, BadObject value, BadScope? caller = null)
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
    public bool HasLocal(string name, BadScope caller, bool useExtensions = true)
    {
        return !useExtensions ? m_ScopeVariables.InnerTable.ContainsKey(name) : m_ScopeVariables.HasProperty(name, caller);
    }

    /// <summary>
    ///     returns true if the specified variable is defined in the current scope or any parent scope
    /// </summary>
    /// <param name="name">The Name</param>
    /// <param name="caller">The Caller of the Scope</param>
    /// <returns>true if the variable is defined</returns>
    public bool HasVariable(string name, BadScope caller)
    {
        return HasLocal(name, caller) || Parent != null && Parent.HasVariable(name, caller);
    }


    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        return HasVariable(propName, caller ?? this) ? GetVariable(propName, caller ?? this) : base.GetProperty(propName, caller);
    }

    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return HasVariable(propName, caller ?? this) || base.HasProperty(propName, caller);
    }


    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);

        return m_ScopeVariables.ToSafeString(done);
    }
}