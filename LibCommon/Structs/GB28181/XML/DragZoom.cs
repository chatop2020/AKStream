using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    [XmlRoot("Control")]
    public class DragZoom : XmlHelper<Control>
    {
        private static DragZoom _instance;

        /// <summary>
        /// 单例模式访问
        /// </summary>
        public static DragZoom Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DragZoom();
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

        [XmlElement("DragZoomIn")] public DragZoomSet DragZoomIn { get; set; }

        [XmlElement("DragZoomOut")] public DragZoomSet DragZoomOut { get; set; }
    }

    public class DragZoomSet
    {
        /// <summary>
        ///
        /// </summary>
        [XmlElement("Length")]
        public int Length { get; set; }

        [XmlElement("Width")] public int Width { get; set; }

        [XmlElement("MidPointX")] public int MidPointX { get; set; }

        [XmlElement("MidPointY")] public int MidPointY { get; set; }

        [XmlElement("LengthX")] public int LengthX { get; set; }

        [XmlElement("LengthY")] public int LengthY { get; set; }
    }
}