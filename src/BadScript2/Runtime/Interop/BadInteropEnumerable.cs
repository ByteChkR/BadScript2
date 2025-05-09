using System.Collections;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Runtime.Interop;

/// <summary>
/// Represents a BadScript Interop Grouping
/// </summary>
public class BadInteropGroup : BadInteropEnumerable
{

    /// <summary>
    /// The Underlying Grouping
    /// </summary>
    private readonly IGrouping<BadObject, BadObject> m_Group;
    
    /// <summary>
    /// Constructs a new Interop Grouping
    /// </summary>
    /// <param name="enumerable">The Grouping to wrap</param>
    public BadInteropGroup(IGrouping<BadObject,BadObject> enumerable) : base(enumerable)
    {
        m_Group = enumerable;
    }

    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return propName == "Key" || base.HasProperty(propName, caller);
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        if(propName == "Key")
        {
            return BadObjectReference.Make("Key", (p) => m_Group.Key);
        }
        return base.GetProperty(propName, caller);
    }
    
}

/// <summary>
///     Implements an Interop Enumerable Object
/// </summary>
public class BadInteropEnumerable : BadObject, IBadEnumerable
{
    public IEnumerable<BadObject> InnerEnumerable => m_Enumerable;
    /// <summary>
    ///     The Prototype for the Interop Enumerable Object
    /// </summary>
    private static readonly BadClassPrototype s_Prototype =
        new BadNativeClassPrototype<BadInteropEnumerator>("Enumerable",
                                                          (_, _) => throw new BadRuntimeException("Cannot call method"),
                                                          () => new[]
                                                          {
                                                              (BadInterfacePrototype)BadNativeClassBuilder
                                                                  .Enumerable.CreateGeneric(new[]
                                                                          {
                                                                              BadAnyPrototype.Instance,
                                                                          }
                                                                      ),
                                                          },
                                                          null
                                                         );

    /// <summary>
    ///     The Enumerable Object
    /// </summary>
    private readonly IEnumerable<BadObject> m_Enumerable;

    /// <summary>
    ///     The Function that returns the Enumerator
    /// </summary>
    private readonly BadFunction m_Func;

    /// <summary>
    ///     Creates a new BadInteropEnumerable
    /// </summary>
    /// <param name="enumerable"></param>
    public BadInteropEnumerable(IEnumerable<BadObject> enumerable)
    {
        m_Enumerable = enumerable;

        m_Func = new BadDynamicInteropFunction("GetEnumerator",
                                               _ => new BadInteropEnumerator(m_Enumerable.GetEnumerator()),
                                               (BadClassPrototype)BadNativeClassBuilder.Enumerator.CreateGeneric(new[]
                                                       {
                                                           BadAnyPrototype.Instance,
                                                       }
                                                   )
                                              );
    }

#region IBadEnumerable Members

    /// <summary>
    ///     Returns the Enumerator
    /// </summary>
    /// <returns>The Enumerator</returns>
    public IEnumerator<BadObject> GetEnumerator()
    {
        return m_Enumerable.GetEnumerator();
    }

    /// <summary>
    ///     Returns the Enumerator
    /// </summary>
    /// <returns>The Enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return m_Enumerable.GetEnumerator();
    }

#endregion


    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return propName == "GetEnumerator" ||
               base.HasProperty(propName, caller);
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        return propName == "GetEnumerator"
                   ? BadObjectReference.Make("GetEnumerator", (p) => m_Func)
                   : base.GetProperty(propName, caller);
    }

    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        return "BadInteropEnumerable";
    }
}