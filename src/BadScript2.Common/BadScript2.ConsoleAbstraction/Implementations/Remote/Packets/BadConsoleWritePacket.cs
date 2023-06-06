using System;
using System.Collections.Generic;
using System.Text;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

public class BadConsoleWritePacket : BadConsolePacket
{
	public BadConsoleWritePacket(bool isWriteLine, string message)
	{
		IsWriteLine = isWriteLine;
		Message = message;
	}

	public bool IsWriteLine { get; }

	public string Message { get; }

	public new static BadConsoleWritePacket Deserialize(byte[] data)
	{
		bool isWriteLine = data[1] == 1;
		int messageSize = BitConverter.ToInt32(data, 2);

		string message = Encoding.UTF8.GetString(data, sizeof(int) + 2, messageSize);

		return new BadConsoleWritePacket(isWriteLine, message);
	}

	public override byte[] Serialize()
	{
		List<byte> data = new List<byte>();
		data.Add((byte)BadConsolePacketType.Write);
		data.Add((byte)(IsWriteLine ? 1 : 0));
		byte[] message = Encoding.UTF8.GetBytes(Message);
		data.AddRange(BitConverter.GetBytes(message.Length));
		data.AddRange(message);

		return data.ToArray();
	}
}
