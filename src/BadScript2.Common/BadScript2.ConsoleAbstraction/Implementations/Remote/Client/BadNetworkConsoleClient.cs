using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands;
using BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client;

/// <summary>
///     Implements the Client for the Remote Console
/// </summary>
public class BadNetworkConsoleClient
{
	/// <summary>
	///     The Address of the Remote Console
	/// </summary>
	private readonly string m_Address;

	/// <summary>
	///     The TCP Client
	/// </summary>
	private readonly TcpClient m_Client;

	/// <summary>
	///     The Command List
	/// </summary>
	private readonly List<BadNetworkConsoleClientCommand> m_Commands = new List<BadNetworkConsoleClientCommand>();

	/// <summary>
	///     The Port of the Remote Console
	/// </summary>
	private readonly int m_Port;

	/// <summary>
	///     If true the client will exit
	/// </summary>
	private bool m_ExitRequested;

	/// <summary>
	///     The Read Thread
	/// </summary>
	private Thread? m_ReadThread;

	/// <summary>
	///     Creates a new Client from the given TcpClient
	/// </summary>
	/// <param name="client">The TCP CLient</param>
	public BadNetworkConsoleClient(TcpClient client)
    {
        m_Client = client;
        m_Address = "";
        m_Port = -1;
    }

	/// <summary>
	///     Creates a new Client from the given Address and Port
	/// </summary>
	/// <param name="address">The Address of the Remote Console</param>
	/// <param name="port">The Port of the Remote Console</param>
	public BadNetworkConsoleClient(string address, int port)
    {
        m_Address = address;
        m_Port = port;
        m_Client = new TcpClient();
        AddCommand(new BadNetworkConsoleClientDisconnectCommand(this));
        AddCommand(new BadNetworkConsoleClientListCommand(this));
    }

	/// <summary>
	///     The Interval in which the Heartbeat is sent
	/// </summary>
	public static int HeartBeatSendInterval { get; set; } = 3000;

	/// <summary>
	///     The Interval in which packets are read from the server
	/// </summary>
	public static int ConsoleReadInputSleep { get; set; } = 100;

	/// <summary>
	///     The Interval in which packets are sent to the server
	/// </summary>
	public static int ConsoleWriteSleep { get; set; } = 100;

	/// <summary>
	///     Enumerable of all Commands
	/// </summary>
	public IEnumerable<BadNetworkConsoleClientCommand> Commands => m_Commands;

	/// <summary>
	///     Adds a Command to the Command List
	/// </summary>
	/// <param name="command">The Command to be added</param>
	public void AddCommand(BadNetworkConsoleClientCommand command)
    {
        m_Commands.Add(command);
    }

	/// <summary>
	///     Starts the Client
	/// </summary>
	public void Start()
    {
        m_ExitRequested = false;
        BadConsole.WriteLine($"[Console Client] Connecting to {m_Address}:{m_Port}");

        if (!m_Client.Connected)
        {
            m_Client.Connect(m_Address, m_Port);
        }

        BadConsole.WriteLine("[Console Client] Connected");


        m_ReadThread = new Thread(Write);
        m_ReadThread.Start();
        Read();
    }

	/// <summary>
	///     Stops the Client
	/// </summary>
	public void Stop()
    {
        m_ExitRequested = true;
    }

	/// <summary>
	///     Processes the given Packet
	/// </summary>
	/// <param name="packet">The Packet</param>
	/// <exception cref="BadNetworkConsoleException">Gets raised if the packet could not be processed</exception>
	private void ProcessPacket(BadConsolePacket packet)
    {
        switch (packet)
        {
            case BadConsoleClearPacket:
                Console.Clear();

                break;
            case BadConsoleDisconnectPacket:
                m_ExitRequested = true;

                break;
            case BadConsoleHelloPacket hello:
                HeartBeatSendInterval = hello.HeartBeatInterval;

                break;
            case BadConsoleWritePacket wp when wp.IsWriteLine:
                Console.WriteLine(wp.Message);

                break;
            case BadConsoleWritePacket wp:
                Console.Write(wp.Message);

                break;
            case BadConsoleColorChangePacket cs when cs.IsBackground:
                Console.BackgroundColor = cs.Color;

                break;
            case BadConsoleColorChangePacket cs:
                Console.ForegroundColor = cs.Color;

                break;
            default:
                throw new BadNetworkConsoleException("Invalid Packet");
        }
    }

	/// <summary>
	///     The Read Thread
	/// </summary>
	/// <exception cref="BadNetworkConsoleException">Gets raised if the incoming packets could not be read</exception>
	private void Read()
    {
        while (!m_ExitRequested)
        {
            if (!m_Client.Connected)
            {
                break;
            }

            if (m_Client.Available != 0)
            {
                byte[] len = new byte[sizeof(int)];
                NetworkStream stream = m_Client.GetStream();
                int read = stream.Read(len, 0, len.Length);

                if (read != len.Length)
                {
                    throw new BadNetworkConsoleException("Invalid Packet Size");
                }

                byte[] packet = new byte[BitConverter.ToInt32(len, 0)];
                read = stream.Read(packet, 0, packet.Length);

                if (read != packet.Length)
                {
                    throw new BadNetworkConsoleException("Invalid Packet");
                }

                ProcessPacket(BadConsolePacket.Deserialize(packet));
            }
            else
            {
                Thread.Sleep(ConsoleWriteSleep);
            }
        }

        if (!m_Client.Connected)
        {
            return;
        }

        NetworkStream str = m_Client.GetStream();
        List<byte> packetData = new List<byte>();
        byte[] packetBytes = BadConsoleDisconnectPacket.Packet.Serialize();
        packetData.AddRange(BitConverter.GetBytes(packetBytes.Length));
        packetData.AddRange(packetBytes);
        str.Write(packetData.ToArray(), 0, packetData.Count);
        m_Client.Dispose();
    }

	/// <summary>
	///     Sends a Heartbeat to the Server
	/// </summary>
	private void SendHeartBeat()
    {
        byte[] packetData = BadConsoleHeartBeatPacket.Packet.Serialize();
        List<byte> packet = new List<byte>();
        packet.AddRange(BitConverter.GetBytes(packetData.Length));
        packet.AddRange(packetData);
        m_Client.GetStream().Write(packet.ToArray(), 0, packet.Count);
    }

	/// <summary>
	///     The Write Thread
	/// </summary>
	private void Write()
    {
        DateTime lastHeartBeat = DateTime.Now;

        while (!m_ExitRequested)
        {
            Task<string> task = Task.Run(Console.ReadLine);

            while (!m_ExitRequested && !task.IsCompleted)
            {
                if (lastHeartBeat + TimeSpan.FromMilliseconds(HeartBeatSendInterval) < DateTime.Now)
                {
                    SendHeartBeat();
                    lastHeartBeat = DateTime.Now;
                }

                Thread.Sleep(ConsoleReadInputSleep);
            }

            if (m_ExitRequested)
            {
                break;
            }

            string message = task.Result ?? "client::disconnect";

            if (message.StartsWith("client::"))
            {
                int idx = message.IndexOf(' ');

                if (idx == -1)
                {
                    idx = message.Length;
                }

                string cmdName = message.Substring(0, idx).Remove(0, "client::".Length);
                string args = message.Remove(0, idx).Trim();
                BadNetworkConsoleClientCommand? cmd = m_Commands.FirstOrDefault(x => x.Name == cmdName);

                if (cmd == null)
                {
                    BadConsole.WriteLine("Command not found: " + cmdName);
                }
                else
                {
                    cmd.Invoke(args);
                }

                continue;
            }

            lastHeartBeat = DateTime.Now;
            byte[] packetData = new BadConsoleReadPacket(message).Serialize();
            List<byte> packet = new List<byte>();
            packet.AddRange(BitConverter.GetBytes(packetData.Length));
            packet.AddRange(packetData);
            m_Client.GetStream().Write(packet.ToArray(), 0, packet.Count);
        }

        m_ReadThread = null;
    }
}