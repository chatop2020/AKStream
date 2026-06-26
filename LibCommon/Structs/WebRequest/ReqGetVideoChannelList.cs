using System;
using LibCommon.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.WebRequest
{
    /// <summary>
    /// 音视频通道查询结构（支持分页）
    /// </summary>
    [Serializable]
    public class ReqGetVideoChannelList : ReqPaginationBase
    {
        private string? _app;
        private bool? _autoRecord;
        private bool? _autoVideo;
        private string? _channelId;
        private string? _channelIdLike;
        private string? _channelName;
        private string? _channelNameLike;
        private DateTime? _createTime;
        private bool? _defaultRtpPort;
        private string? _departmentId;
        private string? _departmentName;
        private string? _departmentNameLike;
        private string? _deviceId;
        private string? _deviceIdLike;
        private DeviceNetworkType? _deviceNetworkType;
        private DeviceStreamType? _deviceStreamType;
        private bool? _enabled;
        private bool? _hasPtz;
        private long? _id;
        private bool? _includeSubDeptartment;
        private string? _ipV4Address;
        private string? _ipV4AddressLike;
        private string? _ipV6Address;
        private string? _ipV6AddressLike;
        private string? _mainId;
        private string? _mediaServerId;
        private MethodByGetStream? _methodByGetStream;
        private bool? _noPlayerBreak;
        private string? _pDepartmentId;
        private string? _pDepartmentName;
        private string? _recordPlanName;
        private bool? _rtpWithTcp;
        private DateTime? _updateTime;
        private string? _vhost;
        private VideoDeviceType? _videoDeviceType;
        private string? _videoSrcUrl;
        private string? _videoSrcUrlLike;

        /// <summary>
        /// 数据库主键
        /// </summary>
        public long? Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// 通道唯一ID
        /// </summary>
        public string? MainId
        {
            get => _mainId;
            set => _mainId = value;
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
        /// vhost
        /// </summary>
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }


        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
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
        /// 设备网络类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceNetworkType? DeviceNetworkType
        {
            get => _deviceNetworkType;
            set => _deviceNetworkType = value;
        }

        /// <summary>
        /// 设备流类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceStreamType? DeviceStreamType
        {
            get => _deviceStreamType;
            set => _deviceStreamType = value;
        }

        /// <summary>
        /// 取流方法
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MethodByGetStream? MethodByGetStream
        {
            get => _methodByGetStream;
            set => _methodByGetStream = value;
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public VideoDeviceType? VideoDeviceType
        {
            get => _videoDeviceType;
            set => _videoDeviceType = value;
        }

        /// <summary>
        /// 是否自动推拉流
        /// </summary>
        public bool? AutoVideo
        {
            get => _autoVideo;
            set => _autoVideo = value;
        }

        /// <summary>
        /// 是否自动录制
        /// </summary>
        public bool? AutoRecord
        {
            get => _autoRecord;
            set => _autoRecord = value;
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
        /// ipv4地址
        /// </summary>
        public string? IpV4Address
        {
            get => _ipV4Address;
            set => _ipV4Address = value;
        }

        /// <summary>
        /// ipv6地址
        /// </summary>
        public string? IpV6Address
        {
            get => _ipV6Address;
            set => _ipV6Address = value;
        }

        /// <summary>
        /// 是否有云台控制
        /// </summary>
        public bool? HasPtz
        {
            get => _hasPtz;
            set => _hasPtz = value;
        }

        /// <summary>
        /// gb28181的设备id
        /// </summary>
        public string? DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        /// <summary>
        /// gb28181的通道id
        /// </summary>
        public string? ChannelId
        {
            get => _channelId;
            set => _channelId = value;
        }

        /// <summary>
        /// gb28181推流是否采用tcp
        /// </summary>
        public bool? RtpWithTcp
        {
            get => _rtpWithTcp;
            set => _rtpWithTcp = value;
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
        /// gb28181推流是否采用默认端口
        /// </summary>
        public bool? DefaultRtpPort
        {
            get => _defaultRtpPort;
            set => _defaultRtpPort = value;
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
        /// 是否已激活
        /// </summary>
        public bool? Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        /// 无人观看时是否断流
        /// </summary>
        public bool? NoPlayerBreak
        {
            get => _noPlayerBreak;
            set => _noPlayerBreak = value;
        }

        /// <summary>
        /// 数据库中用like匹配的通道名称，此字段不为空时，条件中的ChannelName将被置空
        /// </summary>
        public string? ChannelNameLike
        {
            get => _channelNameLike;
            set => _channelNameLike = value;
        }

        /// <summary>
        /// 数据库中用like匹配的部门代码，此字段不为空时，条件中的DepartmentName将被置空
        /// </summary>
        public string? DepartmentNameLike
        {
            get => _departmentNameLike;
            set => _departmentNameLike = value;
        }

        /// <summary>
        /// 数据库中用like匹配的IPV4地址，此字段不为空时，条件中的IpV4Address将被置空
        /// </summary>
        public string? IpV4AddressLike
        {
            get => _ipV4AddressLike;
            set => _ipV4AddressLike = value;
        }

        /// <summary>
        /// 数据库中用like匹配的IPV6地址，此字段不为空时，条件中的IpV6Address将被置空
        /// </summary>
        public string? IpV6AddressLike
        {
            get => _ipV6AddressLike;
            set => _ipV6AddressLike = value;
        }

        /// <summary>
        /// 数据库中用like匹配的视频源地址，此字段不为空时，条件中的VideoSrcUrl将被置空
        /// </summary>
        public string? VideoSrcUrlLike
        {
            get => _videoSrcUrlLike;
            set => _videoSrcUrlLike = value;
        }

        /// <summary>
        /// 数据库中用like匹配的GB28181设备ID，此字段不为空时，条件中的DeviceId将被置空
        /// </summary>
        public string? DeviceIdLike
        {
            get => _deviceIdLike;
            set => _deviceIdLike = value;
        }

        /// <summary>
        /// 数据库中用like匹配的GB28181通道ID，此字段不为空时，条件中的ChannelId将被置空
        /// </summary>
        public string? ChannelIdLike
        {
            get => _channelIdLike;
            set => _channelIdLike = value;
        }

        /// <summary>
        /// 数据是否包含子部门
        /// 当此项为true的时候，条件中除了DepartmentId被保留，其他关于部门信息的条件都将被置空
        /// 同时以此条件匹配数据中的PdepartmentID，这样就需要保证顶级部门的上级部门代码也需要与
        /// 本部门部门代码一致，否则将查不到本部门下的数据
        /// </summary>
        public bool? IncludeSubDeptartment
        {
            get => _includeSubDeptartment;
            set => _includeSubDeptartment = value;
        }
    }
}