using System;
using FreeSql.DataAnnotations;
using LibCommon.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.DBModels
{
    /// <summary>
    /// 摄像头通道实例
    /// </summary>
    [Serializable]
    [Table(Name = "VideoChannels")]
    [Index("idx_vcs_maid", "MainId", true)]
    [Index("idx_vcs_chnn", "ChannelName", false)]
    [Index("idx_vcs_msid", "MediaServerId", false)]
    [Index("idx_vcs_dept", "DepartmentId", false)]
    [Index("idx_vcs_ipv4", "IpV4Address", false)]
    [Index("idx_vcs_ipv6", "IpV6Address", false)]
    [Index("idx_vcs_enbl", "Enabled", false)]
    public class VideoChannel
    {
        private string? _app;
        private bool _autoRecord;
        private bool _autoVideo;
        private string? _channelId;
        private string? _channelName;
        private DateTime _createTime;
        private bool? _defaultRtpPort;
        private string? _departmentId;
        private string? _departmentName;
        private string? _deviceId;
        private DeviceNetworkType _deviceNetworkType;
        private DeviceStreamType _deviceStreamType;
        private bool? _enabled;
        private bool _hasPtz;
        private long _id;
        private string _ipV4Address;
        private string? _ipV6Address;
        private string _mainId;
        private string _mediaServerId;
        private MethodByGetStream _methodByGetStream;
        private bool? _noPlayerBreak;
        private string? _pDepartmentId;
        private string? _pDepartmentName;
        private string? _recordPlanName;
        private int? _recordSecs = 0;
        private bool? _rtpWithTcp;
        private DateTime _updateTime;
        private string? _vhost;
        private VideoDeviceType _videoDeviceType;
        private string? _videoSrcUrl;
        private string? _ffmpegTemplate;
        private bool _isShareChannel;
        private string? _shareUrl;
        private string? _shareDeviceId;


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
        /// 设备的唯一ID
        /// </summary>
        [Column(DbType = "varchar(32) NOT NULL")]
        public string MainId
        {
            get => _mainId;
            set => _mainId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        [Column(DbType = "varchar(64) NOT NULL")]
        public string MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// vhost
        /// </summary>
        [Column(DbType = "varchar(64) NOT NULL")]
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        /// <summary>
        /// app
        /// </summary>
        [Column(DbType = "varchar(64) NOT NULL")]
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// 通道名称，整个系统唯一
        /// </summary>
        [Column(DbType = "varchar(64)")]
        public string? ChannelName
        {
            get => _channelName;
            set => _channelName = value;
        }

        /// <summary>
        /// 部门代码
        /// </summary>
        [Column(DbType = "varchar(20)")]
        public string? DepartmentId
        {
            get => _departmentId;
            set => _departmentId = value;
        }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Column(DbType = "varchar(64)")]
        public string? DepartmentName
        {
            get => _departmentName;
            set => _departmentName = value;
        }

        /// <summary>
        /// 上级部门代码
        /// </summary>
        [Column(DbType = "varchar(20)")]
        public string? PDepartmentId
        {
            get => _pDepartmentId;
            set => _pDepartmentId = value;
        }

        /// <summary>
        /// 上级部门名称
        /// </summary>
        [Column(DbType = "varchar(64)")]
        public string? PDepartmentName
        {
            get => _pDepartmentName;
            set => _pDepartmentName = value;
        }


        /// <summary>
        /// 设备的网络类型
        /// </summary>
        [Column(MapType = typeof(string))]
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceNetworkType DeviceNetworkType
        {
            get => _deviceNetworkType;
            set => _deviceNetworkType = value;
        }

        /// <summary>
        /// 设备的流类型
        /// </summary>
        [Column(MapType = typeof(string))]
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceStreamType DeviceStreamType
        {
            get => _deviceStreamType;
            set => _deviceStreamType = value;
        }

        /// <summary>
        /// 使用哪种方式拉取非rtp设备的流
        /// </summary>
        [Column(MapType = typeof(string))]
        [JsonConverter(typeof(StringEnumConverter))]
        public MethodByGetStream MethodByGetStream
        {
            get => _methodByGetStream;
            set => _methodByGetStream = value;
        }

        /// <summary>
        /// 设备类型，IPC,NVR,DVR
        /// </summary>
        [Column(MapType = typeof(string))]
        [JsonConverter(typeof(StringEnumConverter))]
        public VideoDeviceType VideoDeviceType
        {
            get => _videoDeviceType;
            set => _videoDeviceType = value;
        }


        /// <summary>
        /// 是否自动启用推拉流
        /// </summary>
        public bool AutoVideo
        {
            get => _autoVideo;
            set => _autoVideo = value;
        }

        /// <summary>
        /// 是否自动启用录制计划
        /// </summary>
        public bool AutoRecord
        {
            get => _autoRecord;
            set => _autoRecord = value;
        }

        /// <summary>
        /// 录制时长（秒）
        /// </summary>
        public int? RecordSecs
        {
            get => _recordSecs;
            set => _recordSecs = value;
        }

        /// <summary>
        /// 录制计划模板名称
        /// </summary>
        public string? RecordPlanName
        {
            get => _recordPlanName;
            set => _recordPlanName = value;
        }

        /// <summary>
        /// 设备的ipv4地址
        /// </summary>
        [Column(DbType = "varchar(16)")]
        public string IpV4Address
        {
            get => _ipV4Address;
            set => _ipV4Address = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 设备的ipv6地址
        /// </summary>
        [Column(DbType = "varchar(64)")]
        public string? IpV6Address
        {
            get => _ipV6Address;
            set => _ipV6Address = value;
        }

        /// <summary>
        /// 设备是否有云台控制
        /// </summary>
        public bool HasPtz
        {
            get => _hasPtz;
            set => _hasPtz = value;
        }

        /// <summary>
        /// GB28181设备的SipDevice.DeviceId
        /// </summary>
        [Column(DbType = "varchar(20)")]
        public string? DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        /// <summary>
        /// GB28181设备的SipChannel.DeviceId
        /// </summary>
        [Column(DbType = "varchar(20)")]
        public string? ChannelId
        {
            get => _channelId;
            set => _channelId = value;
        }

        /// <summary>
        /// Rtp设备是否使用Tcp推流
        /// </summary>
        public bool? RtpWithTcp
        {
            get => _rtpWithTcp;
            set => _rtpWithTcp = value;
        }

        /// <summary>
        /// 非Rtp设备的视频流源地址
        /// </summary>
        [Column(DbType = "varchar(255)")]
        public string? VideoSrcUrl
        {
            get => _videoSrcUrl;
            set => _videoSrcUrl = value;
        }

        /// <summary>
        /// Rtp设备是否使用流媒体默认rtp端口，如10000端口
        /// </summary>
        public bool? DefaultRtpPort
        {
            get => _defaultRtpPort;
            set => _defaultRtpPort = value;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }


        /// <summary>
        /// 无人观察时断开流端口，此字段为true时AutoVideo字段必须为Flase
        /// 如果AutoVideo为true,则此字段无效
        /// </summary>
        public bool? NoPlayerBreak
        {
            get => _noPlayerBreak;
            set => _noPlayerBreak = value;
        }

        /// <summary>
        /// ffmpeg的模板名称，可指定这个流使用哪个ffmpeg模板进行拉流
        /// 留空时，采用默认配置进行拉流
        /// </summary>
        public string? FFmpegTemplate
        {
            get => _ffmpegTemplate;
            set => _ffmpegTemplate = value;
        }

        /// <summary>
        /// 是否为可分享通道？
        /// 如果为true,则此通道可以被推往其他服务器
        /// </summary>
        public bool IsShareChannel
        {
            get => _isShareChannel;
            set => _isShareChannel = value;
        }

        /// <summary>
        /// 分享通道地址
        /// 如果IsShareChannel为true,而ShareUrl为空，则表示此通道可以分享给GB28181服务器
        /// 如果IsShareChannel为true,而ShareUrl不为空，则表示此通道可以分享线GB28181服务
        /// 器的同时还可以分享给其他流媒体服务器
        /// </summary>
        public string? ShareUrl
        {
            get => _shareUrl;
            set => _shareUrl = value;
        }

        /// <summary>
        /// 共享通道时此通道的唯一id
        /// gb28181时可以是deviceid
        /// 其他服务时可以按照其他服务的
        /// 规则来确定此id
        /// </summary>
        public string? ShareDeviceId
        {
            get => _shareDeviceId;
            set => _shareDeviceId = value;
        }
    }
}