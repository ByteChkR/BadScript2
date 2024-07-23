using HtmlAgilityPack;
namespace BadHtml.Transformer;

/// <summary>
///     Copies the current style node to the output
/// </summary>
public class BadCopyStyleNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform" />
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "style";
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