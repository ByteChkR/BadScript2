using System.Collections;
using System.Reflection;

using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

/// <summary>
/// Contains the Member Classes for Reflection Objects
/// </summary>
namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

/// <summary>
///     Implements a Reflected 'GetEnumerator' Method
/// </summary>
public class BadReflectedEnumeratorMethod : BadReflectedMethod
{
    /// <summary>
    ///     Creates a new BadReflectedEnumeratorMethod
    /// </summary>
    /// <param name="method">The Reflected Method</param>
    public BadReflectedEnumeratorMethod(MethodInfo method) : base(method) { }

    /// <summary>
    ///     Enumerates an object
    /// </summary>
    /// <param name="instance">The Object to enumerate</param>
    /// <returns>The Enumerator</returns>
    private IEnumerable<BadObject> GetEnumerable(object instance)
    {
        IEnumerable o = (IEnumerable)instance;

        foreach (object obj in o)
        {
            yield return Wrap(obj);
        }
    }

    /// <inheritdoc />
    public override BadObject Get(object? instance)
    {
        return new BadInteropFunction(
            "GetEnumerator",
            _ => new BadInteropEnumerator(GetEnumerable(instance!).GetEnumerator()),
            false,
            BadAnyPrototype.Instance
        );
    }
}