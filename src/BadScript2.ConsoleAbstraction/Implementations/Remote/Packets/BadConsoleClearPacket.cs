namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets
{
    public class BadConsoleClearPacket : BadConsolePacket
    {
        public static readonly BadConsoleClearPacket Packet = new BadConsoleClearPacket();
        private BadConsoleClearPacket() { }

        public new static BadConsoleClearPacket Deserialize(byte[] data)
        {
            return Packet;
        }

        public override byte[] Serialize()
        {
            return new[] { (byte)BadConsolePacketType.Clear };
        }
    }
}