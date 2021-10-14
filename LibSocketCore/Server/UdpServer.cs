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

namespace socket.core.Server
{
    /// <summary>
    /// UPD 服务类
    /// </summary>
    public class UdpServer
    {
        /// <summary>
        /// 用于监听传入连接请求的套接字
        /// </summary>
        private Socket listenSocket;
        /// <summary>
        /// 用于每个套接字I/O操作的缓冲区大小
        /// </summary>
        private int m_receiveBufferSize;
        /// <summary>
        /// 锁
        /// </summary>
        private Mutex mutex = new Mutex();
        /// <summary>
        /// 发送端SocketAsyncEventArgs对象重用池，发送套接字操作
        /// </summary>
        private SocketAsyncEventArgsPool m_sendPool;
        /// <summary>
        /// 发送线程数
        /// </summary>
        private int sendthread = 10;
        /// <summary>
        /// 需要发送的数据,每一个线程一个队列
        /// </summary>
        private ConcurrentQueue<SendingQueue>[] sendQueues;
        /// <summary>
        /// 接收通知事件 item1:远程地址,item2:数据,item3:偏移位,item4:长度
        /// </summary>
        public event Action<EndPoint, byte[], int, int> OnReceive;
        /// <summary>
        /// 发送通知事件 item1:远程地址,item2:已发送长度
        /// </summary>
        public event Action<EndPoint, int> OnSend;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="receiveBufferSize">接收缓存大小</param>
        public UdpServer(int receiveBufferSize)
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
            sendQueues = new ConcurrentQueue<SendingQueue>[sendthread];
            for (int i = 0; i < sendthread; i++)
            {
                sendQueues[i] = new ConcurrentQueue<SendingQueue>();
            }
        }

        /// <summary>
        /// 启动udp服务侦听
        /// </summary>       
        /// <param name="port">绑定端口</param>
        /// <param name="reuseAddress">重复地址</param>
        public void Start(int port, bool reuseAddress = false)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            //创建listens是传入的套接字。
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            listenSocket.ExclusiveAddressUse = !reuseAddress;
            listenSocket.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddress);
            //绑定端口           
            listenSocket.Bind(localEndPoint);
            byte[] receivebuffer = new byte[m_receiveBufferSize];
            SocketAsyncEventArgs receiveSocketArgs = new SocketAsyncEventArgs();
            receiveSocketArgs.RemoteEndPoint = localEndPoint;
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
                    OnReceive(e.RemoteEndPoint, e.Buffer, e.Offset, e.BytesTransferred);
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
                if (sendQueues[(int)thread].TryDequeue(out sending))
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
        /// <param name="remoteEndPoint">远程ip与端口</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        public void Send(EndPoint remoteEndPoint, byte[] data, int offset, int length)
        {
            sendQueues[((IPEndPoint)remoteEndPoint).Port % sendthread].Enqueue(new SendingQueue() { remoteEndPoint = remoteEndPoint, data = data, offset = offset, length = length });
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
                    OnSend(e.RemoteEndPoint, e.BytesTransferred);
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
