using System.Collections;

using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

/// <summary>
///     Implements the 'bs:each' node transformer
/// </summary>
public class BadForEachNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform" />
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "bs:each";
    }

    /// <summary>
    ///     Runs an iteration of the 'bs:each' node
    /// </summary>
    /// <param name="context">The Html Context</param>
    /// <param name="name">The Variable name of the foreach block</param>
    /// <param name="obj">The Value of the current iteration</param>
    /// <param name="indexName">Name if the Index Variable(if null or empty, variable is not defined)</param>
    /// <param name="index">The current Iteration index</param>
    private static void RunIteration(BadHtmlContext context, string name, BadObject obj, string? indexName, int index)
    {
        using BadExecutionContext loopContext = new BadExecutionContext(
            context.ExecutionContext.Scope.CreateChild(
                "bs:while",
                context.ExecutionContext.Scope,
                null,
                BadScopeFlags.Breakable | BadScopeFlags.Continuable
            )
        );
        loopContext.Scope.DefineVariable(name, obj, context.ExecutionContext.Scope);
        if (!string.IsNullOrEmpty(indexName))
        {
            loopContext.Scope.DefineVariable(indexName!, index, context.ExecutionContext.Scope);
        }
        
        foreach (HtmlNode? child in context.InputNode.ChildNodes)
        {
            BadHtmlContext childContext = context.CreateChild(child, context.OutputNode, loopContext);

            Transform(childContext);
        }
    }

    /// <inheritdoc cref="BadHtmlNodeTransformer.TransformNode" />
    protected override void TransformNode(BadHtmlContext context)
    {
        HtmlAttribute? enumerationAttribute = context.InputNode.Attributes["on"];
        HtmlAttribute? itemAttribute = context.InputNode.Attributes["as"];
        HtmlAttribute? indexNameAttribute = context.InputNode.Attributes["index"];

        if (enumerationAttribute == null)
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Missing 'on' attribute in 'bs:each' node",
                context.CreateOuterPosition()
            );
        }

        if (itemAttribute == null)
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Missing 'as' attribute in 'bs:each' node",
                context.CreateOuterPosition()
            );
        }

        if (string.IsNullOrEmpty(enumerationAttribute.Value))
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Empty 'on' attribute in 'bs:each' node",
                context.CreateAttributePosition(enumerationAttribute)
            );
        }

        if (string.IsNullOrEmpty(itemAttribute.Value))
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                "Empty 'as' attribute in 'bs:each' node",
                context.CreateAttributePosition(itemAttribute)
            );
        }
        
        string? indexName = null;
        if (indexNameAttribute != null)
        {
            indexName = indexNameAttribute.Value;
        }

        BadObject enumeration = context.ParseAndExecuteSingle(
            enumerationAttribute.Value,
            context.CreateAttributePosition(enumerationAttribute)
        );

        switch (enumeration)
        {
            case IBadEnumerable badEnumerable:
            {
                int i = 0;
                foreach (BadObject o in badEnumerable)
                {
                    RunIteration(context, itemAttribute.Value, o, indexName, i);
                    i++;
                }

                break;
            }
            case IBadEnumerator badEnumerator:
            {
                int i = 0;
                while (badEnumerator.MoveNext())
                {
                    RunIteration(
                        context,
                        itemAttribute.Value,
                        badEnumerator.Current ??
                        throw BadRuntimeException.Create(
                            context.ExecutionContext.Scope,
                            "Enumerator returned null",
                            context.CreateAttributePosition(enumerationAttribute)
                        ),
                        indexName,
                        i
                    );
                    i++;
                }

                break;
            }
            case IEnumerable enumerable:
            {
                int i = 0;
                foreach (object? o in enumerable)
                {
                    RunIteration(context, itemAttribute.Value, BadObject.Wrap(o), indexName, i);
                    i++;
                }

                break;
            }
            case IEnumerator enumerator:
            {
                int i = 0;
                while (enumerator.MoveNext())
                {
                    RunIteration(
                        context,
                        itemAttribute.Value,
                        BadObject.Wrap(enumerator.Current),
                        indexName,
                        i
                    );
                    i++;
                }

                break;
            }
            default:
                throw BadRuntimeException.Create(
                    context.ExecutionContext.Scope,
                    "Result of 'on' attribute in 'bs:each' node is not enumerable",
                    context.CreateAttributePosition(enumerationAttribute)
                );
        }
    }
}