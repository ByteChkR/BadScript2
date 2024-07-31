using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

public class BadMemberInfo : BadObject
{
    private static readonly BadNativeClassPrototype<BadMemberInfo> s_Prototype =
        new BadNativeClassPrototype<BadMemberInfo>("MemberInfo",
                                                   (c, a) => throw BadRuntimeException.Create(c.Scope,
                                                                  "MemberInfo is a read only object"
                                                                 )
                                                  );

    private readonly Lazy<BadObjectReference> m_GetAttributesReference;
    private readonly Lazy<BadObjectReference> m_GetValueReference;
    private readonly Lazy<BadObjectReference> m_IsReadonlyReference;
    private readonly Lazy<BadObjectReference> m_MemberTypeReference;
    private readonly string m_Name;

    private readonly Lazy<BadObjectReference> m_NameReference;
    private readonly BadScope m_Scope;
    private readonly Lazy<BadObjectReference> m_SetValueReference;

    public BadMemberInfo(string name, BadScope scope)
    {
        m_Name = name;
        m_Scope = scope;
        m_NameReference = new Lazy<BadObjectReference>(() => BadObjectReference.Make("MemberInfo.Name", () => m_Name));

        m_GetAttributesReference =
            new Lazy<BadObjectReference>(() => BadObjectReference.Make("MemberInfo.GetAttributes",
                                                                       () => new BadInteropFunction("GetAttributes",
                                                                            GetAttributes,
                                                                            false,
                                                                            BadArray.Prototype
                                                                           )
                                                                      )
                                        );

        m_GetValueReference = new Lazy<BadObjectReference>(() => BadObjectReference.Make("MemberInfo.GetValue",
                                                                () => new BadInteropFunction("GetValue",
                                                                     GetValue,
                                                                     false,
                                                                     BadAnyPrototype.Instance
                                                                    )
                                                               )
                                                          );

        m_SetValueReference = new Lazy<BadObjectReference>(() => BadObjectReference.Make("MemberInfo.SetValue",
                                                                () => new BadInteropFunction("SetValue",
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
                                                                     () => m_Scope.GetVariableInfo(m_Name)
                                                                         .IsReadOnly
                                                                    )
                                        );

        m_MemberTypeReference = new Lazy<BadObjectReference>(() => BadObjectReference.Make("MemberInfo.MemberType",
                                                                  () => m_Scope.GetVariableInfo(m_Name)
                                                                               .Type ??
                                                                        BadAnyPrototype.Instance
                                                                 )
                                                            );
    }

    public static BadClassPrototype Prototype => s_Prototype;

    private BadObject GetAttributes(BadExecutionContext ctx, BadObject[] args)
    {
        if (m_Scope.Attributes.TryGetValue(m_Name, out BadObject[]? attributes))
        {
            return new BadArray(attributes.ToList());
        }

        return new BadArray();
    }

    private BadObject GetValue(BadExecutionContext ctx, BadObject[] args)
    {
        return m_Scope.GetVariable(m_Name)
                      .Dereference();
    }

    private BadObject SetValue(BadExecutionContext ctx, BadObject[] args)
    {
        if (args.Length == 1)
        {
            m_Scope.GetVariable(m_Name, ctx.Scope)
                   .Set(args[0]);
        }
        else if (args.Length == 2)
        {
            if (args[1] is not IBadBoolean noEvents)
            {
                throw new BadRuntimeException("Second Argument must be a boolean");
            }

            m_Scope.GetVariable(m_Name, ctx.Scope)
                   .Set(args[0], null, noEvents.Value);
        }

        return Null;
    }

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


    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return "MemberInfo: " + m_Name;
    }
}