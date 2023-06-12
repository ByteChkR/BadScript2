using System;
using System.Collections.Generic;

using BadHtml.Transformer;

using BadScript2.Runtime.Objects;

using HtmlAgilityPack;

namespace BadHtml;

public abstract class BadHtmlNodeTransformer
{
	private static readonly List<BadHtmlNodeTransformer> s_Transformers = new List<BadHtmlNodeTransformer>
	{
		new BadExecuteScriptNodeTransformer(),
		new BadForEachNodeTransformer(),
		new BadWhileNodeTransformer(),
		new BadFunctionNodeTransformer(),
		new BadIfNodeTransformer(),
		new BadCopyStyleNodeTransformer(),
		new BadCopyScriptNodeTransformer(),
		new BadTextNodeTransformer(),
		new BadCopyNodeTransformer()
	};

	public abstract bool CanTransform(BadHtmlContext context);

	public abstract void TransformNode(BadHtmlContext context);

	protected static void TransformAttributes(BadHtmlContext context, HtmlNode node)
	{
		foreach (HtmlAttribute attribute in context.InputNode.Attributes)
		{
			//Get Attribute Value
			string value = attribute.Value;

			//Replace all newlines with spaces
			value = value.Replace("\n", " ").Replace("\r", " ");

			//Evaluate Attribute Value with BadScript
			BadObject result = context.ParseAndExecute("$\"" + value + '"', context.CreateAttributePosition(attribute));

			//Set Attribute Value
			node.Attributes[attribute.Name].Value = result.ToString();
		}
	}

	public static void Transform(BadHtmlContext context)
	{
		foreach (BadHtmlNodeTransformer transformer in s_Transformers)
		{
			if (transformer.CanTransform(context))
			{
				transformer.TransformNode(context);

				return;
			}
		}

		throw new InvalidOperationException("No transformer found");
	}
}
