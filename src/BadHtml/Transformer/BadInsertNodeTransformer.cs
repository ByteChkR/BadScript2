using System.Collections.Generic;

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


    private IEnumerable<HtmlNode> GetNodes(BadHtmlContext context, string path, bool global)
    {
        if (path.StartsWith("#"))
        {
            yield return context.OutputDocument.GetElementbyId(path.Remove(1));
        }
        else
        {
            HtmlNode? root = global ? context.OutputDocument.DocumentNode : context.OutputNode;
            foreach (HtmlNode node in root.SelectNodes(path))
            {
                yield return node;
            }
        }
    }

    public override void TransformNode(BadHtmlContext context)
    {
        HtmlAttribute? pathAttribute = context.InputNode.Attributes["into"];
        bool isGlobal = context.InputNode.Attributes["global"]?.Value != "false";

        if (pathAttribute == null)
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Missing 'into' attribute in 'bs:insert' node",
                context.CreateOuterPosition()
            );
        }

        string? path = pathAttribute.Value;

        if (string.IsNullOrEmpty(path))
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Empty 'into' attribute in 'bs:insert' node",
                context.CreateAttributePosition(pathAttribute)
            );
        }


        foreach (HtmlNode outputNode in GetNodes(context, path, isGlobal))
        {
            foreach (HtmlNode node in context.InputNode.ChildNodes)
            {
                BadHtmlContext ctx = context.CreateChild(
                    node,
                    outputNode,
                    new BadExecutionContext(
                        context.ExecutionContext.Scope.CreateChild("bs:insert", context.ExecutionContext.Scope, null)
                    )
                );
                Transform(ctx);
            }
        }
    }
}