using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript2.Common;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Reader;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

using HtmlAgilityPack;

namespace BadHtml;

public class BadHtmlContext
{
    public readonly BadExecutionContext ExecutionContext;
    public readonly string FilePath;
    public readonly HtmlNode InputNode;
    public readonly HtmlNode OutputNode;
    public readonly string Source;
    public readonly BadHtmlTemplateOptions Options;

    public BadHtmlContext(HtmlNode inputNode, HtmlNode outputNode, BadExecutionContext executionContext, string filePath, string source, BadHtmlTemplateOptions options)
    {
        InputNode = inputNode;
        OutputNode = outputNode;
        ExecutionContext = executionContext;
        FilePath = filePath;
        Source = source;
        Options = options;
    }

    public HtmlDocument InputDocument => InputNode.OwnerDocument;
    public HtmlDocument OutputDocument => OutputNode.OwnerDocument;

    public BadHtmlContext CreateChild(HtmlNode inputNode, HtmlNode outputNode, BadExecutionContext? executionContext = null)
    {
        return new BadHtmlContext(inputNode, outputNode, executionContext ?? ExecutionContext, FilePath, Source, Options);
    }

    public BadSourcePosition CreateAttributePosition(HtmlAttribute attribute)
    {
        return new BadSourcePosition(FilePath, Source, attribute.ValueStartIndex, attribute.Value.Length);
    }

    public BadSourcePosition CreateInnerPosition()
    {
        return new BadSourcePosition(FilePath, Source, InputNode.InnerStartIndex, InputNode.InnerLength);
    }

    public BadSourcePosition CreateOuterPosition()
    {
        return new BadSourcePosition(FilePath, Source, InputNode.InnerStartIndex, InputNode.InnerLength);
    }

    private IEnumerable<BadExpression> VisitAll(IEnumerable<BadExpression> expressions)
    {
        foreach (BadExpression expression in expressions)
        {
            foreach (BadExpression innerExpression in expression.GetDescendantsAndSelf())
            {
                yield return innerExpression;
            }
        }
    }

    public BadExpression[] Parse(string code, BadSourcePosition pos)
    {
        try
        {
            BadExpression[] expressions = BadSourceParser.Parse(FilePath, code).ToArray();
            foreach (BadExpression expression in VisitAll(expressions))
            {
                BadSourcePosition newPosition = BadSourcePosition.Create(FilePath, Source, pos.Index + expression.Position.Index, expression.Position.Length);
                expression.SetPosition(newPosition);
            }

            return expressions;
        }
        catch (BadSourceReaderException e)
        {
            if (e.Position == null)
            {
                throw new BadSourceReaderException(e.OriginalMessage, pos);
            }

            throw new BadSourceReaderException(e.OriginalMessage, BadSourcePosition.Create(FilePath, Source, e.Position.Index + pos.Index, e.Position.Length));
        }
    }

    public BadObject Execute(BadExpression[] expressions)
    {
        BadObject result = ExecutionContext.ExecuteScript(expressions);

        if (ExecutionContext.Scope.IsError)
        {
            throw new BadRuntimeErrorException(ExecutionContext.Scope.Error);
        }

        return result.Dereference();
    }
    public BadObject ParseAndExecute(string code, BadSourcePosition pos)
    {
        BadExpression[] expressions = Parse(code, pos);

        return Execute(expressions);
    }
}