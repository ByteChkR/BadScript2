using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace BadScript2.VirtualMachine.Managing;

public class BadVirtualMachineManagerHost
{
    private readonly TcpListener m_Listener;
    private readonly List<BadVirtualMachineSession> m_RunningMachines = new List<BadVirtualMachineSession>();
    private readonly BadVirtualMachineService m_Service;
    private bool m_ShouldExit;

    public BadVirtualMachineManagerHost(BadVirtualMachineService service, TcpListener listener)
    {
        m_Service = service;
        m_Listener = listener;
    }

    private BadVirtualMachineSession StartSession(TcpClient client)
    {
        BadVirtualMachineSession session = new BadVirtualMachineSession(client, m_Service);
        m_RunningMachines.Add(session);

        return session;
    }

    public void StartSynchronously()
    {
        Run();
    }

    public void Start()
    {
        new Thread(Run).Start();
    }

    private void Run()
    {
        m_ShouldExit = false;
        m_Listener.Start();
        bool isAccepting = true;
        m_Listener.BeginAcceptTcpClient(
            ar =>
            {
                StartSession(m_Listener.EndAcceptTcpClient(ar));
                isAccepting = false;
            },
            null
        );
        while (true)
        {
            if (m_ShouldExit)
            {
                break;
            }

            if (!isAccepting)
            {
                isAccepting = true;
                m_Listener.BeginAcceptTcpClient(
                    ar =>
                    {
                        StartSession(m_Listener.EndAcceptTcpClient(ar));
                        isAccepting = false;
                    },
                    null
                );
            }

            for (int i = m_RunningMachines.Count - 1; i >= 0; i--)
            {
                BadVirtualMachineSession session = m_RunningMachines[i];
                if (session == null)
                {
                    continue;
                }

                if (!session.Process())
                {
                    m_RunningMachines.Remove(session);
                }
            }
        }
    }
}