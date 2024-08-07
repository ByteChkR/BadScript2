using HtmlAgilityPack;
namespace BadHtml.Transformer;

/// <summary>
///     Copies the current script node to the output if the lang attribute is not bs2
/// </summary>
public class BadCopyScriptNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform" />
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "script" && context.InputNode.Attributes["lang"]?.Value != "bs2";
    }

    /// <inheritdoc cref="BadHtmlNodeTransformer.TransformNode" />
    protected override void TransformNode(BadHtmlContext context)
    {
        //Deep Clone
        HtmlNode? node = context.InputNode.CloneNode(true);

        //Append Node to output
        context.OutputNode.AppendChild(node);
    }
}