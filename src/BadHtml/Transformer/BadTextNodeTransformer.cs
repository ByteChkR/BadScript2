using System;
using System.Linq;

using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadHtml.Transformer;

/// <summary>
///     Implements a BadScript Text Node Transformer
///     Copies the text nodes to the output and evaluates them with BadScript
/// </summary>
public class BadTextNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform"/>
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "#text";
    }

    /// <inheritdoc cref="BadHtmlNodeTransformer.TransformNode"/>
    protected override void TransformNode(BadHtmlContext context)
    {
        if (context.InputNode.InnerText.All(x => char.IsWhiteSpace(x) || x == '\n' || x == '\r' || x == '\t'))
        {
            if (!context.Options.SkipEmptyTextNodes)
            {
                context.OutputNode.AppendChild(context.OutputDocument.CreateTextNode(context.InputNode.InnerText));
            }

            return;
        }

        //Get Text and Replace all newlines with spaces
        string text = "$@\"" + context.InputNode.InnerText + "\";";

        //Evaluate Text with BadScript
        BadObject result = context.ParseAndExecute(text, context.CreateInnerPosition());

        if (result is not IBadString resultStr)
        {
            Console.WriteLine("Warning: Text node is not a string");
            context.OutputNode.AppendChild(context.OutputDocument.CreateTextNode(result.ToString()));

            return;
        }

        //Append Text to output
        context.OutputNode.AppendChild(context.OutputDocument.CreateTextNode(resultStr.Value));
    }
}