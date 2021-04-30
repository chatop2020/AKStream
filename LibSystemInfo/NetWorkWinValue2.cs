using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using LibCommon.Structs;

namespace LibSystemInfo
{
    public static class NetWorkWinValue2
    {
        private static Object lockObj = new object();
        public static NetWorkStat NetWorkStat = new NetWorkStat();

        private static ulong _sendPer = 0;
        private static ulong _recvPer = 0;

        static NetWorkWinValue2()
        {
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

        [DllImport("IpHlpApi.dll")]
        extern static public uint GetIfTable(byte[] pIfTable, ref uint pdwSize, bool bOrder);

        private static List<NetInfo> GetALLNetInfo()
        {
            List<NetInfo> ninfos = new List<NetInfo>();
            MIB_IFTABLE tbl = GetAllIfTable();
            if (tbl != null)
            {
                tbl.Deserialize();
                for (int i = 0; i < tbl.Table.Length; i++)
                {
                    var netinfo = GetNetInfo(tbl.Table[i]);
                    var obj = ninfos.FindLast(x => x.OutOctets.Equals(netinfo.OutOctets)
                                                   && x.InOctets.Equals(netinfo.InOctets));
                    if (obj == null && netinfo.OutOctets > 0 && netinfo.InOctets > 0)
                    {
                        ninfos.Add(netinfo);
                    }
                }
            }

            return ninfos;
        }

        private static MIB_IFTABLE GetAllIfTable()
        {
            //缓冲区大小
            uint dwSize = 0;

            //获取缓冲区大小
            uint ret = GetIfTable(null, ref dwSize, false);
            if (ret == 50)
            {
                //此函数仅支持于 win98/nt 系统
                return null;
            }

            //定义，获取 MIB_IFTABLE 对象
            MIB_IFTABLE tbl = new MIB_IFTABLE((int) dwSize);
            ret = GetIfTable(tbl.ByteArray, ref dwSize, false);

            //如果不成功
            if (ret != 0)
            {
                return null;
            }

            return tbl;
        }

        /// <summary>
        /// Get NetInfo Class
        /// </summary>
        /// <param name="row">MIB_IFROW Class</param>
        /// <returns>NetInfo Class</returns>
        private static NetInfo GetNetInfo(MIB_IFROW row)
        {
            NetInfo ninfo = new NetInfo();
            for (int i = 0; i < row.dwPhysAddrLen; i++)
            {
                ninfo.PhysAddr += row.bPhysAddr[i].ToString("X2") + ":";
            }

            if (!string.IsNullOrEmpty(ninfo.PhysAddr))
                ninfo.PhysAddr = ninfo.PhysAddr.TrimEnd(':');
            ninfo.InOctets = row.dwInOctets;
            ninfo.OutOctets = row.dwOutOctets;
            return ninfo;
        }

        public static NetWorkStat GetNetworkStat()
        {
            lock (lockObj)
            {
                return NetWorkStat;
            }
        }

        private static void run()
        {
            while (true)
            {
                try
                {
                    lock (lockObj)
                    {
                        var list = GetALLNetInfo();
                        if (list != null && list.Count > 0)
                        {
                            var obj = list[0];
                            if (_recvPer == 0 || _sendPer == 0)
                            {
                                NetWorkStat.Mac = obj.PhysAddr;
                                NetWorkStat.CurrentRecvBytes = 0;
                                NetWorkStat.CurrentSendBytes = 0;
                                NetWorkStat.TotalRecvBytes = 0;
                                NetWorkStat.TotalSendBytes = 0;
                                _recvPer = obj.InOctets;
                                _sendPer = obj.OutOctets;
                            }
                            else
                            {
                                NetWorkStat.Mac = obj.PhysAddr;
                                NetWorkStat.UpdateTime = DateTime.Now;
                                NetWorkStat.CurrentRecvBytes = obj.InOctets - _recvPer;
                                NetWorkStat.CurrentSendBytes = obj.OutOctets - _sendPer;
                                NetWorkStat.TotalRecvBytes = obj.InOctets;
                                NetWorkStat.TotalSendBytes = obj.OutOctets;
                                _recvPer = obj.InOctets;
                                _sendPer = obj.OutOctets;
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