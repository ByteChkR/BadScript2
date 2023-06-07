using HtmlAgilityPack;

namespace BadHtml.Transformer;

public class BadCopyScriptNodeTransformer : BadHtmlNodeTransformer
{
    public override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "script" && context.InputNode.Attributes["lang"]?.Value != "bs2";
    }

    public override void TransformNode(BadHtmlContext context)
    {
        //Deep Clone
        HtmlNode? node = context.InputNode.CloneNode(true);

        //Append Node to output
        context.OutputNode.AppendChild(node);
    }
}