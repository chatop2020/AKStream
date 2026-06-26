using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 移动设备位置数据查询
    /// </summary>
    [XmlRoot("Query")]
    public class MobilePositionQuery : XmlHelper<MobilePositionQuery>
    {
        private static MobilePositionQuery _instance;

        /// <summary>
        /// 以单例模式访问
        /// </summary>
        public static MobilePositionQuery Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MobilePositionQuery();
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
        /// 移动设备位置信息上报时间间隔
        /// </summary>
        [XmlElement("Interval")]
        public int Interval { get; set; }
    }

    /// <summary>
    /// 移动设备位置信息通知
    /// </summary>
    [XmlRoot("Notify")]
    public class MobilePositionNotify : XmlHelper<MobilePositionNotify>
    {
        private static MobilePositionNotify _instance;

        /// <summary>
        /// 单例模式访问
        /// </summary>
        public static MobilePositionNotify Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MobilePositionNotify();
                }

                return _instance;
            }
        }

        /// <summary>
        /// 命令类型
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
        /// 产生通知时间
        /// </summary>
        [XmlElement("Time ")]
        public int Time { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [XmlElement("Longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [XmlElement("Latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// 速度km/h
        /// </summary>
        [XmlElement("Speed")]
        public double Speed { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        [XmlElement("Direction")]
        public double Direction { get; set; }

        /// <summary>
        /// 海拔高度
        /// </summary>
        [XmlElement("Altitude")]
        public double Altitude { get; set; }
    }
}