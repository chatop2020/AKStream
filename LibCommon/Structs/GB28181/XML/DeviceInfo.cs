using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using LibCommon.Structs.GB28181.Sys;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    [Serializable]
    /// <summary>
    /// 设备信息查询响应结果
    /// </summary>
    [XmlRoot("Response")]
    public class DeviceInfo : XmlHelper<DeviceInfo>
    {
        private static DeviceInfo instance;

        private string _devName;

        /// <summary>
        /// 以单例模式访问
        /// </summary>
        public static DeviceInfo Instance
        {
            get
            {
                if (instance == null)
                    instance = new DeviceInfo();
                return instance;
            }
        }

        /// <summary>
        /// 命令类型：设备信息查询(必选)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("CmdType")]
        public CommandType CmdType { get; set; }

        /// <summary>
        /// 命令序列号(必选)
        /// </summary>
        [XmlElement("SN")]
        public int SN { get; set; }

        /// <summary>
        /// 目标设备/区域/系统的编码(必选)
        /// </summary>
        [XmlElement("DeviceID")]
        public string DeviceID { get; set; }

        /// <summary>
        /// 目标设备/区域/系统的名称(可选)
        /// </summary>
        [XmlElement("DeviceName")]
        public string DeviceName
        {
            get { return _devName; }
            set { _devName = value == null ? "" : value.Replace(); }
        }

        /// <summary>
        /// 查询结果(必选)
        /// </summary>
        [XmlElement("Result")]
        public string Result { get; set; }


        /// <summary>
        /// 设备生产商(可选)
        /// </summary>
        [XmlElement("Manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        [XmlElement("Model")]
        public string Model { get; set; }

        /// <summary>
        /// 设备固件版本(可选)
        /// </summary>
        [XmlElement("Firmware")]
        public string Firmware { get; set; }

        /// <summary>
        /// 视频输入通道数(可选)
        /// </summary>
        [XmlElement("Channel")]
        public int Channel { get; set; }

        #region 方法

        public void Save()
        {
            base.Save(this);
        }

        public void Read()
        {
            instance = this.Read(this.GetType());
        }

        #endregion
    }
}