/*  
 * ......................我佛慈悲...................... 
 *                       _oo0oo_ 
 *                      o8888888o 
 *                      88" . "88 
 *                      (| -_- |) 
 *                      0\  =  /0 
 *                    ___/`---'\___ 
 *                  .' \\|     |// '. 
 *                 / \\|||  :  |||// \ 
 *                / _||||| -卍-|||||- \ 
 *               |   | \\\  -  /// |   | 
 *               | \_|  ''\---/''  |_/ | 
 *               \  .-\__  '-'  ___/-. / 
 *             ___'. .'  /--.--\  `. .'___ 
 *          ."" '<  `.___\_<|>_/___.' >' "". 
 *         | | :  `- \`.;`\ _ /`;.`/ - ` : | | 
 *         \  \ `_.   \_ __\ /__ _/   .-` /  / 
 *     =====`-.____`.___ \_____/___.-`___.-'===== 
 *                       `=---=' 
 *                        
 *..................佛祖开光 ,永无BUG................... 
 *  
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using socket.core.Common;

namespace socket.core.Client
{
    /// <summary>
    /// 客户端基础类
    /// </summary>
    internal class TcpClients
    {
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;
        /// <summary>
        /// 发送端SocketAsyncEventArgs对象重用池，发送套接字操作
        /// </summary>
        private SocketAsyncEventArgsPool m_sendPool;
        /// <summary>
        /// 用于每个套接字I/O操作的缓冲区大小
        /// </summary>
        private int m_receiveBufferSize;
        /// <summary>
        /// 接受缓存
        /// </summary>
        private byte[] buffer_receive;
        /// <summary>
        /// 接收对象
        /// </summary>
        private SocketAsyncEventArgs receiveSocketAsyncEventArgs;
        /// <summary>
        /// 发送对象默认数
        /// </summary>
        private int m_minSendSocketAsyncEventArgs = 10;
        /// <summary>
        /// 需要发送的数据
        /// </summary>
        private ConcurrentQueue<SendingQueue> sendQueue;
        /// <summary>
        /// 连接成功事件
        /// </summary>
        internal event Action<bool> OnConnect;
        /// <summary>
        /// 接收通知事件
        /// </summary>
        internal event Action<byte[]> OnReceive;
        /// <summary>
        /// 已送通知事件
        /// </summary>
        internal event Action<int> OnSend;
        /// <summary>
        /// 断开连接通知事件
        /// </summary>
        internal event Action OnClose;
        /// <summary>
        /// 锁
        /// </summary>
        private Mutex mutex = new Mutex();
        /// <summary>
        /// 是否连接服务器
        /// </summary>
        public bool Connected
        {
            get
            {
                if (socket == null)
                {
                    return false;
                }
                return socket.Connected;
            }
        }

        /// <summary>
        /// 设置基本配置
        /// </summary>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        internal TcpClients(int receiveBufferSize)
        {
            m_receiveBufferSize = receiveBufferSize;
            m_sendPool = new SocketAsyncEventArgsPool(m_minSendSocketAsyncEventArgs);
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            buffer_receive = new byte[m_receiveBufferSize];
            sendQueue = new ConcurrentQueue<SendingQueue>();
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip">ip地址或域名</param>
        /// <param name="port">连接端口</param>
        internal void Connect(string ip, int port)
        {
            IPAddress ipaddr;
            if (!IPAddress.TryParse(ip, out ipaddr))
            {
                IPAddress[] iplist = Dns.GetHostAddresses(ip);
                if (iplist != null && iplist.Length > 0)
                {
                    ipaddr = iplist[0];
                }
            }
            IPEndPoint localEndPoint = new IPEndPoint(ipaddr, port);
            socket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            SocketAsyncEventArgs connSocketAsyncEventArgs = new SocketAsyncEventArgs();
            connSocketAsyncEventArgs.RemoteEndPoint = localEndPoint;
            connSocketAsyncEventArgs.Completed += IO_Completed;
            if (!socket.ConnectAsync(connSocketAsyncEventArgs))
            {
                ProcessConnect(connSocketAsyncEventArgs);
            }
        }

        /// <summary>
        /// 连接回调事件
        /// </summary>
        /// <param name="e">操作对象</param>
        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                receiveSocketAsyncEventArgs = new SocketAsyncEventArgs();
                receiveSocketAsyncEventArgs.SetBuffer(buffer_receive, 0, buffer_receive.Length);
                receiveSocketAsyncEventArgs.Completed += IO_Completed;
                if (!socket.ReceiveAsync(receiveSocketAsyncEventArgs))
                {
                    ProcessReceive(receiveSocketAsyncEventArgs);
                }
                if (OnConnect != null)
                {
                    OnConnect(true);
                }
                //发送线程
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    StartSend();
                }));
                thread.IsBackground = true;
                thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }
            else
            {
                if (OnConnect != null)
                {
                    OnConnect(false);
                }
            }
        }

        /// <summary>
        /// 每当套接字上完成接收或发送操作时，都会调用此方法。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">与完成的接收操作关联的SocketAsyncEventArg</param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            //确定刚刚完成哪种类型的操作并调用相关的处理程序
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                default:
                    throw new ArgumentException("套接字上完成的最后一个操作不是接收或发送或连接。");
            }
        }

        #region 接收

        /// <summary>
        /// 接受回调
        /// </summary>
        /// <param name="e">操作对象</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] data = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                if (OnReceive != null)
                {
                    OnReceive(data);
                }
                //将收到的数据回显给客户端  
                if (socket.Connected == true)
                {
                    if (!socket.ReceiveAsync(e))
                    {
                        ProcessReceive(e);
                    }
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }
        #endregion

        #region 发送

        /// <summary>
        /// 异步发送消息 
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        /// <returns>true:已连接到服务端,false:未连接到服务端</returns>
        internal void Send(byte[] data, int offset, int length)
        {
            sendQueue.Enqueue(new SendingQueue() { data = data, offset = offset, length = length });
        }

        /// <summary>
        /// 开始启用发送
        /// </summary>
        private void StartSend()
        {
            while (true)
            {
                SendingQueue sending;
                if (sendQueue.TryDequeue(out sending))
                {
                    Send(sending);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 发送数据到服务端
        /// </summary>
        internal void Send(SendingQueue sendQuere)
        {
            if (!socket.Connected)
            {
                return;
            }
            mutex.WaitOne();
            //如果发送池为空时，临时新建一个放入池中
            if (m_sendPool.Count == 0)
            {
                SocketAsyncEventArgs saea_send = new SocketAsyncEventArgs();
                saea_send.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                m_sendPool.Push(saea_send);
            }
            SocketAsyncEventArgs sendEventArgs = m_sendPool.Pop();
            mutex.ReleaseMutex();
            sendEventArgs.SetBuffer(sendQuere.data, sendQuere.offset, sendQuere.length);
            if (!socket.SendAsync(sendEventArgs))
            {
                ProcessSend(sendEventArgs);
            }
        }

        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="e">操作对象</param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                m_sendPool.Push(e);
                if (OnSend != null)
                {
                    OnSend(e.BytesTransferred);
                }
            }
        }

        #endregion

        #region 断开
        /// <summary>
        /// 客户端主动关闭连接
        /// </summary>
        internal void Close()
        {
            CloseClientSocket(receiveSocketAsyncEventArgs);
        }

        /// <summary>
        /// 客户端断开一个连接
        /// </summary>
        /// <param name="e">操作对象</param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            if (!socket.Connected)
            {
                return;
            }
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                // 关闭与客户端关联的套接字
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                // 抛出客户端进程已经关闭
                catch (Exception) { }
                socket.Close();
                if (OnClose != null)
                {
                    OnClose();
                }
            }
        }
        #endregion  


    }
}
