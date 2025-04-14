using BadScript2.Common;
using BadScript2.IO;

/// <summary>
/// Contains the Source Reader for the BadScript2 Language
/// </summary>
namespace BadScript2.Reader;

/// <summary>
///     Implements the Source Code Reader
/// </summary>
public class BadSourceReader
{
    /// <summary>
    /// The End Index of the Source Code
    /// </summary>
    private readonly int m_EndIndex;

    /// <summary>
    ///     The Source Code
    /// </summary>
    private readonly string m_Source;

    /// <summary>
    ///     Start Index of the Source Code
    /// </summary>
    private readonly int m_StartIndex;


    /// <summary>
    ///     Creates a new Source Code Reader
    /// </summary>
    /// <param name="fileName">Filename of the Source Code</param>
    /// <param name="source">The Source Code</param>
    /// <param name="start">The Start Index of the Source Code</param>
    /// <param name="end">The End Index of the Source Code</param>
    /// <exception cref="ArgumentOutOfRangeException">Gets raised if the start or end index is invalid.</exception>
    public BadSourceReader(string fileName, string source, int start, int end)
    {
        FileName = fileName;
        m_Source = source;

        if (start < 0 || start >= end)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        if (end > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(end));
        }

        m_StartIndex = start;
        m_EndIndex = end;
        CurrentIndex = start;
    }

    /// <summary>
    ///     Creates a new Source Code Reader
    /// </summary>
    /// <param name="fileName">The Filename of the Source Code</param>
    /// <param name="source">The Source Code</param>
    public BadSourceReader(string fileName, string source) : this(fileName, source, 0, source.Length) { }

    /// <summary>
    ///     Preview of the Source Code
    /// </summary>
    public string Preview => IsEof() ? "END OF FILE" : Source.Substring(CurrentIndex);

    /// <summary>
    ///     The Source Code
    /// </summary>
    public string Source => m_Source.Substring(m_StartIndex, m_EndIndex - m_StartIndex);

    /// <summary>
    ///     The Filename of the Source Code
    /// </summary>
    public string FileName { get; }

    /// <summary>
    ///     The Current Index of the Reader
    /// </summary>
    public int CurrentIndex { get; private set; }

    /// <summary>
    ///     The Current Character of the Reader
    /// </summary>
    public char CurrentChar => GetCurrentChar();

    /// <summary>
    ///     Sets the Current Index of the Reader
    /// </summary>
    /// <param name="index">The new Index</param>
    public void SetPosition(int index)
    {
        CurrentIndex = index;
    }

    /// <summary>
    ///     Creates a new Source Code Reader from a File
    /// </summary>
    /// <param name="fileName">The File Name</param>
    /// <returns>BadSourceReader instance with the contents of the specified file.</returns>
    public static BadSourceReader FromFile(string fileName)
    {
        return new BadSourceReader(fileName, BadFileSystem.ReadAllText(fileName));
    }

    /// <summary>
    ///     Returns true if the reader is at the end of the source code
    /// </summary>
    /// <param name="offset">The Offset from the Current Reader Position</param>
    /// <returns>True if the Reader is at the end of the source code.</returns>
    public bool IsEof(int offset = 0)
    {
        return CurrentIndex + offset >= m_EndIndex && CurrentIndex + offset >= m_StartIndex;
    }

    /// <summary>
    ///     Creates a source position with the specified length and the current index of the reader.
    /// </summary>
    /// <param name="length">The Length of the Position</param>
    /// <returns>A new BadSourcePosition Instance.</returns>
    public BadSourcePosition MakeSourcePosition(int length)
    {
        return MakeSourcePosition(CurrentIndex, length);
    }

    /// <summary>
    ///     Creates a source position with the specified length and index.
    /// </summary>
    /// <param name="index">The Start index</param>
    /// <param name="length">The Length of the Position</param>
    /// <returns>A new BadSourcePosition Instance.</returns>
    public BadSourcePosition MakeSourcePosition(int index, int length)
    {
        return BadSourcePosition.Create(FileName, m_Source, index, length);
    }

    /// <summary>
    ///     Returns the Current Character
    /// </summary>
    /// <param name="offset">The Offset from the Current Reader Position</param>
    /// <returns>Current Character. EOF if IsEOF(offset) equals true.</returns>
    public char GetCurrentChar(int offset = 0)
    {
        return IsEof(offset) ? '\0' : m_Source[CurrentIndex + offset];
    }

    /// <summary>
    ///     Moves the Reader to the next character in the source code.
    /// </summary>
    public void MoveNext()
    {
        CurrentIndex++;
    }

    /// <summary>
    ///     Returns true if the current character matches the specified character.
    /// </summary>
    /// <param name="c">The Character to be matched.</param>
    /// <param name="offset">The Offset from the Current Reader Position</param>
    /// <returns>True if the Character matches the current one.</returns>
    public bool Is(char c, int offset = 0)
    {
        return GetCurrentChar(offset) == c;
    }

    /// <summary>
    ///     Returns true if the string matches the specified character.
    /// </summary>
    /// <param name="s">The Character to be matched.</param>
    /// <param name="offset">The Offset from the Current Reader Position</param>
    /// <returns>True if the string matches the current one.</returns>
    public bool Is(string s, int offset = 0)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (!Is(s[i], offset + i))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Returns true if any of the characters match the specified character.
    /// </summary>
    /// <param name="chars">The Characters to be matched.</param>
    /// <returns>True if any of the characters  matches the current one.</returns>
    public bool Is(params char[] chars)
    {
        return Is(0, chars);
    }

    /// <summary>
    ///     Returns true if any of the strings match the specified character.
    /// </summary>
    /// <param name="s">The strings to be matched.</param>
    /// <returns>True if any of the strings  matches the current one.</returns>
    public bool Is(params string[] s)
    {
        return Is(0, s);
    }

    /// <summary>
    ///     Returns true if any of the characters match the specified character.
    /// </summary>
    /// <param name="offset">The Offset from the Current Reader Position</param>
    /// <param name="chars">The Characters to be matched.</param>
    /// <returns>True if any of the characters match the current one.</returns>
    public bool Is(int offset, params char[] chars)
    {
        return chars.Any(x => Is(x, offset));
    }

    /// <summary>
    ///     Returns true if any of the strings match the specified String.
    /// </summary>
    /// <param name="offset">The Offset from the Current Reader Position</param>
    /// <param name="s">The strings to be matched.</param>
    /// <returns>True if any of the strings match the current one.</returns>
    public bool Is(int offset, params string[] s)
    {
        return s.Any(x => Is(x, offset));
    }


    /// <summary>
    ///     Asserts that the current character matches the specified character.
    /// </summary>
    /// <param name="c">The Character to be matched</param>
    /// <returns>The Source Position (Current Reader position with length 1) of the Character</returns>
    /// <exception cref="BadSourceReaderException">Gets raised if the character does not match the specified one</exception>
    public BadSourcePosition Eat(char c)
    {
        if (!Is(c))
        {
            throw new BadSourceReaderException($"Expected '{c}' but got '{(IsEof() ? "EOF" : GetCurrentChar())}'",
                                               MakeSourcePosition(1)
                                              );
        }

        MoveNext();

        return MakeSourcePosition(1);
    }

    /// <summary>
    ///     Asserts that the current character matches one of the specified characters.
    /// </summary>
    /// <param name="c">The Characters to be matched</param>
    /// <returns>The Source Position (Current Reader position with length 1) of the Character</returns>
    /// <exception cref="BadSourceReaderException">Gets raised if the character does not match any of the specified ones</exception>
    public BadSourcePosition Eat(params char[] c)
    {
        if (!c.Any(x => Is(x)))
        {
            throw new
                BadSourceReaderException($"Expected '{string.Join("' or '", c)}' but got '{(IsEof() ? "EOF" : GetCurrentChar())}'",
                                         MakeSourcePosition(1)
                                        );
        }

        MoveNext();

        return MakeSourcePosition(1);
    }

    /// <summary>
    ///     Asserts that the current String matches the specified String.
    /// </summary>
    /// <param name="s">The String to be matched</param>
    /// <returns>The Source Position (Current Reader position with length of the string) of the Character</returns>
    /// <exception cref="BadSourceReaderException">Gets raised if the String does not match the specified one</exception>
    public BadSourcePosition Eat(string s)
    {
        int start = CurrentIndex;

        for (int i = 0; i < s.Length; i++)
        {
            if (!Is(s[i]))
            {
                throw new BadSourceReaderException($"Expected '{s}' but got '{(IsEof() ? "EOF" : GetCurrentChar())}'",
                                                   MakeSourcePosition(start, i)
                                                  );
            }

            MoveNext();
        }

        return MakeSourcePosition(start, s.Length);
    }

    /// <summary>
    ///     Asserts that the current string matches one of the specified strings.
    /// </summary>
    /// <param name="s">The strings to be matched</param>
    /// <returns>The Source Position (Current Reader position with length of the string) of the string</returns>
    /// <exception cref="BadSourceReaderException">Gets raised if the string does not match any of the specified ones</exception>
    public BadSourcePosition Eat(params string[] s)
    {
        string? str = s.FirstOrDefault(x => Is(x));

        if (str == null)
        {
            throw new
                BadSourceReaderException($"Expected '{string.Join("' or '", s)}' but got '{(IsEof() ? "EOF" : GetCurrentChar())}'",
                                         MakeSourcePosition(1)
                                        );
        }

        return Eat(str);
    }

    /// <summary>
    ///     Skips over any character that is not equal to the specified character.
    /// </summary>
    /// <param name="c">The character to be matched.</param>
    public void Seek(char c)
    {
        while (!IsEof() && GetCurrentChar() != c)
        {
            MoveNext();
        }
    }

    /// <summary>
    ///     Skips over any string that is not equal to the specified string.
    /// </summary>
    /// <param name="s">The string to be matched.</param>
    public void Seek(string s)
    {
        while (!IsEof() && !Is(s))
        {
            MoveNext();
        }
    }

    /// <summary>
    ///     Skips over any character that is not equal to any of the specified characters.
    /// </summary>
    /// <param name="c">The characters to be matched.</param>
    public void Seek(params char[] c)
    {
        while (!IsEof() && !Is(c))
        {
            MoveNext();
        }
    }

    /// <summary>
    ///     Skips over any character that is not equal to any of the specified strings.
    /// </summary>
    /// <param name="s">The strings to be matched.</param>
    public void Seek(params string[] s)
    {
        while (!IsEof() && !Is(s))
        {
            MoveNext();
        }
    }

    /// <summary>
    ///     If the current character is equal to the specified character, it will be skipped.
    /// </summary>
    /// <param name="c">The character to be matched.</param>
    public void Skip(char c)
    {
        if (Is(c))
        {
            MoveNext();
        }
    }

    /// <summary>
    ///     If the current character is equal to any of the specified characters, it will be skipped.
    /// </summary>
    /// <param name="c">The characters to be matched.</param>
    public void Skip(params char[] c)
    {
        bool exit = false;

        while (!exit)
        {
            exit = true;

            if (!c.Any(ch => Is(ch)))
            {
                continue;
            }

            MoveNext();
            exit = false;
        }
    }
}