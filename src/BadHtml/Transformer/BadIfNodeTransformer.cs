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
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "bs:if";
    }

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
            BadExecutionContext branchContext = new BadExecutionContext(
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

        {
            BadExecutionContext branchContext = new BadExecutionContext(
                context.ExecutionContext.Scope.CreateChild(
                    "bs:if",
                    context.ExecutionContext.Scope,
                    null
                )
            );

            foreach (HtmlNode? child in elseBranch)
            {
                BadHtmlContext childContext = context.CreateChild(child, context.OutputNode, branchContext);

                Transform(childContext);
            }
        }
    }
}