using System.Reflection;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

/// <summary>
///     Implements a Reflected Method Member
/// </summary>
public class BadReflectedMethod : BadReflectedMember
{
    /// <summary>
    ///     The Reflected Methods
    /// </summary>
    private readonly List<MethodInfo> m_Methods = new List<MethodInfo>();

    /// <summary>
    ///     Creates a new BadReflectedMethod
    /// </summary>
    /// <param name="method">The Reflected Method</param>
    public BadReflectedMethod(MethodInfo method) : base(method.Name)
    {
        m_Methods.Add(method);
    }

    /// <inheritdoc />
    public override bool IsReadOnly => true;


    /// <summary>
    ///     Adds a Method to the Reflected Method
    /// </summary>
    /// <param name="method">The Method to add</param>
    public void AddMethod(MethodInfo method)
    {
        m_Methods.Add(method);
    }

    /// <summary>
    ///     Creates a new Function for the Reflected Method
    /// </summary>
    /// <param name="instance">The Instance to create the Function for</param>
    /// <returns>The Function</returns>
    private BadFunction CreateFunction(object? instance)
    {
        bool isStatic = instance == null;

        return new BadInteropFunction(
            Name,
            args => Invoke(instance, args),
            isStatic,
            BadAnyPrototype.Instance,
            new BadFunctionParameter("args", false, false, true)
        );
    }

    /// <summary>
    ///     Indicates if the given Object can be converted to the given Type
    /// </summary>
    /// <param name="o">The Object to convert</param>
    /// <param name="t">The Type to convert to</param>
    /// <returns>True if the Object can be converted</returns>
    private static bool CanConvert(BadObject o, Type t)
    {
        if (o.CanUnwrap())
        {
            object? obj = o.Unwrap();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (obj == null)
            {
                return !t.IsValueType;
            }

            return t.IsInstanceOfType(obj) || t.IsNumericType() && obj.GetType().IsNumericType();
        }

        if (o is not BadReflectedObject ro)
        {
            return false;
        }

        {
            object obj = ro.Instance;

            return t.IsInstanceOfType(obj);
        }
    }

    /// <summary>
    ///     Converts the given Object to the given Type
    /// </summary>
    /// <param name="o">The Object to convert</param>
    /// <param name="t">The Type to convert to</param>
    /// <returns>The Converted Object</returns>
    /// <exception cref="BadRuntimeException">If the Object cannot be converted</exception>
    private static object? ConvertObject(BadObject o, Type t)
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

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
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

    /// <summary>
    ///     Finds the Implementation of the Method that matches the given Arguments
    /// </summary>
    /// <param name="instance">The Instance to find the Implementation for</param>
    /// <param name="args">The Arguments to find the Implementation for</param>
    /// <param name="info">The Method that was found</param>
    /// <returns>The Converted Arguments</returns>
    /// <exception cref="BadRuntimeException">If no matching Method was found</exception>
    private object?[] FindImplementation(object? instance, IReadOnlyList<BadObject> args, out MethodInfo info)
    {
        foreach (MethodInfo method in m_Methods)
        {
            if (instance == null && !method.IsStatic || instance != null && method.IsStatic)
            {
                continue;
            }

            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length != args.Count)
            {
                continue;
            }

            object?[] converted = new object?[args.Count];
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

    /// <summary>
    ///     Invokes the Method with the given Arguments
    /// </summary>
    /// <param name="instance">The Instance to invoke the Method on</param>
    /// <param name="args">The Arguments to invoke the Method with</param>
    /// <returns>The Result of the Invocation</returns>
    private BadObject Invoke(object? instance, BadObject[] args)
    {
        object?[] implArgs = FindImplementation(instance, args, out MethodInfo info);

        return Wrap(info.Invoke(instance, implArgs));
    }

    /// <inheritdoc />
    public override BadObject Get(object? instance)
    {
        return CreateFunction(instance);
    }

    /// <inheritdoc />
    public override void Set(object? instance, BadObject o)
    {
        throw new BadRuntimeException("Can not set a value to a method");
    }
}