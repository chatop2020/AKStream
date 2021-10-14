using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace socket.core.Common
{
    /// <summary>
    /// 缓存管理类
    /// </summary>
    public class BufferManager
    {
        /// <summary>
        /// 缓冲池控制的总字节数
        /// </summary>
        private int m_numBytes;
        /// <summary>
        /// 缓冲区管理器维护的底层字节数组
        /// </summary>
        private byte[] m_buffer;
        /// <summary>
        /// 偏移位
        /// </summary>
        private Stack<int> m_freeIndexPool;
        /// <summary>
        /// 当前偏移位
        /// </summary>
        private int m_currentIndex;
        /// <summary>
        /// 缓存大小
        /// </summary>
        private int m_bufferSize;

        /// <summary>
        /// 初始化缓存
        /// </summary>
        /// <param name="totalBytes">缓存区总大小</param>
        /// <param name="bufferSize">缓存大小</param>
        public BufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// 分配缓冲池使用的缓冲区空间
        /// </summary>
        public void InitBuffer()
        {
            //创造一个巨大的缓冲区并将其分开出来给每个SocketAsyncEventArg对象
            m_buffer = new byte[m_numBytes];
        }

        /// <summary>
        /// 将缓冲池中的缓冲区分配给指定SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="args">如果缓冲区成功设置，则为true，否则为false</param>
        /// <returns></returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                {
                    return false;
                }
                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                m_currentIndex += m_bufferSize;
            }
            return true;
        }

        /// <summary>
        /// 从SocketAsyncEventArg对象中删除缓冲区。这将缓冲区释放回缓冲池
        /// </summary>
        /// <param name="args">操作对象</param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

    }
}
