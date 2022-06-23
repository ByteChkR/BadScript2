using BadScript2.Common;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Functions;

public abstract class BadFunction : BadObject
{
    public readonly BadWordToken? Name;
    public readonly BadFunctionParameter[] Parameters;

    protected BadFunction(BadWordToken? name, params BadFunctionParameter[] parameters)
    {
        Name = name;
        Parameters = parameters;
    }

    public override BadClassPrototype GetPrototype()
    {
        return BadNativeClassBuilder.GetNative("Function");
    }

    public virtual BadFunction BindParentScope(BadScope scope)
    {
        return this;
    }

    protected void CheckParameters(BadObject[] args)
    {
        for (int i = 0; i < Parameters.Length; i++)
        {
            BadFunctionParameter parameter = Parameters[i];
            if (parameter.IsRestArgs) { }
            else if (args.Length <= i)
            {
                if (!parameter.IsOptional)
                {
                    throw new BadRuntimeException(
                        $"Wrong number of parameters for '{this}'. Expected Argument for '{parameter}'"
                    );
                }
            }
            else
            {
                if (parameter.IsNullChecked && args[i] == Null)
                {
                    throw new BadRuntimeException(
                        $"Null value not allowed for '{this}' parameter '{parameter}'"
                    );
                }
            }
        }
    }

    public void ApplyParameters(BadExecutionContext context, BadObject[] args, BadSourcePosition? position = null)
    {
        for (int i = 0; i < Parameters.Length; i++)
        {
            BadFunctionParameter parameter = Parameters[i];
            if (parameter.IsRestArgs)
            {
                context.Scope.DefineVariable(
                    parameter.Name,
                    new BadArray(args.Skip(i).ToList()),
                    new BadPropertyInfo(BadNativeClassBuilder.GetNative("Array"))
                );
            }
            else if (args.Length <= i)
            {
                if (parameter.IsOptional)
                {
                    context.Scope.DefineVariable(parameter.Name, Null, new BadPropertyInfo(parameter.Type));
                }
                else
                {
                    if (position != null)
                    {
                        throw new BadRuntimeException(
                            $"Wrong number of parameters for '{this}'. Expected Argument for '{parameter}'",
                            position
                        );
                    }

                    throw new BadRuntimeException(
                        $"Wrong number of parameters for '{this}'. Expected Argument for '{parameter}'"
                    );
                }
            }
            else
            {
                if (parameter.IsNullChecked && args[i] == Null)
                {
                    if (position != null)
                    {
                        throw new BadRuntimeException(
                            $"Null value not allowed for '{this}' parameter '{parameter}'",
                            position
                        );
                    }

                    throw new BadRuntimeException(
                        $"Null value not allowed for '{this}' parameter '{parameter}'"
                    );
                }

                context.Scope.DefineVariable(parameter.Name, args[i], new BadPropertyInfo(parameter.Type));
            }
        }
    }


    public IEnumerable<BadObject> Invoke(BadObject[] args, BadExecutionContext caller)
    {
        foreach (BadObject o in InvokeBlock(args, caller))
        {
            yield return o;
        }
    }

    protected abstract IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller);


    public string GetHeader()
    {
        return
            $"{BadStaticKeys.FunctionKey} {Name?.ToString() ?? "<anonymous>"}({string.Join(", ", Parameters.Cast<object>())})";
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return GetHeader();
    }
}