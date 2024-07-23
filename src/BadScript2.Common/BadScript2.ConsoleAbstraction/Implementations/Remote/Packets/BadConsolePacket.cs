using System;
namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
///     Implements the base class for all BadConsole Packets
/// </summary>
public abstract class BadConsolePacket
{
	/// <summary>
	///     Deserializes a BadConsolePacket from the given data
	/// </summary>
	/// <param name="data">The Data Array</param>
	/// <returns>Bad Console Packet instance</returns>
	/// <exception cref="ArgumentOutOfRangeException">Gets raised if the packet could not be deserialized</exception>
	public static BadConsolePacket Deserialize(byte[] data)
    {
        BadConsolePacketType type = (BadConsolePacketType)data[0];

        return type switch
        {
            BadConsolePacketType.Write => BadConsoleWritePacket.Deserialize(data),
            BadConsolePacketType.Read => BadConsoleReadPacket.Deserialize(data),
            BadConsolePacketType.Color => BadConsoleColorChangePacket.Deserialize(data),
            BadConsolePacketType.Clear => BadConsoleClearPacket.Deserialize(data),
            BadConsolePacketType.Disconnect => BadConsoleDisconnectPacket.Deserialize(data),
            BadConsolePacketType.HeartBeat => BadConsoleHeartBeatPacket.Deserialize(data),
            BadConsolePacketType.Hello => BadConsoleHelloPacket.Deserialize(data),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

	/// <summary>
	///     Serializes the Packet to a byte array
	/// </summary>
	/// <returns>Data Array</returns>
	public abstract byte[] Serialize();
}