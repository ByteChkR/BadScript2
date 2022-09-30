using System;
using System.Collections.Generic;
using System.Text;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets
{
    public class BadConsoleReadPacket : BadConsolePacket
    {
        public readonly string Message;

        public BadConsoleReadPacket(string message)
        {
            Message = message;
        }

        public override byte[] Serialize()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)BadConsolePacketType.Read);
            byte[] message = Encoding.UTF8.GetBytes(Message);
            data.AddRange(BitConverter.GetBytes(message.Length));
            data.AddRange(message);

            return data.ToArray();
        }

        public new static BadConsoleReadPacket Deserialize(byte[] data)
        {
            int messageSize = BitConverter.ToInt32(data, 1);

            string message = Encoding.UTF8.GetString(data, sizeof(int) + 1, messageSize);

            return new BadConsoleReadPacket(message);
        }
    }
}