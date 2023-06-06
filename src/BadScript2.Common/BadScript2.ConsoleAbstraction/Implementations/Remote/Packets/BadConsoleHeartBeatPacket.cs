namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

public class BadConsoleHeartBeatPacket : BadConsolePacket
{
	public static readonly BadConsoleHeartBeatPacket Packet = new BadConsoleHeartBeatPacket();

	private BadConsoleHeartBeatPacket() { }

	public override byte[] Serialize()
	{
		return new[]
		{
			(byte)BadConsolePacketType.HeartBeat
		};
	}

	public new static BadConsoleHeartBeatPacket Deserialize(byte[] data)
	{
		return Packet;
	}
}
