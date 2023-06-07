using HtmlAgilityPack;

namespace BadHtml.Transformer;

public class BadCopyNodeTransformer : BadHtmlNodeTransformer
{
    public override bool CanTransform(BadHtmlContext context)
    {
        return true;
    }

    public override void TransformNode(BadHtmlContext context)
    {
        //Clone Node(not children)
        HtmlNode? node = context.InputNode.CloneNode(false);

        //Append Node to output
        context.OutputNode.AppendChild(node);

        TransformAttributes(context, node);
        
        //Iterate through children
        foreach (HtmlNode? child in context.InputNode.ChildNodes)
        {
            BadHtmlContext childContext = context.CreateChild(child, node);

            Transform(childContext);
        }
    }
}