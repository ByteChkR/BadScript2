using System.Text;
using System.Text.RegularExpressions;

using BadScript2.Common;
using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Block;
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

public class BadSourceParser
{
    private readonly BadOperatorTable m_Operators;

    public BadSourceParser(BadSourceReader sourceReader, BadOperatorTable operators)
    {
        Reader = sourceReader;
        m_Operators = operators;
    }

    public BadSourceReader Reader { get; }

    public static BadSourceParser Create(string fileName, string source)
    {
        return new BadSourceParser(new BadSourceReader(fileName, source), BadOperatorTable.Default);
    }

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

    private BadExpression ParseValue(int precedence)
    {
        Reader.SkipNonToken();

        BadExpression? prefixExpr = ParsePrefix(precedence);

        if (prefixExpr != null)
        {
            return prefixExpr;
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
                if (Reader.Is(';'))
                {
                    return new BadReturnExpression(null, pos);
                }

                BadExpression expr = ParseExpression();

                return new BadReturnExpression(expr, pos);
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

        if (Reader.Is(BadStaticKeys.FunctionKey))
        {
            return ParseFunction();
        }

        if (Reader.Is(BadStaticKeys.ClassKey))
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

            return new BadNumberExpression(decimal.Parse(token.Text), token.SourcePosition);
        }

        Reader.SkipNonToken();

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

        return new BadVariableExpression(word.Text, word.SourcePosition);
    }

    public BadExpression ParseExpression(BadExpression? left = null, int precedence = int.MaxValue)
    {
        left ??= ParseValue(precedence);

        while (!Reader.IsEOF())
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
                left = new BadArrayAccessExpression(left, indices.ToArray(), left.Position.Combine(end), isNullChecked);

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


    private BadStringExpression ParseFormatString()
    {
        if (!Reader.Is(BadStaticKeys.FormatStringKey))
        {
            throw new BadSourceReaderException(
                $"Expected string start character but got '{(Reader.IsEOF() ? "EOF" : Reader.GetCurrentChar())}'",
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
            if (Reader.IsNewLine() || Reader.IsEOF())
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
                sb.ToString(),
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
            );
        }

        return new BadFormattedStringExpression(
            args.ToArray(),
            sb.ToString(),
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
        );
    }

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

    private BadClassPrototypeExpression ParseClass()
    {
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
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
        );
    }

    private BadFunctionExpression ParseFunction()
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
                    throw new BadRuntimeException(
                        "Expected Variable Expression",
                        functionNameExpr.Position
                    );
                }

                functionName = expr.Name;
            }
        }

        Reader.Eat('(');
        Reader.SkipNonToken();
        List<BadFunctionParameter> parameters = new List<BadFunctionParameter>();
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
                        throw new BadRuntimeException(
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
                    hadRest = true;
                    Reader.Eat('*');
                    Reader.SkipNonToken();
                }
                else
                {
                    if (Reader.Is('?'))
                    {
                        isOptional = true;
                        hadOptional = true;
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
                            hadOptional = true;
                            Reader.Eat('?');
                            Reader.SkipNonToken();
                        }
                    }
                }

                if (hadOptional && !isOptional && !isRestArgs)
                {
                    throw new BadParserException(
                        "Non-Optional parameters must be in front of optional parameters",
                        Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
                    );
                }

                Reader.SkipNonToken();
                if (parameters.Any(p => p.Name == name))
                {
                    throw new BadParserException(
                        "Duplicate parameter name",
                        Reader.MakeSourcePosition(start, Reader.CurrentIndex - start)
                    );
                }

                parameters.Add(new BadFunctionParameter(name, isOptional, isNullChecked, isRestArgs, typeExpr));
            }
        }

        Reader.SkipNonToken();
        Reader.Eat(')');

        List<BadExpression> block = ParseBlock(start, out bool isSingleLine);

        if (isSingleLine)
        {
            block[0] = new BadReturnExpression(block[0], block[0].Position);
        }

        if (functionName == null)
        {
            return new BadFunctionExpression(
                null,
                parameters,
                block,
                Reader.MakeSourcePosition(start, Reader.CurrentIndex - start),
                functionReturn
            );
        }

        return new BadFunctionExpression(
            functionName,
            parameters,
            block,
            Reader.MakeSourcePosition(start, Reader.CurrentIndex - start),
            functionReturn
        );
    }

    public IEnumerable<BadExpression> Parse()
    {
        BadLogger.Log($"Parsing File: {Reader.FileName}", "SourceParser");
        Reader.SkipNonToken();
        while (!Reader.IsEOF())
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