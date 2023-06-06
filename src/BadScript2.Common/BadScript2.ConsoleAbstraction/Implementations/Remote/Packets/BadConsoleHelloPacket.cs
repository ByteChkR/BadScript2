using System;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

public class BadConsoleHelloPacket : BadConsolePacket
{
	public BadConsoleHelloPacket(int heartBeatInterval)
	{
		HeartBeatInterval = heartBeatInterval;
	}

	public int HeartBeatInterval { get; }

	public new static BadConsoleHelloPacket Deserialize(byte[] data)
	{
		return new BadConsoleHelloPacket(BitConverter.ToInt32(data, 1));
	}

	public override byte[] Serialize()
	{
		byte[] data = new byte[5];
		data[0] = (byte)BadConsolePacketType.Hello;
		BitConverter.GetBytes(HeartBeatInterval).CopyTo(data, 1);

		return data;
	}
}
