using BadScript2.Runtime;
using BadScript2.Runtime.Error;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

public class BadInsertNodeTransformer : BadHtmlNodeTransformer
{
	public override bool CanTransform(BadHtmlContext context)
	{
		return context.InputNode.Name == "bs:insert";
	}

	public override void TransformNode(BadHtmlContext context)
	{
		HtmlAttribute? pathAttribute = context.InputNode.Attributes["into"];
		bool isGlobal = context.InputNode.Attributes["global"]?.Value != "false";

		if (pathAttribute == null)
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Missing 'into' attribute in 'bs:insert' node",
				context.CreateOuterPosition());
		}

		string? path = pathAttribute.Value;

		if (string.IsNullOrEmpty(path))
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Empty 'into' attribute in 'bs:insert' node",
				context.CreateAttributePosition(pathAttribute));
		}

		//Either it is an ID(starting with #) or a XPath
		//	If its an ID the "isGlobal" flag is ignored
		HtmlNode? outputNode = path.StartsWith("#") ? context.OutputDocument.GetElementbyId(path.Remove(1)) :
			isGlobal ?
				context.OutputDocument.DocumentNode.SelectSingleNode(path) :
				context.OutputNode.SelectSingleNode(path);

		if (outputNode == null)
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				$"Could not find node '{path}'",
				context.CreateAttributePosition(pathAttribute));
		}

		foreach (HtmlNode node in context.InputNode.ChildNodes)
		{
			BadHtmlContext ctx = context.CreateChild(node,
				outputNode,
				new BadExecutionContext(
					context.ExecutionContext.Scope.CreateChild("bs:insert", context.ExecutionContext.Scope, null)));
			Transform(ctx);
		}
	}
}
