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

using socket.core.Common;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace socket.core.Client
{
    /// <summary>
    /// udp客户端
    /// </summary>
    public class UdpClients
    {
        private int localPort = 5061;
        /// <summary>
        /// 用于监听传入连接请求的套接字
        /// </summary>
        private Socket listenSocket;
        /// <summary>
        /// 发送线程数
        /// </summary>
        private int sendthread = 10;
        /// <summary>
        /// 锁
        /// </summary>
        private Mutex mutex = new Mutex();
        /// <summary>
        /// 需要发送的数据
        /// </summary>
        private ConcurrentQueue<SendingQueue> sendQueue;
        /// <summary>
        /// 发送端SocketAsyncEventArgs对象重用池，发送套接字操作
        /// </summary>
        private SocketAsyncEventArgsPool m_sendPool;
        /// <summary>
        /// 接收通知事件 item1:远程地址,item2:数据,item3:偏移位,item4:长度
        /// </summary>
        public event Action<byte[], int, int> OnReceive;
        /// <summary>
        /// 发送通知事件 item1:远程地址,item2:已发送长度
        /// </summary>
        public event Action<int> OnSend;
        public event Action<SocketAsyncEventArgs> OnSendEx;
        /// <summary>
        /// 用于每个套接字I/O操作的缓冲区大小
        /// </summary>
        private int m_receiveBufferSize;
        /// <summary>
        /// 远程地址
        /// </summary>
        private IPEndPoint remoteEndPoint;

        /// <summary>
        /// 本地的udp端口
        /// </summary>
        public int LocalPort
        {
            get => localPort;
            set => localPort = value;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="receiveBufferSize">接收端缓存大小</param>
        public UdpClients(int receiveBufferSize)
        {
            m_receiveBufferSize = receiveBufferSize;
            m_sendPool = new SocketAsyncEventArgsPool(10);
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            sendQueue = new ConcurrentQueue<SendingQueue>();
        }

        /// <summary>
        /// 启动udp服务侦听
        /// </summary>
        /// <param name="ip">ip或者域名</param>
        /// <param name="port">绑定端口</param>
        public int Start(string ip, int port,int localPort=5061)
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
            remoteEndPoint = new IPEndPoint(ipaddr, port);
            //创建listens是传入的套接字。
            listenSocket = new Socket(remoteEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                //绑定端口           
                listenSocket.Bind(new IPEndPoint(IPAddress.Any, localPort));
                this.localPort = localPort;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            byte[] receivebuffer = new byte[m_receiveBufferSize];
            SocketAsyncEventArgs receiveSocketArgs = new SocketAsyncEventArgs();
            //receiveSocketArgs.SocketFlags
            receiveSocketArgs.RemoteEndPoint = remoteEndPoint;
            receiveSocketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            receiveSocketArgs.SetBuffer(receivebuffer, 0, receivebuffer.Length);
            StartReceive(receiveSocketArgs);
            //发送线程
            for (int i = 0; i < sendthread; i++)
            {
                Thread thread = new Thread(StartSend);
                thread.IsBackground = true;
                thread.Priority = ThreadPriority.AboveNormal;
                thread.Start(i);
            }

            return this.localPort;
        }

        /// <summary>
        /// 开始接收
        /// </summary>
        /// <param name="receiveSocketArgs">操作对象</param>
        private void StartReceive(SocketAsyncEventArgs receiveSocketArgs)
        {
            if (!listenSocket.ReceiveFromAsync(receiveSocketArgs))
            {
                ProcessReceive(receiveSocketArgs);
            }
        }

        /// <summary>
        /// 接收回调
        /// </summary>
        /// <param name="e">操作对象</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if (OnReceive != null)
                {
                    OnReceive(e.Buffer, e.Offset, e.BytesTransferred);
                }
            }
            StartReceive(e);
        }

        /// <summary>
        /// 开始启用发送
        /// </summary>
        /// <param name="thread">线程序号</param>
        private void StartSend(object thread)
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
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        public void Send(byte[] data, int offset, int length)
        {
            sendQueue.Enqueue(new SendingQueue() { remoteEndPoint = remoteEndPoint, data = data, offset = offset, length = length });
        }

        /// <summary>
        /// 发送数据(因为UDP是无连接地址，所以直接可以指定任何发送地址)
        /// </summary>
        /// <param name="remoteEndPoint">发送地址和端口</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        public void Send(EndPoint remoteEndPoint, byte[] data, int offset, int length)
        {
            sendQueue.Enqueue(new SendingQueue() { remoteEndPoint = remoteEndPoint, data = data, offset = offset, length = length });
        }

        /// <summary>
        /// 异步发送消息 
        /// </summary>
        /// <param name="sendQuere">发送消息体</param>
        private void Send(SendingQueue sendQuere)
        {
            mutex.WaitOne();
            if (m_sendPool.Count == 0)
            {
                SocketAsyncEventArgs saea_send = new SocketAsyncEventArgs();
                saea_send.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                m_sendPool.Push(saea_send);
            }
            SocketAsyncEventArgs socketArgs = m_sendPool.Pop();
            mutex.ReleaseMutex();
            socketArgs.RemoteEndPoint = sendQuere.remoteEndPoint;
            socketArgs.SetBuffer(sendQuere.data, sendQuere.offset, sendQuere.length);
            if (!listenSocket.SendToAsync(socketArgs))
            {
                ProcessSend(socketArgs);
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

                if (OnSendEx != null)
                {
                    OnSendEx(e);
                }
            }
        }

        /// <summary>
        /// 处理完成事件
        /// </summary>
        /// <param name="sender">socket对象</param>
        /// <param name="e">操作对象</param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.SendTo:
                    ProcessSend(e);
                    break;
                default:
                    break;
            }
        }

    }
}
