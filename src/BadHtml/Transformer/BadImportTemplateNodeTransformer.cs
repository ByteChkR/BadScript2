using System;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

public class BadImportTemplateNodeTransformer : BadHtmlNodeTransformer
{
	public override bool CanTransform(BadHtmlContext context)
	{
		return context.InputNode.Name == "bs:template";
	}

	public override void TransformNode(BadHtmlContext context)
	{
		HtmlAttribute? pathAttribute = context.InputNode.Attributes["path"];
		HtmlAttribute? modelAttribute = context.InputNode.Attributes["model"];

		if (pathAttribute == null)
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Missing 'path' attribute in 'bs:template' node",
				context.CreateOuterPosition());
		}

		string? model = modelAttribute?.Value;

		if (string.IsNullOrEmpty(model))
		{
			Console.WriteLine("Missing 'model' attribute in 'bs:template' node");
		}

		string? path = pathAttribute.Value;

		if (string.IsNullOrEmpty(path))
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Empty 'path' attribute in 'bs:template' node",
				context.CreateAttributePosition(pathAttribute));
		}

		BadObject modelObj = model == null ?
			BadObject.Null :
			context.ParseAndExecute(model, context.CreateAttributePosition(modelAttribute!));

		BadHtmlTemplate template = BadHtmlTemplate.Create(path);
		HtmlDocument res = template.RunTemplate(modelObj, context.Options);

		context.InputNode.AppendChildren(res.DocumentNode.ChildNodes);
	}
}
