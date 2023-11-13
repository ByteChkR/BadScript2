using BadScript2.Utility;

namespace BadScript2.Common.Logging;

/// <summary>
///     Represents a Log Message
/// </summary>
public struct BadLog : IEquatable<BadLog>
{
	/// <summary>
	///     The Contents of the Message
	/// </summary>
	public readonly string Message;

	/// <summary>
	///     The Mask of the message
	/// </summary>
	public readonly BadLogMask Mask;

	/// <summary>
	///     The Type of the message
	/// </summary>
	public readonly BadLogType Type;

	/// <summary>
	///     The (optional) position of the message
	/// </summary>
	public readonly BadSourcePosition? Position;

	/// <summary>
	///     Creates a new Log Message
	/// </summary>
	/// <param name="message">The message</param>
	/// <param name="mask">The mask of the message</param>
	/// <param name="position">The source position of the message</param>
	/// <param name="type">The Log Type</param>
	public BadLog(
		string message,
		BadLogMask? mask = null,
		BadSourcePosition? position = null,
		BadLogType type = BadLogType.Log)
	{
		Message = message;
		Type = type;
		Position = position;
		Mask = mask ?? BadLogMask.Default;
	}

	/// <summary>
	///     Converts a string message to a log object
	/// </summary>
	/// <param name="message">The log content</param>
	/// <returns>BadLog Instance</returns>
	public static implicit operator BadLog(string message)
	{
		return new BadLog(message);
	}

	/// <summary>
	///     Returns a string representation of the log
	/// </summary>
	/// <returns>String representation of the log</returns>
	public override string ToString()
	{
		if (Position != null)
		{
			return $"[{Type}][{Mask}] {Message} at {Position.GetPositionInfo()}";
		}

		return $"[{Type}][{Mask}] {Message}";
	}

	/// <summary>
	///     Returns true if the log is equal to the other log
	/// </summary>
	/// <param name="other">Other Log</param>
	/// <returns>True if the log is equal to the other log</returns>
	public bool Equals(BadLog other)
	{
		return Message == other.Message && Mask.Equals(other.Mask) && Type == other.Type;
	}

	/// <summary>
	///     Returns true if the log is equal to the other object
	/// </summary>
	/// <param name="obj">Other Object</param>
	/// <returns>True if the log is equal to the other log</returns>
	public override bool Equals(object? obj)
	{
		return obj is BadLog other && Equals(other);
	}


	/// <summary>
	///     Returns the Hash Code for this Instance
	/// </summary>
	/// <returns>Hash Code</returns>
	public override int GetHashCode()
	{
		return BadHashCode.Combine(Message, Mask, (int)Type);
	}

	public static bool operator ==(BadLog left, BadLog right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(BadLog left, BadLog right)
	{
		return !left.Equals(right);
	}
}
