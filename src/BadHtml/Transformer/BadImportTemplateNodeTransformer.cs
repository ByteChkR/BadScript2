using System;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

/// <summary>
///     Imports the template specified in the path attribute of the current bs:template node
/// </summary>
public class BadImportTemplateNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform" />
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "bs:template";
    }

    /// <inheritdoc cref="BadHtmlNodeTransformer.TransformNode" />
    protected override void TransformNode(BadHtmlContext context)
    {
        HtmlAttribute? pathAttribute = context.InputNode.Attributes["path"];
        HtmlAttribute? modelAttribute = context.InputNode.Attributes["model"];

        if (pathAttribute == null)
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope,
                                             "Missing 'path' attribute in 'bs:template' node",
                                             context.CreateOuterPosition()
                                            );
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
                                             context.CreateAttributePosition(pathAttribute)
                                            );
        }

        BadObject modelObj = model == null
                                 ? BadObject.Null
                                 : context.ParseAndExecuteSingle(model,
                                                                 context.CreateAttributePosition(modelAttribute!)
                                                                );

        BadHtmlTemplate template = BadHtmlTemplate.Create(path, context.FileSystem);
        HtmlDocument res = template.RunTemplate(modelObj, context.Options);

        context.InputNode.AppendChildren(res.DocumentNode.ChildNodes);
    }
}