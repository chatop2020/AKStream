using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace socket.core.Client
{
    /// <summary>
    /// push 推出数据
    /// </summary>
    public class TcpPushClient
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
        public TcpPushClient(int receiveBufferSize)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                tcpClients = new TcpClients(receiveBufferSize);
                tcpClients.OnConnect += TcpServer_eventactionConnect;
                tcpClients.OnReceive += TcpServer_eventactionReceive;
                tcpClients.OnSend += TcpClients_OnSend;
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
            tcpClients.Send(data, offset, length);
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
                OnReceive(data);
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
            if (OnClose != null)
                OnClose();
        }
    }
}
