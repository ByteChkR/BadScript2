using System;
using System.Collections.Generic;
using System.Text;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
///     Gets sent to the client to write a message to the console
/// </summary>
public class BadConsoleWritePacket : BadConsolePacket
{
	/// <summary>
	///     Constructs a new BadConsoleWritePacket instance
	/// </summary>
	/// <param name="isWriteLine">Should a newline be appended?</param>
	/// <param name="message">The Message to be written</param>
	public BadConsoleWritePacket(bool isWriteLine, string message)
    {
        IsWriteLine = isWriteLine;
        Message = message;
    }

	/// <summary>
	///     Should a newline be appended?
	/// </summary>
	public bool IsWriteLine { get; }

	/// <summary>
	///     The Message to be written
	/// </summary>
	public string Message { get; }

	/// <summary>
	///     Deserializes the Packet
	/// </summary>
	/// <param name="data">The Data Array</param>
	/// <returns>Bad Console Packet instance</returns>
	public new static BadConsoleWritePacket Deserialize(byte[] data)
    {
        bool isWriteLine = data[1] == 1;
        int messageSize = BitConverter.ToInt32(data, 2);

        string message = Encoding.UTF8.GetString(data, sizeof(int) + 2, messageSize);

        return new BadConsoleWritePacket(isWriteLine, message);
    }

    public override byte[] Serialize()
    {
        List<byte> data = new List<byte>
        {
	        (byte)BadConsolePacketType.Write,
	        (byte)(IsWriteLine ? 1 : 0),
        };
        byte[] message = Encoding.UTF8.GetBytes(Message);
        data.AddRange(BitConverter.GetBytes(message.Length));
        data.AddRange(message);

        return data.ToArray();
    }
}