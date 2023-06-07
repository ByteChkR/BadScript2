using System;
using System.Linq;

using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadHtml.Transformer;

public class BadTextNodeTransformer : BadHtmlNodeTransformer
{
    public override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "#text";
    }

    public override void TransformNode(BadHtmlContext context)
    {
        if(context.InputNode.InnerText.All(x=>char.IsWhiteSpace(x) || x == '\n' || x == '\t'))
        {
            context.OutputNode.AppendChild(context.OutputDocument.CreateTextNode(context.InputNode.InnerText));
            return;
        }
        //Get Text and Replace all newlines with spaces
        string text = "$\"" + context.InputNode.InnerText.Replace("\n", " ").Replace("\r", " ") + '"';

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