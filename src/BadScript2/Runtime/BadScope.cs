using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime
{
    /// <summary>
    ///     Implements the Scope for the Script Engine
    /// </summary>
    public class BadScope : BadObject
    {
        /// <summary>
        ///     The Caller of the Current Scope
        /// </summary>
        private readonly BadScope? Caller;

        /// <summary>
        ///     The Scope Variables
        /// </summary>
        private readonly BadTable m_ScopeVariables = new BadTable();

        /// <summary>
        ///     The Name of the Scope (for Debugging)
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///     The Parent Scope
        /// </summary>
        public readonly BadScope? Parent;

        /// <summary>
        ///     Creates a new Scope
        /// </summary>
        /// <param name="name">The Name of the Scope</param>
        /// <param name="caller">The Caller of the Scope</param>
        /// <param name="flags">The Flags of the Scope</param>
        public BadScope(string name, BadScope? caller = null, BadScopeFlags flags = BadScopeFlags.RootScope)
        {
            Name = name;
            Flags = flags;
            Caller = caller;
        }

        /// <summary>
        ///     Creates a new Scope
        /// </summary>
        /// <param name="parent">The Parent Scope</param>
        /// <param name="caller">The Caller of the Scope</param>
        /// <param name="name">The Name of the Scope</param>
        /// <param name="flags">The Flags of the Scope</param>
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
            (_, args) =>
            {
                if (args.Length != 1 || args[0] is not IBadString name)
                {
                    throw new BadRuntimeException("Expected Name in Scope Constructor");
                }

                return CreateScope(name.Value);
            }
        );

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
        /// <param name="name">Scope Name</param>
        /// <returns>New Scope Instance</returns>
        private static BadScope CreateScope(string name)
        {
            return new BadScope(name);
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
            if (Parent != null)
            {
                Parent.UnsetError();
            }
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

                current = current.Caller;
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
        /// <param name="flags">Scope Flags</param>
        /// <returns>New BadScope Instance</returns>
        public BadScope CreateChild(string name, BadScope? caller, BadScopeFlags flags = BadScopeFlags.RootScope)
        {
            return new BadScope(this, caller, name, flags);
        }

        /// <summary>
        ///     Defines a new Variable in the current scope
        /// </summary>
        /// <param name="name">Variable Name</param>
        /// <param name="value">Variable Value</param>
        /// <param name="info">Variable Info</param>
        /// <exception cref="BadRuntimeException">Gets raised if the specified variable is already defined.</exception>
        public void DefineVariable(BadObject name, BadObject value, BadPropertyInfo? info = null)
        {
            if (HasLocal(name))
            {
                throw new BadRuntimeException($"Variable {name} is already defined");
            }

            m_ScopeVariables.GetProperty(name).Set(value, info);
        }

        /// <summary>
        ///     Returns the variable info of the specified variable
        /// </summary>
        /// <param name="name">Variable Name</param>
        /// <returns>Variable Info</returns>
        /// <exception cref="BadRuntimeException">Gets raised if the variable can not be found</exception>
        public BadPropertyInfo GetVariableInfo(BadObject name)
        {
            if (HasLocal(name))
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
        ///     Returns a variable reference of the specified variable
        /// </summary>
        /// <param name="name">Variable Name</param>
        /// <returns>Variable Reference</returns>
        /// <exception cref="BadRuntimeException">Gets raised if the variable can not be found</exception>
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

        /// <summary>
        ///     Sets a variable with the specified name to the specified value
        /// </summary>
        /// <param name="name">The Name</param>
        /// <param name="value">The Value</param>
        /// <exception cref="BadRuntimeException">Gets raised if the variable can not be found</exception>
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

        /// <summary>
        ///     returns true if the specified variable is defined in the current scope
        /// </summary>
        /// <param name="name">The Name</param>
        /// <returns>true if the variable is defined</returns>
        public bool HasLocal(BadObject name)
        {
            return m_ScopeVariables.HasProperty(name);
        }

        /// <summary>
        ///     returns true if the specified variable is defined in the current scope or any parent scope
        /// </summary>
        /// <param name="name">The Name</param>
        /// <returns>true if the variable is defined</returns>
        public bool HasVariable(BadObject name)
        {
            return HasLocal(name) || Parent != null && Parent.HasVariable(name);
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
}