using BadScript2.Common;
using BadScript2.IO;

namespace BadScript2.Reader;

public class BadSourceReader
{
    public readonly string FileName;
    private readonly string m_Source;

    public BadSourceReader(string fileName, string source)
    {
        FileName = fileName;
        m_Source = source;
    }

    public int CurrentIndex { get; private set; }
    public char CurrentChar => GetCurrentChar();

    public void SetPosition(int index)
    {
        CurrentIndex = index;
    }

    public static BadSourceReader FromFile(string fileName)
    {
        return new BadSourceReader(fileName, BadFileSystem.ReadAllText(fileName));
    }

    public bool IsEof(int offset = 0)
    {
        return CurrentIndex + offset >= m_Source.Length && CurrentIndex + offset >= 0;
    }

    public BadSourcePosition MakeSourcePosition(int length)
    {
        return MakeSourcePosition(CurrentIndex, length);
    }

    public BadSourcePosition MakeSourcePosition(int index, int length)
    {
        return BadSourcePosition.Create(FileName, m_Source, index, length);
    }

    public char GetCurrentChar(int offset = 0)
    {
        return IsEof(offset) ? '\0' : m_Source[CurrentIndex + offset];
    }

    public void MoveNext()
    {
        CurrentIndex++;
    }

    public bool Is(char c, int offset = 0)
    {
        return GetCurrentChar(offset) == c;
    }

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

    public bool Is(params char[] c)
    {
        return Is(0, c);
    }

    public bool Is(params string[] s)
    {
        return Is(0, s);
    }

    public bool Is(int offset, params char[] c)
    {
        return c.Any(x => Is(x, offset));
    }

    public bool Is(int offset, params string[] c)
    {
        return c.Any(x => Is(x, offset));
    }


    public BadSourcePosition Eat(char c)
    {
        if (!Is(c))
        {
            throw new BadSourceReaderException(
                $"Expected '{c}' but got '{(IsEof() ? "EOF" : GetCurrentChar())}'",
                MakeSourcePosition(1)
            );
        }

        MoveNext();

        return MakeSourcePosition(1);
    }

    public BadSourcePosition Eat(params char[] c)
    {
        if (c.Any(x => Is(x)))
        {
            MoveNext();

            return MakeSourcePosition(1);
        }

        throw new BadSourceReaderException(
            $"Expected '{string.Join("' or '", c)}' but got '{(IsEof() ? "EOF" : GetCurrentChar())}'",
            MakeSourcePosition(1)
        );
    }

    public BadSourcePosition Eat(string s)
    {
        int start = CurrentIndex;
        for (int i = 0; i < s.Length; i++)
        {
            if (!Is(s[i]))
            {
                throw new BadSourceReaderException(
                    $"Expected '{s}' but got '{(IsEof() ? "EOF" : GetCurrentChar())}'",
                    MakeSourcePosition(start, i)
                );
            }

            MoveNext();
        }

        return MakeSourcePosition(start, s.Length);
    }

    public BadSourcePosition Eat(params string[] s)
    {
        string? str = s.FirstOrDefault(x => Is(x));
        if (str == null)
        {
            throw new BadSourceReaderException(
                $"Expected '{string.Join("' or '", s)}' but got '{(IsEof() ? "EOF" : GetCurrentChar())}'",
                MakeSourcePosition(1)
            );
        }

        return Eat(str);
    }

    public void Seek(char c)
    {
        while (!IsEof() && GetCurrentChar() != c)
        {
            MoveNext();
        }
    }

    public void Seek(string s)
    {
        while (!IsEof() && !Is(s))
        {
            MoveNext();
        }
    }

    public void Seek(params char[] c)
    {
        while (!IsEof() && !Is(c))
        {
            MoveNext();
        }
    }

    public void Seek(params string[] s)
    {
        while (!IsEof() && !Is(s))
        {
            MoveNext();
        }
    }

    public void Skip(char c)
    {
        if (Is(c))
        {
            MoveNext();
        }
    }

    public void Skip(params char[] c)
    {
        bool exit = false;
        while (!exit)
        {
            exit = true;
            foreach (char ch in c)
            {
                if (Is(ch))
                {
                    MoveNext();
                    exit = false;

                    break;
                }
            }
        }
    }
}