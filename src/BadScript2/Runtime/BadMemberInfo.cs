using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

/// <summary>
/// MemberInfo class that represents a member in the BadScript2 runtime.
/// </summary>
public class BadMemberInfo : BadObject
{
    /// <summary>
    /// The prototype for the MemberInfo class.
    /// </summary>
    private static readonly BadNativeClassPrototype<BadMemberInfo> s_Prototype =
        new BadNativeClassPrototype<BadMemberInfo>("MemberInfo",
                                                   (c, a) => throw BadRuntimeException.Create(c.Scope,
                                                                  "MemberInfo is a read only object"
                                                                 ),
                                                                 null
                                                  );

    /// <summary>
    /// Reference to the GetAttributes method.
    /// </summary>
    private readonly Lazy<BadObjectReference> m_GetAttributesReference;
    /// <summary>
    /// Reference to the GetValue method.
    /// </summary>
    private readonly Lazy<BadObjectReference> m_GetValueReference;
    /// <summary>
    /// Reference to the IsReadonly property.
    /// </summary>
    private readonly Lazy<BadObjectReference> m_IsReadonlyReference;
    /// <summary>
    /// Reference to the MemberType property.
    /// </summary>
    private readonly Lazy<BadObjectReference> m_MemberTypeReference;
    /// <summary>
    /// Name of the member.
    /// </summary>
    private readonly string m_Name;

    /// <summary>
    /// Reference to the Name property.
    /// </summary>
    private readonly Lazy<BadObjectReference> m_NameReference;
    /// <summary>
    /// The containing scope of the member.
    /// </summary>
    private readonly BadScope m_Scope;
    /// <summary>
    /// Reference to the SetValue method.
    /// </summary>
    private readonly Lazy<BadObjectReference> m_SetValueReference;

    /// <summary>
    /// Constructs a new instance of the BadMemberInfo class.
    /// </summary>
    /// <param name="name">The name of the member.</param>
    /// <param name="scope">The scope containing the member.</param>
    public BadMemberInfo(string name, BadScope scope)
    {
        m_Name = name;
        m_Scope = scope;
        m_NameReference = new Lazy<BadObjectReference>(() => BadObjectReference.Make("MemberInfo.Name", (p) => m_Name));

        m_GetAttributesReference =
            new Lazy<BadObjectReference>(() => BadObjectReference.Make("MemberInfo.GetAttributes",
                                                                       (p) => new BadInteropFunction("GetAttributes",
                                                                            GetAttributes,
                                                                            false,
                                                                            BadArray.Prototype
                                                                           )
                                                                      )
                                        );

        m_GetValueReference = new Lazy<BadObjectReference>(() => BadObjectReference.Make("MemberInfo.GetValue",
                                                                (p) => new BadInteropFunction("GetValue",
                                                                     GetValue,
                                                                     false,
                                                                     BadAnyPrototype.Instance
                                                                    )
                                                               )
                                                          );

        m_SetValueReference = new Lazy<BadObjectReference>(() => BadObjectReference.Make("MemberInfo.SetValue",
                                                                (p) => new BadInteropFunction("SetValue",
                                                                     SetValue,
                                                                     false,
                                                                     BadAnyPrototype.Instance,
                                                                     new BadFunctionParameter("value",
                                                                          false,
                                                                          false,
                                                                          false,
                                                                          null,
                                                                          BadAnyPrototype.Instance
                                                                         ),
                                                                     new BadFunctionParameter("noChangeEvent",
                                                                          true,
                                                                          true,
                                                                          false,
                                                                          null,
                                                                          BadNativeClassBuilder.GetNative("bool")
                                                                         )
                                                                    )
                                                               )
                                                          );

        m_IsReadonlyReference =
            new Lazy<BadObjectReference>(() =>
                                             BadObjectReference.Make("MemberInfo.IsReadonly",
                                                                     (p) => m_Scope.GetVariableInfo(m_Name)
                                                                         .IsReadOnly
                                                                    )
                                        );

        m_MemberTypeReference = new Lazy<BadObjectReference>(() => BadObjectReference.Make("MemberInfo.MemberType",
                                                                  (p) => m_Scope.GetVariableInfo(m_Name)
                                                                             .Type ??
                                                                         BadAnyPrototype.Instance
                                                                 )
                                                            );
    }

    /// <summary>
    /// The MemberInfo prototype.
    /// </summary>
    public static BadClassPrototype Prototype => s_Prototype;

    /// <summary>
    /// Gets the attributes of the member.
    /// </summary>
    /// <param name="ctx">The calling context.</param>
    /// <param name="args">The arguments passed to the method.</param>
    /// <returns>A BadObject representing the attributes of the member.</returns>
    private BadObject GetAttributes(BadExecutionContext ctx, BadObject[] args)
    {
        if (m_Scope.Attributes.TryGetValue(m_Name, out BadObject[]? attributes))
        {
            return new BadArray(attributes.ToList());
        }

        return new BadArray();
    }

    /// <summary>
    /// Returns the value of the member.
    /// </summary>
    /// <param name="ctx">The calling context.</param>
    /// <param name="args">The arguments passed to the method.</param>
    /// <returns>A BadObject representing the value of the member.</returns>
    private BadObject GetValue(BadExecutionContext ctx, BadObject[] args)
    {
        return m_Scope.GetVariable(m_Name)
                      .Dereference(null);
    }

    /// <summary>
    /// Sets the value of the member.
    /// </summary>
    /// <param name="ctx">The calling context.</param>
    /// <param name="args">The arguments passed to the method.</param>
    /// <returns>NULL</returns>
    /// <exception cref="BadRuntimeException">Thrown if the second argument is not a boolean.</exception>
    private BadObject SetValue(BadExecutionContext ctx, BadObject[] args)
    {
        if (args.Length == 1)
        {
            m_Scope.GetVariable(m_Name, ctx.Scope)
                   .Set(args[0], null);
        }
        else if (args.Length == 2)
        {
            if (args[1] is not IBadBoolean noEvents)
            {
                throw new BadRuntimeException("Second Argument must be a boolean");
            }

            m_Scope.GetVariable(m_Name, ctx.Scope)
                   .Set(args[0], null, null, noEvents.Value);
        }

        return Null;
    }

    /// <summary>
    /// Returns true if the member has the specified property.
    /// </summary>
    /// <param name="propName">The name of the property to check.</param>
    /// <param name="caller">The calling scope.</param>
    /// <returns>True if the member has the specified property, false otherwise.</returns>
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        switch (propName)
        {
            case "Name":
            case "GetAttributes":
            case "GetValue":
            case "SetValue":
            case "IsReadonly":
            case "MemberType":
                return true;
            default:
                return base.HasProperty(propName, caller);
        }
    }

    /// <summary>
    /// Retrieves the value of the specified property.
    /// </summary>
    /// <param name="propName">The name of the property to retrieve.</param>
    /// <param name="caller">The calling scope.</param>
    /// <returns>A BadObjectReference representing the value of the property.</returns>
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        switch (propName)
        {
            case "Name":
                return m_NameReference.Value;
            case "GetAttributes":
                return m_GetAttributesReference.Value;
            case "GetValue":
                return m_GetValueReference.Value;
            case "SetValue":
                return m_SetValueReference.Value;
            case "IsReadonly":
                return m_IsReadonlyReference.Value;
            case "MemberType":
                return m_MemberTypeReference.Value;
            default:
                return base.GetProperty(propName, caller);
        }
    }


    /// <summary>
    /// Returns the prototype of the MemberInfo class.
    /// </summary>
    /// <returns>The prototype of the MemberInfo class.</returns>
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        return "MemberInfo: " + m_Name;
    }
}