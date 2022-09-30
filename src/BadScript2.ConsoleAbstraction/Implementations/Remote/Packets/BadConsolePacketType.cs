namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets
{
    public enum BadConsolePacketType : byte
    {
        Write,
        Read,
        Color,
        Clear,
        Disconnect,
        HeartBeat
    }
}