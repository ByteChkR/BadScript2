using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

public class BadWhileNodeTransformer : BadHtmlNodeTransformer
{
    public override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "bs:while";
    }

    private bool Evaluate(BadHtmlContext context, HtmlAttribute attribute, BadExpression[] expressions)
    {
        BadObject resultObj = context.Execute(expressions);

        if (resultObj is not IBadBoolean result)
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Result of 'test' attribute in 'bs:if' node is not a boolean",
                context.CreateAttributePosition(attribute)
            );
        }

        return result.Value;
    }

    public override void TransformNode(BadHtmlContext context)
    {
        HtmlAttribute? conditionAttribute = context.InputNode.Attributes["test"];
        if (conditionAttribute == null)
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope, "Missing 'test' attribute in 'bs:if' node", context.CreateOuterPosition());
        }

        if (string.IsNullOrEmpty(conditionAttribute.Value))
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope, "Empty 'test' attribute in 'bs:if' node", context.CreateAttributePosition(conditionAttribute));
        }

        BadExpression[] expressions = context.Parse(conditionAttribute.Value, context.CreateAttributePosition(conditionAttribute));
        while (Evaluate(context, conditionAttribute, expressions))
        {
            BadExecutionContext loopContext = new BadExecutionContext(
                context.ExecutionContext.Scope.CreateChild(
                    "bs:while",
                    context.ExecutionContext.Scope,
                    null,
                    BadScopeFlags.Breakable | BadScopeFlags.Continuable
                )
            );
            foreach (HtmlNode? child in context.InputNode.ChildNodes)
            {
                BadHtmlContext childContext = context.CreateChild(child, context.OutputNode, loopContext);

                Transform(childContext);
            }
        }
    }
}