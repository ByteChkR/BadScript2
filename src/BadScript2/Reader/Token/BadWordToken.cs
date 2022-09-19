using BadScript2.Common;

namespace BadScript2.Reader.Token;

/// <summary>
/// Implements a WordToken
/// </summary>
public class BadWordToken : BadToken
{
    /// <summary>
    /// Constructor for a WordToken
    /// </summary>
    /// <param name="position">The Source Position of the Token</param>
    public BadWordToken(BadSourcePosition position) : base(position) { }

    /// <summary>
    /// Creates an Instance of the Word Token from a String. This is used for Creating token without an underlying Source File.
    /// </summary>
    /// <param name="s">The String</param>
    /// <returns>BadWordToken Instance</returns>
    public static BadWordToken MakeWord(string s)
    {
        return new BadWordToken(BadSourcePosition.FromSource(s, 0, s.Length));
    }

    /// <summary>
    /// Implicit Operator that converts a String into a WordToken
    /// </summary>
    /// <param name="s">Word String</param>
    /// <returns>BadWordToken Instance containing the specified string.</returns>
    public static implicit operator BadWordToken(string s)
    {
        return MakeWord(s);
    }
}