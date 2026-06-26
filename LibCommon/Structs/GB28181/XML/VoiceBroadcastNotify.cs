using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 语音广播通知
    /// <see cref="GB/T 28181-2016 附录A.2.5通知命令(d,e节点)"/>
    /// </summary>
    [XmlRoot("Notify")]
    public class VoiceBroadcastNotify : XmlHelper<VoiceBroadcastNotify>
    {
        private static VoiceBroadcastNotify _instance;

        /// <summary>
        /// 单例模式
        /// </summary>
        public static VoiceBroadcastNotify Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VoiceBroadcastNotify();
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
        /// 命令序列号
        /// </summary>
        [XmlElement("SN")]
        public int SN { get; set; }

        /// <summary>
        /// 语音输入设备的设备编码
        /// </summary>
        [XmlElement("SourceID")]
        public string SourceID { get; set; }

        /// <summary>
        /// 语音输出设备的设备编码
        /// </summary>
        [XmlElement("TargetID")]
        public string TargetID { get; set; }
    }
}