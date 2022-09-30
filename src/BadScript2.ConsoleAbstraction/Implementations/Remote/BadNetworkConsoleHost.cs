using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote
{
    public class BadNetworkConsoleHost : IBadConsole
    {
        private readonly ConcurrentQueue<BadConsoleReadPacket> m_IncomingPackets = new ConcurrentQueue<BadConsoleReadPacket>();
        private readonly TcpListener m_Listener;
        private readonly object m_Lock = new object();
        private readonly ConcurrentQueue<BadConsolePacket> m_OutgoingPackets = new ConcurrentQueue<BadConsolePacket>();
        private ConsoleColor m_BackgroundColor = Console.BackgroundColor;
        private TcpClient m_Client;
        private bool m_ExitRequested;


        private ConsoleColor m_ForegroundColor = Console.ForegroundColor;
        private Thread m_MessageThread;

        public BadNetworkConsoleHost(TcpListener listner)
        {
            m_Listener = listner;
        }

        public static int ReadSleepTimeout { get; set; } = 100;
        public static int ReceiveSleepTimeout { get; set; } = 100;

        public void Write(string str)
        {
            m_OutgoingPackets.Enqueue(new BadConsoleWritePacket(false, str));
        }

        public void WriteLine(string str)
        {
            m_OutgoingPackets.Enqueue(new BadConsoleWritePacket(true, str));
        }

        public string ReadLine()
        {
            BadConsoleReadPacket ret;
            while (!m_IncomingPackets.TryDequeue(out ret))
            {
                Thread.Sleep(ReadSleepTimeout);
            }

            return ret.Message;
        }

        public Task<string> ReadLineAsync()
        {
            return Task.Run(ReadLine);
        }

        public void Clear()
        {
            m_OutgoingPackets.Enqueue(BadConsoleClearPacket.Packet);
        }

        public ConsoleColor ForegroundColor
        {
            get => m_ForegroundColor;
            set
            {
                m_ForegroundColor = value;
                m_OutgoingPackets.Enqueue(new BadConsoleColorChangePacket(false, value));
            }
        }

        public ConsoleColor BackgroundColor
        {
            get => m_BackgroundColor;
            set
            {
                m_BackgroundColor = value;
                m_OutgoingPackets.Enqueue(new BadConsoleColorChangePacket(true, value));
            }
        }

        public void Start()
        {
            if (m_MessageThread != null)
            {
                throw new Exception("Message Thread already running");
            }

            lock (m_Lock)
            {
                m_ExitRequested = false;
            }

            m_MessageThread = new Thread(MessageThread);
            m_MessageThread.Start();
        }

        public void Stop()
        {
            if (m_MessageThread == null)
            {
                throw new Exception("Message Thread is not running");
            }

            lock (m_Lock)
            {
                m_ExitRequested = true;
            }
        }

        private void MessageThread()
        {
            while (!m_ExitRequested)
            {
                m_Listener.Start();
                BadConsole.WriteLine($"[Console Host] Waiting for Connection on {m_Listener.LocalEndpoint}");
                bool accepted = false;
                m_Listener.BeginAcceptTcpClient(
                    ar =>
                    {
                        m_Client = m_Listener.EndAcceptTcpClient(ar);
                        accepted = true;
                    },
                    null
                );
                while (!accepted && !m_ExitRequested)
                {
                    Thread.Sleep(100);
                }
                m_Listener.Stop();
                bool done;
                DateTime lastHeartBeat = DateTime.Now;
                while (!m_ExitRequested && m_Client.Connected)
                {
                    done = false;
                
                    if (m_Client.Available != 0)
                    {
                        done = true;
                        lastHeartBeat = DateTime.Now;
                        byte[] len = new byte[sizeof(int)];
                        NetworkStream stream = m_Client.GetStream();
                        int read = stream.Read(len, 0, len.Length);
                        if (read != len.Length)
                        {
                            throw new Exception("Invalid Packet Size");
                        }

                        byte[] packet = new byte[BitConverter.ToInt32(len, 0)];
                        read = stream.Read(packet, 0, packet.Length);

                        if (read != packet.Length)
                        {
                            throw new Exception("Invalid Packet");
                        }

                        BadConsolePacket packetObj = BadConsolePacket.Deserialize(packet);
                        if (packetObj is BadConsoleReadPacket rp)
                        {
                            m_IncomingPackets.Enqueue(rp);
                        }
                        else if(packetObj is BadConsoleHeartBeatPacket)
                        {
                            //Ignore Packet
                        }
                        else if (packetObj is BadConsoleDisconnectPacket)
                        {
                            m_Client.Dispose();
                        }
                        else
                        {
                            throw new Exception("Invalid Packet Type");
                        }
                    }

                    if (m_OutgoingPackets.Count != 0)
                    {
                        if (m_OutgoingPackets.TryDequeue(out BadConsolePacket packet))
                        {
                            done = true;
                            NetworkStream stream = m_Client.GetStream();
                            List<byte> packetData = new List<byte>();
                            byte[] packetBytes = packet.Serialize();
                            packetData.AddRange(BitConverter.GetBytes(packetBytes.Length));
                            packetData.AddRange(packetBytes);
                            stream.Write(packetData.ToArray(), 0, packetData.Count());
                        }
                    }

                    if (!done)
                    {
                        if(lastHeartBeat + TimeSpan.FromSeconds(5) < DateTime.Now)
                        {
                            m_Client.Dispose();
                        }
                        Thread.Sleep(ReceiveSleepTimeout);
                    }
                }
            }

            if (m_Client.Connected)
            {
                NetworkStream stream = m_Client.GetStream();
                List<byte> packetData = new List<byte>();
                byte[] packetBytes = BadConsoleDisconnectPacket.Packet.Serialize();
                packetData.AddRange(BitConverter.GetBytes(packetBytes.Length));
                packetData.AddRange(packetBytes);
                stream.Write(packetData.ToArray(), 0, packetData.Count());
                m_Client.Dispose();
            }

            m_MessageThread = null;
        }
    }
}