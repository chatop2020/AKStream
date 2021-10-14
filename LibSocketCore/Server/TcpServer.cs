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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Concurrent;
using socket.core.Common;

namespace socket.core.Server
{
    /// <summary>
    /// tcp Socket监听基库
    /// </summary>
    internal class TcpServer
    {
        /// <summary>
        /// 连接标示 自增长
        /// </summary>
        private int connectId;
        /// <summary>
        /// 同时处理的最大连接数
        /// </summary>
        private int m_numConnections;
        /// <summary>
        /// 用于每个套接字I/O操作的缓冲区大小
        /// </summary>
        private int m_receiveBufferSize;
        /// <summary>
        /// 所有套接字接收操作的一个可重用的大型缓冲区集合。
        /// </summary>
        private BufferManager m_bufferManager;
        /// <summary>
        /// 用于监听传入连接请求的套接字
        /// </summary>
        private Socket listenSocket;
        /// <summary>
        /// 接受端SocketAsyncEventArgs对象重用池，接受套接字操作
        /// </summary>
        private SocketAsyncEventArgsPool m_receivePool;
        /// <summary>
        /// 发送端SocketAsyncEventArgs对象重用池，发送套接字操作
        /// </summary>
        private SocketAsyncEventArgsPool m_sendPool;
        /// <summary>
        /// 超时，如果超时，服务端断开连接，客户端需要重连操作
        /// </summary>
        private int overtime;
        /// <summary>
        /// 超时检查间隔时间(秒)
        /// </summary>
        private int overtimecheck = 1;
        /// <summary>
        /// 能接到最多客户端个数的原子操作
        /// </summary>
        private Semaphore m_maxNumberAcceptedClients;
        /// <summary>
        /// 已经连接的对象池
        /// </summary>
        internal ConcurrentDictionary<int, ConnectClient> connectClient;
        /// <summary>
        /// 客户端列表
        /// </summary>
        internal ConcurrentDictionary<int, string> clientList;
        /// <summary>
        /// 发送线程数
        /// </summary>
        private int sendthread = 10;
        /// <summary>
        /// 需要发送的数据
        /// </summary>
        private ConcurrentQueue<SendingQueue>[] sendQueues;
        /// <summary>
        /// 锁
        /// </summary>
        private Mutex mutex = new Mutex();
        /// <summary>
        /// 连接成功事件 item1:connectId
        /// </summary>
        internal event Action<int> OnAccept;
        /// <summary>
        /// 接收通知事件 item1:connectId,item2:数据,item3:偏移位,item4:长度
        /// </summary>
        internal event Action<int, byte[], int, int> OnReceive;
        /// <summary>
        /// 已发送通知事件 item1:connectId,item2:长度
        /// </summary>
        internal event Action<int, int> OnSend;
        /// <summary>
        /// 断开连接通知事件 item1:connectId,
        /// </summary>
        internal event Action<int> OnClose;

        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overTime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        internal TcpServer(int numConnections, int receiveBufferSize, int overTime)
        {
            overtime = overTime;
            m_numConnections = numConnections;
            m_receiveBufferSize = receiveBufferSize;
            m_bufferManager = new BufferManager(receiveBufferSize * m_numConnections, receiveBufferSize);
            m_receivePool = new SocketAsyncEventArgsPool(m_numConnections);
            m_sendPool = new SocketAsyncEventArgsPool(m_numConnections);
            m_maxNumberAcceptedClients = new Semaphore(m_numConnections, m_numConnections);
            Init();
        }

        /// <summary>
        /// 初始化服务器通过预先分配的可重复使用的缓冲区和上下文对象。这些对象不需要预先分配或重用，但这样做是为了说明API如何可以易于用于创建可重用对象以提高服务器性能。
        /// </summary>
        private void Init()
        {
            connectClient = new ConcurrentDictionary<int, ConnectClient>();
            clientList = new ConcurrentDictionary<int, string>();
            sendQueues = new ConcurrentQueue<SendingQueue>[sendthread];
            for (int i = 0; i < sendthread; i++)
            {
                sendQueues[i] = new ConcurrentQueue<SendingQueue>();
            }
            //分配一个大字节缓冲区，所有I/O操作都使用一个。这个侍卫对内存碎片
            m_bufferManager.InitBuffer();
            //预分配的接受对象池socketasynceventargs，并分配缓存
            SocketAsyncEventArgs saea_receive;
            //分配的发送对象池socketasynceventargs，但是不分配缓存
            SocketAsyncEventArgs saea_send;
            for (int i = 0; i < m_numConnections; i++)
            {
                //预先接受端分配一组可重用的消息
                saea_receive = new SocketAsyncEventArgs();
                saea_receive.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                //分配缓冲池中的字节缓冲区的socketasynceventarg对象
                m_bufferManager.SetBuffer(saea_receive);
                m_receivePool.Push(saea_receive);
                //预先发送端分配一组可重用的消息
                saea_send = new SocketAsyncEventArgs();
                saea_send.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                m_sendPool.Push(saea_send);
            }
        }

        /// <summary>
        /// 启动tcp服务侦听
        /// </summary>       
        /// <param name="port">监听端口</param>
        internal void Start(int port)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            //创建listens是传入的套接字。
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.NoDelay = true;
            //绑定端口
            listenSocket.Bind(localEndPoint);
            //挂起的连接队列的最大长度。
            listenSocket.Listen(1000);
            //在监听套接字上接受
            StartAccept(null);
            //发送线程
            for (int i = 0; i < sendthread; i++)
            {
                Thread thread = new Thread(StartSend);
                thread.IsBackground = true;
                thread.Priority = ThreadPriority.AboveNormal;
                thread.Start(i);
            }
            //超时机制
            if (overtime > 0)
            {
                Thread heartbeat = new Thread(new ThreadStart(() =>
                {
                    Heartbeat();
                }));
                heartbeat.IsBackground = true;
                heartbeat.Priority = ThreadPriority.Lowest;
                heartbeat.Start();
            }
        }

        /// <summary>
        /// 超时机制
        /// </summary>
        private void Heartbeat()
        {
            //计算超时次数 ，超过count就当客户端断开连接。服务端清除该连接资源
            int count = overtime / overtimecheck;
            while (true)
            {
                foreach (var item in connectClient.Values)
                {
                    if (item.keep_alive >= count)
                    {
                        item.keep_alive = 0;
                        CloseClientSocket(item.saea_receive);
                    }
                }
                foreach (var item in connectClient.Values)
                {
                    item.keep_alive++;
                }
                Thread.Sleep(overtimecheck * 1000);
            }
        }

        #region Accept

        /// <summary>
        /// 开始接受客户端的连接请求的操作。
        /// </summary>
        /// <param name="acceptEventArg">发布时要使用的上下文对象服务器侦听套接字上的接受操作</param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            }
            else
            {
                // 套接字必须被清除，因为上下文对象正在被重用。
                acceptEventArg.AcceptSocket = null;
            }
            m_maxNumberAcceptedClients.WaitOne();
            //准备一个客户端接入
            if (!listenSocket.AcceptAsync(acceptEventArg))
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// 当异步连接完成时调用此方法
        /// </summary>
        /// <param name="e">操作对象</param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            connectId++;
            //把连接到的客户端信息添加到集合中
            ConnectClient connecttoken = new ConnectClient();
            connecttoken.socket = e.AcceptSocket;
            //从接受端重用池获取一个新的SocketAsyncEventArgs对象
            connecttoken.saea_receive = m_receivePool.Pop();
            connecttoken.saea_receive.UserToken = connectId;
            connecttoken.saea_receive.AcceptSocket = e.AcceptSocket;
            connectClient.TryAdd(connectId, connecttoken);
            clientList.TryAdd(connectId, e.AcceptSocket.RemoteEndPoint.ToString());
            //一旦客户机连接，就准备接收。
            if (!e.AcceptSocket.ReceiveAsync(connecttoken.saea_receive))
            {
                ProcessReceive(connecttoken.saea_receive);
            }
            //事件回调
            if (OnAccept != null)
            {
                OnAccept(connectId);
            }
            //接受第二连接的请求
            StartAccept(e);
        }

        #endregion

        #region 接受处理 receive

        /// <summary>
        /// 接受处理回调
        /// </summary>
        /// <param name="e">操作对象</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            //检查远程主机是否关闭连接
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                int connectId = (int)e.UserToken;
                ConnectClient client;
                if (!connectClient.TryGetValue(connectId, out client))
                {
                    return;
                }
                //如果接收到数据，超时记录设置为0
                if (overtime > 0)
                {
                    if (client != null)
                    {
                        client.keep_alive = 0;
                    }
                }
                //回调               
                if (OnReceive != null)
                {
                    if (client != null)
                    {
                        OnReceive(connectId, e.Buffer, e.Offset, e.BytesTransferred);
                    }
                }
                //准备下次接收数据      
                try
                {
                    if (!e.AcceptSocket.ReceiveAsync(e))
                    {
                        ProcessReceive(e);
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    if (OnClose != null)
                    {
                        OnClose(connectId);
                    }
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        #endregion

        #region 发送处理 send

        /// <summary>
        /// 开始启用发送
        /// </summary>
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
        /// 异步发送消息 
        /// </summary>
        /// <param name="connectId">连接ID</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        internal void Send(int connectId, byte[] data, int offset, int length)
        {
            sendQueues[connectId % sendthread].Enqueue(new SendingQueue() { connectId = connectId, data = data, offset = offset, length = length });
        }

        /// <summary>
        /// 异步发送消息 
        /// </summary>
        /// <param name="sendQuere">发送消息体</param>
        private void Send(SendingQueue sendQuere)
        {
            ConnectClient client;
            if (!connectClient.TryGetValue(sendQuere.connectId, out client))
            {
                return;
            }
            //如果发送池为空时，临时新建一个放入池中
            mutex.WaitOne();
            if (m_sendPool.Count == 0)
            {
                SocketAsyncEventArgs saea_send = new SocketAsyncEventArgs();
                saea_send.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                m_sendPool.Push(saea_send);
            }
            SocketAsyncEventArgs sendEventArgs = m_sendPool.Pop();
            mutex.ReleaseMutex();
            sendEventArgs.UserToken = sendQuere.connectId;
            sendEventArgs.SetBuffer(sendQuere.data, sendQuere.offset, sendQuere.length);
            try
            {
                if (!client.socket.SendAsync(sendEventArgs))
                {
                    ProcessSend(sendEventArgs);
                }
            }
            catch (ObjectDisposedException ex)
            {
                if (OnClose != null)
                {
                    OnClose(sendQuere.connectId);
                }
            }
            sendQuere = null;
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
                    OnSend((int)e.UserToken, e.BytesTransferred);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        #endregion

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
                case SocketAsyncOperation.Accept:
                    ProcessAccept(e);
                    break;
                default:
                    break;
            }
        }

        #region 断开连接处理


        /// <summary>
        /// 客户端断开一个连接
        /// </summary>
        /// <param name="connectId">连接标记</param>
        internal void Close(int connectId)
        {
            ConnectClient client;
            if (!connectClient.TryGetValue(connectId, out client))
            {
                return;
            }
            CloseClientSocket(client.saea_receive);
        }

        /// <summary>
        /// 断开一个连接
        /// </summary>
        /// <param name="e">操作对象</param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                int connectId = (int)e.UserToken;
                ConnectClient client;
                string clientip;
                if (!connectClient.TryGetValue(connectId, out client))
                {
                    return;
                }
                if (client.socket.Connected == false)
                {
                    connectClient.TryRemove(connectId, out client);
                    clientList.TryRemove(connectId, out clientip);
                    return;
                }
                try
                {
                    client.socket.Shutdown(SocketShutdown.Both);
                }
                // 抛出客户端进程已经关闭
                catch (Exception) { }
                client.socket.Close();
                m_receivePool.Push(e);
                m_maxNumberAcceptedClients.Release();
                if (OnClose != null)
                {
                    OnClose(connectId);
                }
                connectClient.TryRemove(connectId, out client);
                clientList.TryRemove(connectId, out clientip);
                client = null;
            }
        }

        #endregion

        #region 附加数据

        /// <summary>
        /// 给连接对象设置附加数据
        /// </summary>
        /// <param name="connectId">连接标识</param>
        /// <param name="data">附加数据</param>
        /// <returns>true:设置成功,false:设置失败</returns>
        internal bool SetAttached(int connectId, object data)
        {
            ConnectClient client;
            if (!connectClient.TryGetValue(connectId, out client))
            {
                return false;
            }
            client.attached = data;
            return true;
        }

        /// <summary>
        /// 获取连接对象的附加数据
        /// </summary>
        /// <param name="connectId">连接标识</param>
        /// <returns>附加数据，如果没有找到则返回null</returns>
        internal T GetAttached<T>(int connectId)
        {
            ConnectClient client;
            if (!connectClient.TryGetValue(connectId, out client))
            {
                return default(T);
            }
            else
            {
                return (T)client.attached;
            }
        }
        #endregion
    }

}