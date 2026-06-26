using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 报警通知
    /// </summary>
    [XmlRoot("Notify")]
    public class Alarm : XmlHelper<Alarm>
    {
        private static Alarm _instance;

        /// <summary>
        /// 单例模式访问
        /// </summary>
        public static Alarm Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Alarm();
                }

                return _instance;
            }
        }

        /// <summary>
        /// 命令类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
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
        /// 报警级别
        /// </summary>
        [XmlElement("AlarmPriority")]
        public string AlarmPriority { get; set; }

        /// <summary>
        /// 报警方式
        /// </summary>
        [XmlElement("AlarmMethod")]
        public string AlarmMethod { get; set; }

        /// <summary>
        /// 报警时间
        /// </summary>
        [XmlElement("AlarmTime")]
        public string AlarmTime { get; set; }

        /// <summary>
        /// 报警内容描述
        /// </summary>
        [XmlElement("AlarmDescription")]
        public string AlarmDescription { get; set; }

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

        [XmlElement("Info")] public Info InfoEx { get; set; }

        public class Info
        {
            [XmlElement("AlarmType")] public string AlarmType { get; set; }
        }
    }

    /// <summary>
    /// 报警通知
    /// </summary>
    [XmlRoot("Response")]
    public class AlarmInfo : XmlHelper<AlarmInfo>
    {
        private static AlarmInfo _instance;

        /// <summary>
        /// 以单例模式访问
        /// </summary>
        public static AlarmInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AlarmInfo();
                }

                return _instance;
            }
        }

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
        /// 执行结果
        /// </summary>
        [XmlElement("Result")]
        public string Result { get; set; }
    }
}