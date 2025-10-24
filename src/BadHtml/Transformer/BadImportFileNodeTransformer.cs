using System.Linq;
using BadScript2;
using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using HtmlAgilityPack;

namespace BadHtml.Transformer;

/// <summary>
///     Imports the file specified in the path attribute of the current bs:import node
/// </summary>
public class BadImportFileNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform" />
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "bs:import";
    }

    /// <inheritdoc cref="BadHtmlNodeTransformer.TransformNode" />
    protected override void TransformNode(BadHtmlContext context)
    {
        HtmlAttribute? pathAttribute = context.InputNode.Attributes["path"];

        var pos = context.CreateOuterPosition();
        if (pathAttribute == null)
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope,
                                             "Missing 'path' attribute in 'bs:import' node",
                                             pos
                                            );
        }
        string? path = pathAttribute.Value;

        if (string.IsNullOrEmpty(path))
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope,
                                             "Empty 'path' attribute in 'bs:import' node",
                                             context.CreateAttributePosition(pathAttribute)
                                            );
        }

        path = context.ExecutionContext.GetFullPath(path, pos);
        var data = context.ExecutionContext.ReadAllText(path, pos);

        context.OutputNode.AppendChild(context.OutputDocument.CreateTextNode(data));
    }
}