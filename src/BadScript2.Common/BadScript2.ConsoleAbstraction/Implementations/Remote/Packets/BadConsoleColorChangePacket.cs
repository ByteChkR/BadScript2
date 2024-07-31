using System;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
///     Gets used to change the color of the console
/// </summary>
public class BadConsoleColorChangePacket : BadConsolePacket
{
	/// <summary>
	///     Constructs a new BadConsoleColorChangePacket instance
	/// </summary>
	/// <param name="isBackground">Is the Changed color the background color?</param>
	/// <param name="color">The Color to change to</param>
	public BadConsoleColorChangePacket(bool isBackground, ConsoleColor color)
    {
        IsBackground = isBackground;
        Color = color;
    }

	/// <summary>
	///     The Color to change to
	/// </summary>
	public ConsoleColor Color { get; }

	/// <summary>
	///     Is the Changed color the background color?
	/// </summary>
	public bool IsBackground { get; }

	/// <summary>
	///     Deserializes the Packet
	/// </summary>
	/// <param name="data">The Data Array</param>
	/// <returns>Bad Console Packet instance</returns>
	public new static BadConsoleColorChangePacket Deserialize(byte[] data)
    {
        bool isBackground = data[1] == 1;
        ConsoleColor color = (ConsoleColor)data[2];

        return new BadConsoleColorChangePacket(isBackground, color);
    }

    /// <inheritdoc />
    public override byte[] Serialize()
    {
        return new[] { (byte)BadConsolePacketType.Color, (byte)(IsBackground ? 1 : 0), (byte)Color };
    }
}