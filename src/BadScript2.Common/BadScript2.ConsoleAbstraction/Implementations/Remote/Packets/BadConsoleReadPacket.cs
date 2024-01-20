using System;
using System.Collections.Generic;
using System.Text;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
///     Gets used to send a Read Result to the server
/// </summary>
public class BadConsoleReadPacket : BadConsolePacket
{
	/// <summary>
	///     Constructs a new BadConsoleReadPacket instance
	/// </summary>
	/// <param name="message">The Line that was read</param>
	public BadConsoleReadPacket(string message)
    {
        Message = message;
    }

	/// <summary>
	///     The Line that was read
	/// </summary>
	public string Message { get; }

    public override byte[] Serialize()
    {
        List<byte> data = new List<byte>
        {
            (byte)BadConsolePacketType.Read,
        };
        byte[] message = Encoding.UTF8.GetBytes(Message);
        data.AddRange(BitConverter.GetBytes(message.Length));
        data.AddRange(message);

        return data.ToArray();
    }

    /// <summary>
    ///     Deserializes the Packet
    /// </summary>
    /// <param name="data">The Data Array</param>
    /// <returns>Bad Console Packet instance</returns>
    public new static BadConsoleReadPacket Deserialize(byte[] data)
    {
        int messageSize = BitConverter.ToInt32(data, 1);

        string message = Encoding.UTF8.GetString(data, sizeof(int) + 1, messageSize);

        return new BadConsoleReadPacket(message);
    }
}