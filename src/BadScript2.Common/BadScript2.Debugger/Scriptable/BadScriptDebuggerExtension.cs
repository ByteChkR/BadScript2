using System;
using System.Linq;

using BadScript2.Common;
using BadScript2.Debugging;
using BadScript2.Parser;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Debugger.Scriptable;

/// <summary>
///     Implements Interop Extensions for the Debugger Objects
/// </summary>
public class BadScriptDebuggerExtension : BadInteropExtension
{
    /// <inheritdoc />
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<BadDebuggerStep>("Position", step => BadObject.Wrap(step.Position));
        provider.RegisterObject<BadDebuggerStep>("Scope", step => step.Context.Scope);

        provider.RegisterObject<BadDebuggerStep>("GetSourceView",
                                                 step => new BadDynamicInteropFunction<string[]>("GetSourceView",
                                                      (_, a) => GetSourceView(step,
                                                                              a.Select(int.Parse)
                                                                               .ToArray()
                                                                             ),
                                                      BadNativeClassBuilder.GetNative("string"),
                                                      new BadFunctionParameter("breakpointLines",
                                                                               true,
                                                                               true,
                                                                               false,
                                                                               null,
                                                                               BadNativeClassBuilder.GetNative("Array")
                                                                              )
                                                     )
                                                );

        provider.RegisterObject<BadDebuggerStep>("SourceView",
                                                 step => step.GetSourceView(Array.Empty<int>(), out int _, out int _)
                                                );

        provider.RegisterObject<BadDebuggerStep>("Line",
                                                 step =>
                                                 {
                                                     step.GetSourceView(Array.Empty<int>(),
                                                                        out int _,
                                                                        out int lineInSource
                                                                       );

                                                     return lineInSource;
                                                 }
                                                );

        provider.RegisterObject<BadDebuggerStep>("Evaluate",
                                                 step => new BadDynamicInteropFunction<string>("Evaluate",
                                                      (_, s) =>
                                                      {
                                                          BadObject obj = BadObject.Null;

                                                          foreach (BadObject o in step.Context.Execute(BadSourceParser
                                                                           .Create("<debugger>", s)
                                                                           .Parse()
                                                                       ))
                                                          {
                                                              obj = o;
                                                          }

                                                          return obj;
                                                      },
                                                      BadAnyPrototype.Instance,
                                                      new BadFunctionParameter("exprString", false, true, false, null, BadNativeClassBuilder.GetNative("string"))
                                                     )
                                                );

        provider.RegisterObject<BadSourcePosition>("Index", pos => pos.Index);
        provider.RegisterObject<BadSourcePosition>("Length", pos => pos.Length);
        provider.RegisterObject<BadSourcePosition>("FileName", pos => pos.FileName ?? BadObject.Null);
        provider.RegisterObject<BadSourcePosition>("Source", pos => pos.Source);
    }

    /// <summary>
    ///     Returns the Source View for the Debugger Step
    /// </summary>
    /// <param name="step">The Debugger Step</param>
    /// <param name="breakpoints">The Breakpoints</param>
    /// <returns>The Source View</returns>
    private static BadObject GetSourceView(BadDebuggerStep step, int[] breakpoints)
    {
        return step.GetSourceView(breakpoints, out int _, out int _);
    }
}