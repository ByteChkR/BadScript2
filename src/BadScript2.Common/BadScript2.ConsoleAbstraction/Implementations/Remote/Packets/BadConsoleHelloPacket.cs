using System;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
///     Gets used as handshake between the client and the server
/// </summary>
public class BadConsoleHelloPacket : BadConsolePacket
{
	/// <summary>
	///     Constructs a new BadConsoleHelloPacket instance
	/// </summary>
	/// <param name="heartBeatInterval">The heartbeat intervall</param>
	public BadConsoleHelloPacket(int heartBeatInterval)
    {
        HeartBeatInterval = heartBeatInterval;
    }

	/// <summary>
	///     The Heartbeat interval that the server uses.
	/// </summary>
	public int HeartBeatInterval { get; }

	/// <summary>
	///     Deserializes the Packet
	/// </summary>
	/// <param name="data">The Data Array</param>
	/// <returns>Bad Console Packet instance</returns>
	public new static BadConsoleHelloPacket Deserialize(byte[] data)
    {
        return new BadConsoleHelloPacket(BitConverter.ToInt32(data, 1));
    }

	/// <inheritdoc />
    public override byte[] Serialize()
    {
        byte[] data = new byte[5];
        data[0] = (byte)BadConsolePacketType.Hello;
        BitConverter.GetBytes(HeartBeatInterval).CopyTo(data, 1);

        return data;
    }
}