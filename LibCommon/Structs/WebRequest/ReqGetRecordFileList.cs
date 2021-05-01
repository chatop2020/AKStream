using System;

namespace LibCommon.Structs.WebRequest
{
    [Serializable]
    public class ReqGetRecordFileList : ReqPaginationBase
    {
        private string? _app;
        private string? _channelId;
        private string? _channelName;
        private DateTime? _createTime;
        private bool? _deleted;
        private string? _departmentId;
        private string? _departmentName;
        private string? _deviceId;
        private string? _downloadUrl;
        private long? _duration;
        private DateTime? _endTime;
        private long? _fileSize;
        private long? _id;
        private string? _ipV4Address;
        private string? _ipV6Address;
        private string? _mainId;
        private string? _mediaServerId;
        private string? _mediaServerIp;
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
        public long? Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// 音视频通道唯一 id
        /// </summary>
        public string? MainId
        {
            get => _mainId;
            set => _mainId = value;
        }

        /// <summary>
        /// 流媒体服务器id
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        /// <summary>
        /// 流媒体服务器ip
        /// </summary>
        public string? MediaServerIp
        {
            get => _mediaServerIp;
            set => _mediaServerIp = value;
        }

        /// <summary>
        /// 音视频通道名称
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
        /// gb28181设备id
        /// </summary>
        public string? DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        /// <summary>
        /// gb28181设备通道id
        /// </summary>
        public string? ChannelId
        {
            get => _channelId;
            set => _channelId = value;
        }

        /// <summary>
        /// 非gb28181设备的视频源地址
        /// </summary>
        public string? VideoSrcUrl
        {
            get => _videoSrcUrl;
            set => _videoSrcUrl = value;
        }

        /// <summary>
        /// 设备ipv4地址
        /// </summary>
        public string? IpV4Address
        {
            get => _ipV4Address;
            set => _ipV4Address = value;
        }

        /// <summary>
        /// 设备ipv6地址
        /// </summary>
        public string? IpV6Address
        {
            get => _ipV6Address;
            set => _ipV6Address = value;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        /// <summary>
        /// 视频时长
        /// </summary>
        public long? Duration
        {
            get => _duration;
            set => _duration = value;
        }

        /// <summary>
        /// 视频文件路径
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
        /// 播放下载地址
        /// </summary>
        public string? DownloadUrl
        {
            get => _downloadUrl;
            set => _downloadUrl = value;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }

        /// <summary>
        /// 更新时间
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
        /// 是否可撤销删除
        /// </summary>
        public bool? Undo
        {
            get => _undo;
            set => _undo = value;
        }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool? Deleted
        {
            get => _deleted;
            set => _deleted = value;
        }
    }
}