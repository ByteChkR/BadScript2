using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

using HtmlAgilityPack;
namespace BadHtml.Transformer;

/// <summary>
///     Implements the 'bs:while' node transformer
/// </summary>
public class BadWhileNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform" />
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "bs:while";
    }

    /// <summary>
    ///     Evaluates the Expressions inside the attribute
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attribute">HtmlAttribute</param>
    /// <param name="expressions">Parsed Expressions</param>
    /// <returns>The Result of the Execution</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the result is not of type IBadBoolean</exception>
    private static bool Evaluate(BadHtmlContext context, HtmlAttribute attribute, BadExpression[] expressions)
    {
        BadSourcePosition pos = context.CreateAttributePosition(attribute);
        BadObject resultObj = context.Execute(expressions, pos);

        if (resultObj is not IBadBoolean result)
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Result of 'test' attribute in 'bs:if' node is not a boolean",
                pos
            );
        }

        return result.Value;
    }

    /// <inheritdoc cref="BadHtmlNodeTransformer.TransformNode" />
    protected override void TransformNode(BadHtmlContext context)
    {
        HtmlAttribute? conditionAttribute = context.InputNode.Attributes["test"];

        if (conditionAttribute == null)
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Missing 'test' attribute in 'bs:if' node",
                context.CreateOuterPosition()
            );
        }

        if (string.IsNullOrEmpty(conditionAttribute.Value))
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Empty 'test' attribute in 'bs:if' node",
                context.CreateAttributePosition(conditionAttribute)
            );
        }

        BadExpression[] expressions =
            context.Parse(conditionAttribute.Value, context.CreateAttributePosition(conditionAttribute));

        while (Evaluate(context, conditionAttribute, expressions))
        {
            using BadExecutionContext loopContext = new BadExecutionContext(
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