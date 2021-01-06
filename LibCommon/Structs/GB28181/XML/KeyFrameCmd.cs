using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 强制关键帧命令
    /// </summary>
    [XmlRoot("Control")]
    public class KeyFrameCmd : XmlHelper<KeyFrameCmd>
    {
        private static KeyFrameCmd _instance;

        /// <summary>
        /// 单例模式访问
        /// </summary>
        public static KeyFrameCmd Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new KeyFrameCmd();
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
        /// 强制关键帧
        /// </summary>
        [XmlElement("IFrameCmd")]
        public string IFrameCmd { get; set; }


        public string IFameCmd { get; set; }
    }
}