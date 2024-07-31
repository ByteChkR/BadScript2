using System.Collections.Generic;
using System.Linq;

using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

/// <summary>
///     Implements a BadScript Function Node Transformer
/// </summary>
public class BadFunctionNodeTransformer : BadHtmlNodeTransformer
{
    /// <inheritdoc cref="BadHtmlNodeTransformer.CanTransform" />
    protected override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "bs:function";
    }

    /// <summary>
    ///     Returns true if the specified parameter is optional
    /// </summary>
    /// <param name="value">The Parameter Value</param>
    /// <returns>True if Optional</returns>
    private static bool IsOptional(string value)
    {
        return value.EndsWith("?") || value.EndsWith("?!");
    }

    /// <summary>
    ///     Returns true if the specified parameter is null checked
    /// </summary>
    /// <param name="value">The Parameter Value</param>
    /// <returns>True if null checked</returns>
    private static bool IsNullChecked(string value)
    {
        return value.EndsWith("!") || value.EndsWith("!?");
    }

    /// <summary>
    ///     Returns true if the specified parameter is the rest argument
    /// </summary>
    /// <param name="value">The Parameter Value</param>
    /// <returns>True if rest argument</returns>
    private static bool IsRestArgs(string value)
    {
        return value.EndsWith("*");
    }

    /// <summary>
    ///     Returns the Parameter Type for the specified attribute
    /// </summary>
    /// <param name="context">The Html Context</param>
    /// <param name="attribute">The Parameter Attribute</param>
    /// <returns>Expression that evaluates to a Bad Type</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the Parameter Type could not be parsed.</exception>
    private static BadExpression? GetParameterType(BadHtmlContext context, HtmlAttribute attribute)
    {
        string name = attribute.Value;

        while (name.EndsWith("*") || name.EndsWith("?") || name.EndsWith("!"))
        {
            name = name.Remove(name.Length - 1);
        }

        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        BadExpression[] expressions = context.Parse(name + ';', context.CreateAttributePosition(attribute));

        if (expressions.Length != 1)
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope,
                                             $"Invalid parameter type expression for parameter {attribute.Name} in 'bs:function' node",
                                             context.CreateAttributePosition(attribute)
                                            );
        }

        return expressions[0];
    }

    /// <inheritdoc cref="BadHtmlNodeTransformer.TransformNode" />
    protected override void TransformNode(BadHtmlContext context)
    {
        HtmlAttribute? nameAttribute = context.InputNode.Attributes["name"];

        if (nameAttribute == null)
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope,
                                             "Missing 'name' attribute in 'bs:function' node",
                                             context.CreateOuterPosition()
                                            );
        }

        if (string.IsNullOrEmpty(nameAttribute.Value))
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope,
                                             "Empty 'name' attribute in 'bs:function' node",
                                             context.CreateAttributePosition(nameAttribute)
                                            );
        }

        IEnumerable<HtmlAttribute> parameterAttributes =
            context.InputNode.Attributes.Where(x => x.Name.StartsWith("param:"));

        BadFunctionParameter[] parameters = parameterAttributes
                                            .Select(x => new BadFunctionParameter(x.Name.Remove(0, "param:".Length),
                                                         IsOptional(x.Value),
                                                         IsNullChecked(x.Value),
                                                         IsRestArgs(x.Value),
                                                         GetParameterType(context, x)
                                                        )
                                                   )
                                            .ToArray();

        BadInteropFunction func = new BadInteropFunction(nameAttribute.Value,
                                                         (ctx, args) =>
                                                             InvokeFunction(nameAttribute.Value,
                                                                            context,
                                                                            parameters,
                                                                            ctx,
                                                                            args
                                                                           ),
                                                         false,
                                                         BadNativeClassBuilder.GetNative("string"),
                                                         parameters
                                                        );

        context.ExecutionContext.Scope.DefineVariable(nameAttribute.Value,
                                                      func,
                                                      context.ExecutionContext.Scope,
                                                      new BadPropertyInfo(func.GetPrototype(), true)
                                                     );
    }

    /// <summary>
    ///     Invokes the specified function
    /// </summary>
    /// <param name="name">Function Name</param>
    /// <param name="context">Html Context</param>
    /// <param name="parameters">The Function Parameters</param>
    /// <param name="caller">The Caller Context</param>
    /// <param name="arguments">The Function Arguments</param>
    /// <returns>The Result of the Function Invocation</returns>
    private BadObject InvokeFunction(string name,
                                     BadHtmlContext context,
                                     BadFunctionParameter[] parameters,
                                     BadExecutionContext caller,
                                     BadObject[] arguments)
    {
        using BadExecutionContext ctx = new BadExecutionContext(context.ExecutionContext.Scope.CreateChild(ToString(),
                                                                     caller.Scope,
                                                                     null,
                                                                     BadScopeFlags.Returnable |
                                                                     BadScopeFlags.AllowThrow |
                                                                     BadScopeFlags.CaptureThrow
                                                                    )
                                                               );

        // ReSharper disable once UseObjectOrCollectionInitializer
        HtmlDocument outputDocument = new HtmlDocument();
        outputDocument.OptionUseIdAttribute = true;
        outputDocument.LoadHtml("");

        BadFunction.ApplyParameters(BadFunction.GetHeader(name, BadNativeClassBuilder.GetNative("string"), parameters),
                                    parameters,
                                    ctx,
                                    arguments,
                                    context.CreateOuterPosition()
                                   );

        foreach (HtmlNode? child in context.InputNode.ChildNodes)
        {
            BadHtmlContext childContext = new BadHtmlContext(child,
                                                             outputDocument.DocumentNode,
                                                             ctx,
                                                             context.FilePath,
                                                             context.Source,
                                                             context.Options,
                                                             context.FileSystem
                                                            );
            Transform(childContext);
        }

        return outputDocument.DocumentNode.InnerHtml;
    }
}