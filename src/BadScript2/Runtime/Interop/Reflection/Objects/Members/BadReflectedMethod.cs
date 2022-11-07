using System.Reflection;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

public class BadReflectedMethod : BadReflectedMember
{
    public readonly List<MethodInfo> Methods = new List<MethodInfo>();

    public BadReflectedMethod(MethodInfo method) : base(method.Name)
    {
        Methods.Add(method);
    }


    public void AddMethod(MethodInfo method)
    {
        Methods.Add(method);
    }

    private BadFunction CreateFunction(object instance)
    {
        return new BadInteropFunction(Name, args => Invoke(instance, args), new BadFunctionParameter("args", false, false, true, null));
    }

    private bool CanConvert(BadObject o, Type t)
    {
        if (o.CanUnwrap())
        {
            object? obj = o.Unwrap();
            if (obj == null)
            {
                return !t.IsValueType;
            }

            return t.IsInstanceOfType(obj) || t.IsNumericType() && obj.GetType().IsNumericType();
        }

        if (o is BadReflectedObject ro)
        {
            object obj = ro.Instance;

            return t.IsInstanceOfType(obj);
        }

        return false;
    }

    private object? ConvertObject(BadObject o, Type t)
    {
        object? obj;
        if (o.CanUnwrap())
        {
            obj = o.Unwrap();
        }
        else if (o is BadReflectedObject ro)
        {
            obj = ro.Instance;
        }
        else
        {
            throw new BadRuntimeException("Cannot convert object");
        }

        if (obj == null)
        {
            return null;
        }

        if (t.IsInstanceOfType(obj))
        {
            return obj;
        }

        if (t.IsNumericType())
        {
            return Convert.ChangeType(obj, t);
        }

        throw new BadRuntimeException("Cannot convert object");
    }

    private object?[] FindImplementation(BadObject[] args, out MethodInfo info)
    {
        foreach (MethodInfo method in Methods)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != args.Length)
            {
                continue;
            }

            object?[] converted = new object?[args.Length];
            bool skipThis = false;
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                BadObject argument = args[i];

                if (!CanConvert(argument, parameter.ParameterType))
                {
                    skipThis = true;

                    break;
                }

                converted[i] = ConvertObject(argument, parameter.ParameterType);
            }

            if (skipThis)
            {
                continue;
            }

            info = method;

            return converted;
        }

        throw new BadRuntimeException("No matching method found");
    }

    private BadObject Invoke(object instance, BadObject[] args)
    {
        object?[] implArgs = FindImplementation(args, out MethodInfo info);

        return Wrap(info.Invoke(instance, implArgs));
    }

    public override BadObject Get(object instance)
    {
        return CreateFunction(instance);
    }
}