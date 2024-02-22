using System.Collections.Generic;
using System.Linq;

using BadScript2.Common;
using BadScript2.IO;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Reader;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

using HtmlAgilityPack;

// ReSharper disable once InvalidXmlDocComment
///<summary>
///	A Html Template Generator based on BadScript2
/// </summary>
namespace BadHtml;

/// <summary>
///     Implements the Html Context for the Transformation Process
/// </summary>
public class BadHtmlContext
{
	/// <summary>
	///     The Execution Context that is used to evaluate badscript code
	/// </summary>
	public readonly BadExecutionContext ExecutionContext;

	/// <summary>
	///     The File Path of the Template
	/// </summary>
	public readonly string FilePath;

	/// <summary>
	///     The Current Input Node
	/// </summary>
	public readonly HtmlNode InputNode;

	/// <summary>
	///     The Html Template Options
	/// </summary>
	public readonly BadHtmlTemplateOptions Options;

	/// <summary>
	///     The Current Output Node
	/// </summary>
	public readonly HtmlNode OutputNode;

	/// <summary>
	///     The Source Code of the Template
	/// </summary>
	public readonly string Source;

	/// <summary>
	///     Constructs a new Html Context
	/// </summary>
	/// <param name="inputNode">The Input Node</param>
	/// <param name="outputNode">The Output Node</param>
	/// <param name="executionContext">The Execution Context</param>
	/// <param name="filePath">The File Path of the Template</param>
	/// <param name="source">The Source of the Template</param>
	/// <param name="options">The Html Template Options</param>
	/// <param name="fileSystem">The Filesystem of the Current Template Context</param>
	public BadHtmlContext(
        HtmlNode inputNode,
        HtmlNode outputNode,
        BadExecutionContext executionContext,
        string filePath,
        string source,
        BadHtmlTemplateOptions options,
        IFileSystem fileSystem)
    {
        InputNode = inputNode;
        OutputNode = outputNode;
        ExecutionContext = executionContext;
        FilePath = filePath;
        Source = source;
        Options = options;
        FileSystem = fileSystem;
    }

	/// <summary>
	///     The Filesystem of the Current Template Context
	/// </summary>
	public IFileSystem FileSystem { get; }

	/// <summary>
	///     The Input Document
	/// </summary>
	public HtmlDocument InputDocument => InputNode.OwnerDocument;

	/// <summary>
	///     The Output Document
	/// </summary>
	public HtmlDocument OutputDocument => OutputNode.OwnerDocument;

	/// <summary>
	///     Creates a child context with the specified input, output node and optional execution context
	/// </summary>
	/// <param name="inputNode">The Input Node</param>
	/// <param name="outputNode">The Output Node</param>
	/// <param name="executionContext">
	///     The Optional Execution Context. If not specified, the context will be inherited from
	///     this instance
	/// </param>
	/// <returns>Child Context</returns>
	public BadHtmlContext CreateChild(
        HtmlNode inputNode,
        HtmlNode outputNode,
        BadExecutionContext? executionContext = null)
    {
        return new BadHtmlContext(
            inputNode,
            outputNode,
            executionContext ?? ExecutionContext,
            FilePath,
            Source,
            Options,
            FileSystem
        );
    }

	/// <summary>
	///     Creates the Source Position of the specified Attribute
	/// </summary>
	/// <param name="attribute">The Attribute</param>
	/// <returns>Source Position</returns>
	public BadSourcePosition CreateAttributePosition(HtmlAttribute attribute)
    {
        return new BadSourcePosition(FilePath, Source, attribute.ValueStartIndex, attribute.Value.Length);
    }

	/// <summary>
	///     Creates the Source Position of the current Input Nodes Inner Content
	/// </summary>
	/// <returns>Source Position</returns>
	public BadSourcePosition CreateInnerPosition()
    {
        return new BadSourcePosition(FilePath, Source, InputNode.InnerStartIndex, InputNode.InnerLength);
    }

	/// <summary>
	///     Creates the Source Position of the current Input Nodes Outer Content
	/// </summary>
	/// <returns>Source Position</returns>
	public BadSourcePosition CreateOuterPosition()
    {
        return new BadSourcePosition(FilePath, Source, InputNode.InnerStartIndex, InputNode.InnerLength);
    }

	/// <summary>
	///     Returns an enumeration of all expressions in the specified expressions and their descendants
	/// </summary>
	/// <param name="expressions">The Expression Enumeration</param>
	/// <returns>Enumeration of all Expressions in the Tree</returns>
	private static IEnumerable<BadExpression> VisitAll(IEnumerable<BadExpression> expressions)
    {
        return expressions.SelectMany(expression => expression.GetDescendantsAndSelf());
    }

	/// <summary>
	///     Parses a single Expression from the specified code and returns it with its position set to the specified position
	/// </summary>
	/// <param name="code">The Bad Script Source Code</param>
	/// <param name="pos">The Source Position of the Code</param>
	/// <returns>Parsed Expression</returns>
	/// <exception cref="BadSourceReaderException">Gets raised if the Source Could not be parsed.</exception>
	public BadExpression ParseSingle(string code, BadSourcePosition pos)
    {
        try
        {
            BadSourceParser parser = BadSourceParser.Create(FilePath, code);
            BadExpression expression = parser.ParseExpression();

            foreach (BadExpression expr in expression.GetDescendantsAndSelf())
            {
                BadSourcePosition newPosition = BadSourcePosition.Create(
                    FilePath,
                    Source,
                    pos.Index + expr.Position.Index,
                    expr.Position.Length
                );
                expr.SetPosition(newPosition);
            }

            return expression;
        }
        catch (BadSourceReaderException e)
        {
            if (e.Position == null)
            {
                throw new BadSourceReaderException(e.OriginalMessage, pos);
            }

            throw new BadSourceReaderException(
                e.OriginalMessage,
                BadSourcePosition.Create(FilePath, Source, e.Position.Index + pos.Index, e.Position.Length)
            );
        }
    }

	/// <summary>
	///     Parses the specified code and returns the expressions with their positions set to the specified position
	/// </summary>
	/// <param name="code">The Bad Script Source Code</param>
	/// <param name="pos">The Source Position of the Code</param>
	/// <returns>Parsed Expressions</returns>
	/// <exception cref="BadSourceReaderException">Gets raised if the Source Could not be parsed.</exception>
	public BadExpression[] Parse(string code, BadSourcePosition pos)
    {
        try
        {
            BadExpression[] expressions = BadSourceParser.Parse(FilePath, code).ToArray();

            foreach (BadExpression expression in VisitAll(expressions))
            {
                BadSourcePosition newPosition = BadSourcePosition.Create(
                    FilePath,
                    Source,
                    pos.Index + expression.Position.Index,
                    expression.Position.Length
                );
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

            throw new BadSourceReaderException(
                e.OriginalMessage,
                BadSourcePosition.Create(FilePath, Source, e.Position.Index + pos.Index, e.Position.Length)
            );
        }
    }

	/// <summary>
	///     Executes the specified expressions
	/// </summary>
	/// <param name="expressions">The Expressions</param>
	/// <param name="position">The Source Position of the Expressions</param>
	/// <returns>The Result of the Execution</returns>
	/// <exception cref="BadRuntimeErrorException">Gets raised if the execution failed.</exception>
	public BadObject Execute(IEnumerable<BadExpression> expressions, BadSourcePosition position)
    {
        try
        {
            BadObject result = ExecutionContext.ExecuteScript(expressions);

            if (ExecutionContext.Scope.IsError)
            {
                throw new BadRuntimeErrorException(ExecutionContext.Scope.Error);
            }

            return result.Dereference();
        }
        catch (BadRuntimeException e)
        {
            throw new BadRuntimeException(e.Message, position);
        }
    }

	/// <summary>
	///     Executes the specified expression
	/// </summary>
	/// <param name="expression">The Expression</param>
	/// <param name="position">The Source Position of the Expression</param>
	/// <returns>The Result of the Execution</returns>
	/// <exception cref="BadRuntimeErrorException">Gets raised if the execution failed.</exception>
	/// <exception cref="BadRuntimeException">Gets raised if the execution failed.</exception>
	public BadObject Execute(BadExpression expression, BadSourcePosition position)
    {
        try
        {
            BadObject result = ExecutionContext.ExecuteScript(expression);

            if (ExecutionContext.Scope.IsError)
            {
                throw new BadRuntimeErrorException(ExecutionContext.Scope.Error);
            }

            return result.Dereference();
        }
        catch (BadRuntimeException e)
        {
            throw new BadRuntimeException(e.Message, position);
        }
    }

	/// <summary>
	///     Parses and executes the specified code
	/// </summary>
	/// <param name="code">The Bad Script Source Code</param>
	/// <param name="pos">The Source Position of the Code</param>
	/// <returns>The Result of the Execution</returns>
	public BadObject ParseAndExecute(string code, BadSourcePosition pos)
    {
        BadExpression[] expressions = Parse(code, pos);

        return Execute(expressions, pos);
    }

	/// <summary>
	///     Parses and executes the specified code and returns the result of the last expression
	/// </summary>
	/// <param name="code">The Bad Script Source Code</param>
	/// <param name="pos">The Source Position of the Code</param>
	/// <returns>The Result of the Execution</returns>
	public BadObject ParseAndExecuteSingle(string code, BadSourcePosition pos)
    {
        BadExpression expression = ParseSingle(code, pos);

        return Execute(expression, pos);
    }
}