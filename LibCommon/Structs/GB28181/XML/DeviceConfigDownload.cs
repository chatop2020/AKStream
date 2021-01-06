using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 设备配置信息查询应答
    /// </summary>
    [XmlRoot("Response")]
    public class DeviceConfigDownload : XmlHelper<DeviceConfigDownload>
    {
        private static DeviceConfigDownload _instance;

        /// <summary>
        /// 单例模式
        /// </summary>
        public static DeviceConfigDownload Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DeviceConfigDownload();
                }

                return _instance;
            }
        }

        /// <summary>
        /// 命令类型：设备配置获取(必选)
        /// </summary>
        [XmlElement("CmdType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CommandType CmdType { get; set; }

        /// <summary>
        /// 命令序列号(必选)
        /// </summary>
        [XmlElement("SN")]
        public int SN { get; set; }

        /// <summary>
        /// 设备/区域编码(必选)
        /// </summary>
        [XmlElement("DeviceID")]
        public string DeviceID { get; set; }

        /// <summary>
        /// 查询结果标志(必选)
        /// </summary>
        [XmlElement("Result")]
        public string Result { get; set; }

        /// <summary>
        /// 基本配置参数
        /// </summary>
        [XmlElement("BasicParam")]
        public BasicParamAttr BasicParam { get; set; }

        /// <summary>
        /// 基本配置参数
        /// </summary>
        public class BasicParamAttr
        {
            /// <summary>
            /// 设备名称(必选)
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 注册过期时间(必选)
            /// </summary>
            public string Expiration { get; set; }

            /// <summary>
            /// 心跳间隔时间(必选)
            /// </summary>
            public int HeartBeatInterval { get; set; }

            /// <summary>
            /// 心跳超时次数(必选)
            /// </summary>
            public int HeartBeatCount { get; set; }

            /// <summary>
            /// 定位功能支持情况(可选，默认值取0)
            /// 0，不支持
            /// 1，支持GPS定位
            /// 2，支持北斗定位
            /// </summary>
            public int PositionCapability { get; set; }

            /// <summary>
            /// 经度(可选)
            /// </summary>
            public double Longitude { get; set; }

            /// <summary>
            /// 纬度(可选)
            /// </summary>
            public double Latitude { get; set; }
        }
    }

    /// <summary>
    /// 设备配置
    /// </summary>
    [XmlRoot("Control")]
    public class DeviceConfig : XmlHelper<DeviceConfig>
    {
        private static DeviceConfig _instance;

        /// <summary>
        /// 单例模式访问
        /// </summary>
        public static DeviceConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DeviceConfig();
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
        /// 设备配置参数类型
        /// </summary>
        [XmlElement("ConfigType")]
        public string ConfigType { get; set; }

        [XmlElement("BasicParam")] public DeviceParam BasicParam { get; set; }
    }

    /// <summary>
    /// 设备配置参数(基本)
    /// </summary>
    public class DeviceParam
    {
        /// <summary>
        ///
        /// </summary>
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Expiration")] public int Expiration { get; set; }

        [XmlElement("HeartBeatInterval")] public int HeartBeatInterval { get; set; }

        [XmlElement("HeartBeatCount")] public int HeartBeatCount { get; set; }
    }
}