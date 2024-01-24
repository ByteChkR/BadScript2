using System.Collections.Generic;

using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

/// <summary>
///     Implements the 'bs:if' node transformer
/// </summary>
public class BadIfNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform" />
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "bs:if";
    }

    /// <summary>
    ///     Dissects the HTML content of the 'bs:if' node into a list of branches
    /// </summary>
    /// <param name="context">The Current HTML Context</param>
    /// <param name="node">The HTML Node to Dissect</param>
    /// <param name="elseBranch">The Else branch(if any is found)</param>
    /// <returns>List of Branches</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the Html Node does not have a valid shape</exception>
    private static List<(HtmlAttribute, IEnumerable<HtmlNode>)> DissectContent(
        BadHtmlContext context,
        HtmlNode node,
        out IEnumerable<HtmlNode>? elseBranch)
    {
        List<(HtmlAttribute, IEnumerable<HtmlNode>)> branches = new List<(HtmlAttribute, IEnumerable<HtmlNode>)>();
        List<HtmlNode> currentBranch = new List<HtmlNode>();
        HtmlAttribute? conditionAttribute = node.Attributes["test"];

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

        foreach (HtmlNode child in node.ChildNodes)
        {
            if (child.Name == "bs:else")
            {
                branches.Add((conditionAttribute!, currentBranch));

                currentBranch = new List<HtmlNode>();
                HtmlAttribute? nextCondition = child.Attributes["test"];

                if (conditionAttribute == null)
                {
                    if (nextCondition != null)
                    {
                        throw BadRuntimeException.Create(
                            context.ExecutionContext.Scope,
                            "Found bs:else node with attribute 'test' after bs:else node without 'test' attribute",
                            context.CreateAttributePosition(nextCondition)
                        );
                    }

                    throw BadRuntimeException.Create(
                        context.ExecutionContext.Scope,
                        "Found bs:else node after bs:else node without 'test' attribute",
                        context.CreateInnerPosition()
                    );
                }

                conditionAttribute = child.Attributes["test"];

                if (conditionAttribute == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(conditionAttribute.Value))
                {
                    throw BadRuntimeException.Create(
                        context.ExecutionContext.Scope,
                        "Empty 'test' attribute in 'bs:else' node",
                        context.CreateAttributePosition(conditionAttribute)
                    );
                }
            }
            else
            {
                currentBranch.Add(child);
            }
        }

        if (conditionAttribute == null)
        {
            elseBranch = currentBranch;
        }
        else
        {
            elseBranch = null;
            branches.Add((conditionAttribute, currentBranch));
        }

        return branches;
    }

    /// <summary>
    ///     Evaluates a condition attribute
    /// </summary>
    /// <param name="context">The Current HTML Context</param>
    /// <param name="conditionAttribute">The Condition Attribute</param>
    /// <returns>Result of the Condition</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the result is not of type 'bool'</exception>
    private static bool EvaluateCondition(BadHtmlContext context, HtmlAttribute conditionAttribute)
    {
        BadObject resultObj = context.ParseAndExecuteSingle(
            conditionAttribute.Value,
            context.CreateAttributePosition(conditionAttribute)
        );

        if (resultObj is not IBadBoolean result)
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Result of 'test' attribute in 'bs:if' node is not a boolean",
                context.CreateAttributePosition(conditionAttribute)
            );
        }

        return result.Value;
    }

    /// <inheritdoc cref="BadHtmlNodeTransformer.TransformNode" />
    protected override void TransformNode(BadHtmlContext context)
    {
        List<(HtmlAttribute, IEnumerable<HtmlNode>)> branches =
            DissectContent(context, context.InputNode, out IEnumerable<HtmlNode>? elseBranch);

        bool executedAny = false;

        foreach ((HtmlAttribute condition, IEnumerable<HtmlNode> block) branch in branches)
        {
            if (!EvaluateCondition(context, branch.condition))
            {
                continue;
            }

            executedAny = true;
            using BadExecutionContext branchContext = new BadExecutionContext(
                context.ExecutionContext.Scope.CreateChild(
                    "bs:if",
                    context.ExecutionContext.Scope,
                    null
                )
            );

            foreach (HtmlNode? child in branch.block)
            {
                BadHtmlContext childContext = context.CreateChild(child, context.OutputNode, branchContext);

                Transform(childContext);
            }
        }

        if (executedAny || elseBranch == null)
        {
            return;
        }

        using BadExecutionContext elseContext = new BadExecutionContext(
            context.ExecutionContext.Scope.CreateChild(
                "bs:if",
                context.ExecutionContext.Scope,
                null
            )
        );

        foreach (HtmlNode? child in elseBranch)
        {
            BadHtmlContext childContext = context.CreateChild(child, context.OutputNode, elseContext);

            Transform(childContext);
        }
    }
}