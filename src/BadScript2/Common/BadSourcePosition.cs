namespace BadScript2.Common;

public class BadSourcePosition
{
    private BadSourcePosition(string? fileName, string source, int index, int length)
    {
        FileName = fileName?.Replace('\\', '/');
        Source = source;
        Index = index;
        Length = length;
    }

    private BadSourcePosition(int index, int length, string source)
    {
        Source = source;
        Index = index;
        Length = length;
    }

    private BadSourcePosition(string fileName, int index, int length) : this(
        fileName,
        File.ReadAllText(fileName),
        index,
        length
    ) { }

    public string? FileName { get; }
    public string Source { get; }
    public int Index { get; }
    public int Length { get; }

    public string Text => GetExcerpt(0);

    public static BadSourcePosition Create(string fileName, string source, int index, int length)
    {
        return new BadSourcePosition(fileName, source, index, length);
    }

    public static BadSourcePosition FromFile(string fileName, int index, int length)
    {
        return new BadSourcePosition(fileName, index, length);
    }

    public static BadSourcePosition FromSource(string source, int index, int length)
    {
        return new BadSourcePosition("<nofile>", source, index, length);
    }


    public string GetExcerpt(int len = 10)
    {
        return GetExcerpt(len, len);
    }


    public string GetExcerpt(int left, int right)
    {
        int start = Math.Max(0, Index - left);
        int end = Math.Min(Source.Length, Index + Length + right);

        return Source.Substring(start, end - start);
    }

    public string GetPositionInfo()
    {
        int line = 1;
        int lineStart = 0;
        for (int i = 0; i < Index; i++)
        {
            if (Source[i] == '\n')
            {
                line++;
                lineStart = i;
            }
        }

        return $"file://{FileName} : Line {line}";
    }

    public BadSourcePosition Combine(BadSourcePosition other)
    {
        if (FileName != other.FileName && Source != other.Source)
        {
            throw new InvalidOperationException("Cannot combine positions from different sources");
        }

        if (Index < other.Index)
        {
            return new BadSourcePosition(FileName, Source, Index, other.Index + other.Length - Index);
        }

        return new BadSourcePosition(FileName, Source, other.Index, Index + Length - other.Index);
    }
}