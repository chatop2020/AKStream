using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 录像信息
    /// </summary>
    [XmlRoot("Notify")]
    public class MediaStatus : XmlHelper<MediaStatus>
    {
        private static MediaStatus _instance;

        /// <summary>
        /// 以单例模式访问
        /// </summary>
        public static MediaStatus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MediaStatus();
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
        /// 通知事件类型（取值121表示历史媒体文件发送结束）
        /// </summary>
        [XmlElement("NotifyType")]
        public string NotifyType { get; set; }
    }
}