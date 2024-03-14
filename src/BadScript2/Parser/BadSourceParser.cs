using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Module;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Parser.Operators;
using BadScript2.Reader;
using BadScript2.Reader.Token;
using BadScript2.Reader.Token.Primitive;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types.Interface;

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

    /// <summary>
    ///     The Meta Data of the current expression
    /// </summary>
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

    /// <summary>
    ///     Creates a BadSourceParser Instance based on the source and filename provided
    /// </summary>
    /// <param name="fileName">File Name of the Source File</param>
    /// <param name="source">Contents of the Source File</param>
    /// <param name="start">Start Index of the Source File</param>
    /// <param name="end">End Index of the Source File</param>
    /// <returns>BadSourceParser Instance</returns>
    public static BadSourceParser Create(string fileName, string source, int start, int end)
    {
        return new BadSourceParser(new BadSourceReader(fileName, source, start, end), BadOperatorTable.Instance);
    }

    /// <summary>
    ///     Parses a BadExpression from the Source Reader
    /// </summary>
    /// <param name="fileName">File Name of the Source File</param>
    /// <param name="source">Contents of the Source File</param>
    /// <returns>The Parsed Expression</returns>
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

        BadUnaryPrefixOperator? op = m_Operators.FindUnaryPrefixOperator(symbol.Text, precedence);


        if (op != null)
        {
            return op.Parse(this);
        }

        Reader.SetPosition(symbol.SourcePosition.Index);

        return null;
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
            Reader.Eat(BadStaticKeys.IF_KEY);
            Reader.SkipNonToken();
            Reader.Eat('(');
            Reader.SkipNonToken();
            BadExpression condition = ParseExpression();
            Reader.SkipNonToken();
            Reader.Eat(')');
            Reader.SkipNonToken();
            List<BadExpression> block = ParseBlock(out bool _);
            Reader.SkipNonToken();
            conditionMap[condition] = block.ToArray();

            if (!Reader.IsKey(BadStaticKeys.ELSE_KEY))
            {
                continue;
            }

            Reader.Eat(BadStaticKeys.ELSE_KEY);
            isElse = true;
            Reader.SkipNonToken();
            readNext = Reader.IsKey(BadStaticKeys.IF_KEY);
        }
        while (readNext);

        BadExpression[]? elseBlock = null;

        if (isElse)
        {
            elseBlock = ParseBlock(out bool _).ToArray();
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
        Reader.Eat(BadStaticKeys.FOR_EACH_KEY);
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
        List<BadExpression> block = ParseBlock(out bool _);
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
        Reader.Eat(BadStaticKeys.LOCK_KEY);
        Reader.SkipNonToken();
        Reader.Eat('(');
        Reader.SkipNonToken();
        BadExpression collection = ParseExpression();
        Reader.SkipNonToken();
        Reader.Eat(')');
        Reader.SkipNonToken();
        List<BadExpression> block = ParseBlock(out bool _);
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
        Reader.Eat(BadStaticKeys.FOR_KEY);
        Reader.SkipNonToken();
        Reader.Eat('(');
        Reader.SkipNonToken();
        BadExpression vDef = ParseExpression();
        Reader.SkipNonToken();
        Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
        Reader.SkipNonToken();
        BadExpression condition = ParseExpression();
        Reader.SkipNonToken();
        Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
        Reader.SkipNonToken();
        BadExpression vInc = ParseExpression();
        Reader.SkipNonToken();
        Reader.Eat(')');
        Reader.SkipNonToken();
        List<BadExpression> block = ParseBlock(out bool _);
        Reader.SkipNonToken();

        return new BadForExpression(
            vDef,
            condition,
            vInc,
            block.ToArray(),
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
        );
    }

    /// <summary>
    ///     Parses the MetaData of the current expression
    /// </summary>
    private void ParseMeta()
    {
        if (!Reader.Is("@|"))
        {
            return;
        }

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

                        if (!Reader.Is('.'))
                        {
                            continue;
                        }

                        type += '.';
                        Reader.MoveNext();
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

                        if (!Reader.Is('.'))
                        {
                            continue;
                        }

                        type += '.';
                        Reader.MoveNext();
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
                    return ParseFunction(start, null, null, false, false, BadFunctionCompileLevel.None, p);
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

        if (Reader.IsKey(BadStaticKeys.LOCK_KEY))
        {
            return ParseLock();
        }

        if (Reader.IsKey(BadStaticKeys.FOR_EACH_KEY))
        {
            return ParseForEach();
        }

        if (Reader.IsKey(BadStaticKeys.FOR_KEY))
        {
            return ParseFor();
        }

        if (Reader.IsKey(BadStaticKeys.IF_KEY))
        {
            return ParseIf();
        }

        if (Reader.IsKey(BadStaticKeys.CONTINUE_KEY))
        {
            BadSourcePosition pos = Reader.Eat(BadStaticKeys.CONTINUE_KEY);

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

        if (Reader.IsKey(BadStaticKeys.BREAK_KEY))
        {
            BadSourcePosition pos = Reader.Eat(BadStaticKeys.BREAK_KEY);

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

        if (Reader.IsKey(BadStaticKeys.THROW_KEY))
        {
            BadSourcePosition pos = Reader.Eat(BadStaticKeys.THROW_KEY);

            if (Reader.IsWordChar())
            {
                Reader.SetPosition(pos.Index);
            }
            else
            {
                return new BadThrowExpression(ParseExpression(), pos);
            }
        }

        if (Reader.IsKey(BadStaticKeys.RETURN_KEY))
        {
            BadSourcePosition pos = Reader.Eat(BadStaticKeys.RETURN_KEY);

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

                if (Reader.IsKey(BadStaticKeys.REF_KEY))
                {
                    isRef = true;
                    Reader.Eat(BadStaticKeys.REF_KEY);
                    Reader.SkipNonToken();
                }

                BadExpression expr = ParseExpression();

                return new BadReturnExpression(expr, pos, isRef);
            }
        }

        if (Reader.IsKey(BadStaticKeys.TRUE) ||
            Reader.IsKey(BadStaticKeys.FALSE))
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
        bool isStatic = false;
        BadFunctionCompileLevel compileLevel = BadFunctionCompileLevel.None;
        int constStart = Reader.CurrentIndex;

        if (Reader.IsKey(BadStaticKeys.CONSTANT_DEFINITION_KEY))
        {
            isConstant = true;
            Reader.Eat(BadStaticKeys.CONSTANT_DEFINITION_KEY);
            Reader.SkipNonToken();
        }
        else if (Reader.IsKey(BadStaticKeys.STATIC_DEFINITION_KEY))
        {
            isStatic = true;
            Reader.Eat(BadStaticKeys.STATIC_DEFINITION_KEY);
            Reader.SkipNonToken();
        }

        int compiledStart = Reader.CurrentIndex;

        if (Reader.IsKey(BadStaticKeys.COMPILED_DEFINITION_KEY))
        {
            compileLevel = BadFunctionCompileLevel.Compiled;
            Reader.Eat(BadStaticKeys.COMPILED_DEFINITION_KEY);
            Reader.SkipNonToken();

            if (Reader.IsKey(BadStaticKeys.COMPILED_FAST_DEFINITION_KEY))
            {
                compileLevel = BadFunctionCompileLevel.CompiledFast;
                Reader.Eat(BadStaticKeys.COMPILED_FAST_DEFINITION_KEY);
                Reader.SkipNonToken();
            }
        }

        if (Reader.IsKey(BadStaticKeys.FUNCTION_KEY))
        {
            return ParseFunction(isConstant, isStatic, compileLevel);
        }

        if (compileLevel != BadFunctionCompileLevel.None || isConstant)
        {
            Reader.SetPosition(compiledStart < constStart ? compiledStart : constStart);
        }


        if (Reader.IsKey(BadStaticKeys.CLASS_KEY))
        {
            return ParseClass();
        }

        if (Reader.IsKey(BadStaticKeys.INTERFACE_KEY))
        {
            return ParseInterface();
        }

        if (Reader.IsKey(BadStaticKeys.NEW_KEY))
        {
            return ParseNew();
        }

        if (Reader.IsKey(BadStaticKeys.TRY_KEY))
        {
            return ParseTry();
        }

        if (Reader.IsKey(BadStaticKeys.USING_KEY))
        {
            return ParseUsing();
        }

        if (Reader.IsKey(BadStaticKeys.WHILE))
        {
            return ParseWhile();
        }

        if (Reader.IsKey(BadStaticKeys.NULL))
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

        if (Reader.IsStringQuote() || Reader.IsStringQuote(0, true))
        {
            BadStringToken token = Reader.ParseString();

            return new BadStringExpression(token.Value, token.SourcePosition);
        }

        if (Reader.Is(BadStaticKeys.FORMAT_STRING_KEY))
        {
            return ParseFormatString();
        }


        if (Reader.Is(BadStaticKeys.MULTI_LINE_FORMAT_STRING_KEY))
        {
            return ParseMultiLineFormatString();
        }


        if (Reader.Is(BadStaticKeys.MULTI_LINE_STRING_KEY))
        {
            BadStringToken token = Reader.ParseMultiLineString();

            return new BadStringExpression(token.Value, token.SourcePosition);
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

        if (word.Text == BadStaticKeys.VARIABLE_DEFINITION_KEY ||
            word.Text == BadStaticKeys.CONSTANT_DEFINITION_KEY)
        {
            bool isReadOnly = word.Text == BadStaticKeys.CONSTANT_DEFINITION_KEY;
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

        if (precedence <= 0 || !Reader.Is("=>") && !Reader.Is('*') && !Reader.Is('?') && !Reader.Is('!'))
        {
            return new BadVariableExpression(word.Text, word.SourcePosition);
        }

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
                        isStatic,
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

            if (Reader.Is(BadStaticKeys.STATEMENT_END_KEY))
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

            BadBinaryOperator? op = m_Operators.FindBinaryOperator(symbol.Text, precedence);


            if (op == null)
            {
                Reader.SetPosition(symbol.SourcePosition.Index);

                return left;
            }

            left = op.Parse(left, this);
        }

        return left;
    }


    /// <summary>
    ///     Parses a Multiline Format string expression. Moves the reader to the next token.
    /// </summary>
    /// <returns>Instance of BadStringExpression</returns>
    /// <exception cref="BadSourceReaderException">Gets raised if the string is not properly terminated.</exception>
    private BadStringExpression ParseMultiLineFormatString()
    {
        if (!Reader.Is(BadStaticKeys.MULTI_LINE_FORMAT_STRING_KEY))
        {
            throw new BadSourceReaderException(
                $"Expected string start character but got '{(Reader.IsEof() ? "EOF" : Reader.GetCurrentChar())}'",
                Reader.MakeSourcePosition(1)
            );
        }

        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.MULTI_LINE_FORMAT_STRING_KEY);
        StringBuilder sb = new StringBuilder();
        List<BadExpression> args = new List<BadExpression>();

        while (!Reader.IsStringQuote())
        {
            if (Reader.IsEof())
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

                if (!Reader.Is('}'))
                {
                    throw new BadSourceReaderException(
                        "Expected '}'",
                        Reader.MakeSourcePosition(idx, 1)
                    );
                }

                Reader.Eat('}');
                sb.Append("}}");

                continue;
            }

            sb.Append(Reader.GetCurrentChar());

            Reader.MoveNext();
        }

        Reader.Eat(BadStaticKeys.QUOTE);

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
        if (!Reader.Is(BadStaticKeys.FORMAT_STRING_KEY))
        {
            throw new BadSourceReaderException(
                $"Expected string start character but got '{(Reader.IsEof() ? "EOF" : Reader.GetCurrentChar())}'",
                Reader.MakeSourcePosition(1)
            );
        }

        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.FORMAT_STRING_KEY);
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

                if (!Reader.Is('}'))
                {
                    throw new BadParserException(
                        "Expected '}'",
                        Reader.MakeSourcePosition(idx, 1)
                    );
                }

                Reader.Eat('}');
                sb.Append("}}");

                continue;
            }

            if (Reader.Is(BadStaticKeys.ESCAPE_CHARACTER))
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

        Reader.Eat(BadStaticKeys.QUOTE);

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
        Reader.Eat(BadStaticKeys.WHILE);
        Reader.SkipNonToken();
        Reader.Eat('(');
        BadExpression condition = ParseExpression();
        Reader.Eat(')');

        List<BadExpression> block = ParseBlock(out bool _);

        return new BadWhileExpression(
            condition,
            block,
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
        );
    }

    /// <summary>
    ///     Parses a Block Expression. Moves the reader to the next token.
    /// </summary>
    /// <param name="isSingleLine">Indicates if the Block is in single line format.</param>
    /// <returns>List of Parsed Expressions</returns>
    private List<BadExpression> ParseBlock(out bool isSingleLine)
    {
        Reader.SkipNonToken();

        List<BadExpression> block = new List<BadExpression>();

        if (Reader.Is('{'))
        {
            isSingleLine = false;
            Reader.Eat('{');
            Reader.SkipNonToken();

            while (!Reader.Is(BadStaticKeys.BLOCK_END_KEY))
            {
                Reader.SkipNonToken();
                BadExpression expr = ParseExpression();
                block.Add(expr);
                Reader.SkipNonToken();

                if (!RequireSemicolon(expr))
                {
                    continue;
                }

                Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
                Reader.SkipNonToken();
            }

            Reader.Eat(BadStaticKeys.BLOCK_END_KEY);
        }
        else
        {
            isSingleLine = true;
            BadExpression expr = ParseExpression();

            if (RequireSemicolon(expr))
            {
                Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
            }

            Reader.SkipNonToken();
            block.Add(expr);
        }

        Reader.SkipNonToken();

        return block;
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
    private List<BadExpression> ParseFunctionBlock(int start, out bool isSingleLine)
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

            while (!Reader.Is(BadStaticKeys.BLOCK_END_KEY))
            {
                Reader.SkipNonToken();
                BadExpression expr = ParseExpression();
                block.Add(expr);
                Reader.SkipNonToken();

                if (!RequireSemicolon(expr))
                {
                    continue;
                }

                Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
                Reader.SkipNonToken();
            }

            Reader.Eat(BadStaticKeys.BLOCK_END_KEY);
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
    ///     Parses a Using Statement Expression. Moves the reader to the next token.
    /// </summary>
    /// <param name="start">The Start index of the expression</param>
    /// <returns>Instance of BadUsingStatementExpression</returns>
    /// <exception cref="BadParserException">Gets raised if the using statement is malformed.</exception>
    private BadExpression ParseUsingStatement(int start)
    {
        Reader.SkipNonToken();
        if (!Reader.Is(BadStaticKeys.CONSTANT_DEFINITION_KEY))
        {
            throw new BadParserException(
                "Expected Constant Variable Definition",
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        //Parse single expression
        BadExpression expr = ParseExpression();
        if (expr is not BadAssignExpression assignExpr)
        {
            throw new BadParserException(
                "Expected Constant Variable Definition",
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        if (assignExpr.Left is not BadVariableDefinitionExpression varDef)
        {
            throw new BadParserException(
                "Expected Constant Variable Definition",
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        Reader.SkipNonToken();

        //Reader.Eat(BadStaticKeys.STATEMENT_END_KEY); //Not Needed, the parser will automatically eat the statement end key


        return new BadUsingStatementExpression(Reader.MakeSourcePosition(start, Reader.CurrentIndex - start), varDef.Name, expr);
    }

    /// <summary>
    ///     Parses a Using Block or Statement. Moves the reader to the next token.
    /// </summary>
    /// <returns>Instance of BadUsingExpression or BadUsingStatementExpression</returns>
    /// <exception cref="BadParserException">Gets raised if the using statement is malformed.</exception>
    private BadExpression ParseUsing()
    {
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.USING_KEY);
        Reader.SkipNonToken();
        if (!Reader.Is('('))
        {
            return ParseUsingStatement(start);
        }

        Reader.Eat('(');
        Reader.SkipNonToken();
        if (!Reader.Is(BadStaticKeys.CONSTANT_DEFINITION_KEY))
        {
            throw new BadParserException(
                "Expected Constant Variable Definition",
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        //Parse single expression
        BadExpression expr = ParseExpression();
        if (expr is not BadAssignExpression assignExpr)
        {
            throw new BadParserException(
                "Expected Constant Variable Definition",
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        if (assignExpr.Left is not BadVariableDefinitionExpression varDef)
        {
            throw new BadParserException(
                "Expected Constant Variable Definition",
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        Reader.SkipNonToken();

        Reader.Eat(')');

        Reader.SkipNonToken();
        List<BadExpression> block = ParseBlock(out bool isSingleLine);
        Reader.SkipNonToken();
        if (isSingleLine)
        {
            Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
        }

        Reader.SkipNonToken();

        return new BadUsingExpression(varDef.Name, block.ToArray(), Reader.MakeSourcePosition(start, Reader.CurrentIndex - start), expr);
    }

    /// <summary>
    ///     Parses a Try Catch Block. Moves the reader to the next token.
    /// </summary>
    /// <returns>Instance of BadTryCatchExpression</returns>
    private BadExpression ParseTry()
    {
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.TRY_KEY);
        Reader.SkipNonToken();
        List<BadExpression> block = ParseBlock(out bool isSingleLine);
        Reader.SkipNonToken();

        if (isSingleLine)
        {
            Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
        }

        Reader.SkipNonToken();
        Reader.Eat(BadStaticKeys.CATCH_KEY);
        Reader.SkipNonToken();
        Reader.Eat('(');
        Reader.SkipNonToken();
        BadWordToken errorName = Reader.ParseWord();
        Reader.SkipNonToken();
        Reader.Eat(')');
        Reader.SkipNonToken();
        List<BadExpression> errorBlock = ParseBlock(out isSingleLine);
        Reader.SkipNonToken();

        if (isSingleLine)
        {
            Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
        }

        Reader.SkipNonToken();

        BadExpression[] finallyBlock = Array.Empty<BadExpression>();
        if (Reader.Is(BadStaticKeys.FINALLY_KEY))
        {
            Reader.Eat(BadStaticKeys.FINALLY_KEY);
            Reader.SkipNonToken();
            List<BadExpression> finallyExprs = ParseBlock(out isSingleLine);
            Reader.SkipNonToken();
            if (isSingleLine)
            {
                Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
            }

            Reader.SkipNonToken();
            finallyBlock = finallyExprs.ToArray();
        }

        return new BadTryCatchExpression(
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start),
            block.ToArray(),
            errorBlock.ToArray(),
            finallyBlock,
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
        Reader.Eat(BadStaticKeys.NEW_KEY);
        Reader.SkipNonToken();
        BadExpression right = ParseExpression();

        if (right is not BadInvocationExpression invoc)
        {
            throw new BadParserException("Expected Invocation Expression", right.Position);
        }

        return new BadNewExpression(invoc, Reader.MakeSourcePosition(start, Reader.CurrentIndex - start));
    }

    /// <summary>
    ///     Parses an Interface Constraint. Moves the reader to the next token.
    /// </summary>
    /// <returns>Instance of BadInterfaceConstraint</returns>
    /// <exception cref="BadParserException">Gets raised if the interface constraint is malformed.</exception>
    private BadInterfaceConstraint ParseInterfaceConstraint()
    {
        int start = Reader.CurrentIndex;
        string? name = null;
        BadExpression? type = null;

        if (!Reader.Is('('))
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

            if (!Reader.Is('('))
            {
                type = nameExpr;
                name = Reader.ParseWord().Text;
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
        }

        if (name == null)
        {
            throw new BadParserException(
                "Expected Name",
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        Reader.SkipNonToken();
        if (Reader.Is('('))
        {
            List<BadFunctionParameter> parameters = ParseParameters(start);

            Reader.SkipNonToken();

            Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);


            return new BadInterfaceFunctionConstraint(name, type, parameters.ToArray());
        }


        Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);

        return new BadInterfacePropertyConstraint(name, type);
    }

    /// <summary>
    ///     Parses an Interface prototype. Moves the reader to the next token.
    /// </summary>
    /// <returns>Instance of BadInterfacePrototypeExpression</returns>
    private BadInterfacePrototypeExpression ParseInterface()
    {
        BadMetaData? meta = m_MetaData;
        m_MetaData = null;
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.INTERFACE_KEY);
        Reader.SkipNonToken();
        BadWordToken name = Reader.ParseWord();
        Reader.SkipNonToken();
        List<BadExpression> interfaces = new List<BadExpression>();

        if (Reader.Is(':'))
        {
            Reader.Eat(':');

            while (!Reader.IsEof())
            {
                Reader.SkipNonToken();
                interfaces.Add(ParseExpression());
                Reader.SkipNonToken();

                if (!Reader.Is(','))
                {
                    break;
                }

                Reader.Eat(',');
                Reader.SkipNonToken();
            }
        }

        Reader.Eat('{');
        Reader.SkipNonToken();
        List<BadInterfaceConstraint> constraints = new List<BadInterfaceConstraint>();

        while (!Reader.Is('}'))
        {
            Reader.SkipNonToken();
            BadInterfaceConstraint expr = ParseInterfaceConstraint();

            constraints.Add(expr);

            Reader.SkipNonToken();
        }

        Reader.Eat('}');
        Reader.SkipNonToken();

        return new BadInterfacePrototypeExpression(
            name.Text,
            constraints.ToArray(),
            interfaces.ToArray(),
            meta,
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
        );
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
        Reader.Eat(BadStaticKeys.CLASS_KEY);
        Reader.SkipNonToken();
        BadWordToken name = Reader.ParseWord();
        Reader.SkipNonToken();

        List<BadFunctionParameter>? primaryConstructor = null;
        List<BadExpression>? baseInvocationParameters = null;
        BadSourcePosition? primaryConstructorPosition = null;
        BadSourcePosition? baseInvocationParametersPosition = null;
        List<BadExpression> members = new List<BadExpression>();
        List<BadExpression> staticMembers = new List<BadExpression>();
        if (Reader.Is('('))
        {
            int ctorStart = Reader.CurrentIndex;
            primaryConstructor = ParseParameters(start);
            primaryConstructorPosition = Reader.MakeSourcePosition(ctorStart, Reader.CurrentIndex - ctorStart);
            Reader.SkipNonToken();
        }

        List<BadExpression> baseClasses = new List<BadExpression>();

        if (Reader.Is(':'))
        {
            Reader.Eat(':');

            while (!Reader.IsEof())
            {
                Reader.SkipNonToken();
                BadExpression baseExpr = ParseExpression();
                if (baseClasses.Count == 0)
                {
                    if (baseExpr is BadInvocationExpression baseInvoc)
                    {
                        baseInvocationParameters = baseInvoc.Arguments.ToList();
                        baseInvocationParametersPosition = baseInvoc.Position;
                        baseExpr = baseInvoc.Left;
                    }
                }
                else if (baseExpr is BadInvocationExpression)
                {
                    throw new BadParserException(
                        "Base Class Invocation must be the first base class",
                        baseExpr.Position
                    );
                }

                baseClasses.Add(baseExpr);
                Reader.SkipNonToken();

                if (!Reader.Is(','))
                {
                    break;
                }

                Reader.Eat(',');
                Reader.SkipNonToken();
            }
        }

        if (!Reader.Is(BadStaticKeys.STATEMENT_END_KEY))
        {
            Reader.Eat('{');
            Reader.SkipNonToken();

            while (!Reader.Is('}'))
            {
                Reader.SkipNonToken();
                BadExpression expr = ParseExpression();

                if (expr is BadFunctionExpression
                    {
                        IsStatic: true,
                    })
                {
                    staticMembers.Add(expr);
                }
                else if (expr is BadFunctionExpression fExpr && fExpr.Name?.Text == name.Text)
                {
                    if (primaryConstructor != null)
                    {
                        throw new BadParserException(
                            "Primary Constructor already defined",
                            expr.Position
                        );
                    }

                    fExpr.SetName(BadStaticKeys.CONSTRUCTOR_NAME);
                    members.Add(expr);
                }
                else
                {
                    members.Add(expr);
                }

                if (expr is IBadNamedExpression nExpr && nExpr.GetName() == name.Text)
                {
                    throw new BadParserException(
                        "Class Member cannot have the same name as the class",
                        expr.Position
                    );
                }

                Reader.SkipNonToken();

                if (RequireSemicolon(expr))
                {
                    Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
                }

                Reader.SkipNonToken();
            }

            Reader.Eat('}');
        }
        else
        {
            Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
        }

        Reader.SkipNonToken();
        if (primaryConstructor != null)
        {
            List<BadExpression> block = new List<BadExpression>();

            //2. Add the primary constructor as a function to the class
            BadFunctionExpression ctor = new BadFunctionExpression(BadStaticKeys.CONSTRUCTOR_NAME, primaryConstructor, block, primaryConstructorPosition!, false, null, false, false);
            members.Add(ctor);
            if (baseInvocationParameters != null)
            {
                //  2.1. (call base class constructor if baseInvocationParameters is not null)
                BadInvocationExpression baseInvocation = new BadInvocationExpression(
                    new BadVariableExpression("base", primaryConstructorPosition!),
                    baseInvocationParameters,
                    baseInvocationParametersPosition!
                );
                block.Add(baseInvocation);
            }
            else
            {
                BadVariableExpression thisExpr = new BadVariableExpression(BadStaticKeys.THIS_KEY, primaryConstructorPosition!);
                foreach (BadFunctionParameter parameter in primaryConstructor)
                {
                    //  2.2. (if baseInvocationParameters is null, assign the parameters to the class members)
                    // this.{parameter.Name} = {parameter.Name};
                    block.Add(
                        new BadAssignExpression(
                            new BadMemberAccessExpression(thisExpr, parameter.Name, primaryConstructorPosition!),
                            new BadVariableExpression(parameter.Name, primaryConstructorPosition!),
                            primaryConstructorPosition!
                        )
                    );

                    //3. (if baseInvocationParameters is null, define the properties for the parameters)
                    //let {type} {parameter.Name};
                    members.Add(new BadVariableDefinitionExpression(parameter.Name, primaryConstructorPosition!, parameter.TypeExpr, true));
                }
            }
        }

        return new BadClassPrototypeExpression(
            name.Text,
            members.ToArray(),
            staticMembers.ToArray(),
            baseClasses.ToArray(),
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start),
            meta
        );
    }

    /// <summary>
    ///     Parses a Function Parameter. Moves the reader to the next token.
    /// </summary>
    /// <returns>Instance of BadFunctionParameter</returns>
    /// <exception cref="BadParserException">Gets raised if the parameter is malformed.</exception>
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

    /// <summary>
    ///     Parses a Function Parameter List. Moves the reader to the next token.
    /// </summary>
    /// <param name="start">The Start index of parent expression</param>
    /// <returns>List of BadFunctionParameter</returns>
    /// <exception cref="BadParserException">Gets raised if the parameter list is malformed.</exception>
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

                if (hadOptional && param is { IsOptional: false, IsRestArgs: false })
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

    /// <summary>
    ///     Parses a Function Definition. Moves the reader to the next token.
    /// </summary>
    /// <param name="start">The Start index of parent expression</param>
    /// <param name="functionName">The Name of the Function</param>
    /// <param name="functionReturn">The Return Expression of the Function</param>
    /// <param name="isConstant">Indicates that the function is declared as a constant. I.e. the result will be cached</param>
    /// <param name="isStatic">Indicates that the function is declared as static. I.e. it can be called without an instance</param>
    /// <param name="compileLevel">The Compile level of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    /// <returns>Instance of BadFunctionExpression</returns>
    private BadFunctionExpression ParseFunction(
        int start,
        string? functionName,
        BadExpression? functionReturn,
        bool isConstant,
        bool isStatic,
        BadFunctionCompileLevel compileLevel,
        List<BadFunctionParameter> parameters)
    {
        BadMetaData? meta = m_MetaData;
        m_MetaData = null;
        List<BadExpression> block = ParseFunctionBlock(start, out bool isSingleLine);

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
                isSingleLine,
                isStatic,
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
            isSingleLine,
            isStatic,
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
    /// <param name="isStatic">Is the Function Static</param>
    /// <param name="compileLevel">The Compile level of the Function</param>
    /// <returns>Instance of BadFunctionExpression</returns>
    /// <exception cref="BadParserException">Gets raised if the function header is invalid.</exception>
    private BadFunctionExpression ParseFunction(bool isConstant, bool isStatic, BadFunctionCompileLevel compileLevel)
    {
        int start = Reader.CurrentIndex;
        Reader.Eat(BadStaticKeys.FUNCTION_KEY);
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

        return ParseFunction(start, functionName, functionReturn, isConstant, isStatic, compileLevel, parameters);
    }

    /// <summary>
    ///     Returns true if the given expression requires a semicolon.
    /// </summary>
    /// <param name="expr">The Expression to check</param>
    /// <returns>True if the expression requires a semicolon. False otherwise.</returns>
    private static bool RequireSemicolon(BadExpression expr)
    {
        if (expr is BadNamedExportExpression named)
        {
            return RequireSemicolon(named.Expression);
        }

        if (expr is BadDefaultExportExpression def)
        {
            return RequireSemicolon(def.Expression);
        }

        return expr is not (
            BadInterfacePrototypeExpression
            or BadClassPrototypeExpression
            or BadIfExpression
            or BadForExpression
            or BadWhileExpression
            or BadForEachExpression
            or BadTryCatchExpression
            or BadLockExpression
            or BadUsingExpression
            or BadFunctionExpression
            {
                IsSingleLine: false,
            });
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

            if (!RequireSemicolon(expr))
            {
                Reader.SkipNonToken();

                yield return expr;

                continue;
            }

            Reader.Eat(BadStaticKeys.STATEMENT_END_KEY);
            Reader.SkipNonToken();

            yield return expr;
        }
    }
}