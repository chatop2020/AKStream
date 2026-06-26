using System;
using System.Collections.Generic;
using LibCommon.Structs.GB28181.XML;

namespace LibCommon.Structs
{
    /// <summary>
    /// 用于保存channel的录像文件列表
    /// </summary>
    [Serializable]
    public class VideoChannelRecordInfo
    {
        private DateTime _expires;


        private long _id;
        private List<RecordInfo.RecItem> _recItems = new List<RecordInfo.RecItem>();
        private int _taskId;
        private int _tatolCount;

        public long Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// 查询的taskId=报文中的sn
        /// </summary>
        public int TaskId
        {
            get => _taskId;
            set => _taskId = value;
        }


        /// <summary>
        /// 记录列表
        /// </summary>
        public List<RecordInfo.RecItem> RecItems
        {
            get => _recItems;
            set => _recItems = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 列表总数
        /// </summary>
        public int TatolCount
        {
            get => _tatolCount;
            set => _tatolCount = value;
        }

        /// <summary>
        /// 当前已经获取到的数量
        /// </summary>
        public int GetCount
        {
            get => _recItems.Count;
        }

        /// <summary>
        /// 数据过期时间
        /// </summary>
        public DateTime Expires
        {
            get => _expires;
            set => _expires = value;
        }
    }
}