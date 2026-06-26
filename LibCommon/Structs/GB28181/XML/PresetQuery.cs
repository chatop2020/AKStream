using System.Collections.Generic;
using System.Xml.Serialization;
using LibCommon.Structs.GB28181.Sys;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 设备预置位查询
    /// </summary>
    [XmlRoot("Query")]
    public class PresetQuery : XmlHelper<PresetQuery>
    {
        private static PresetQuery _instance;

        public static PresetQuery Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PresetQuery();
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
    }

    /// <summary>
    /// 设备预置位查询结果
    /// </summary>
    [XmlRoot("Response")]
    public class PresetInfo : XmlHelper<PresetInfo>
    {
        private static PresetInfo _instance;

        /// <summary>
        /// 以单例模式访问
        /// </summary>
        public static PresetInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PresetInfo();
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
        /// 设备预置位列表，用于平台间或平台与设备间的预置位查询
        /// </summary>
        [XmlElement("PresetList")]
        public List<PresetValue> PresetList { get; set; }

        public class PresetValue
        {
            private List<Item> _presetItem = new List<Item>();

            /// <summary>
            /// 列表项个数，当未配置预置位时取值为0
            /// </summary>
            [XmlAttribute("Num")]
            public int Num { get; set; }

            /// <summary>
            /// 当前配置的预置位记录，当未配置预置位时不填写
            /// </summary>
            [XmlElement("Item")]
            public List<Item> Items
            {
                get { return _presetItem; }
            }
        }

        /// <summary>
        /// 预置位信息
        /// </summary>
        public class Item
        {
            private string _presetName;

            /// <summary>
            /// 预置位编码
            /// </summary>
            [XmlElement("PresetID")]
            public string PresetID { get; set; }

            /// <summary>
            /// 预置位名称
            /// </summary>
            [XmlElement("PresetName")]
            public string PresetName
            {
                get { return _presetName; }
                set { _presetName = value == null ? "" : value.Replace(); }
            }
        }
    }
}