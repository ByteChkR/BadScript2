using System.Text;
using System.Text.RegularExpressions;

using BadScript2.Common;
using BadScript2.Reader.Token;
using BadScript2.Reader.Token.Primitive;

namespace BadScript2.Reader
{
    public static class BadSourceReaderExtensions
    {
        public static bool IsWordStart(this BadSourceReader reader, int offset = 0)
        {
            return char.IsLetter(reader.GetCurrentChar(offset)) ||
                   reader.GetCurrentChar(offset) == '_';
        }

        public static bool IsWordChar(this BadSourceReader reader, int offset = 0)
        {
            return char.IsLetterOrDigit(reader.GetCurrentChar(offset)) ||
                   reader.GetCurrentChar(offset) == '_';
        }

        public static bool IsStringQuote(this BadSourceReader reader, int offset = 0)
        {
            return reader.GetCurrentChar(offset) == BadStaticKeys.Quote;
        }

        public static bool IsNumberStart(this BadSourceReader reader, int offset = 0)
        {
            return reader.IsDigit(offset) ||
                   reader.GetCurrentChar(offset) == BadStaticKeys.DecimalSeparator ||
                   reader.GetCurrentChar(offset) == BadStaticKeys.NegativeSign;
        }

        public static bool IsDigit(this BadSourceReader reader, int offset = 0)
        {
            return char.IsDigit(reader.GetCurrentChar(offset));
        }

        public static bool IsNewLine(this BadSourceReader reader, int offset = 0)
        {
            return reader.Is(offset, BadStaticKeys.NewLine);
        }

        public static void SkipWhiteSpace(this BadSourceReader reader)
        {
            reader.Skip(BadStaticKeys.Whitespace);
        }

        public static void SkipNewLine(this BadSourceReader reader)
        {
            reader.Skip(BadStaticKeys.NewLine);
        }

        public static void SkipToEndOfLine(this BadSourceReader reader)
        {
            reader.Seek(BadStaticKeys.NewLine);
        }

        public static void SkipWhiteSpaceAndNewLine(this BadSourceReader reader)
        {
            reader.Skip(BadStaticKeys.Whitespace.Concat(BadStaticKeys.NewLine).ToArray());
        }

        public static void SkipComment(this BadSourceReader reader)
        {
            if (reader.Is(BadStaticKeys.SingleLineComment))
            {
                reader.Eat(BadStaticKeys.SingleLineComment);
                reader.SkipToEndOfLine();
                reader.SkipNewLine();
            }
            else if (reader.Is(BadStaticKeys.MultiLineCommentStart))
            {
                reader.Eat(BadStaticKeys.MultiLineCommentStart);
                reader.Seek(BadStaticKeys.MultiLineCommentEnd);
                reader.Eat(BadStaticKeys.MultiLineCommentEnd);
            }
        }

        public static void SkipNonToken(this BadSourceReader reader)
        {
            int current;

            do
            {
                current = reader.CurrentIndex;
                reader.SkipComment();
                reader.SkipWhiteSpaceAndNewLine();
            }
            while (current != reader.CurrentIndex);
        }


        public static BadWordToken ParseWord(this BadSourceReader reader)
        {
            if (!reader.IsWordStart())
            {
                throw new BadSourceReaderException(
                    $"Expected word start character but got '{(reader.IsEOF() ? "EOF" : reader.GetCurrentChar())}'",
                    reader.MakeSourcePosition(1)
                );
            }

            int start = reader.CurrentIndex;
            reader.MoveNext();
            while (reader.IsWordChar())
            {
                reader.MoveNext();
            }

            return new BadWordToken(reader.MakeSourcePosition(start, reader.CurrentIndex - start));
        }

        public static BadNumberToken ParseNumber(this BadSourceReader reader)
        {
            if (!reader.IsNumberStart())
            {
                throw new BadSourceReaderException(
                    $"Expected number start character but got '{(reader.IsEOF() ? "EOF" : reader.GetCurrentChar())}'",
                    reader.MakeSourcePosition(1)
                );
            }

            int start = reader.CurrentIndex;
            reader.MoveNext();
            bool hasDecimal = false;
            while (reader.IsDigit() || (!hasDecimal && reader.Is(BadStaticKeys.DecimalSeparator)))
            {
                if (reader.Is(BadStaticKeys.DecimalSeparator))
                {
                    hasDecimal = true;
                }

                reader.MoveNext();
            }

            if (reader.Last(BadStaticKeys.DecimalSeparator))
            {
                reader.SetPosition(reader.CurrentIndex - 1);
            }

            return new BadNumberToken(reader.MakeSourcePosition(start, reader.CurrentIndex - start));
        }

        public static BadBooleanToken ParseBoolean(this BadSourceReader reader)
        {
            if (reader.Is(BadStaticKeys.True))
            {
                return new BadBooleanToken(reader.Eat(BadStaticKeys.True));
            }

            if (reader.Is(BadStaticKeys.False))
            {
                return new BadBooleanToken(reader.Eat(BadStaticKeys.False));
            }

            throw new BadSourceReaderException(
                $"Expected boolean but got '{(reader.IsEOF() ? "EOF" : reader.GetCurrentChar())}'",
                reader.MakeSourcePosition(1)
            );
        }

        public static BadNullToken ParseNull(this BadSourceReader reader)
        {
            return new BadNullToken(reader.Eat(BadStaticKeys.Null));
        }

        public static BadSymbolToken? TryParseSymbols(this BadSourceReader reader, string symbols)
        {
            if (reader.Is(symbols))
            {
                return new BadSymbolToken(reader.Eat(symbols));
            }

            return null;
        }

        public static BadSymbolToken? TryParseSymbols(this BadSourceReader reader, IEnumerable<string> symbols)
        {
            string? symbol = symbols.FirstOrDefault(x => reader.Is(x));
            if (symbol == null)
            {
                return null;
            }

            return reader.TryParseSymbols(symbol);
        }

        public static BadStringToken ParseString(this BadSourceReader reader)
        {
            if (!reader.IsStringQuote())
            {
                throw new BadSourceReaderException(
                    $"Expected string start character but got '{(reader.IsEOF() ? "EOF" : reader.GetCurrentChar())}'",
                    reader.MakeSourcePosition(1)
                );
            }

            int start = reader.CurrentIndex;
            reader.MoveNext();
            bool isEscaped = false;
            StringBuilder sb = new StringBuilder("\"");
            while (!reader.IsStringQuote())
            {
                if (reader.IsNewLine() || reader.IsEOF())
                {
                    throw new BadSourceReaderException(
                        "String not terminated",
                        reader.MakeSourcePosition(start, reader.CurrentIndex - start)
                    );
                }

                if (reader.Is(BadStaticKeys.EscapeCharacter))
                {
                    isEscaped = true;
                    reader.MoveNext();
                }

                if (isEscaped)
                {
                    isEscaped = false;
                    sb.Append(Regex.Unescape($"\\{reader.GetCurrentChar()}"));
                }
                else
                {
                    sb.Append(reader.GetCurrentChar());
                }

                reader.MoveNext();
            }

            reader.Eat(BadStaticKeys.Quote);

            sb.Append("\"");

            return new BadStringToken(sb.ToString(), reader.MakeSourcePosition(start, reader.CurrentIndex - start));
        }

        public static bool IsWhiteSpace(this BadSourceReader reader, int offset = 0)
        {
            return reader.Is(offset, BadStaticKeys.Whitespace) ||
                   reader.Is(offset, BadStaticKeys.NewLine);
        }

        public static bool Last(this BadSourceReader reader, char c)
        {
            int index = -1;
            while (reader.IsWhiteSpace(index))
            {
                index--;
            }

            return reader.GetCurrentChar(index) == c;
        }
    }
}