using System;
using System.Net.NetworkInformation;
using System.Threading;
using LibCommon.Structs;

namespace LibSystemInfo
{
    public class NetWorkWinValue3
    {
        private static Object lockObj = new object();
        public static NetWorkStat NetWorkStat = new NetWorkStat();

        private static ulong _sendPer = 0;
        private static ulong _recvPer = 0;

        static NetWorkWinValue3()
        {
            NetWorkStat.Mac = "00-00-00-00-00-00";
            NetWorkStat.CurrentRecvBytes = 0;
            NetWorkStat.CurrentSendBytes = 0;
            NetWorkStat.TotalRecvBytes = 0;
            NetWorkStat.TotalSendBytes = 0;
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    run();
                }
                catch
                {
                }
            })).Start();
        }

        public static NetWorkStat GetNetworkStat()
        {
            lock (lockObj)
            {
                return NetWorkStat;
            }
        }

        private static string InsertFormat(string input, int interval, string value)
        {
            for (int i = interval; i < input.Length; i += interval + 1)
                input = input.Insert(i, value);
            return input;
        }

        private static void run()
        {
            while (true)
            {
                try
                {
                    lock (lockObj)
                    {
                       
                        NetworkInterface[] nifs = NetworkInterface.GetAllNetworkInterfaces();
                        if (nifs != null && nifs.Length > 0)
                        {
                            long tmpRecvByte = 0;
                            long tmpSendByte = 0;
                            foreach (var nif in nifs)
                            {
                                if (nif.OperationalStatus == OperationalStatus.Up &&
                                    (nif.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                     nif.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                                {
                                    IPv4InterfaceStatistics statis = nif.GetIPv4Statistics();
                                    if (statis.BytesReceived > 0 || statis.BytesSent > 0)
                                    {
                                        tmpRecvByte += statis.BytesReceived;
                                        tmpSendByte += statis.BytesSent;
                                        if (string.IsNullOrEmpty(NetWorkStat.Mac) || NetWorkStat.Mac=="00-00-00-00-00-00")
                                        {
                                            NetWorkStat.Mac = nif.GetPhysicalAddress().ToString();
                                            NetWorkStat.Mac = InsertFormat(NetWorkStat.Mac, 2, "-").TrimEnd('-').ToUpper();
                                        }
                                    }
                                }
                            }

                            if (_recvPer == 0 || _sendPer == 0)
                            {
                                NetWorkStat.CurrentRecvBytes = 0;
                                NetWorkStat.CurrentSendBytes = 0;
                                NetWorkStat.TotalRecvBytes = 0;
                                NetWorkStat.TotalSendBytes = 0;
                                _recvPer = (ulong) tmpRecvByte;
                                _sendPer = (ulong) tmpSendByte;
                            }
                            else
                            {
                                NetWorkStat.UpdateTime = DateTime.Now;
                                NetWorkStat.CurrentRecvBytes = (ulong) tmpRecvByte - _recvPer;
                                NetWorkStat.CurrentSendBytes = (ulong) tmpSendByte - _sendPer;
                                NetWorkStat.TotalRecvBytes = (ulong) tmpRecvByte;
                                NetWorkStat.TotalSendBytes = (ulong) tmpSendByte;
                                _recvPer = (ulong) tmpRecvByte;
                                _sendPer = (ulong) tmpSendByte;
                            }
                        }
                    }

                    Thread.Sleep(1000);
                }
                catch
                {
                    //
                }
            }
        }
    }
}