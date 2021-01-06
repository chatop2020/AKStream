using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Xml;
using SIPSorcery.Sys;

namespace LibCommon.Structs.GB28181.Sys.Net
{
    public static class LocalIPConfig
    {
        public const string ALL_LOCAL_IPADDRESSES_KEY = "*";

        public const string
            LINK_LOCAL_BLOCK_PREFIX = "169.254"; // Used by hosts attempting to acquire a DHCP address. See RFC 3330.


        public static List<IPAddress> GetLocalIPv4Addresses()
        {
            List<IPAddress> localAddresses = new List<IPAddress>();

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();

                UnicastIPAddressInformationCollection localIPs = adapterProperties.UnicastAddresses;
                foreach (UnicastIPAddressInformation localIP in localIPs)
                {
                    if (localIP.Address.AddressFamily == AddressFamily.InterNetwork &&
                        !localIP.Address.ToString().StartsWith(LINK_LOCAL_BLOCK_PREFIX))
                    {
                        localAddresses.Add(localIP.Address);
                    }
                }
            }

            return localAddresses;
        }

        public static IPAddress GetDefaultIPv4Address()
        {
            var adapters = from adapter in NetworkInterface.GetAllNetworkInterfaces()
                where adapter.OperationalStatus == OperationalStatus.Up && adapter.Supports(NetworkInterfaceComponent
                                                                            .IPv4)
                                                                        && adapter.GetIPProperties().GatewayAddresses
                                                                            .Count > 0 &&
                                                                        adapter.GetIPProperties().GatewayAddresses[0]
                                                                            .Address.ToString() != "0.0.0.0"
                select adapter;

            if (adapters == null || adapters.Count() == 0)
            {
                throw new ApplicationException(
                    "The default IPv4 address could not be determined as there are were no interfaces with a gateway.");
            }
            else
            {
                UnicastIPAddressInformationCollection localIPs = adapters.First().GetIPProperties().UnicastAddresses;
                foreach (UnicastIPAddressInformation localIP in localIPs)
                {
                    if (localIP.Address.AddressFamily == AddressFamily.InterNetwork &&
                        !localIP.Address.ToString().StartsWith(LINK_LOCAL_BLOCK_PREFIX) &&
                        !IPAddress.IsLoopback(localIP.Address))
                    {
                        return localIP.Address;
                    }
                }
            }

            return null;
        }

        public static List<IPEndPoint> GetLocalIPv4EndPoints(int port)
        {
            List<IPEndPoint> localEndPoints = new List<IPEndPoint>();
            List<IPAddress> localAddresses = GetLocalIPv4Addresses();

            foreach (IPAddress localAddress in localAddresses)
            {
                localEndPoints.Add(new IPEndPoint(localAddress, port));
            }

            return localEndPoints;
        }

        public static List<IPEndPoint> ParseIPSockets(XmlNode socketNodes)
        {
            List<IPEndPoint> endPoints = new List<IPEndPoint>();
            List<IPAddress> localAddresses = GetLocalIPv4Addresses();

            foreach (XmlNode socketNode in socketNodes.ChildNodes)
            {
                string socketString = socketNode.InnerText;

                int port = IPSocket.ParsePortFromSocket(socketString);
                if (socketString.StartsWith(ALL_LOCAL_IPADDRESSES_KEY))
                {
                    foreach (IPAddress ipAddress in localAddresses)
                    {
                        endPoints.Add(new IPEndPoint(ipAddress, port));
                    }
                }
                else
                {
                    endPoints.Add(IPSocket.ParseSocketString(socketString));
                }
            }

            return endPoints;
        }
    }
}