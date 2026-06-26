using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    /// <summary>
    /// 踢掉多个session的回复结构
    /// </summary>
    [Serializable]
    public class ResZLMediaKitKickSessions : ResZLMediaKitKickSession
    {
        private int _count_hit;

        /// <summary>
        /// 筛选命中客户端个数
        /// </summary>
        public int Count_Hit
        {
            get => _count_hit;
            set => _count_hit = value;
        }
    }
}