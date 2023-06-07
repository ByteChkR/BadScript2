using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Parser.Operators;
using BadScript2.Reader;
using BadScript2.Reader.Token;
using BadScript2.Reader.Token.Primitive;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Parser;

/// <summary>
///     The Parser of the Language.
///     It turns Source Code into an Expression Tree
/// </summary>
public class BadSourceParser
{
	/// <summary>
	///     The Operator Table that is used to parse the Source Code
	/// </summary>
	private readonly BadOperatorTable m_Operators;

    private BadMetaData? m_MetaData;

    /// <summary>
    ///     Constructor of the Parser
    /// </summary>
    /// <param name="sourceReader">The Source Reader</param>
    /// <param name="operators">The Operator Table that is used to parse the Source Code</param>
    public BadSourceParser(BadSourceReader sourceReader, BadOperatorTable operators)
    {
        Reader = sourceReader;
        m_Operators = operators;
    }

    /// <summary>
    ///     The Source Reader
    /// </summary>
    public BadSourceReader Reader { get; }

    /// <summary>
    ///     Creates a BadSourceParser Instance based on the source and filename provided
    /// </summary>
    /// <param name="fileName">File Name of the Source File</param>
    /// <param name="source">Contents of the Source File</param>
    /// <returns>BadSourceParser Instance</returns>
    public static BadSourceParser Create(string fileName, string source)
    {
        return new BadSourceParser(new BadSourceReader(fileName, source), BadOperatorTable.Instance);
    }

    public static BadSourceParser Create(string fileName, string source, int start, int end)
    {
        return new BadSourceParser(new BadSourceReader(fileName, source, start, end), BadOperatorTable.Instance);
    }

    public static IEnumerable<BadExpression> Parse(string fileName, string source)
    {
        return Create(fileName, source).Parse();
    }

    /// <summary>
    ///     Parses a Prefix Expression that has precedence greater than the provided precedence.
    ///     Moves the Reader to the next Token
    /// </summary>
    /// <param name="precedence">The Precedence</param>
    /// <returns>The Parsed Expression</returns>
    private BadExpression? ParsePrefix(int precedence)
    {
        // Parse Symbol
        BadSymbolToken? symbol = Reader.TryParseSymbols(m_Operators.UnaryPrefixSymbols);

        if (symbol == null)
        {
            return null;
        }

        BadUnaryPrefixOperator? op = m_Operators.FindUnaryPrefixOperator(symbol.Text);


        if (op == null || op.Precedence > precedence)
        {
            Reader.SetPosition(symbol.SourcePosition.Index);

            return null;
        }

        return op.Parse(this);
    }

    /// <summary>
    ///     Parses an If Expression. Moves the Reader to the Next Token
    /// </summary>
    /// <returns>BadIfExpression Instance</returns>
    private BadExpression ParseIf()
    {
        int start = Reader.CurrentIndex;

        Dictionary<BadExpression, BadExpression[]> conditionMap = new Dictionary<BadExpression, BadExpression[]>();

        bool readNext;
        bool isElse;

        do
        {
            readNext = false;
            isElse = false;
            Reader.Eat(BadStaticKeys.IfKey);
            Reader.SkipNonToken();
            Reader.Eat('(');
            Reader.SkipNonToken();
            BadExpression condition = ParseExpression();
            Reader.SkipNonToken();
            Reader.Eat(')');
            Reader.SkipNonToken();
            List<BadExpression> block = ParseBlock(start, out bool _);
            Reader.SkipNonToken();
            conditionMap[condition] = block.ToArray();

            if (Reader.Is(BadStaticKeys.ElseKey))
            {
                Reader.Eat(BadStaticKeys.ElseKey);
                isElse = true;
                Reader.SkipNonToken();
                readNext = Reader.Is(BadStaticKeys.IfKey);
            }
        }
        while (readNext);

        BadExpression[]? elseBlock = null;

        if (isElse)
        {
            elseBlock = ParseBlock(start, out bool _).ToArray();
        }

        return new BadIfExpression(
            conditionMap,
            elseBlock,
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
        );
    }

    /// <summary>
    ///     Parses a For Each Expression. Moves the Reader to the Next Token
    /// </summary>
    /// <returns>Instance of BadForEachExpression</returns>
    private BadExpression ParseForEach()
    {
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.ForEachKey);
        Reader.SkipNonToken();
        Reader.Eat('(');
        Reader.SkipNonToken();
        BadWordToken variableName = Reader.ParseWord();
        Reader.SkipNonToken();
        Reader.Eat("in");
        Reader.SkipNonToken();
        BadExpression collection = ParseExpression();
        Reader.SkipNonToken();
        Reader.Eat(')');
        Reader.SkipNonToken();
        List<BadExpression> block = ParseBlock(start, out bool _);
        Reader.SkipNonToken();

        return new BadForEachExpression(
            collection,
            variableName,
            block.ToArray(),
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
        );
    }


    /// <summary>
    ///     Parses a Lock Expression. Moves the Reader to the Next Token
    /// </summary>
    /// <returns>Instance of BadLockExpression</returns>
    private BadExpression ParseLock()
    {
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.LockKey);
        Reader.SkipNonToken();
        Reader.Eat('(');
        Reader.SkipNonToken();
        BadExpression collection = ParseExpression();
        Reader.SkipNonToken();
        Reader.Eat(')');
        Reader.SkipNonToken();
        List<BadExpression> block = ParseBlock(start, out bool _);
        Reader.SkipNonToken();

        return new BadLockExpression(
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start),
            collection,
            block.ToArray()
        );
    }

    /// <summary>
    ///     Parses a For Loop Expression. Moves the Reader to the Next Token
    /// </summary>
    /// <returns>Instance of BadForExpression</returns>
    private BadExpression ParseFor()
    {
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.ForKey);
        Reader.SkipNonToken();
        Reader.Eat('(');
        Reader.SkipNonToken();
        BadExpression vDef = ParseExpression();
        Reader.SkipNonToken();
        Reader.Eat(BadStaticKeys.StatementEndKey);
        Reader.SkipNonToken();
        BadExpression condition = ParseExpression();
        Reader.SkipNonToken();
        Reader.Eat(BadStaticKeys.StatementEndKey);
        Reader.SkipNonToken();
        BadExpression vInc = ParseExpression();
        Reader.SkipNonToken();
        Reader.Eat(')');
        Reader.SkipNonToken();
        List<BadExpression> block = ParseBlock(start, out bool _);
        Reader.SkipNonToken();

        return new BadForExpression(
            vDef,
            condition,
            vInc,
            block.ToArray(),
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
        );
    }

    private void ParseMeta()
    {
        if (Reader.Is("@|"))
        {
            Reader.Eat("@|");
            Reader.SkipNonToken();
            StringBuilder rootMeta = new StringBuilder();
            StringBuilder returnMeta = new StringBuilder();
            string returnType = "any";
            Dictionary<string, (string, StringBuilder)> meta = new Dictionary<string, (string, StringBuilder)>();

            StringBuilder GetMeta(string name, string type)
            {
                if (meta.TryGetValue(name, out (string, StringBuilder) val))
                {
                    return val.Item2;
                }

                val = (type, new StringBuilder());
                meta[name] = val;

                return val.Item2;
            }

            while (!Reader.Is("|@"))
            {
                if (Reader.Is("|PARAM"))
                {
                    Reader.Eat("|PARAM");
                    Reader.SkipNonToken();
                    string name = Reader.ParseWord().Text;
                    Reader.SkipNonToken();
                    string type = "any";

                    if (!Reader.Is(':'))
                    {
                        type = "";

                        while (!Reader.IsEof() && (Reader.IsWordStart() || Reader.Is('.')))
                        {
                            type += Reader.ParseWord().Text;

                            if (Reader.Is('.'))
                            {
                                type += '.';
                                Reader.MoveNext();
                            }
                        }

                        Reader.SkipNonToken();
                    }

                    Reader.Eat(':');
                    Reader.SkipNonToken();
                    StringBuilder m = GetMeta(name, type);

                    while (!Reader.IsEof() && !Reader.Is("|@") && !Reader.IsNewLine())
                    {
                        m.Append(Reader.GetCurrentChar());
                        Reader.MoveNext();
                    }

                    Reader.SkipNonToken();
                }
                else if (Reader.Is("|RET"))
                {
                    Reader.Eat("|RET");
                    Reader.SkipNonToken();
                    string type = "any";

                    if (!Reader.Is(':'))
                    {
                        type = "";

                        while (!Reader.IsEof() && (Reader.IsWordStart() || Reader.Is('.')))
                        {
                            type += Reader.ParseWord().Text;

                            if (Reader.Is('.'))
                            {
                                type += '.';
                                Reader.MoveNext();
                            }
                        }

                        Reader.SkipNonToken();
                    }

                    Reader.Eat(':');
                    Reader.SkipNonToken();

                    while (!Reader.IsEof() && !Reader.Is("|@") && !Reader.IsNewLine())
                    {
                        returnMeta.Append(Reader.GetCurrentChar());
                        Reader.MoveNext();
                    }

                    Reader.SkipNonToken();

                    returnType = type;
                }
                else
                {
                    if (Reader.IsNewLine(-1))
                    {
                        Reader.SkipNonToken();

                        if (Reader.Is("|"))
                        {
                            continue;
                        }
                    }

                    while (!Reader.IsEof() && !Reader.IsNewLine() && !Reader.Is("|@"))
                    {
                        rootMeta.Append(Reader.GetCurrentChar());
                        Reader.MoveNext();
                    }

                    rootMeta.AppendLine();
                    Reader.SkipNonToken();
                }
            }

            Reader.Eat("|@");
            Reader.SkipNonToken();

            m_MetaData = new BadMetaData(
                rootMeta.ToString().Trim(),
                returnMeta.ToString().Trim(),
                returnType,
                meta.ToDictionary(
                    x => x.Key,
                    x => new BadParameterMetaData(
                        x.Value.Item1,
                        x.Value.Item2.ToString().Trim()
                    )
                )
            );
        }
    }

    /// <summary>
    ///     Parses a Value Expression or a Prefix Function with precedence greater than the provided precedence. Moves the
    ///     Reader to the Next Token
    /// </summary>
    /// <param name="precedence">The Precedence</param>
    /// <returns>Value Expression or Operator Prefix Expression</returns>
    /// <exception cref="BadRuntimeException">Gets raised if a Variable Expression is malformed.</exception>
    private BadExpression ParseValue(int precedence)
    {
        Reader.SkipNonToken();

        ParseMeta();

        Reader.SkipNonToken();

        BadExpression? prefixExpr = ParsePrefix(precedence);

        if (prefixExpr != null)
        {
            return prefixExpr;
        }

        if (Reader.Is('('))
        {
            int start = Reader.CurrentIndex;

            try
            {
                List<BadFunctionParameter> p = ParseParameters(start);

                Reader.SkipNonToken();

                if (!Reader.Is("=>"))
                {
                    Reader.SetPosition(start);
                }
                else
                {
                    return ParseFunction(start, null, null, false, BadFunctionCompileLevel.None, p);
                }
            }
            catch (Exception)
            {
                Reader.SetPosition(start);
            }
        }

        if (Reader.Is('('))
        {
            Reader.Eat('(');
            BadExpression expr = ParseExpression();
            Reader.Eat(')');

            return expr;
        }

        if (Reader.Is('['))
        {
            int start = Reader.CurrentIndex;
            Reader.Eat('[');
            Reader.SkipNonToken();
            List<BadExpression> initExpressions = new List<BadExpression>();

            if (!Reader.Is(']'))
            {
                bool parseNext;

                do
                {
                    parseNext = false;

                    Reader.SkipNonToken();
                    initExpressions.Add(ParseExpression());

                    Reader.SkipNonToken();

                    if (Reader.Is(','))
                    {
                        Reader.Eat(',');
                        Reader.SkipNonToken();
                        parseNext = true;
                    }
                }
                while (parseNext);
            }

            Reader.SkipNonToken();
            Reader.Eat(']');

            return new BadArrayExpression(
                initExpressions.ToArray(),
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        if (Reader.Is('{'))
        {
            int start = Reader.CurrentIndex;
            Reader.Eat('{');
            Dictionary<BadWordToken, BadExpression> table = new Dictionary<BadWordToken, BadExpression>();
            Reader.SkipNonToken();

            if (!Reader.Is('}'))
            {
                bool parseNext;

                do
                {
                    parseNext = false;
                    Reader.SkipNonToken();
                    BadWordToken key = Reader.ParseWord();
                    Reader.SkipNonToken();
                    Reader.Eat(':');
                    Reader.SkipNonToken();
                    BadExpression value = ParseExpression();
                    table[key] = value;
                    Reader.SkipNonToken();

                    if (Reader.Is(','))
                    {
                        parseNext = true;
                        Reader.Eat(',');
                    }
                }
                while (parseNext);
            }

            Reader.Eat('}');

            return new BadTableExpression(
                table,
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        if (Reader.Is(BadStaticKeys.LockKey))
        {
            return ParseLock();
        }

        if (Reader.Is(BadStaticKeys.ForEachKey))
        {
            return ParseForEach();
        }

        if (Reader.Is(BadStaticKeys.ForKey))
        {
            return ParseFor();
        }

        if (Reader.Is(BadStaticKeys.IfKey))
        {
            return ParseIf();
        }

        if (Reader.Is(BadStaticKeys.ContinueKey))
        {
            BadSourcePosition pos = Reader.Eat(BadStaticKeys.ContinueKey);

            if (Reader.IsWordChar())
            {
                Reader.SetPosition(pos.Index);
            }
            else
            {
                Reader.SkipNonToken();

                return new BadContinueExpression(pos);
            }
        }

        if (Reader.Is(BadStaticKeys.BreakKey))
        {
            BadSourcePosition pos = Reader.Eat(BadStaticKeys.BreakKey);

            if (Reader.IsWordChar())
            {
                Reader.SetPosition(pos.Index);
            }
            else
            {
                Reader.SkipNonToken();

                return new BadBreakExpression(pos);
            }
        }

        if (Reader.Is(BadStaticKeys.ThrowKey))
        {
            BadSourcePosition pos = Reader.Eat(BadStaticKeys.ThrowKey);

            if (Reader.IsWordChar())
            {
                Reader.SetPosition(pos.Index);
            }
            else
            {
                return new BadThrowExpression(ParseExpression(), pos);
            }
        }

        if (Reader.Is(BadStaticKeys.ReturnKey))
        {
            BadSourcePosition pos = Reader.Eat(BadStaticKeys.ReturnKey);

            if (Reader.IsWordChar())
            {
                Reader.SetPosition(pos.Index);
            }
            else
            {
                Reader.SkipNonToken();
                bool isRef = false;

                if (Reader.Is(';'))
                {
                    return new BadReturnExpression(null, pos, false);
                }

                if (Reader.Is(BadStaticKeys.RefKey))
                {
                    isRef = true;
                    Reader.Eat(BadStaticKeys.RefKey);
                    Reader.SkipNonToken();
                }

                BadExpression expr = ParseExpression();

                return new BadReturnExpression(expr, pos, isRef);
            }
        }

        if (Reader.Is(BadStaticKeys.True) ||
            Reader.Is(BadStaticKeys.False))
        {
            BadBooleanToken token = Reader.ParseBoolean();

            if (Reader.IsWordChar())
            {
                Reader.SetPosition(token.SourcePosition.Index);
            }
            else
            {
                return new BadBooleanExpression(bool.Parse(token.Text), token.SourcePosition);
            }
        }

        bool isConstant = false;
        BadFunctionCompileLevel compileLevel = BadFunctionCompileLevel.None;
        int constStart = Reader.CurrentIndex;

        if (Reader.Is(BadStaticKeys.ConstantDefinitionKey))
        {
            isConstant = true;
            Reader.Eat(BadStaticKeys.ConstantDefinitionKey);
            Reader.SkipNonToken();
        }

        int compiledStart = Reader.CurrentIndex;

        if (Reader.Is(BadStaticKeys.CompiledDefinitionKey))
        {
            compileLevel = BadFunctionCompileLevel.Compiled;
            Reader.Eat(BadStaticKeys.CompiledDefinitionKey);
            Reader.SkipNonToken();

            if (Reader.Is(BadStaticKeys.CompiledFastDefinitionKey))
            {
                compileLevel = BadFunctionCompileLevel.CompiledFast;
                Reader.Eat(BadStaticKeys.CompiledFastDefinitionKey);
                Reader.SkipNonToken();
            }
        }

        if (Reader.Is(BadStaticKeys.FunctionKey))
        {
            return ParseFunction(isConstant, compileLevel);
        }

        if (compileLevel != BadFunctionCompileLevel.None || isConstant)
        {
            if (compiledStart < constStart)
            {
                Reader.SetPosition(compiledStart);
            }
            else
            {
                Reader.SetPosition(constStart);
            }
        }

        if (Reader.Is(BadStaticKeys.ClassKey) && Reader.IsWhiteSpace(BadStaticKeys.ClassKey.Length))
        {
            return ParseClass();
        }

        if (Reader.Is(BadStaticKeys.NewKey) && Reader.IsWhiteSpace(BadStaticKeys.NewKey.Length))
        {
            return ParseNew();
        }

        if (Reader.Is(BadStaticKeys.TryKey))
        {
            return ParseTry();
        }

        if (Reader.Is(BadStaticKeys.While))
        {
            return ParseWhile();
        }

        if (Reader.Is(BadStaticKeys.Null))
        {
            BadNullToken token = Reader.ParseNull();

            if (Reader.IsWordChar())
            {
                Reader.SetPosition(token.SourcePosition.Index);
            }
            else
            {
                return new BadNullExpression(token.SourcePosition);
            }
        }

        if (Reader.IsStringQuote())
        {
            BadStringToken token = Reader.ParseString();

            return new BadStringExpression(token.Value, token.SourcePosition);
        }

        if (Reader.Is(BadStaticKeys.FormatStringKey))
        {
            return ParseFormatString();
        }

        if (Reader.IsNumberStart())
        {
            BadNumberToken token = Reader.ParseNumber();

            return new BadNumberExpression(
                decimal.Parse(token.Text, NumberFormatInfo.InvariantInfo),
                token.SourcePosition
            );
        }

        BadValueParser? valueParser = m_Operators.GetValueParser(this);

        if (valueParser != null)
        {
            return valueParser.ParseValue(this);
        }

        Reader.SkipNonToken();

        int wordStart = Reader.CurrentIndex;
        BadWordToken word = Reader.ParseWord();

        if (word.Text == BadStaticKeys.VariableDefinitionKey ||
            word.Text == BadStaticKeys.ConstantDefinitionKey)
        {
            bool isReadOnly = word.Text == BadStaticKeys.ConstantDefinitionKey;
            Reader.SkipNonToken();
            BadExpression nameExpr = ParseValue(0);
            Reader.SkipNonToken();

            while (Reader.Is("."))
            {
                Reader.Eat(".");
                BadWordToken right = Reader.ParseWord();
                nameExpr = new BadMemberAccessExpression(
                    nameExpr,
                    right,
                    nameExpr.Position.Combine(right.SourcePosition)
                );
                Reader.SkipNonToken();
            }

            BadWordToken name;
            BadExpression? type = null;
            Reader.SkipNonToken();

            if (Reader.IsWordStart())
            {
                type = nameExpr;
                name = Reader.ParseWord();
            }
            else
            {
                if (nameExpr is not BadVariableExpression expr)
                {
                    throw new BadRuntimeException("Expected variable name", nameExpr.Position);
                }

                name = expr.Name;
            }

            Reader.SkipNonToken();

            return new BadVariableDefinitionExpression(name.Text, word.SourcePosition, type, isReadOnly);
        }

        Reader.SkipNonToken();

        if (precedence > 0 && (Reader.Is("=>") || Reader.Is('*') || Reader.Is('?') || Reader.Is('!')))
        {
            int start = Reader.CurrentIndex;

            while (Reader.Is('*') || Reader.Is('?') || Reader.Is('!'))
            {
                Reader.MoveNext();
            }

            Reader.SkipNonToken();

            if (!Reader.Is("=>"))
            {
                Reader.SetPosition(start);
            }
            else
            {
                Reader.SetPosition(start);
                BadFunctionParameter p;

                if (Reader.Is("=>"))
                {
                    p = new BadFunctionParameter(word.Text, false, false, false);
                }
                else
                {
                    Reader.SetPosition(wordStart);
                    p = ParseParameter();
                }

                Reader.SkipNonToken();

                if (!Reader.Is("=>"))
                {
                    Reader.SetPosition(start);
                }
                else
                {
                    return ParseFunction(
                        start,
                        null,
                        null,
                        false,
                        BadFunctionCompileLevel.None,
                        new List<BadFunctionParameter>
                        {
                            p,
                        }
                    );
                }
            }
        }

        return new BadVariableExpression(word.Text, word.SourcePosition);
    }

    /// <summary>
    ///     Parses an Expression with a precedence greater than the given precedence. Moves the reader to the next token.
    /// </summary>
    /// <param name="left">The (optional) Left side of the expression</param>
    /// <param name="precedence">The Minimum Precedence</param>
    /// <returns>Parsed Expression</returns>
    public BadExpression ParseExpression(BadExpression? left = null, int precedence = int.MaxValue)
    {
        left ??= ParseValue(precedence);

        while (!Reader.IsEof())
        {
            Reader.SkipNonToken();

            if (Reader.Is(BadStaticKeys.StatementEndKey))
            {
                return left;
            }


            if (Reader.Is('[') || Reader.Is("?["))
            {
                bool isNullChecked = Reader.Is("?[");

                if (isNullChecked)
                {
                    Reader.Eat("?[");
                }
                else
                {
                    Reader.Eat('[');
                }

                Reader.SkipNonToken();
                bool isReverse = false;

                if (Reader.Is('^'))
                {
                    Reader.Eat('^');
                    isReverse = true;
                }

                List<BadExpression> indices = new List<BadExpression>();

                if (!Reader.Is(']'))
                {
                    bool readNext;

                    do
                    {
                        readNext = false;
                        Reader.SkipNonToken();
                        indices.Add(ParseExpression());

                        Reader.SkipNonToken();

                        if (Reader.Is(','))
                        {
                            readNext = true;
                            Reader.Eat(',');
                        }

                        Reader.SkipNonToken();
                    }
                    while (readNext);
                }

                BadSourcePosition end = Reader.Eat(']');
                Reader.SkipNonToken();

                if (isReverse)
                {
                    left = new BadArrayAccessReverseExpression(
                        left,
                        indices.ToArray(),
                        left.Position.Combine(end),
                        isNullChecked
                    );
                }
                else
                {
                    left = new BadArrayAccessExpression(
                        left,
                        indices.ToArray(),
                        left.Position.Combine(end),
                        isNullChecked
                    );
                }

                continue;
            }

            if (Reader.Is('('))
            {
                Reader.Eat('(');
                Reader.SkipNonToken();
                List<BadExpression> args = new List<BadExpression>();

                if (!Reader.Is(')'))
                {
                    bool readNext;

                    do
                    {
                        readNext = false;
                        Reader.SkipNonToken();
                        args.Add(ParseExpression());

                        Reader.SkipNonToken();

                        if (Reader.Is(','))
                        {
                            readNext = true;
                            Reader.Eat(',');
                        }

                        Reader.SkipNonToken();
                    }
                    while (readNext);
                }

                BadSourcePosition rightPos = Reader.MakeSourcePosition(1);
                Reader.Eat(')');
                left = new BadInvocationExpression(left, args.ToArray(), left.Position.Combine(rightPos));
                Reader.SkipNonToken();

                continue;
            }

            // Parse Symbol
            BadSymbolToken? symbol = Reader.TryParseSymbols(m_Operators.BinarySymbols);

            if (symbol == null)
            {
                return left;
            }

            BadBinaryOperator? op = m_Operators.FindBinaryOperator(symbol.Text);


            if (op == null || op.Precedence > precedence)
            {
                Reader.SetPosition(symbol.SourcePosition.Index);

                return left;
            }

            left = op.Parse(left, this);
        }

        return left;
    }


    /// <summary>
    ///     Parses a Format Expression. Moves the reader to the next token.
    /// </summary>
    /// <returns>
    ///     Instance of BadStringExpression if no Format Parameters are found. Instance of BadFormattedStringExpression
    ///     otherwise.
    /// </returns>
    /// <exception cref="BadSourceReaderException">
    ///     Gets Raised if the First Character sequence is not
    ///     BadStaticKeys.FormatStringKey
    /// </exception>
    /// <exception cref="BadParserException">Gets Raised if the string is not properly Terminated.</exception>
    private BadStringExpression ParseFormatString()
    {
        if (!Reader.Is(BadStaticKeys.FormatStringKey))
        {
            throw new BadSourceReaderException(
                $"Expected string start character but got '{(Reader.IsEof() ? "EOF" : Reader.GetCurrentChar())}'",
                Reader.MakeSourcePosition(1)
            );
        }

        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.FormatStringKey);
        bool isEscaped = false;
        StringBuilder sb = new StringBuilder();
        List<BadExpression> args = new List<BadExpression>();

        while (!Reader.IsStringQuote())
        {
            if (Reader.IsNewLine() || Reader.IsEof())
            {
                throw new BadSourceReaderException(
                    "String not terminated",
                    Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
                );
            }

            if (Reader.Is('{'))
            {
                Reader.Eat('{');
                Reader.SkipNonToken();

                if (!Reader.Is('{'))
                {
                    BadExpression expr = ParseExpression();
                    Reader.SkipNonToken();
                    Reader.Eat('}');
                    sb.Append($"{{{args.Count}}}");
                    args.Add(expr);

                    continue;
                }

                Reader.Eat('{');
                sb.Append("{{");

                continue;
            }

            if (Reader.Is('}'))
            {
                int idx = Reader.CurrentIndex;
                Reader.Eat('}');

                if (Reader.Is('}'))
                {
                    Reader.Eat('}');
                    sb.Append("}}");

                    continue;
                }

                throw new BadParserException(
                    "Expected '}'",
                    Reader.MakeSourcePosition(idx, 1)
                );
            }

            if (Reader.Is(BadStaticKeys.EscapeCharacter))
            {
                isEscaped = true;
                Reader.MoveNext();
            }

            if (isEscaped)
            {
                isEscaped = false;
                sb.Append(Regex.Unescape($"\\{Reader.GetCurrentChar()}"));
            }
            else
            {
                sb.Append(Reader.GetCurrentChar());
            }

            Reader.MoveNext();
        }

        Reader.Eat(BadStaticKeys.Quote);

        if (args.Count == 0)
        {
            return new BadStringExpression(
                '"' + sb.ToString() + '"',
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        return new BadFormattedStringExpression(
            args.ToArray(),
            sb.ToString(),
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
        );
    }

    /// <summary>
    ///     Parses a While Loop. Moves the reader to the next token.
    /// </summary>
    /// <returns>Instance of BadWhileExpression</returns>
    private BadWhileExpression ParseWhile()
    {
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.While);
        Reader.SkipNonToken();
        Reader.Eat('(');
        BadExpression condition = ParseExpression();
        Reader.Eat(')');

        List<BadExpression> block = ParseBlock(start, out bool _);

        return new BadWhileExpression(
            condition,
            block,
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
        );
    }

    /// <summary>
    ///     Parses a Block. Moves the reader to the next token.
    /// </summary>
    /// <param name="start">The Start index of the loop</param>
    /// <param name="isSingleLine">Indicates if the Block is in single line format.</param>
    /// <returns>List of Parsed Expressions</returns>
    /// <exception cref="BadParserException">
    ///     Gets Raised if there is no block start character and no single-line block start
    ///     sequence.
    /// </exception>
    private List<BadExpression> ParseBlock(int start, out bool isSingleLine)
    {
        Reader.SkipNonToken();

        List<BadExpression> block = new List<BadExpression>();

        if (Reader.Is("=>"))
        {
            Reader.Eat("=>");
            block.Add(ParseExpression());
            isSingleLine = true;
        }
        else if (Reader.Is('{'))
        {
            isSingleLine = false;
            Reader.Eat('{');
            Reader.SkipNonToken();

            while (!Reader.Is(BadStaticKeys.BlockEndKey))
            {
                Reader.SkipNonToken();
                block.Add(ParseExpression());
                Reader.SkipNonToken();

                if (!Reader.Last(BadStaticKeys.BlockEndKey) && Reader.Is(BadStaticKeys.StatementEndKey))
                {
                    Reader.Eat(BadStaticKeys.StatementEndKey);
                }

                Reader.SkipNonToken();
            }

            Reader.Eat(BadStaticKeys.BlockEndKey);
        }
        else
        {
            throw new BadParserException(
                "Expected Expression Body",
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        Reader.SkipNonToken();

        return block;
    }

    /// <summary>
    ///     Parses a Try Catch Block. Moves the reader to the next token.
    /// </summary>
    /// <returns>Instance of BadTryCatchExpression</returns>
    private BadExpression ParseTry()
    {
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.TryKey);
        Reader.SkipNonToken();
        List<BadExpression> block = ParseBlock(start, out bool isSingleLine);
        Reader.SkipNonToken();

        if (isSingleLine)
        {
            Reader.Eat(BadStaticKeys.StatementEndKey);
        }

        Reader.SkipNonToken();
        Reader.Eat(BadStaticKeys.CatchKey);
        Reader.SkipNonToken();
        Reader.Eat('(');
        Reader.SkipNonToken();
        BadWordToken errorName = Reader.ParseWord();
        Reader.SkipNonToken();
        Reader.Eat(')');
        Reader.SkipNonToken();
        List<BadExpression> errorBlock = ParseBlock(start, out isSingleLine);
        Reader.SkipNonToken();

        if (isSingleLine)
        {
            Reader.Eat(BadStaticKeys.StatementEndKey);
        }

        Reader.SkipNonToken();

        return new BadTryCatchExpression(
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start),
            block.ToArray(),
            errorBlock.ToArray(),
            errorName.Text
        );
    }


    /// <summary>
    ///     Parses a New Expression. Moves the reader to the next token.
    /// </summary>
    /// <returns>Instance of BadNewExpression</returns>
    /// <exception cref="BadParserException">Gets Raised if the Expression after the new key is not an invocation expression.</exception>
    private BadNewExpression ParseNew()
    {
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.NewKey);
        Reader.SkipNonToken();
        BadExpression right = ParseExpression();

        if (right is not BadInvocationExpression invoc)
        {
            throw new BadParserException("Expected Invocation Expression", right.Position);
        }

        return new BadNewExpression(invoc, Reader.MakeSourcePosition(start, Reader.CurrentIndex - start));
    }

    /// <summary>
    ///     Parses a Class Structure. Moves the reader to the next token.
    /// </summary>
    /// <returns>Instance of a BadClassPrototypeExpression</returns>
    private BadClassPrototypeExpression ParseClass()
    {
        BadMetaData? meta = m_MetaData;
        m_MetaData = null;
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.ClassKey);
        Reader.SkipNonToken();
        BadWordToken name = Reader.ParseWord();
        Reader.SkipNonToken();
        BadExpression? baseClass = null;

        if (Reader.Is(':'))
        {
            Reader.Eat(':');
            Reader.SkipNonToken();
            baseClass = ParseExpression();
            Reader.SkipNonToken();
        }

        Reader.Eat('{');
        Reader.SkipNonToken();
        List<BadExpression> members = new List<BadExpression>();

        while (!Reader.Is('}'))
        {
            Reader.SkipNonToken();
            members.Add(ParseExpression());
            Reader.SkipNonToken();

            if (!Reader.Last('}') && Reader.Is(BadStaticKeys.StatementEndKey))
            {
                Reader.Eat(BadStaticKeys.StatementEndKey);
            }

            Reader.SkipNonToken();
        }

        Reader.Eat('}');
        Reader.SkipNonToken();

        return new BadClassPrototypeExpression(
            name.Text,
            members.ToArray(),
            baseClass,
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start),
            meta
        );
    }

    private BadFunctionParameter ParseParameter()
    {
        BadExpression nameExpr = ParseValue(0);
        Reader.SkipNonToken();

        while (Reader.Is("."))
        {
            Reader.Eat(".");
            BadWordToken right = Reader.ParseWord();
            nameExpr = new BadMemberAccessExpression(
                nameExpr,
                right,
                nameExpr.Position.Combine(right.SourcePosition)
            );
            Reader.SkipNonToken();
        }

        Reader.SkipNonToken();
        string name;
        BadExpression? typeExpr = null;
        Reader.SkipNonToken();

        if (Reader.IsWordStart())
        {
            name = Reader.ParseWord().Text;
            typeExpr = nameExpr;
        }
        else
        {
            if (nameExpr is not BadVariableExpression expr)
            {
                throw new BadParserException(
                    "Expected Variable Expression",
                    nameExpr.Position
                );
            }

            name = expr.Name;
        }


        bool isOptional = false;
        bool isNullChecked = false;
        bool isRestArgs = false;

        if (Reader.Is('*'))
        {
            isRestArgs = true;
            Reader.Eat('*');
            Reader.SkipNonToken();
        }
        else
        {
            if (Reader.Is('?'))
            {
                isOptional = true;
                Reader.Eat('?');
                Reader.SkipNonToken();

                if (Reader.Is('!'))
                {
                    isNullChecked = true;
                    Reader.Eat('!');
                    Reader.SkipNonToken();
                }
            }
            else if (Reader.Is('!'))
            {
                isNullChecked = true;
                Reader.Eat('!');
                Reader.SkipNonToken();

                if (Reader.Is('?'))
                {
                    isOptional = true;
                    Reader.Eat('?');
                    Reader.SkipNonToken();
                }
            }
        }

        Reader.SkipNonToken();

        return new BadFunctionParameter(name, isOptional, isNullChecked, isRestArgs, typeExpr);
    }

    private List<BadFunctionParameter> ParseParameters(int start)
    {
        List<BadFunctionParameter> parameters = new List<BadFunctionParameter>();

        Reader.Eat('(');
        Reader.SkipNonToken();

        if (!Reader.Is(')'))
        {
            bool first = true;
            bool hadOptional = false;
            bool hadRest = false;

            while (Reader.Is(',') || first)
            {
                if (hadRest)
                {
                    throw new BadParserException(
                        "Rest parameter must be last parameter",
                        Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
                    );
                }

                if (!first)
                {
                    Reader.Eat(',');
                    Reader.SkipNonToken();
                }

                first = false;


                BadFunctionParameter param = ParseParameter();

                if (hadOptional && !param.IsOptional && !param.IsRestArgs)
                {
                    throw new BadParserException(
                        "Non-Optional parameters must be in front of optional parameters",
                        Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
                    );
                }

                Reader.SkipNonToken();

                if (parameters.Any(p => p.Name == param.Name))
                {
                    throw new BadParserException(
                        "Duplicate parameter name",
                        Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
                    );
                }

                hadOptional |= param.IsOptional;
                hadRest |= param.IsRestArgs;


                parameters.Add(param);
            }
        }

        Reader.SkipNonToken();
        Reader.Eat(')');

        return parameters;
    }

    private BadFunctionExpression ParseFunction(
        int start,
        string? functionName,
        BadExpression? functionReturn,
        bool isConstant,
        BadFunctionCompileLevel compileLevel,
        List<BadFunctionParameter> parameters)
    {
        BadMetaData? meta = m_MetaData;
        m_MetaData = null;
        List<BadExpression> block = ParseBlock(start, out bool isSingleLine);

        if (isSingleLine)
        {
            block[0] = new BadReturnExpression(block[0], block[0].Position, false);
        }

        if (functionName == null)
        {
            return new BadFunctionExpression(
                null,
                parameters,
                block,
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start),
                isConstant,
                meta,
                compileLevel,
                functionReturn
            );
        }

        return new BadFunctionExpression(
            functionName,
            parameters,
            block,
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start),
            isConstant,
            meta,
            compileLevel,
            functionReturn
        );
    }

    /// <summary>
    ///     Parses a function definition. Moves the reader to the next token.
    /// </summary>
    /// <param name="isConstant">
    ///     Indicates that the function is declared as a constant. I.e. it is readonly inside the scope it
    ///     is executed in
    /// </param>
    /// <returns>Instance of BadFunctionExpression</returns>
    /// <exception cref="BadParserException">Gets raised if the function header is invalid.</exception>
    private BadFunctionExpression ParseFunction(bool isConstant, BadFunctionCompileLevel compileLevel)
    {
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.FunctionKey);
        Reader.SkipNonToken();

        string? functionName = null;
        BadExpression? functionReturn = null;

        if (!Reader.Is('('))
        {
            BadExpression functionNameExpr = ParseValue(0);
            Reader.SkipNonToken();

            while (Reader.Is("."))
            {
                Reader.Eat(".");
                BadWordToken right = Reader.ParseWord();
                functionNameExpr = new BadMemberAccessExpression(
                    functionNameExpr,
                    right,
                    functionNameExpr.Position.Combine(right.SourcePosition)
                );
                Reader.SkipNonToken();
            }

            Reader.SkipNonToken();

            if (!Reader.Is('('))
            {
                functionReturn = functionNameExpr;
                functionName = Reader.ParseWord().Text;
            }
            else
            {
                if (functionNameExpr is not BadVariableExpression expr)
                {
                    throw new BadParserException(
                        "Expected Variable Expression",
                        functionNameExpr.Position
                    );
                }

                functionName = expr.Name;
            }
        }

        List<BadFunctionParameter> parameters = ParseParameters(start);

        return ParseFunction(start, functionName, functionReturn, isConstant, compileLevel, parameters);
    }

    /// <summary>
    ///     Parses the File from start to end.
    /// </summary>
    /// <returns>Returns an Enumerable of BadExpressions</returns>
    public IEnumerable<BadExpression> Parse()
    {
        Reader.SkipNonToken();

        while (!Reader.IsEof())
        {
            Reader.SkipNonToken();
            BadExpression expr = ParseExpression();
            Reader.SkipNonToken();

            if (!Reader.Last(BadStaticKeys.BlockEndKey) && Reader.Is(BadStaticKeys.StatementEndKey))
            {
                Reader.Eat(BadStaticKeys.StatementEndKey);
            }

            Reader.SkipNonToken();

            yield return expr;
        }
    }
}