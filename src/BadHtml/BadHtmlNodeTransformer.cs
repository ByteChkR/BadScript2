using System;
using System.Collections.Generic;

using BadHtml.Transformer;

using BadScript2.Runtime.Objects;

using HtmlAgilityPack;

namespace BadHtml;

/// <summary>
///     Base class of all Node transformers
/// </summary>
public abstract class BadHtmlNodeTransformer
{
	/// <summary>
	///     List of all node transformers used in the engine
	/// </summary>
	private static readonly List<BadHtmlNodeTransformer> s_Transformers = new List<BadHtmlNodeTransformer>
    {
        new BadInsertNodeTransformer(),
        new BadImportTemplateNodeTransformer(),
        new BadImportFileNodeTransformer(),
        new BadExecuteScriptNodeTransformer(),
        new BadForEachNodeTransformer(),
        new BadWhileNodeTransformer(),
        new BadFunctionNodeTransformer(),
        new BadIfNodeTransformer(),
        new BadCopyStyleNodeTransformer(),
        new BadCopyScriptNodeTransformer(),
        new BadComponentNodeTransformer(),
        new BadTextNodeTransformer(),
        new BadCopyNodeTransformer(),
    };

	/// <summary>
	///     Returns true if the transformer can transform the specified node
	/// </summary>
	/// <param name="context">The Html Context</param>
	/// <returns>True if can transform node</returns>
	protected abstract bool CanTransform(BadHtmlContext context);

	/// <summary>
	///     Transforms the input node
	/// </summary>
	/// <param name="context">The Html Context</param>
	protected abstract void TransformNode(BadHtmlContext context);

	/// <summary>
	///     Transforms all attributes of the input node and writes it to the specified output node
	/// </summary>
	/// <param name="context">The Html Context</param>
	/// <param name="outputNode">The Output Node</param>
	protected static void TransformAttributes(BadHtmlContext context, HtmlNode outputNode)
    {
        foreach (HtmlAttribute attribute in context.InputNode.Attributes)
        {
            //Get Attribute Value
            string value = attribute.Value;

            //Replace all newlines with spaces
            value = value.Replace("\n", " ").Replace("\r", " ");

            //Evaluate Attribute Value with BadScript
            BadObject result =
                context.ParseAndExecute("$\"" + value + "\";", context.CreateAttributePosition(attribute));

            //Set Attribute Value
            outputNode.Attributes[attribute.Name].Value = result.ToString();
        }
    }

	/// <summary>
	///     Transforms the input node with one of the registered transformers
	/// </summary>
	/// <param name="context">The Html Context</param>
	/// <exception cref="InvalidOperationException">Gets raised if the node can not be transformed</exception>
	public static void Transform(BadHtmlContext context)
    {
        foreach (BadHtmlNodeTransformer transformer in s_Transformers)
        {
            if (!transformer.CanTransform(context))
            {
                continue;
            }

            transformer.TransformNode(context);

            return;
        }

        throw new InvalidOperationException("No transformer found");
    }
}