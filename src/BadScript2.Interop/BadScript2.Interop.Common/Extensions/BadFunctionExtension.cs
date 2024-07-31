using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Extensions;

/// <summary>
///     Implements Function Extensions
/// </summary>
public class BadFunctionExtension : BadInteropExtension
{
    /// <inheritdoc />
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<BadFunction>("Name", f => f.Name?.Text ?? "<anonymous>");
        provider.RegisterObject<BadFunction>("ReturnType", f => f.ReturnType);

        provider.RegisterObject<BadFunction>("Parameters",
                                             f => new BadArray(f.Parameters.Select(x => BadObject.Wrap(x))
                                                                .ToList()
                                                              )
                                            );

        provider.RegisterObject<BadFunction>("Invoke",
                                             f => new BadDynamicInteropFunction<BadObject>("Invoke",
                                                  (ctx, a) =>
                                                  {
                                                      BadObject r = BadObject.Null;

                                                      BadObject[] args;

                                                      if (a is BadArray arr)
                                                      {
                                                          args = arr.InnerArray.ToArray();
                                                      }
                                                      else if (BadNativeClassBuilder.Enumerable
                                                               .IsSuperClassOf(a.GetPrototype()))
                                                      {
                                                          args = BadNativeClassHelper.ExecuteEnumerate(ctx, a)
                                                              .ToArray();
                                                      }
                                                      else
                                                      {
                                                          throw new BadRuntimeException("Invalid Argument Type");
                                                      }

                                                      foreach (BadObject o in f.Invoke(args, ctx))
                                                      {
                                                          r = o;
                                                      }

                                                      return r;
                                                  },
                                                  f.ReturnType,
                                                  new BadFunctionParameter("args",
                                                                           false,
                                                                           false,
                                                                           false,
                                                                           null,
                                                                           BadNativeClassBuilder.Enumerable
                                                                          )
                                                 )
                                            );

        provider.RegisterObject<BadFunction>("Meta", f => f.MetaData);

        provider.RegisterObject<BadFunctionParameter>("Name", p => p.Name);
        provider.RegisterObject<BadFunctionParameter>("IsNullChecked", p => p.IsNullChecked);
        provider.RegisterObject<BadFunctionParameter>("IsOptional", p => p.IsOptional);
        provider.RegisterObject<BadFunctionParameter>("IsRestArgs", p => p.IsRestArgs);
        provider.RegisterObject<BadFunctionParameter>("Type", p => p.Type ?? BadObject.Null);
    }
}