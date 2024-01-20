using System.Collections;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop;

/// <summary>
///     Implements an Interop Enumerable Object
/// </summary>
public class BadInteropEnumerable : BadObject, IBadEnumerable
{
    /// <summary>
    /// The Prototype for the Interop Enumerable Object
    /// </summary>
    private static readonly BadClassPrototype s_Prototype = new BadNativeClassPrototype<BadInteropEnumerator>(
        "Enumerable",
        (_, _) => throw new BadRuntimeException("Cannot call method"),
        BadNativeClassBuilder.Enumerable
    );

    /// <summary>
    /// The Enumerable Object
    /// </summary>
    private readonly IEnumerable<BadObject> m_Enumerable;
    /// <summary>
    /// The Function that returns the Enumerator
    /// </summary>
    private readonly BadFunction m_Func;

    /// <summary>
    /// Creates a new BadInteropEnumerable
    /// </summary>
    /// <param name="enumerable"></param>
    public BadInteropEnumerable(IEnumerable<BadObject> enumerable)
    {
        m_Enumerable = enumerable;

        m_Func = new BadDynamicInteropFunction(
            "GetEnumerator",
            _ => new BadInteropEnumerator(m_Enumerable.GetEnumerator()),
            BadAnyPrototype.Instance
        );
    }

    /// <summary>
    /// Returns the Enumerator
    /// </summary>
    /// <returns>The Enumerator</returns>
    public IEnumerator<BadObject> GetEnumerator()
    {
        return m_Enumerable.GetEnumerator();
    }

    /// <summary>
    /// Returns the Enumerator
    /// </summary>
    /// <returns>The Enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return m_Enumerable.GetEnumerator();
    }

    
    /// <inheritdoc/>
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <inheritdoc/>
    public override bool HasProperty(BadObject propName, BadScope? caller = null)
    {
        return propName is IBadString
               {
                   Value: "GetEnumerator",
               } ||
               base.HasProperty(propName, caller);
    }

    /// <inheritdoc/>
    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        return propName is IBadString
        {
            Value: "GetEnumerator",
        }
            ? BadObjectReference.Make("GetEnumerator", () => m_Func)
            : base.GetProperty(propName, caller);
    }

    /// <inheritdoc/>
    public override string ToSafeString(List<BadObject> done)
    {
        return "BadInteropEnumerable";
    }
}