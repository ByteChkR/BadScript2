using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Extensions;

/// <summary>
///     Implements TypeSystem Extensions
/// </summary>
public class BadTypeSystemExtension : BadInteropExtension
{
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterGlobal(
            "IsInstanceOf",
            obj => new BadDynamicInteropFunction<BadClassPrototype>(
                "IsInstanceOf",
                (_, proto) => IsInstanceOf(proto, obj),
                BadNativeClassBuilder.GetNative("bool"),
                new BadFunctionParameter("prototype", false, true, false)
            )
        );

        provider.RegisterObject<BadClass>(
            "GetClassScope",
            c => new BadDynamicInteropFunction("GetClassScope", _ => c.Scope, BadScope.Prototype)
        );

        provider.RegisterObject<BadClassPrototype>(
            "CreateInstance",
            p => new BadDynamicInteropFunction(
                "CreateInstance",
                ctx =>
                {
                    BadObject obj = BadObject.Null;

                    foreach (BadObject o in p.CreateInstance(ctx))
                    {
                        obj = o;
                    }

                    return obj;
                },
                p
            )
        );

        provider.RegisterObject<BadClassPrototype>("Meta", f => f.MetaData);

        provider.RegisterObject<BadClassPrototype>(
            "IsAssignableFrom",
            proto => new BadDynamicInteropFunction<BadObject>(
                "IsAssignableFrom",
                (_, o) => IsAssignableFrom(o, proto),
                BadNativeClassBuilder.GetNative("bool")
            )
        );
        provider.RegisterObject<BadClassPrototype>(
            "IsBaseClassOf",
            proto => new BadDynamicInteropFunction<BadClassPrototype>(
                "IsBaseClassOf",
                (_, super) => IsBaseClassOf(proto, super),
                BadNativeClassBuilder.GetNative("bool")
            )
        );

        provider.RegisterObject<BadClassPrototype>(
            "IsSuperClassOf",
            proto => new BadDynamicInteropFunction<BadClassPrototype>(
                "IsSuperClassOf",
                (_, super) => IsBaseClassOf(super, proto),
                BadNativeClassBuilder.GetNative("bool")
            )
        );

        provider.RegisterObject<BadClassPrototype>(
            "GetBaseClass",
            p => new BadDynamicInteropFunction(
                "GetBaseClass",
                _ => p.GetBaseClass() ?? BadObject.Null,
                BadClassPrototype.Prototype
            )
        );

        provider.RegisterObject<BadClassPrototype>(
            "Name",
            proto => proto.Name
        );
    }


    /// <summary>
    ///     Returns true if the given object is an instance of the given prototype
    /// </summary>
    /// <param name="obj">The Object</param>
    /// <param name="proto">The Prototype</param>
    /// <returns>True if the object is an instance of the given prototype</returns>
    private static BadObject IsAssignableFrom(BadObject obj, BadClassPrototype proto)
    {
        return proto.IsAssignableFrom(obj);
    }

    /// <summary>
    ///     Returns true if the given prototype is a base class of the given prototype
    /// </summary>
    /// <param name="proto">Prototype</param>
    /// <param name="super">Super Type</param>
    /// <returns>True if the given prototype is a base class of the given prototype</returns>
    private static BadObject IsBaseClassOf(
        BadClassPrototype proto,
        BadClassPrototype super)
    {
        return super.IsSuperClassOf(proto);
    }

    /// <summary>
    ///     Returns true if the given object is an instance of the given prototype
    /// </summary>
    /// <param name="obj">The Object</param>
    /// <param name="proto">The Prototype</param>
    /// <returns>True if the object is an instance of the given prototype</returns>
    private static BadObject IsInstanceOf(BadClassPrototype proto, BadObject obj)
    {
        return proto.IsAssignableFrom(obj);
    }
}