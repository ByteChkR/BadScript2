using BadScript2.IO;

namespace BadScript2.Common;

/// <summary>
///     Describes a specific position inside a source file
/// </summary>
public class BadSourcePosition
{
    private string? m_PositionInfo;

    /// <summary>
    ///     Constructor for a Source Position
    /// </summary>
    /// <param name="fileName">The (optional but recommended) filename</param>
    /// <param name="source">The source code.</param>
    /// <param name="index">The Start Index</param>
    /// <param name="length">The Length</param>
    public BadSourcePosition(string? fileName, string source, int index, int length)
    {
        FileName = fileName?.Replace('\\', '/');
        Source = source;
        Index = index;
        Length = length;
    }


    /// <summary>
    ///     Constructor for a Source Position
    /// </summary>
    /// <param name="fileName">The filename</param>
    /// <param name="index">The Start Index</param>
    /// <param name="length">The Length</param>
    private BadSourcePosition(string fileName, int index, int length) : this(
        fileName,
        BadFileSystem.ReadAllText(fileName),
        index,
        length
    ) { }

    /// <summary>
    ///     The Filename of the Source Code.
    /// </summary>
    public string? FileName { get; }

    /// <summary>
    ///     The Source Code
    /// </summary>
    public string Source { get; }

    /// <summary>
    ///     The Start Index of the Position
    /// </summary>
    public int Index { get; }

    /// <summary>
    ///     The Length of the Position
    /// </summary>
    public int Length { get; }

    private string? m_Text;
    /// <summary>
    ///     Returns the Position as a string.
    /// </summary>
    public string Text => m_Text ??= GetExcerpt(0);

    /// <summary>
    ///     Creates a new Source Position
    /// </summary>
    /// <param name="fileName">The (optional but recommended) filename</param>
    /// <param name="source">The source code.</param>
    /// <param name="index">The Start Index</param>
    /// <param name="length">The Length</param>
    /// <returns>Created SourcePosition</returns>
    public static BadSourcePosition Create(string fileName, string source, int index, int length)
    {
        return new BadSourcePosition(fileName, source, index, length);
    }

    /// <summary>
    ///     Creates a new Source Position
    /// </summary>
    /// <param name="fileName">The filename</param>
    /// <param name="index">The Start Index</param>
    /// <param name="length">The Length</param>
    /// <returns>Created SourcePosition</returns>
    public static BadSourcePosition FromFile(string fileName, int index, int length)
    {
        return new BadSourcePosition(fileName, index, length);
    }

    /// <summary>
    ///     Creates a new Source Position
    /// </summary>
    /// <param name="source">The source code.</param>
    /// <param name="index">The Start Index</param>
    /// <param name="length">The Length</param>
    /// <returns>Created SourcePosition</returns>
    public static BadSourcePosition FromSource(string source, int index, int length)
    {
        return new BadSourcePosition("<nofile>", source, index, length);
    }


    /// <summary>
    ///     Returns the excerpt of the source code.
    /// </summary>
    /// <param name="len">The additional Characters before and after the excerpt</param>
    /// <returns>String Excerpt</returns>
    public string GetExcerpt(int len = 10)
    {
        return GetExcerpt(len, len);
    }


    /// <summary>
    ///     Returns the excerpt of the source code.
    /// </summary>
    /// <param name="left">The additional Characters before the excerpt</param>
    /// <param name="right">The additional Characters after the excerpt</param>
    /// <returns>String Excerpt</returns>
    public string GetExcerpt(int left, int right)
    {
        int start = Math.Max(0, Index - left);
        int end = Math.Min(Source.Length, Index + Length + right);

        return Source.Substring(start, end - start);
    }

    /// <summary>
    ///     Returns position info.
    ///     Format: file://[FileName] : Line [Line]
    /// </summary>
    /// <returns>String Representation</returns>
    public string GetPositionInfo()
    {
        if (m_PositionInfo != null)
        {
            return m_PositionInfo;
        }

        int line = 1;

        for (int i = 0; i < Index; i++)
        {
            if (Source[i] == '\n')
            {
                line++;
            }
        }

        m_PositionInfo = $"file://{FileName} : Line {line}";

        return m_PositionInfo;
    }

    /// <summary>
    ///     Combines two Source Positions
    /// </summary>
    /// <param name="other">The Other position</param>
    /// <returns>Combined Source Position</returns>
    /// <exception cref="InvalidOperationException">Gets raised if the filenames do not match</exception>
    public BadSourcePosition Combine(BadSourcePosition other)
    {
        if (FileName != other.FileName && Source != other.Source)
        {
            throw new InvalidOperationException("Cannot combine positions from different sources");
        }

        return Index < other.Index
            ? new BadSourcePosition(FileName, Source, Index, other.Index + other.Length - Index)
            : new BadSourcePosition(FileName, Source, other.Index, Index + Length - other.Index);
    }
}