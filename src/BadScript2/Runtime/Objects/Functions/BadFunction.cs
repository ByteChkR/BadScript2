using BadScript2.Common;
using BadScript2.Common.Logging;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;

namespace BadScript2.Runtime.Objects.Functions;

public abstract class BadFunction : BadObject
{
    public readonly bool IsConstant;
    private readonly Dictionary<int, BadObject> m_Cache = new Dictionary<int, BadObject>();
    public readonly BadWordToken? Name;
    public readonly BadFunctionParameter[] Parameters;

    protected BadFunction(BadWordToken? name, bool isConstant, params BadFunctionParameter[] parameters)
    {
        Name = name;
        IsConstant = isConstant;
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
            if (parameter.IsRestArgs)
            {
                //Do Nothing
            }
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

    private int? GetHash(BadObject[] args)
    {
        int hash = 0;

        //Generate Hash from arguments
        foreach (BadObject o in args)
        {
            if (o is not IBadNative native)
            {
                return null;
            }

            if (hash == 0)
            {
                hash = native.Value.GetHashCode();
            }
            else
            {
                hash = HashCode.Combine(hash, native.Value);
            }
        }

        return hash;
    }

    public IEnumerable<BadObject> Invoke(BadObject[] args, BadExecutionContext caller)
    {
        if (IsConstant && BadNativeOptimizationSettings.Instance.UseConstantFunctionCaching)
        {
            int? hash = GetHash(args);
            if (hash != null && m_Cache.ContainsKey(hash.Value))
            {
                BadLogger.Warn($"Found Cached value with Hash {hash}", "Runtime");

                yield return m_Cache[hash.Value];

                yield break;
            }
        }

        BadObject? ret = null;
        foreach (BadObject o in InvokeBlock(args, caller))
        {
            ret = o;

            yield return o;
        }

        if (IsConstant && ret != null && BadNativeOptimizationSettings.Instance.UseConstantFunctionCaching)
        {
            int? hash = GetHash(args);
            if (hash != null)
            {
                BadLogger.Warn($"Caching Result {ret.ToSafeString()} for function '{GetHeader()}'", "Runtime");
                m_Cache[hash.Value] = ret;
            }
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