namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

public class BadConsoleDisconnectPacket : BadConsolePacket
{
	public static readonly BadConsoleDisconnectPacket Packet = new BadConsoleDisconnectPacket();

	private BadConsoleDisconnectPacket() { }

	public override byte[] Serialize()
	{
		return new[]
		{
			(byte)BadConsolePacketType.Disconnect
		};
	}

	public new static BadConsoleDisconnectPacket Deserialize(byte[] data)
	{
		return Packet;
	}
}
