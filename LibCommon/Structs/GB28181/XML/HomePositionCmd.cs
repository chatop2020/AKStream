using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 看守位控制命令
    /// </summary>
    [XmlRoot("Control")]
    public class HomePositionCmd : XmlHelper<HomePositionCmd>
    {
        private static HomePositionCmd _instance;

        /// <summary>
        /// 单例模式访问
        /// </summary>
        public static HomePositionCmd Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HomePositionCmd();
                }

                return _instance;
            }
        }

        /// <summary>
        /// 命令类型
        /// </summary>
        [XmlElement("CmdType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CommandType CommandType { get; set; }

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
        /// 看守位
        /// </summary>
        [XmlElement("HomePosition")]
        public HomePositionSet HomePosition { get; set; }
    }

    public class HomePositionSet
    {
        /// <summary>
        /// 看守位 1：开启 0：关闭
        /// </summary>
        [XmlElement("Enabled")]
        public int Enabled { get; set; }

        [XmlElement("ResetTime")] public int ResetTime { get; set; }

        [XmlElement("PresetIndex")] public int PresetIndex { get; set; }
    }
}