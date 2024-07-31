using System;
using System.Collections.Generic;
using System.Linq;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

/// <summary>
///     A transformer that allows the user to invoke a function by specifying the function namd and its arguments as a
///     valid html node.
///     Example: Function Test(a, b) exists; Execute with <Test a="1" b="2" />
/// </summary>
public class BadComponentNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform" />
    protected override bool CanTransform(BadHtmlContext context)
    {
        string? nodeName = context.InputNode.OriginalName;

        if (context.ExecutionContext.Scope.HasVariable(nodeName, context.ExecutionContext.Scope))
        {
            BadObject componentDefinition = context.ExecutionContext.Scope.GetVariable(nodeName)
                                                   .Dereference();

            if (componentDefinition is BadFunction function)
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc cref="BadHtmlNodeTransformer.TransformNode" />
    protected override void TransformNode(BadHtmlContext context)
    {
        string? nodeName = context.InputNode.OriginalName;

        BadObject componentDefinition = context.ExecutionContext.Scope.GetVariable(nodeName)
                                               .Dereference();
        BadFunction function = (BadFunction)componentDefinition;

        HtmlAttribute[] attributes = context.InputNode.Attributes.ToArray();

        string[] indexMap = function.Parameters.Select(x => x.Name)
                                    .ToArray();
        Dictionary<string, BadObject> arguments = new Dictionary<string, BadObject>();

        foreach (HtmlAttribute attribute in attributes)
        {
            int index = Array.IndexOf(indexMap, attribute.OriginalName);

            if (index != -1)
            {
                BadObject arg = context.ParseAndExecuteSingle(attribute.Value,
                                                              context.CreateAttributePosition(attribute)
                                                             );
                arguments[attribute.OriginalName] = arg;
            }
        }

        List<BadObject> args = new List<BadObject>();

        foreach (string argName in indexMap)
        {
            args.Add(arguments.TryGetValue(argName, out BadObject? argument) ? argument : BadObject.Null);
        }

        BadObject result = BadObject.Null;

        foreach (BadObject o in function.Invoke(args.ToArray(), context.ExecutionContext))
        {
            result = o;
        }

        result = result.Dereference();

        if (result is not IBadString str)
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope,
                                             "Component must return a string",
                                             context.CreateOuterPosition()
                                            );
        }

        context.OutputNode.AppendChild(HtmlNode.CreateNode(str.Value));
    }
}