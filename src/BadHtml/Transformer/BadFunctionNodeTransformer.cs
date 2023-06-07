using System.Collections.Generic;
using System.Linq;

using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

public class BadFunctionNodeTransformer : BadHtmlNodeTransformer
{
    public override bool CanTransform(BadHtmlContext context)
    {
        return context.InputNode.Name == "bs:function";
    }

    private bool IsOptional(string value)
    {
        return value.EndsWith("?") || value.EndsWith("?!");
    }

    private bool IsNullChecked(string value)
    {
        return value.EndsWith("!") || value.EndsWith("!?");
    }

    private bool IsRestArgs(string value)
    {
        return value.EndsWith("*");
    }

    private BadExpression? GetParameterType(BadHtmlContext context, HtmlAttribute attribute)
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

        BadExpression[] expressions = context.Parse(name, context.CreateAttributePosition(attribute));
        if (expressions.Length != 1)
        {
            throw BadRuntimeException.Create(
                context.ExecutionContext.Scope,
                $"Invalid parameter type expression for parameter {attribute.Name} in 'bs:function' node",
                context.CreateAttributePosition(attribute)
            );
        }

        return expressions[0];
    }

    public override void TransformNode(BadHtmlContext context)
    {
        HtmlAttribute? nameAttribute = context.InputNode.Attributes["name"];
        if (nameAttribute == null)
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope, "Missing 'name' attribute in 'bs:function' node", context.CreateOuterPosition());
        }

        if (string.IsNullOrEmpty(nameAttribute.Value))
        {
            throw BadRuntimeException.Create(context.ExecutionContext.Scope, "Empty 'name' attribute in 'bs:function' node", context.CreateAttributePosition(nameAttribute));
        }

        IEnumerable<HtmlAttribute> parameterAttributes = context.InputNode.Attributes.Where(x => x.Name.StartsWith("param:"));

        BadFunctionParameter[] parameters = parameterAttributes.Select(
                x => new BadFunctionParameter(
                    x.Name.Remove(0, "param:".Length),
                    IsOptional(x.Value),
                    IsNullChecked(x.Value),
                    IsRestArgs(x.Value),
                    GetParameterType(context, x)
                )
            )
            .ToArray();

        BadInteropFunction func = new BadInteropFunction(nameAttribute.Value, (ctx, args) => InvokeFunction(nameAttribute.Value, context, parameters, ctx, args), parameters);
        context.ExecutionContext.Scope.DefineVariable(nameAttribute.Value, func, context.ExecutionContext.Scope, new BadPropertyInfo(func.GetPrototype(), true));
    }

    private BadObject InvokeFunction(string name, BadHtmlContext context, BadFunctionParameter[] parameters, BadExecutionContext caller, BadObject[] arguments)
    {
        BadExecutionContext ctx = new BadExecutionContext(
            context.ExecutionContext.Scope.CreateChild(
                ToString(),
                caller.Scope,
                null,
                BadScopeFlags.Returnable | BadScopeFlags.AllowThrow | BadScopeFlags.CaptureThrow
            )
        );

        HtmlDocument outputDocument = new HtmlDocument();
        BadFunction.ApplyParameters(BadFunction.GetHeader(name, parameters), parameters, ctx, arguments, context.CreateOuterPosition());
        foreach (HtmlNode? child in context.InputNode.ChildNodes)
        {
            BadHtmlContext childContext = new BadHtmlContext(child, outputDocument.DocumentNode, ctx, context.FilePath, context.Source, context.Options);
            Transform(childContext);
        }

        return outputDocument.DocumentNode.InnerHtml;
    }
}