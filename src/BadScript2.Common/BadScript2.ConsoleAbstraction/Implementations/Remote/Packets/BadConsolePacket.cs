using System;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
/// Implements the base class for all BadConsole Packets
/// </summary>
public abstract class BadConsolePacket
{
	/// <summary>
	/// Deserializes a BadConsolePacket from the given data
	/// </summary>
	/// <param name="data">The Data Array</param>
	/// <returns>Bad Console Packet instance</returns>
	/// <exception cref="ArgumentOutOfRangeException">Gets raised if the packet could not be deserialized</exception>
	public static BadConsolePacket Deserialize(byte[] data)
	{
		BadConsolePacketType type = (BadConsolePacketType)data[0];

		switch (type)
		{
			case BadConsolePacketType.Write:
				return BadConsoleWritePacket.Deserialize(data);
			case BadConsolePacketType.Read:
				return BadConsoleReadPacket.Deserialize(data);
			case BadConsolePacketType.Color:
				return BadConsoleColorChangePacket.Deserialize(data);
			case BadConsolePacketType.Clear:
				return BadConsoleClearPacket.Deserialize(data);
			case BadConsolePacketType.Disconnect:
				return BadConsoleDisconnectPacket.Deserialize(data);
			case BadConsolePacketType.HeartBeat:
				return BadConsoleHeartBeatPacket.Deserialize(data);
			case BadConsolePacketType.Hello:
				return BadConsoleHelloPacket.Deserialize(data);
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	/// <summary>
	/// Serializes the Packet to a byte array
	/// </summary>
	/// <returns>Data Array</returns>
	public abstract byte[] Serialize();
}
