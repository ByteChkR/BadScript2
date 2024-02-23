using System;

using BadScript2.Runtime.Objects;

namespace BadHtml.Transformer;

/// <summary>
///     Executes the current script block if the lang attribute is bs2
/// </summary>
public class BadExecuteScriptNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform" />
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "script" && context.InputNode.Attributes["lang"]?.Value == "bs2";
    }

    /// <inheritdoc cref="BadHtmlNodeTransformer.TransformNode" />
    protected override void TransformNode(BadHtmlContext context)
    {
        string code = context.InputNode.InnerText;

        BadObject result = context.ParseAndExecute(code, context.CreateInnerPosition());

    }
}