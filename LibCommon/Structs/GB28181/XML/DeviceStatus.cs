using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    [Serializable]
    public class Alarmstatus
    {
        private List<AlarmStatuItem> _statuItems = new List<AlarmStatuItem>();


        /// <summary>
        /// 设备项
        /// </summary>
        [XmlElement("Item")]
        public List<AlarmStatuItem> Items
        {
            get { return _statuItems; }
        }
    }


    public class AlarmStatuItem : XmlHelper<DeviceStatus>
    {
        private string _deviceId;
        private string _dutyStatus;

        [XmlElement("DeviceID")]
        public string DeviceId
        {
            get => _deviceId;
            set => _deviceId = value ?? throw new ArgumentNullException(nameof(value));
        }

        [XmlElement("DutyStatus")]
        public string DutyStatus
        {
            get => _dutyStatus;
            set => _dutyStatus = value ?? throw new ArgumentNullException(nameof(value));
        }
    }


    /// <summary>
    /// 设备状态
    /// </summary>
    [XmlRoot("Response")]
    public class DeviceStatus : XmlHelper<DeviceStatus>
    {
        private static DeviceStatus _instance;

        /// <summary>
        /// 以单例模式访问
        /// </summary>
        public static DeviceStatus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DeviceStatus();
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
        /// 查询结果标志
        /// </summary>
        [XmlElement("Result")]
        public string Result { get; set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        [XmlElement("Online")]
        public string Online { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [XmlElement("Status")]
        public string Status { get; set; }

        /// <summary>
        /// 不正常工作原因
        /// </summary>
        [XmlElement("Reason")]
        public string Reason { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [XmlElement("Encode")]
        public string Encode { get; set; }

        /// <summary>
        /// 录制的
        /// </summary>
        [XmlElement("Record")]
        public string Record { get; set; }

        /// <summary>
        /// 设备时间
        /// </summary>
        [XmlElement("DeviceTime")]
        public DateTime DeviceTime { get; set; }

        /// <summary>
        /// 报警状态
        /// </summary>
        [XmlElement("Alarmstatus")]
        public Alarmstatus Alarmstatus { get; set; }
    }
}