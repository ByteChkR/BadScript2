using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Interop.Common.Extensions;

/// <summary>
///     Implements TypeSystem Extensions
/// </summary>
public class BadTypeSystemExtension : BadInteropExtension
{
    /// <inheritdoc />
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterGlobal("IsInstanceOf",
                                obj => new BadDynamicInteropFunction<BadClassPrototype>("IsInstanceOf",
                                     (_, proto) => IsInstanceOf(proto, obj),
                                     BadNativeClassBuilder.GetNative("bool"),
                                     new BadFunctionParameter("prototype", false, true, false)
                                    )
                               );

        provider.RegisterObject<BadClass>("GetClassScope",
                                          c => new BadDynamicInteropFunction("GetClassScope",
                                                                             _ => c.Scope,
                                                                             BadScope.Prototype
                                                                            )
                                         );

        provider.RegisterObject<BadClassPrototype>("CreateInstance",
            p => new BadDynamicInteropFunction("CreateInstance",
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


        provider.RegisterObject<BadClassPrototype>("GenericTypeCount",
            p => p is BadExpressionClassPrototype ecp ? 
                ecp.GenericParameters.Count : 
                p is BadInterfacePrototype ip ? 
                    ip.GenericParameters.Count : 0
        );


        provider.RegisterObject<BadClassPrototype>("CreateGenericType",
            proto => new BadDynamicInteropFunction<BadArray>("CreateGenericType",
                (ctx, o) =>
                {
                    var args = o.InnerArray.ToList();
                    if (proto is BadExpressionClassPrototype ecp)
                    {
                        if (ecp.IsResolved) return ecp;
                        if (args.Count < ecp.GenericParameters.Count)
                        {
                            args.AddRange(Enumerable.Repeat(BadAnyPrototype.Instance, ecp.GenericParameters.Count - args.Count));
                        }
                        if (args.Count != ecp.GenericParameters.Count)
                        {
                            throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Count");
                        }
                        return ecp.CreateGeneric(args.ToArray());
                    }
                    if (proto is BadInterfacePrototype ip)
                    {
                        if (ip.IsResolved) return ip;
                        if (args.Count < ip.GenericParameters.Count)
                        {
                            args.AddRange(Enumerable.Repeat(BadAnyPrototype.Instance, ip.GenericParameters.Count - args.Count));
                        }
                        if (args.Count != ip.GenericParameters.Count)
                        {
                            throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Count");
                        }
                        return ip.CreateGeneric(args.ToArray());
                    }
                    return proto;

                },
                BadNativeClassBuilder.GetNative("Type")
            ));

        provider.RegisterObject<BadClassPrototype>("Meta", f => f.MetaData);

        provider.RegisterObject<BadClassPrototype>("IsAssignableFrom",
                                                   proto => new BadDynamicInteropFunction<BadObject>("IsAssignableFrom",
                                                        (_, o) => IsAssignableFrom(o, proto),
                                                        BadNativeClassBuilder.GetNative("bool")
                                                       )
                                                  );

        provider.RegisterObject<BadClassPrototype>("IsBaseClassOf",
                                                   proto =>
                                                       new BadDynamicInteropFunction<BadClassPrototype>("IsBaseClassOf",
                                                            (_, super) => IsBaseClassOf(proto, super),
                                                            BadNativeClassBuilder.GetNative("bool")
                                                           )
                                                  );

        provider.RegisterObject<BadClassPrototype>("IsSuperClassOf",
                                                   proto =>
                                                       new BadDynamicInteropFunction<
                                                           BadClassPrototype>("IsSuperClassOf",
                                                                              (_, super) => IsBaseClassOf(super, proto),
                                                                              BadNativeClassBuilder.GetNative("bool")
                                                                             )
                                                  );

        provider.RegisterObject<BadClassPrototype>("GetBaseClass",
                                                   p => new BadDynamicInteropFunction("GetBaseClass",
                                                        _ => p.GetBaseClass() ?? BadObject.Null,
                                                        BadClassPrototype.Prototype
                                                       )
                                                  );

        provider.RegisterObject<BadClassPrototype>("Name",
            proto => proto.Name
        );
        provider.RegisterObject<BadClassPrototype>("Interfaces",
            proto => new BadArray(proto.Interfaces.Select(BadObject (x) => x).ToList())
        );
        provider.RegisterObject<BadClassPrototype>("IsAbstract",
            proto => proto.IsAbstract
        );
        provider.RegisterObject<BadClassPrototype>("IsInterface",
            proto => proto is BadInterfacePrototype
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
    private static BadObject IsBaseClassOf(BadClassPrototype proto,
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