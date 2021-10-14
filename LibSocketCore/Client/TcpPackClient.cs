using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace socket.core.Client
{
    /// <summary>
    /// 推和拉组合体，自带分包处理机制
    /// </summary>
    public class TcpPackClient
    {
        /// <summary>
        /// 基础类
        /// </summary>
        private TcpClients tcpClients;
        /// <summary>
        /// 连接成功事件 item1:是否连接成功
        /// </summary>
        public event Action<bool> OnConnect;
        /// <summary>
        /// 接收通知事件 item1:数据
        /// </summary>
        public event Action<byte[]> OnReceive;
        /// <summary>
        /// 已发送通知事件 item1:长度
        /// </summary>
        public event Action<int> OnSend;
        /// <summary>
        /// 断开连接通知事件
        /// </summary>
        public event Action OnClose;
        /// <summary>
        /// 接收到的数据缓存
        /// </summary>
        private List<byte> queue;
        /// <summary>
        /// 包头标记
        /// </summary>
        private uint headerFlag;
        /// <summary>
        /// 是否连接服务器
        /// </summary>
        public bool Connected
        {
            get
            {
                if (tcpClients == null)
                {
                    return false;
                }
                return tcpClients.Connected;
            }
        }

        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="headerFlag">包头标记范围0~1023(0x3FF),当包头标识等于0时，不校验包头</param>
        public TcpPackClient(int receiveBufferSize, uint headerFlag)
        {
            if (headerFlag < 0 || headerFlag > 1023)
            {
                headerFlag = 0;
            }
            this.headerFlag = headerFlag;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                queue = new List<byte>();
                tcpClients = new TcpClients(receiveBufferSize);
                tcpClients.OnConnect += TcpServer_eventactionConnect;
                tcpClients.OnReceive += TcpServer_eventactionReceive;
                tcpClients.OnClose += TcpServer_eventClose;
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip">ip地址或域名</param>
        /// <param name="port">端口</param>
        public void Connect(string ip, int port)
        {
            while (tcpClients == null)
            {
                Thread.Sleep(10);
            }
            tcpClients.Connect(ip, port);
        }

        /// <summary>
        /// 连接成功事件方法
        /// </summary>
        /// <param name="success">是否成功连接</param>
        private void TcpServer_eventactionConnect(bool success)
        {
            if (OnConnect != null)
                OnConnect(success);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        public void Send(byte[] data, int offset, int length)
        {
            data = AddHead(data.Skip(offset).Take(length).ToArray());
            tcpClients.Send(data, 0, data.Length);
        }

        /// <summary>
        /// 已发送长度
        /// </summary>
        /// <param name="length"></param>
        private void TcpClients_OnSend(int length)
        {
            if (OnSend != null)
                OnSend(length);
        }

        /// <summary>
        /// 接收通知事件方法
        /// </summary>
        /// <param name="data">数据</param>
        private void TcpServer_eventactionReceive(byte[] data)
        {
            if (OnReceive != null)
            {
                queue.AddRange(data);
                byte[] datas = Read();
                if (datas != null && datas.Length > 0)
                {
                    OnReceive(datas);
                }
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close()
        {
            tcpClients.Close();
        }

        /// <summary>
        /// 断开连接通知事件方法
        /// </summary>
        private void TcpServer_eventClose()
        {
            queue.Clear();
            if (OnClose != null)
                OnClose();
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
        /// <returns></returns>
        private byte[] Read()
        {
            uint header = BitConverter.ToUInt32(queue.ToArray(), 0);
            if (headerFlag != (header >> 22))
            {
                return null;
            }
            uint len = header & 0x3fffff;
            if (len > queue.Count - 4)
            {
                return null;
            }
            byte[] f = queue.Skip(4).Take((int)len).ToArray();
            queue.RemoveRange(0, (int)len + 4);
            return f;
        }
    }
}
