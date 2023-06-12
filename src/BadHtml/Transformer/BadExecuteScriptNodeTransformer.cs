using System;

using BadScript2.Runtime.Objects;

namespace BadHtml.Transformer;

public class BadExecuteScriptNodeTransformer : BadHtmlNodeTransformer
{
	public override bool CanTransform(BadHtmlContext context)
	{
		return context.InputNode.Name == "script" && context.InputNode.Attributes["lang"]?.Value == "bs2";
	}

	public override void TransformNode(BadHtmlContext context)
	{
		string code = context.InputNode.InnerText;

		BadObject result = context.ParseAndExecute(code, context.CreateInnerPosition());

		Console.WriteLine("Result of Script Block: " + result);
	}
}
