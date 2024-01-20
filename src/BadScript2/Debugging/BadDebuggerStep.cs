using System.Text;

using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Utility;

namespace BadScript2.Debugging;

/// <summary>
///     Represents a Debugging Step
/// </summary>
public readonly struct BadDebuggerStep : IEquatable<BadDebuggerStep>
{
	/// <summary>
	///     The Source of the Step
	/// </summary>
	public readonly object? StepSource;

	/// <summary>
	///     The Execution Context of where the Step was executed
	/// </summary>
	public readonly BadExecutionContext Context;

	/// <summary>
	///     The Source Position of the Step
	/// </summary>
	public readonly BadSourcePosition Position;

	/// <summary>
	///     Creates a new Debugger Step
	/// </summary>
	/// <param name="context">The Execution Context of where the Step was executed</param>
	/// <param name="position">The Source Position of the Step</param>
	/// <param name="stepSource">The Source of the Step</param>
	public BadDebuggerStep(BadExecutionContext context, BadSourcePosition position, object? stepSource)
    {
        Context = context;
        Position = position;
        StepSource = stepSource;
    }

	/// <summary>
	///     Returns a string representation of the Step
	/// </summary>
	/// <returns>String representation</returns>
	public string GetInfo()
    {
        return
            $"######################################\nDebug Step at {Position.GetPositionInfo()}\n\nStepSource: {StepSource}\n\nContext: {Context.Scope.Name}: {Context.Scope}\n\n######################################\n";
    }


	/// <summary>
	///     Returns a line listing of the Step
	/// </summary>
	/// <param name="breakpoints">The Line numbers of the breakpoints</param>
	/// <param name="topInSource">Indicates the start line of the excerpt</param>
	/// <param name="lineInSource">Indicates Current line of the code</param>
	/// <param name="lineDelta">The Amount of lines before and after the Source Position</param>
	/// <returns>String Representation</returns>
	public string GetSourceView(int[] breakpoints, out int topInSource, out int lineInSource, int lineDelta = 4)
    {
        return GetSourceView(lineDelta, lineDelta, breakpoints, out topInSource, out lineInSource);
    }


	/// <summary>
	///     Returns a line listing of the Step
	/// </summary>
	/// <param name="breakpoints">The Line numbers of the breakpoints</param>
	/// <param name="topInSource">Indicates the start line of the excerpt</param>
	/// <param name="lineInSource">Indicates Current line of the code</param>
	/// <param name="top">The Amount of lines before the Source Position</param>
	/// <param name="bottom">The Amount of lines after the Source Position</param>
	/// <returns>String Representation</returns>
	public string GetSourceView(int top, int bottom, int[] breakpoints, out int topInSource, out int lineInSource)
    {
        StringBuilder sb = new StringBuilder($"File: {Position.FileName}\n");
        string[] lines = GetLines(top, bottom, out topInSource, out lineInSource);

        for (int i = 0; i < lines.Length; i++)
        {
            int ln = topInSource + i;
            string line = lines[i];
            string prefix = ln.ToString();

            if (ln == lineInSource)
            {
                prefix = ">>";
            }
            else
            {
	            if (breakpoints.Any(breakpoint => ln == breakpoint))
	            {
		            prefix = "@";
	            }
            }

            sb.AppendLine($"{prefix}\t| {line}");
        }

        return sb.ToString();
    }

	/// <summary>
	///     Returns string representation of the Step
	/// </summary>
	/// <returns>String representation</returns>
	public override string ToString()
    {
        return GetInfo();
    }

	/// <summary>
	///     Returns a line excerpt of the Step
	/// </summary>
	/// <param name="topInSource">Indicates the start line of the excerpt</param>
	/// <param name="lineInSource">Indicates Current line of the code</param>
	/// <param name="top">The Amount of lines before the Source Position</param>
	/// <param name="bottom">The Amount of lines after the Source Position</param>
	/// <returns>String Representation</returns>
	public string[] GetLines(int top, int bottom, out int topInSource, out int lineInSource)
    {
        lineInSource = 1;

        for (int i = 0; i < Position.Index; i++)
        {
            if (Position.Source[i] == '\n')
            {
                lineInSource++;
            }
        }

        topInSource = Math.Max(1, lineInSource - top);

        string[] lines = Position.Source.Split('\n');


        List<string> lns = new List<string>();

        for (int i = topInSource - 1;
             i < topInSource - 1 + Math.Min(top + bottom, lines.Length - (topInSource - 1));
             i++)
        {
            lns.Add(lines[i]);
        }

        return lns.ToArray();
    }

	/// <summary>
	///     Returns true if the Step is equal to another object
	/// </summary>
	/// <param name="other">The other object</param>
	/// <returns>True if equal</returns>
	public bool Equals(BadDebuggerStep other)
    {
        return Equals(StepSource, other.StepSource) && Context.Equals(other.Context) && Position.Equals(other.Position);
    }

	/// <summary>
	///     Returns true if the Step is equal to another object
	/// </summary>
	/// <param name="obj">The other object</param>
	/// <returns>True if equal</returns>
	public override bool Equals(object? obj)
    {
        return obj is BadDebuggerStep other && Equals(other);
    }

	/// <summary>
	///     Returns the Hash Code of the Step
	/// </summary>
	/// <returns></returns>
	public override int GetHashCode()
    {
        return BadHashCode.Combine(StepSource, Context, Position);
    }

	/// <summary>
	///     Implements the == operator
	/// </summary>
	/// <param name="left">The left side</param>
	/// <param name="right">The right side</param>
	/// <returns>True if equal</returns>
	public static bool operator ==(BadDebuggerStep left, BadDebuggerStep right)
    {
        return left.Equals(right);
    }

	/// <summary>
	///     Implements the != operator
	/// </summary>
	/// <param name="left">The left side</param>
	/// <param name="right">The right side</param>
	/// <returns>True if not equal</returns>
	public static bool operator !=(BadDebuggerStep left, BadDebuggerStep right)
    {
        return !left.Equals(right);
    }
}