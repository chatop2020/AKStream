
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace socket.core.Common
{
    /// <summary>
    /// 表示可重用的SocketAsyncEventArgs对象的集合。
    /// </summary>
    internal class SocketAsyncEventArgsPool
    {
        /// <summary>
        /// 重用池原数据
        /// </summary>
        private Stack<SocketAsyncEventArgs> m_pool;

        /// <summary>
        /// 将对象池初始化为指定的大小
        /// </summary>
        /// <param name="capacity">最大数量该池可以容纳的SocketAsyncEventArgs对象</param>
        internal SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// 将一个SocketAsyncEventArgs实例添加到池中
        /// </summary>
        /// <param name="item">SocketAsyncEventArgs实例添加到缓存池</param>
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("添加到SocketAsyncEventArgsPool的项目不能为空");
            }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }

        /// <summary>
        /// 从池中移除一个SocketAsyncEventArgs实例并返回从缓存池中移除的对象
        /// </summary>
        /// <returns></returns>
        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        /// <summary>
        /// 池中的SocketAsyncEventArgs实例的数量
        /// </summary>
        public int Count
        {
            get { return m_pool.Count; }
        }
    }
}
