using BadScript2.Common;
using BadScript2.Debugging;
using BadScript2.Parser;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Debugger.Scriptable;

/// <summary>
///     Implements Interop Extensions for the Debugger Objects
/// </summary>
public class BadScriptDebuggerExtension : BadInteropExtension
{
    protected override void AddExtensions()
    {
        RegisterObject<BadDebuggerStep>("Position", step => BadObject.Wrap(step.Position));
        RegisterObject<BadDebuggerStep>("Scope", step => step.Context.Scope);
        RegisterObject<BadDebuggerStep>("SourceView", step => step.GetSourceView(out int _, out int _));
        RegisterObject<BadDebuggerStep>(
            "Line",
            step =>
            {
                step.GetSourceView(out int _, out int lineInSource);

                return lineInSource;
            }
        );
        RegisterObject<BadDebuggerStep>(
            "Evaluate",
            step => new BadDynamicInteropFunction<string>(
                "Evaluate",
                (_, s) =>
                {
                    BadObject obj = BadObject.Null;

                    foreach (BadObject o in step.Context.Execute(BadSourceParser.Create("<debugger>", s).Parse()))
                    {
                        obj = o;
                    }

                    return obj;
                }
            )
        );

        RegisterObject<BadSourcePosition>("Index", pos => pos.Index);
        RegisterObject<BadSourcePosition>("Length", pos => pos.Length);
        RegisterObject<BadSourcePosition>("FileName", pos => pos.FileName ?? BadObject.Null);
        RegisterObject<BadSourcePosition>("Source", pos => pos.Source);
    }
}