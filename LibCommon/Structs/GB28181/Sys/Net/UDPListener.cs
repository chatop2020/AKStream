using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SIPSorcery.Sys;

namespace LibCommon.Structs.GB28181.Sys.Net
{
    public class UDPListener
    {
        private const string THREAD_NAME = "udplistener-";
        private bool m_closed;
        private IPEndPoint m_localEndPoint;
        private Guid m_socketId = Guid.NewGuid();
        private UdpClient m_udpClient;

        public Action<UDPListener, IPEndPoint, IPEndPoint, byte[]> PacketReceived;

        public UDPListener(IPEndPoint endPoint)
        {
            m_localEndPoint = endPoint;
            Initialise();
        }

        private void Initialise()
        {
            try
            {
                m_udpClient = new UdpClient(m_localEndPoint);

                Thread listenThread = new Thread(new ThreadStart(Listen));
                listenThread.Name = THREAD_NAME + Crypto.GetRandomString(4);
                listenThread.Start();
            }
            catch (Exception excp)
            {
                throw excp;
            }
        }

        private void Dispose(bool disposing)
        {
            try
            {
                this.Close();
            }
            catch
            {
            }
        }

        private void Listen()
        {
            try
            {
                byte[] buffer = null;
                while (!m_closed)
                {
                    IPEndPoint inEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    try
                    {
                        buffer = m_udpClient.Receive(ref inEndPoint);
                    }
                    catch (SocketException)
                    {
                        // ToDo. Pretty sure these exceptions get thrown when an ICMP message comes back indicating there is no listening
                        // socket on the other end. It would be nice to be able to relate that back to the socket that the data was sent to
                        // so that we know to stop sending.
                        continue;
                    }
                    catch
                    {
                        // There is no point logging this as without processing the ICMP message it's not possible to know which socket the rejection came from.
                        inEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        continue;
                    }

                    if (buffer == null || buffer.Length == 0)
                    {
                        // No need to care about zero byte packets.
                    }
                    else
                    {
                        if (PacketReceived != null)
                        {
                            PacketReceived(this, m_localEndPoint, inEndPoint, buffer);
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public void Send(IPEndPoint destinationEndPoint, string message)
        {
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
            Send(destinationEndPoint, messageBuffer);
        }

        public void Send(IPEndPoint destinationEndPoint, byte[] buffer)
        {
            Send(destinationEndPoint, buffer, buffer.Length);
        }

        public void Send(IPEndPoint destinationEndPoint, byte[] buffer, int length)
        {
            try
            {
                if (destinationEndPoint == null)
                {
                    throw new ApplicationException("An empty destination was specified to Send in SIPUDPChannel.");
                }
                else
                {
                    if (m_udpClient != null && m_udpClient.Client != null)
                    {
                        m_udpClient.Send(buffer, length, destinationEndPoint);
                    }
                }
            }
            catch (Exception excp)
            {
                throw excp;
            }
        }

        public void Close()
        {
            try
            {
                m_closed = true;
                m_udpClient.Close();
            }
            catch
            {
                // ignored
            }
        }
    }
}