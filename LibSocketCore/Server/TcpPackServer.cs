using socket.core.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace socket.core.Server
{
    /// <summary>
    /// 推和拉组合体，自带分包处理机制
    /// </summary>
    public class TcpPackServer
    {
        /// <summary>
        /// 基础类
        /// </summary>
        private TcpServer tcpServer;
        /// <summary>
        /// 连接成功事件  item1:connectId
        /// </summary>
        public event Action<int> OnAccept;
        /// <summary>
        /// 接收通知事件  item1:connectId,item2:数据
        /// </summary>
        public event Action<int, byte[]> OnReceive;
        /// <summary>
        /// 发送通知事件  item1:connectId,item2:长度
        /// </summary>
        public event Action<int, int> OnSend;
        /// <summary>
        /// 断开连接通知事件  item1:connectId,
        /// </summary>
        public event Action<int> OnClose;
        /// <summary>
        /// 接收到的数据缓存
        /// </summary>
        private Dictionary<int, List<byte>> queue;
        /// <summary>
        /// 包头标记
        /// </summary>
        private uint headerFlag;
        /// <summary>
        /// 客户端列表
        /// </summary>
        public ConcurrentDictionary<int, string> ClientList
        {
            get
            {
                if (tcpServer != null)
                {
                    return tcpServer.clientList;
                }
                else
                {
                    return new ConcurrentDictionary<int, string>();
                }
            }
        }

        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        /// <param name="headerFlag">包头标记范围0~1023(0x3FF),当包头标识等于0时，不校验包头</param>
        public TcpPackServer(int numConnections, int receiveBufferSize, int overtime, uint headerFlag)
        {
            if (headerFlag < 0 || headerFlag > 1023)
            {
                headerFlag = 0;
            }
            this.headerFlag = headerFlag;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                queue = new Dictionary<int, List<byte>>();
                tcpServer = new TcpServer(numConnections, receiveBufferSize, overtime);
                tcpServer.OnAccept += TcpServer_eventactionAccept;
                tcpServer.OnReceive += TcpServer_eventactionReceive;
                tcpServer.OnSend += TcpServer_OnSend;
                tcpServer.OnClose += TcpServer_eventClose;
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 开启监听服务
        /// </summary>        
        /// <param name="port">监听端口</param>
        public void Start(int port)
        {
            while (tcpServer == null)
            {
                Thread.Sleep(10);
            }
            tcpServer.Start(port);
        }

        /// <summary>
        /// 连接成功事件方法
        /// </summary>
        /// <param name="connectId">连接标记</param>
        private void TcpServer_eventactionAccept(int connectId)
        {
            if (OnAccept != null)
                OnAccept(connectId);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connectId">连接ID</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        public void Send(int connectId, byte[] data, int offset, int length)
        {
            data = AddHead(data.Skip(offset).Take(length).ToArray());
            tcpServer.Send(connectId, data, 0, data.Length);
        }

        /// <summary>
        /// 发送成功事件方法
        /// </summary>
        /// <param name="connectId">连接标记</param>
        /// <param name="length">长度</param>
        private void TcpServer_OnSend(int connectId, int length)
        {
            if (OnSend != null)
            {
                OnSend(connectId, length);
            }
        }
               
        /// <summary>
        /// 接收通知事件方法
        /// </summary>
        /// <param name="connectId">连接标记</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        private void TcpServer_eventactionReceive(int connectId, byte[] data, int offset, int length)
        {
            if (OnReceive != null)
            {
                if (!queue.ContainsKey(connectId))
                {
                    queue.Add(connectId, new List<byte>());
                }
                byte[] r = new byte[length];
                Buffer.BlockCopy(data, offset, r, 0, length);
                queue[connectId].AddRange(r);
                byte[] datas = Read(connectId);
                if (datas != null && datas.Length > 0)
                {
                    OnReceive(connectId, datas);
                }
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="connectId">连接ID</param>
        public void Close(int connectId)
        {
            tcpServer.Close(connectId);
        }

        /// <summary>
        /// 断开连接通知事件方法
        /// </summary>
        /// <param name="connectId">连接标记</param>
        private void TcpServer_eventClose(int connectId)
        {
            if (queue.ContainsKey(connectId))
            {
                queue.Remove(connectId);
            }
            if (OnClose != null)
                OnClose(connectId);
        }

        /// <summary>
        /// 在数据起始位置增加4字节包头
        /// </summary>     
        /// <param name="data">数据</param>
        /// <returns></returns>
        private byte[] AddHead(byte[] data)
        {
            uint len = (uint)data.Length;
            uint header = (headerFlag << 22) | len;
            byte[] head = System.BitConverter.GetBytes(header);
            return head.Concat(data).ToArray();
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="connectId">连接标记</param>
        /// <returns></returns>
        private byte[] Read(int connectId)
        {
            if (!queue.ContainsKey(connectId))
            {
                return null;
            }
            List<byte> data = queue[connectId];
            uint header = BitConverter.ToUInt32(data.ToArray(), 0);
            if (headerFlag != (header >> 22))
            {
                return null;
            }
            uint len = header & 0x3fffff;
            if (len > data.Count - 4)
            {
                return null;
            }
            byte[] f = data.Skip(4).Take((int)len).ToArray();
            queue[connectId].RemoveRange(0, (int)len + 4);
            return f;
        }

        /// <summary>
        /// 给连接对象设置附加数据
        /// </summary>
        /// <param name="connectId">连接标识</param>
        /// <param name="data">附加数据</param>
        /// <returns>true:设置成功,false:设置失败</returns>
        public bool SetAttached(int connectId, object data)
        {
            return tcpServer.SetAttached(connectId, data);
        }

        /// <summary>
        /// 获取连接对象的附加数据
        /// </summary>
        /// <param name="connectId">连接标识</param>
        /// <returns>返回附加数据</returns>
        public T GetAttached<T>(int connectId)
        {
            return tcpServer.GetAttached<T>(connectId);
        }
    }
}
