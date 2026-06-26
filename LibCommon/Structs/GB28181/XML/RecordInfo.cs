using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using LibCommon.Enums;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SIPSorcery.SIP;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 录像文件查询
    /// </summary>
    [XmlRoot("Query")]
    public class RecordQuery : XmlHelper<RecordQuery>
    {
        private static readonly RecordQuery _instance = new RecordQuery();

        /// <summary>
        /// 以单例模式访问
        /// </summary>
        public static RecordQuery Instance => _instance;

        /// <summary>
        /// 指令类型
        /// </summary>
        [XmlElement("CmdType")]
        public CommandType CmdType { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [XmlElement("SN")]
        public int SN { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        [XmlElement("DeviceID")]
        public string DeviceID { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [XmlElement("StartTime")]
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [XmlElement("EndTime")]
        public string EndTime { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [XmlElement("FilePath")]
        public string FilePath { get; set; }

        /// <summary>
        /// 录像地址
        /// </summary>
        [XmlElement("Address")]
        public string Address { get; set; }

        /// <summary>
        /// 保密属性 0不涉密 1涉密
        /// </summary>
        [XmlElement("Secrecy")]
        public int Secrecy { get; set; }

        /// <summary>
        /// 录像产生类型 time alarm manual all
        /// </summary>
        [XmlElement("Type")]
        public string Type { get; set; }

        /// <summary>
        /// 录像产生者ID
        /// </summary>
        [XmlElement("RecorderID")]
        public string RecorderID { get; set; }
    }

    /// <summary>
    /// 录像信息
    /// </summary>
    [XmlRoot("Response")]
    public class RecordInfo : XmlHelper<RecordInfo>
    {
        private static RecordInfo _instance;

        /// <summary>
        /// 以单例模式访问
        /// </summary>
        public static RecordInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RecordInfo();
                }

                return _instance;
            }
        }

        /// <summary>
        /// 指令类型
        /// </summary>
        [XmlElement("CmdType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CommandType CmdType { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [XmlElement("SN")]
        public int SN { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        [XmlElement("DeviceID")]
        public string DeviceID { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [XmlElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 录像总条数
        /// </summary>
        [XmlElement("SumNum")]
        public int SumNum { get; set; }

        /// <summary>
        /// 录像列表
        /// </summary>
        [XmlElement("RecordList")]
        public RecordList RecordItems { get; set; }

        /// <summary>
        /// 录像列表信息
        /// </summary>
        public class RecordList
        {
            private List<RecItem> _recordItems = new List<RecItem>();

            /// <summary>
            /// 设备项
            /// </summary>
            [XmlElement("Item")]
            public List<RecItem> Items
            {
                get { return _recordItems; }
            }
        }

        [Serializable]
        public class RecItem
        {
            private string _app;
            private string _callId;
            private int _cSeq;
            private string _fromTag;
            private SIPRequest? _inviteSipRequest = null;
            private SIPResponse? _inviteSipResponse = null;
            private MediaServerStreamInfo? _mediaServerStreamInfo;
            private PushStatus _pushStatus;
            private SipChannel? _sipChannel = null;
            private SipDevice? _sipDevice = null;
            private string _ssrcId;
            private string _stream;
            private string _toTag;
            private string _vhost;

            [XmlIgnore]
            /// <summary>
            /// 额外真加SSRCId
            /// 总共9位，1-5位为channel.deivce的2-6位，6-9位为此设备的序列号
            /// 不足9位要前补0
            /// 真正的ssrc是10位，第一位为1时是回放流，第一位是0时的实时流
            /// 流形式位0/1+后9位就是这个流的ssrc
            /// </summary>
            public string SsrcId
            {
                get => _ssrcId;
                set => _ssrcId = value ?? throw new ArgumentNullException(nameof(value));
            }

            [XmlIgnore]
            /// <summary>
            /// 额外真加Stream
            /// 它是SSRCID的16进制表示方法
            /// </summary>
            public string Stream
            {
                get => _stream;
                set => _stream = value ?? throw new ArgumentNullException(nameof(value));
            }

            [XmlIgnore]
            public string App
            {
                get => _app;
                set => _app = value ?? throw new ArgumentNullException(nameof(value));
            }

            [XmlIgnore]
            public string Vhost
            {
                get => _vhost;
                set => _vhost = value ?? throw new ArgumentNullException(nameof(value));
            }

            /// <summary>
            /// 推流时的request
            /// 要把请求实时视频时的req和res存起来，因为在结束时要用到这两个内容
            /// </summary>

            [JsonIgnore]
            [XmlIgnore]
            [BsonIgnore]
            public SIPRequest? InviteSipRequest
            {
                get => _inviteSipRequest;
                set => _inviteSipRequest = value;
            }

            [JsonIgnore]
            [XmlIgnore]
            [BsonIgnore]
            /// <summary>
            /// 推流时的response
            /// 要把请求实时视频时的req和res存起来，因为在结束时要用到这两个内容
            /// </summary>
            public SIPResponse? InviteSipResponse
            {
                get => _inviteSipResponse;
                set => _inviteSipResponse = value;
            }

            [JsonIgnore]
            [XmlIgnore]

            /// <summary>
            /// 录像文件所在Sip设备
            /// </summary>
            public SipDevice? SipDevice
            {
                get => _sipDevice;
                set => _sipDevice = value;
            }

            [JsonIgnore]
            [XmlIgnore]

            /// <summary>
            /// 录像文件所在Sip通道
            /// </summary>

            public SipChannel? SipChannel
            {
                get => _sipChannel;
                set => _sipChannel = value;
            }


            public string FromTag
            {
                get => _fromTag;
                set => _fromTag = value ?? throw new ArgumentNullException(nameof(value));
            }

            public string ToTag
            {
                get => _toTag;
                set => _toTag = value ?? throw new ArgumentNullException(nameof(value));
            }

            public string CallId
            {
                get => _callId;
                set => _callId = value ?? throw new ArgumentNullException(nameof(value));
            }

            public int CSeq
            {
                get => _cSeq;
                set => _cSeq = value;
            }

            /// <summary>
            /// 推流状态
            /// </summary>
            [XmlIgnore]
            public MediaServerStreamInfo? MediaServerStreamInfo
            {
                get => _mediaServerStreamInfo;
                set => _mediaServerStreamInfo = value;
            }


            /// <summary>
            /// 推流状态
            /// </summary>
            [XmlIgnore]
            [JsonConverter(typeof(StringEnumConverter))]
            public PushStatus PushStatus
            {
                get => _pushStatus;
                set => _pushStatus = value;
            }

            /// <summary>
            /// 设备编码
            /// </summary>
            [XmlElement("DeviceID")]
            public string DeviceID { get; set; }

            /// <summary>
            /// 录像名称
            /// </summary>
            [XmlElement("Name")]
            public string Name { get; set; }

            /// <summary>
            /// 文件路径
            /// </summary>
            [XmlElement("FilePath")]
            public string FilePath { get; set; }

            /// <summary>
            /// 录像地址
            /// </summary>
            [XmlElement("Address")]
            public string Address { get; set; }

            /// <summary>
            /// 开始时间
            /// </summary>
            [XmlElement("StartTime")]
            public string StartTime { get; set; }

            /// <summary>
            /// 结束时间
            /// </summary>
            [XmlElement("EndTime")]
            public string EndTime { get; set; }

            /// <summary>
            /// 保密属性 0不涉密 1涉密
            /// </summary>
            [XmlElement("Secrecy")]
            public int Secrecy { get; set; }

            /// <summary>
            /// 录像产生类型 time alarm manual all
            /// </summary>
            [XmlElement("Type")]
            public string Type { get; set; }

            /// <summary>
            /// 录像产生者ID
            /// </summary>
            [XmlElement("RecorderID")]
            public string RecorderID { get; set; }
        }
    }
}