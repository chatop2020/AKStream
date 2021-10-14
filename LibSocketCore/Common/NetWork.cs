using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace socket.core.Common
{
    /// <summary>
    /// 获取网卡相关信息
    /// </summary>
    public class NetWork
    {
        /// <summary>
        /// 获取本地网络信息
        /// </summary>
        /// <returns>item1:ip地址,item2:子网掩码,item3:默认网关,item4:广播地址</returns>
        public static List<(string, string, string, string)> GetNetwork()
        {
            List<(string, string, string, string)> netscript = new List<(string, string, string, string)>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface netWork in nics)
            {
                IPInterfaceProperties myip = netWork.GetIPProperties();
                if (myip.GatewayAddresses.Count == 0)
                {
                    continue;
                }
                foreach (var item in myip.UnicastAddresses)
                {
                    if (!item.Address.IsIPv6LinkLocal && !item.Address.IsIPv6Teredo)
                    {
                        byte[] broadcast = new byte[4];
                        for (int i = 0; i < item.Address.GetAddressBytes().Length; i++)
                        {
                            //广播地址=子网掩码按位求反 再 或IP地址 
                            broadcast[i] = (byte)((~item.IPv4Mask.GetAddressBytes()[i]) | item.Address.GetAddressBytes()[i]);
                        }
                        netscript.Add((
                            item.Address.ToString(),
                            item.IPv4Mask.ToString(),
                            myip.GatewayAddresses[0].Address.ToString(),
                            new IPAddress(broadcast).ToString()
                            ));
                    }
                }
            }
            return netscript;
        }

    }
}
