using System;
using FreeSql.DataAnnotations;

namespace LibCommon.Structs.DBModels
{
    /// <summary>
    /// 录制文件实例
    /// </summary>
    [Serializable]
    [Table(Name = "RecordFile")]
    [Index("idx_rfs_maid", "MainId", false)]
    [Index("idx_rfs_chnn", "ChannelName", false)]
    [Index("idx_rfs_msid", "MediaServerId", false)]
    [Index("idx_rfs_dept", "DepartmentId", false)]
    [Index("idx_rfs_ipv4", "IpV4Address", false)]
    [Index("idx_rfs_ipv6", "IpV6Address", false)]
    [Index("idx_rfs_del", "Deleted", false)]
    [Index("idx_rfs_undo", "Undo", false)]
    [Index("idx_rfs_setime", "StartTime,EndTime", false)]
    [Index("idx_rfs_stime", "StartTime", false)]
    [Index("idx_rfs_etime", "EndTime", false)]
    [Index("idx_rfs_rede", "RecordDate", false)]
    public class RecordFile
    {
        private string? _app;
        private string? _channelId;
        private string? _channelName;
        private DateTime _createTime;
        private bool? _deleted;
        private string? _departmentId;
        private string? _departmentName;
        private string? _deviceId;
        private string? _downloadUrl;
        private long? _duration;
        private DateTime? _endTime;
        private long? _fileSize;
        private long _id;
        private string _mainId;
        private string _mediaServerId;
        private string _mediaServerIp;
        private string? _pDepartmentId;
        private string? _pDepartmentName;
        private string? _recordDate;
        private DateTime? _startTime;
        private string? _streamid;
        private bool? _undo;
        private DateTime? _updateTime;
        private string? _vhost;
        private string? _videoPath;
        private string? _videoSrcUrl;

        /// <summary>
        /// 数据库主键
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// 通道唯一id
        /// </summary>
        public string MainId
        {
            get => _mainId;
            set => _mainId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 流媒体服务器IP地址
        /// </summary>
        public string MediaServerIp
        {
            get => _mediaServerIp;
            set => _mediaServerIp = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 通道名称
        /// </summary>
        public string? ChannelName
        {
            get => _channelName;
            set => _channelName = value;
        }

        /// <summary>
        /// 部门代码
        /// </summary>
        public string? DepartmentId
        {
            get => _departmentId;
            set => _departmentId = value;
        }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string? DepartmentName
        {
            get => _departmentName;
            set => _departmentName = value;
        }

        /// <summary>
        /// 上级部门代码
        /// </summary>
        public string? PDepartmentId
        {
            get => _pDepartmentId;
            set => _pDepartmentId = value;
        }

        /// <summary>
        /// 上级部门名称
        /// </summary>
        public string? PDepartmentName
        {
            get => _pDepartmentName;
            set => _pDepartmentName = value;
        }

        /// <summary>
        /// GB28181设备ID
        /// </summary>
        public string? DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        /// <summary>
        /// GB21818设备通道ID
        /// </summary>
        public string? ChannelId
        {
            get => _channelId;
            set => _channelId = value;
        }

        /// <summary>
        /// 非gb28181设备的视频流源地址
        /// </summary>
        public string? VideoSrcUrl
        {
            get => _videoSrcUrl;
            set => _videoSrcUrl = value;
        }


        /// <summary>
        /// 文件的开始时间
        /// </summary>
        public DateTime? StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        /// <summary>
        /// 文件的结束时间
        /// </summary>
        public DateTime? EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        /// <summary>
        /// 文件的时长
        /// </summary>
        public long? Duration
        {
            get => _duration;
            set => _duration = value;
        }

        /// <summary>
        /// 文件的所在位置 
        /// </summary>
        public string? VideoPath
        {
            get => _videoPath;
            set => _videoPath = value;
        }

        /// <summary>
        /// 文件大小 
        /// </summary>
        public long? FileSize
        {
            get => _fileSize;
            set => _fileSize = value;
        }

        /// <summary>
        /// vhost
        /// </summary>
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        /// <summary>
        /// stream
        /// </summary>
        public string? Streamid
        {
            get => _streamid;
            set => _streamid = value;
        }

        /// <summary>
        /// app
        /// </summary>
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// 文件下载与播放地址
        /// </summary>
        public string? DownloadUrl
        {
            get => _downloadUrl;
            set => _downloadUrl = value;
        }

        /// <summary>
        /// 记录创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }

        /// <summary>
        /// 记录更新时间
        /// </summary>
        public DateTime? UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }

        /// <summary>
        /// 记录日期
        /// </summary>
        public string? RecordDate
        {
            get => _recordDate;
            set => _recordDate = value;
        }

        /// <summary>
        /// 是否可撤销删除操作
        /// </summary>
        public bool? Undo
        {
            get => _undo;
            set => _undo = value;
        }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? Deleted
        {
            get => _deleted;
            set => _deleted = value;
        }
    }
}