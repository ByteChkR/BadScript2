using HtmlAgilityPack;

namespace BadHtml.Transformer;

public class BadCopyStyleNodeTransformer : BadHtmlNodeTransformer
{
	public override bool CanTransform(BadHtmlContext context)
	{
		return context.InputNode.Name == "style";
	}

	public override void TransformNode(BadHtmlContext context)
	{
		//Deep Clone
		HtmlNode? node = context.InputNode.CloneNode(true);

		//Append Node to output
		context.OutputNode.AppendChild(node);
	}
}
