using System.Text;
using System.Text.RegularExpressions;

using BadScript2.Common;
using BadScript2.Reader.Token;
using BadScript2.Reader.Token.Primitive;

namespace BadScript2.Reader;

/// <summary>
///     Extensions for the Source Reader.
/// </summary>
public static class BadSourceReaderExtensions
{
	/// <summary>
	///     Returns true if the Current Character of the Reader is a valid Word Start Character.
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <param name="offset">The Offset from the Current Reader Position</param>
	/// <returns>True if the Character is a Valid Word Start Character</returns>
	public static bool IsWordStart(this BadSourceReader reader, int offset = 0)
	{
		return char.IsLetter(reader.GetCurrentChar(offset)) ||
		       reader.GetCurrentChar(offset) == '_';
	}

	/// <summary>
	///     Returns true if the Current Character of the Reader is a valid Word Character.
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <param name="offset">The Offset from the Current Reader Position</param>
	/// <returns>True if the Current Character is a valid Word Character</returns>
	public static bool IsWordChar(this BadSourceReader reader, int offset = 0)
	{
		return char.IsLetterOrDigit(reader.GetCurrentChar(offset)) ||
		       reader.GetCurrentChar(offset) == '_';
	}

	public static bool IsKey(this BadSourceReader reader, char c, int offset = 0)
	{
		return reader.Is(c, offset) && !reader.IsWordChar(offset + 1);
	}

	public static bool IsKey(this BadSourceReader reader, string s, int offset = 0)
	{
		return reader.Is(s, offset) && !reader.IsWordChar(offset + s.Length);
	}

	/// <summary>
	///     Returns true if the Current Character is a String Quote Character
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <param name="offset">The Offset from the Current Reader Position</param>
	/// <returns>True if the Current Character is a Quote</returns>
	public static bool IsStringQuote(this BadSourceReader reader, int offset = 0, bool singleQuote = false)
	{
		if (singleQuote)
		{
			return reader.GetCurrentChar(offset) == BadStaticKeys.SingleQuote;
		}

		return reader.GetCurrentChar(offset) == BadStaticKeys.Quote;
	}

	/// <summary>
	///     Returns true if the Current Character is a valid Number Character
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <param name="offset">The Offset from the Current Reader Position</param>
	/// <returns>True if the Current Character is a valid Number Character</returns>
	public static bool IsNumberStart(this BadSourceReader reader, int offset = 0)
	{
		return reader.IsDigit(offset) ||
		       reader.GetCurrentChar(offset) == BadStaticKeys.DecimalSeparator;
	}

	/// <summary>
	///     Returns true if the Current Character is a Digit
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <param name="offset">The Offset from the Current Reader Position</param>
	/// <returns>True if the Current Character is a Digit</returns>
	public static bool IsDigit(this BadSourceReader reader, int offset = 0)
	{
		return char.IsDigit(reader.GetCurrentChar(offset));
	}

	/// <summary>
	///     Returns true if the Current Character is a Newline Character
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <param name="offset">The Offset from the Current Reader Position</param>
	/// <returns>True if the Current Character is a NewLine Character</returns>
	public static bool IsNewLine(this BadSourceReader reader, int offset = 0)
	{
		return reader.Is(offset, BadStaticKeys.NewLine);
	}

	/// <summary>
	///     Skips all whitespace characters
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	public static void SkipWhiteSpace(this BadSourceReader reader)
	{
		reader.Skip(BadStaticKeys.Whitespace);
	}


	/// <summary>
	///     Skips all newline characters
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	public static void SkipNewLine(this BadSourceReader reader)
	{
		reader.Skip(BadStaticKeys.NewLine);
	}

	/// <summary>
	///     Skips all characters untile a newline is found
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	public static void SkipToEndOfLine(this BadSourceReader reader)
	{
		reader.Seek(BadStaticKeys.NewLine);
	}

	/// <summary>
	///     Skips all whitespace and newline characters
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	public static void SkipWhiteSpaceAndNewLine(this BadSourceReader reader)
	{
		reader.Skip(BadStaticKeys.Whitespace.Concat(BadStaticKeys.NewLine).ToArray());
	}

	/// <summary>
	///     Skips a comment
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
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

	/// <summary>
	///     Skips all whitespace, newline characters and comments
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
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


	/// <summary>
	///     Parses a Word Token
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <returns>The Resulting BadWordToken instance.</returns>
	/// <exception cref="BadSourceReaderException">
	///     Gets Raised if the Current Character is not a Valid Word Start Character
	///     <seealso cref="BadSourceReaderExtensions.IsWordStart" />
	/// </exception>
	public static BadWordToken ParseWord(this BadSourceReader reader)
	{
		if (!reader.IsWordStart())
		{
			throw new BadSourceReaderException(
				$"Expected word start character but got '{(reader.IsEof() ? "EOF" : reader.GetCurrentChar())}'",
				reader.MakeSourcePosition(1));
		}

		int start = reader.CurrentIndex;
		reader.MoveNext();

		while (reader.IsWordChar())
		{
			reader.MoveNext();
		}

		return new BadWordToken(reader.MakeSourcePosition(start, reader.CurrentIndex - start));
	}

	/// <summary>
	///     Parses a BadNumberToken
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <returns>The Resulting BadNumberToken</returns>
	/// <exception cref="BadSourceReaderException">Gets raised if the Start Character is not a valid number character</exception>
	public static BadNumberToken ParseNumber(this BadSourceReader reader)
	{
		if (!reader.IsNumberStart())
		{
			throw new BadSourceReaderException(
				$"Expected number start character but got '{(reader.IsEof() ? "EOF" : reader.GetCurrentChar())}'",
				reader.MakeSourcePosition(1));
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

	/// <summary>
	///     Parses a BadBoolean Token
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <returns>The Resulting BadBoolean Token</returns>
	/// <exception cref="BadSourceReaderException">
	///     Gets Raised if the Current Character sequence is not Equal to
	///     BadStaticKeys.True and not equal to BadStaticKeys.False
	/// </exception>
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
			$"Expected boolean but got '{(reader.IsEof() ? "EOF" : reader.GetCurrentChar())}'",
			reader.MakeSourcePosition(1));
	}

	/// <summary>
	///     Parses a BadNullToken
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <returns>The Resulting BadNullToken Instance</returns>
	public static BadNullToken ParseNull(this BadSourceReader reader)
	{
		return new BadNullToken(reader.Eat(BadStaticKeys.Null));
	}

	/// <summary>
	///     Tries to parse symbols
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <param name="symbols">The Symbol Sequence to be parsed</param>
	/// <returns>The Bad Symbol Token if the Symbol was matched to the Current Character Sequence. Null otherwise</returns>
	public static BadSymbolToken? TryParseSymbols(this BadSourceReader reader, string symbols)
	{
		if (reader.Is(symbols))
		{
			return new BadSymbolToken(reader.Eat(symbols));
		}

		return null;
	}

	/// <summary>
	///     Tries to parse a list of symbols
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <param name="symbols">The Symbol Sequences to be parsed</param>
	/// <returns>The Bad Symbol Token if one of the Symbols was matched to the Current Character Sequence. Null otherwise</returns>
	public static BadSymbolToken? TryParseSymbols(this BadSourceReader reader, IEnumerable<string> symbols)
	{
		string? symbol =
			symbols.FirstOrDefault(x => x.All(c => char.IsLetter(c) || c == '_') ? reader.IsKey(x) : reader.Is(x));

		if (symbol == null)
		{
			return null;
		}

		return reader.TryParseSymbols(symbol);
	}

	/// <summary>
	///     Parses a BadStringToken
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <returns>The Resulting BadStringToken Instance</returns>
	/// <exception cref="BadSourceReaderException">
	///     Gets Raised if the Start Character is not a String Quote or the Sequence is
	///     not properly terminated.
	/// </exception>
	public static BadStringToken ParseString(this BadSourceReader reader)
	{
		if (!reader.IsStringQuote() && !reader.IsStringQuote(0, true))
		{
			throw new BadSourceReaderException(
				$"Expected string start character but got '{(reader.IsEof() ? "EOF" : reader.GetCurrentChar())}'",
				reader.MakeSourcePosition(1));
		}

		bool singleQuote = reader.Is(BadStaticKeys.SingleQuote);
		int start = reader.CurrentIndex;
		reader.MoveNext();
		bool isEscaped = false;
		StringBuilder sb = new StringBuilder("\"");

		while (!reader.IsStringQuote(0, singleQuote))
		{
			if (reader.IsNewLine() || reader.IsEof())
			{
				throw new BadSourceReaderException("String not terminated",
					reader.MakeSourcePosition(start, reader.CurrentIndex - start));
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

		if (singleQuote)
		{
			reader.Eat(BadStaticKeys.SingleQuote);
		}
		else
		{
			reader.Eat(BadStaticKeys.Quote);
		}

		sb.Append("\"");

		return new BadStringToken(sb.ToString(), reader.MakeSourcePosition(start, reader.CurrentIndex - start));
	}

	public static BadStringToken ParseMultiLineString(this BadSourceReader reader)
	{
		int start = reader.CurrentIndex;
		reader.Eat(BadStaticKeys.MultiLineStringKey);

		StringBuilder sb = new StringBuilder("\"");

		while (!reader.IsStringQuote())
		{
			if (reader.IsEof())
			{
				throw new BadSourceReaderException("String not terminated",
					reader.MakeSourcePosition(start, reader.CurrentIndex - start));
			}


			sb.Append(reader.GetCurrentChar());

			reader.MoveNext();
		}

		reader.Eat(BadStaticKeys.Quote);

		sb.Append("\"");

		return new BadStringToken(sb.ToString(), reader.MakeSourcePosition(start, reader.CurrentIndex - start));
	}

	/// <summary>
	///     Returns true if the Current Character is any whitespace or newline characters
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <param name="offset">The Offset from the Current Reader Position</param>
	/// <returns>True if the Current Character is a Whitespace or Newline Character</returns>
	public static bool IsWhiteSpace(this BadSourceReader reader, int offset = 0)
	{
		return reader.Is(offset, BadStaticKeys.Whitespace) ||
		       reader.Is(offset, BadStaticKeys.NewLine);
	}

	/// <summary>
	///     Returns true if the last non-whitespace character is the specified character
	/// </summary>
	/// <param name="reader">The Reader Instance</param>
	/// <param name="c">The Character that should be matched</param>
	/// <returns>True if the character is the last non-whitespace character.</returns>
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
