using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

/// <summary>
/// Implements the 'bs:if' node transformer
/// </summary>
public class BadIfNodeTransformer : BadHtmlNodeTransformer
{
	public override bool CanTransform(BadHtmlContext context)
	{
		return context.InputNode.Name == "bs:if";
	}

	public override void TransformNode(BadHtmlContext context)
	{
		HtmlAttribute? conditionAttribute = context.InputNode.Attributes["test"];

		if (conditionAttribute == null)
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Missing 'test' attribute in 'bs:if' node",
				context.CreateOuterPosition());
		}

		if (string.IsNullOrEmpty(conditionAttribute.Value))
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Empty 'test' attribute in 'bs:if' node",
				context.CreateAttributePosition(conditionAttribute));
		}

		BadObject resultObj = context.ParseAndExecute(conditionAttribute.Value,
			context.CreateAttributePosition(conditionAttribute));

		if (resultObj is not IBadBoolean result)
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Result of 'test' attribute in 'bs:if' node is not a boolean",
				context.CreateAttributePosition(conditionAttribute));
		}

		if (result.Value)
		{
			BadExecutionContext branchContext = new BadExecutionContext(context.ExecutionContext.Scope.CreateChild(
				"bs:if",
				context.ExecutionContext.Scope,
				null));

			foreach (HtmlNode? child in context.InputNode.ChildNodes)
			{
				BadHtmlContext childContext = context.CreateChild(child, context.OutputNode, branchContext);

				Transform(childContext);
			}
		}
	}
}
