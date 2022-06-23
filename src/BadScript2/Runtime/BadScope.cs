using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

public class BadScope : BadObject
{
    public readonly BadScope? Caller;

    private readonly BadTable m_ScopeVariables = new BadTable();

    public readonly string Name;

    public readonly BadScope? Parent;

    public BadScope(string name, BadScope? caller = null, BadScopeFlags flags = BadScopeFlags.RootScope)
    {
        Name = name;
        Flags = flags;
        Caller = caller;
    }

    private BadScope(
        BadScope parent,
        BadScope? caller,
        string name,
        BadScopeFlags flags = BadScopeFlags.RootScope) : this(
        name,
        caller,
        ClearCaptures(parent.Flags) | flags
    )
    {
        Parent = parent;
    }

    public BadScopeFlags Flags { get; private set; }

    private bool CountInStackTrace => (Flags & BadScopeFlags.CaptureReturn) != 0;

    public bool IsBreak { get; private set; }
    public bool IsContinue { get; private set; }
    public bool IsError { get; private set; }

    public BadObject? ReturnValue { get; private set; }
    public BadRuntimeError? Error { get; private set; }

    public override BadClassPrototype GetPrototype()
    {
        return new BadNativeClassPrototype<BadScope>(
            "Scope",
            (ctx, args) => throw new BadRuntimeException("Cannot call constructor on a scope")
        );
    }

    public void SetFlags(BadScopeFlags flags)
    {
        Flags = flags;
    }

    public void UnsetError()
    {
        IsError = false;
        Error = null;
        if (Parent != null)
        {
            Parent.UnsetError();
        }
    }

    public string GetStackTrace()
    {
        return GetStackTrace(this);
    }

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

            current = current.Caller;
        }

        return string.Join("\n", stack.Select(s => s.Name));
    }

    private static BadScopeFlags ClearCaptures(BadScopeFlags flags)
    {
        return flags &
               ~(BadScopeFlags.CaptureReturn |
                 BadScopeFlags.CaptureBreak |
                 BadScopeFlags.CaptureContinue |
                 BadScopeFlags.CaptureThrow);
    }

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

    public void SetErrorObject(BadRuntimeError error)
    {
        Error = error;
        IsError = true;
        if ((Flags & BadScopeFlags.CaptureThrow) == 0)
        {
            Parent?.SetErrorObject(error);
        }
    }

    public void SetError(BadObject obj, BadRuntimeError? inner)
    {
        if ((Flags & BadScopeFlags.AllowThrow) == 0)
        {
            throw new BadRuntimeException("Throw not allowed in this scope");
        }

        SetErrorObject(new BadRuntimeError(inner, obj, GetStackTrace()));
    }

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

    public BadTable GetTable()
    {
        return m_ScopeVariables;
    }


    public BadScope CreateChild(string name, BadScope? caller, BadScopeFlags flags = BadScopeFlags.RootScope)
    {
        return new BadScope(this, caller, name, flags);
    }

    public void DefineVariable(BadObject name, BadObject value, BadPropertyInfo? info = null)
    {
        if (HasLocal(name))
        {
            throw new BadRuntimeException($"Variable {name} is already defined");
        }

        m_ScopeVariables.GetProperty(name).Set(value, info);
    }

    public BadObjectReference GetVariable(BadObject name)
    {
        if (HasLocal(name))
        {
            return m_ScopeVariables.GetProperty(name);
        }

        if (Parent == null)
        {
            throw new BadRuntimeException($"Variable '{name}' is not defined");
        }

        return Parent!.GetVariable(name);
    }

    public void SetVariable(BadObject name, BadObject value)
    {
        if (HasLocal(name))
        {
            m_ScopeVariables.GetProperty(name).Set(value);
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

    public bool HasLocal(BadObject name)
    {
        return m_ScopeVariables.HasProperty(name);
    }

    public bool HasVariable(BadObject name)
    {
        return HasLocal(name) || (Parent != null && Parent.HasVariable(name));
    }

    public override BadObjectReference GetProperty(BadObject propName)
    {
        if (HasVariable(propName))
        {
            return GetVariable(propName);
        }

        return base.GetProperty(propName);
    }

    public override bool HasProperty(BadObject propName)
    {
        return HasVariable(propName) || base.HasProperty(propName);
    }


    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);

        return m_ScopeVariables.ToSafeString(done);
    }
}