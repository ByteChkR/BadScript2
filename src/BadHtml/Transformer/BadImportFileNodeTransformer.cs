using BadScript2.IO;
using BadScript2.Runtime.Error;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

/// <summary>
///     Imports the file specified in the path attribute of the current bs:import node
/// </summary>
public class BadImportFileNodeTransformer : BadHtmlNodeTransformer
{
    public override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "bs:import";
    }

    public override void TransformNode(BadHtmlContext context)
    {
        HtmlAttribute? pathAttribute = context.InputNode.Attributes["path"];

        if (pathAttribute == null)
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Missing 'path' attribute in 'bs:import' node",
                context.CreateOuterPosition()
            );
        }


        string? path = pathAttribute.Value;

        if (string.IsNullOrEmpty(path))
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Empty 'path' attribute in 'bs:import' node",
                context.CreateAttributePosition(pathAttribute)
            );
        }

        string data = BadFileSystem.ReadAllText(path);

        context.OutputNode.AppendChild(context.OutputDocument.CreateTextNode(data));
    }
}