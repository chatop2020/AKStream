using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using LibCommon.Structs.GB28181.Sys;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181.XML
{
    [Serializable]
    /// <summary>
    /// 设备目录查询结果信息
    /// </summary>
    [XmlRoot("Response")]
    public class Catalog : XmlHelper<Catalog>
    {
        private static Catalog _instance;

        /// <summary>
        /// 单例模式访问
        /// </summary>
        public static Catalog Instance => _instance ?? (_instance = new Catalog());


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
        /// 设备总条数
        /// </summary>
        [XmlElement("SumNum")]
        public int SumNum { get; set; }

        /// <summary>
        /// 列表显示条数
        /// </summary>
        [XmlElement("DeviceList")]
        public DevList DeviceList { get; set; }

        [Serializable]
        /// <summary>
        /// 设备列表
        /// </summary>
        public class DevList
        {
            private List<Item> _devItem = new List<Item>();

            /// <summary>
            /// 设备项
            /// </summary>
            [XmlElement("Item")]
            public List<Item> Items
            {
                get { return _devItem; }
            }
        }

        [Serializable]
        /// <summary>
        /// 设备信息
        /// </summary>
        public class Item
        {
            private string _address;


            private string _manufacturer;

            private string _model;

            private string _name;

            private string _owner;

            /// <summary>
            /// 设备/区域/系统编码(必选)
            /// </summary>
            [JsonProperty("ChannelID")]
            [XmlElement("DeviceID")]
            public string DeviceID { get; set; }

            /// <summary>
            /// 设备/区域/系统名称(必选)
            /// </summary>
            [XmlElement("Name")]
            public string Name
            {
                get { return _name; }
                set { _name = value == null ? "" : value.Replace(); }
            }

            /// <summary>
            /// 当为设备时，设备厂商(必选)
            /// </summary>
            [XmlElement("Manufacturer")]
            public string Manufacturer
            {
                get { return _manufacturer; }
                set { _manufacturer = value == null ? "" : value.Replace(); }
            }

            /// <summary>
            /// 当为设备时，设备型号(必选)
            /// </summary>
            [XmlElement("Model")]
            public string Model
            {
                get { return _model; }
                set { _model = value == null ? "" : value.Replace(); }
            }

            /// <summary>
            /// 当为设备时，设备归属(必选)
            /// </summary>
            [XmlElement("Owner")]
            public string Owner
            {
                get { return _owner; }
                set { _owner = value == null ? "" : value.Replace(); }
            }

            /// <summary>
            /// 行政区域(必选)
            /// </summary>
            [XmlElement("CivilCode")]
            public string CivilCode { get; set; }

            /// <summary>
            /// 警区(可选)
            /// </summary>
            [XmlElement("Block")]
            public string Block { get; set; }

            /// <summary>
            /// 当为设备时，安装地址(必选)
            /// </summary>
            [XmlElement("Address")]
            public string Address
            {
                get { return _address; }
                set { _address = value == null ? "" : value.Replace(); }
            }

            /// <summary>
            /// 当为设备时，是否有子设备(必选)，
            /// 1有
            /// 0没有
            /// </summary>
            [XmlIgnore]
            public int? Parental { get; set; }

            [XmlElement("Parental")]
            public string ParentalValue
            {
                get => Parental.HasValue ? Parental.Value.ToString() : null;
                set => Parental = int.TryParse(value, out int result) ? result : (int?) null;
            }

            /// <summary>
            /// 父设备/区域/系统ID(必选)
            /// </summary>
            [XmlElement("ParentID")]
            public string ParentID { get; set; }

            /// <summary>
            /// 虚拟分组ID
            /// </summary>
            [XmlElement("BusinessGroupID")]
            public string BusinessGroupID { get; set; }

            /// <summary>
            /// 信令安全模式(可选)缺省为0； 
            /// 0：不采用
            /// 2：S/MIME签名方式 
            /// 3：S/MIME加密签名同时采用方式 
            /// 4：数字摘要方式
            /// </summary>
            [XmlIgnore]
            public int? SafetyWay { get; set; }

            [XmlElement("SafetyWay")]
            public string SafetyWayValue
            {
                get => SafetyWay.HasValue ? SafetyWay.Value.ToString() : null;
                set => SafetyWay = int.TryParse(value, out int result) ? result : (int?) null;
            }

            /// <summary>
            /// 注册方式(必选)缺省为1；
            /// 1:符合IETF FRC 3261标准的认证注册模式；
            /// 2:基于口令的双向认证注册模式；
            /// 3:基于数字证书的双向认证注册模式；
            /// </summary>
            [XmlIgnore]
            public int? RegisterWay { get; set; }

            [XmlElement("RegisterWay")]
            public string RegisterWayValue
            {
                get => RegisterWay.HasValue ? RegisterWay.Value.ToString() : null;
                set => RegisterWay = int.TryParse(value, out int result) ? result : (int?) null;
            }

            /// <summary>
            /// 证书序列号（有证书的设备必选）
            /// </summary>
            [XmlElement("CertNum")]
            public string CertNum { get; set; }

            /// <summary>
            /// 证书有效标志(有证书的设备必选)，
            /// 0无效
            /// 1有效
            /// </summary>
            [XmlIgnore]
            public int? Certifiable { get; set; }

            [XmlElement("Certifiable")]
            public string CertifiableValue
            {
                get { return Certifiable.HasValue ? Certifiable.Value.ToString() : null; }
                set { Certifiable = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 证书无效原因码(可选)
            /// </summary>
            [XmlIgnore]
            public int? ErrCode { get; set; }

            [XmlElement("ErrCode")]
            public string ErrCodeValue
            {
                get { return ErrCode.HasValue ? ErrCode.Value.ToString() : null; }
                set { ErrCode = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 证书终止有效期(可选)
            /// </summary>
            [XmlElement("EndTime")]
            public string EndTime { get; set; }

            /// <summary>
            /// 保密属性(必选)
            /// 0：不涉密
            /// 1涉密
            /// </summary>
            [XmlIgnore]
            public int? Secrecy { get; set; }

            [XmlElement("Secrecy")]
            public string SecrecyValue
            {
                get { return Secrecy.HasValue ? Secrecy.Value.ToString() : null; }
                set { Secrecy = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 设备/区域/系统IP地址（可选）
            /// </summary>
            [XmlElement("IPAddress")]
            public string IPAddress { get; set; }

            /// <summary>
            /// 设备/区域/系统端口(可选)
            /// </summary>
            [XmlIgnore]
            public ushort? Port { get; set; }

            [XmlElement("Port")]
            public string PortValue
            {
                get { return Port.HasValue ? Port.Value.ToString() : null; }
                set { Port = ushort.TryParse(value, out ushort result) ? result : (ushort?) null; }
            }

            /// <summary>
            /// 设备口令（可选）
            /// </summary>
            [XmlElement("Password")]
            public string Password { get; set; }

            /// <summary>
            /// 设备状态(必选)
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            [XmlElement("Status")]
            public DevStatus Status { get; set; }

            /// <summary>
            /// 经度(可选)
            /// </summary>
            [XmlIgnore]
            public double? Longitude { get; set; }

            [XmlElement("Longitude")]
            public string LongitudeValue
            {
                get { return Longitude.HasValue ? Longitude.Value.ToString() : null; }
                set { Longitude = double.TryParse(value, out double result) ? result : (double?) null; }
            }

            /// <summary>
            /// 纬度(可选)
            /// </summary>
            [XmlIgnore]
            public double? Latitude { get; set; }

            [XmlElement("Latitude")]
            public string LatitudeValue
            {
                get { return Latitude.HasValue ? Latitude.Value.ToString() : null; }
                set { Latitude = double.TryParse(value, out double result) ? result : (double?) null; }
            }

            /// <summary>
            /// 信息项
            /// </summary>
            [XmlElement("Info")]
            public Info InfList { get; set; }

            /// <summary>
            /// 远程设备终结点
            /// </summary>
            public string RemoteEP { get; set; }
        }

        [Serializable]
        /// <summary>
        /// 扩展信息
        /// </summary>
        public class Info
        {
            /// <summary>
            /// 摄像机类型扩展，标识摄像机类型
            /// 1，球机
            /// 2，半球
            /// 3，固定枪机
            /// 4，遥控枪机
            /// 当目录项为摄像机时可选
            /// </summary>
            [XmlIgnore]
            public int? PTZType { get; set; }

            [XmlElement("PTZType")]
            public string PTZTypeValue
            {
                get { return PTZType.HasValue ? PTZType.Value.ToString() : null; }
                set { PTZType = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 摄像机位置类型扩展
            /// 1，省际检查站
            /// 2，党政机关
            /// 3，车站码头
            /// 4，中心广场
            /// 5，体育场馆
            /// 6，商业中心
            /// 7，宗教场所
            /// 8，校园周边
            /// 9，治安复杂区域
            /// 10，交通干线
            /// 当目录项为摄像机时可选
            /// </summary>
            [XmlIgnore]
            public int? PositionType { get; set; }

            [XmlElement("PositionType")]
            public string PositionTypeValue
            {
                get { return PositionType.HasValue ? PositionType.Value.ToString() : null; }
                set { PositionType = int.TryParse(value, out int result) ? result : (int?) null; }
            }


            /// <summary>
            /// 摄像机按照位置室外、室内属性
            /// 1，室外
            /// 2，室内
            /// 当目录项为摄像机时可选，缺省为1
            /// </summary>
            [XmlIgnore]
            public int? RoomType { get; set; }

            [XmlElement("RoomType")]
            public string RoomTypeValue
            {
                get { return RoomType.HasValue ? RoomType.Value.ToString() : null; }
                set { RoomType = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 摄像机用途属性
            /// 1，治安
            /// 2，交通
            /// 3，重点
            /// 当目录项为摄像机时可选
            /// </summary>
            [XmlIgnore]
            public int? UseType { get; set; }

            [XmlElement("UseType")]
            public string UseTypeValue
            {
                get { return UseType.HasValue ? UseType.Value.ToString() : null; }
                set { UseType = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 摄像机补光属性
            /// 1，无补光
            /// 2，红外补光
            /// 3，白光补光
            /// 当目录项为摄像机时可选，缺省为1
            /// </summary>
            [XmlIgnore]
            public int? SupplyLightType { get; set; }

            [XmlElement("SupplyLightType")]
            public string SupplyLightTypeValue
            {
                get { return SupplyLightType.HasValue ? SupplyLightType.Value.ToString() : null; }
                set { SupplyLightType = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 摄像机监视方位属性
            /// 1，东
            /// 2，西
            /// 3，南
            /// 4，北
            /// 5，东南
            /// 6，东北
            /// 7，西南
            /// 8，西北
            /// 当目录项为摄像机时且为固定摄像机或设置看守位摄像机时可选
            /// </summary>
            [XmlIgnore]
            public int? DirectionType { get; set; }

            [XmlElement("DirectionType")]
            public string DirectionTypeValue
            {
                get { return DirectionType.HasValue ? DirectionType.Value.ToString() : null; }
                set { DirectionType = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 摄像机支持的分辨率，可有多个分辨率值，各个取值间以"/"分隔。
            /// 分辨率取值参见附录F中SDP f字段规定。
            /// 当目录项为摄像机时可选
            /// </summary>
            [XmlElement("Resolution")]
            public string Resolution { get; set; }

            /// <summary>
            /// 虚拟组织所属的业务分组ID，
            /// 业务分组根据特定的业务需求制定，
            /// 一个业务分组包含一组特定的虚拟组织。
            /// </summary>
            [XmlElement("BusinessGroupID")]
            public string BusinessGroupID { get; set; }

            /// <summary>
            /// 下载倍速范围(可选)，各可选参数以"/"分隔
            /// 如设备支持1,2,4倍下载则应写为"1/2/4"
            /// </summary>
            [XmlElement("DownloadSpeed")]
            public string DownloadSpeed { get; set; }

            /// <summary>
            /// 空域编码能力
            /// 0，不支持
            /// 1，1级增强
            /// 2，2级增强
            /// 3，3级增强
            /// (可选)
            /// </summary>
            [XmlIgnore]
            public int? SVCSpaceSupportMode { get; set; }

            [XmlElement("SVCSpaceSupportMode")]
            public string SVCSpaceSupportModeValue
            {
                get { return SVCSpaceSupportMode.HasValue ? SVCSpaceSupportMode.Value.ToString() : null; }
                set { SVCSpaceSupportMode = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 时域编码能力
            /// 0，不支持
            /// 1，1级增强
            /// 2，2级增强
            /// 3，3级增强
            /// </summary>
            [XmlIgnore]
            public int? SVCTimeSupportMode { get; set; }

            [XmlElement("SVCTimeSupportMode")]
            public string SVCTimeSupportModeValue
            {
                get { return SVCTimeSupportMode.HasValue ? SVCTimeSupportMode.Value.ToString() : null; }
                set { SVCTimeSupportMode = int.TryParse(value, out int result) ? result : (int?) null; }
            }
        }
    }

    /// <summary>
    /// 设备目录订阅通知结果信息
    /// </summary>
    [XmlRoot("Notify")]
    public class NotifyCatalog : XmlHelper<NotifyCatalog>
    {
        private static NotifyCatalog _instance;

        /// <summary>
        /// 单例模式访问
        /// </summary>
        public static NotifyCatalog Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NotifyCatalog();
                return _instance;
            }
        }

        /// <summary>
        /// 命令类型
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
        /// 设备总条数
        /// </summary>
        [XmlElement("SumNum")]
        public int SumNum { get; set; }

        /// <summary>
        /// 列表显示条数
        /// </summary>
        [XmlElement("DeviceList")]
        public DevList DeviceList { get; set; }

        /// <summary>
        /// 设备列表
        /// </summary>
        public class DevList
        {
            private List<Item> _devItem = new List<Item>();

            /// <summary>
            /// 设备项
            /// </summary>
            [XmlElement("Item")]
            public List<Item> Items
            {
                get { return _devItem; }
            }
        }

        /// <summary>
        /// 设备信息
        /// </summary>
        public class Item
        {
            /// <summary>
            /// 设备/区域/系统编码(必选)
            /// </summary>
            [XmlElement("DeviceID")]
            public string DeviceID { get; set; }

            /// <summary>
            /// 事件类型
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            [XmlElement("Event")]
            public EventType Event { get; set; }

            /// <summary>
            /// 设备/区域/系统名称(必选)
            /// </summary>
            [XmlElement("Name")]
            public string Name { get; set; }

            /// <summary>
            /// 当为设备时，设备厂商(必选)
            /// </summary>
            [XmlElement("Manufacturer")]
            public string Manufacturer { get; set; }

            /// <summary>
            /// 当为设备时，设备型号(必选)
            /// </summary>
            [XmlElement("Model")]
            public string Model { get; set; }

            /// <summary>
            /// 当为设备时，设备归属(必选)
            /// </summary>
            [XmlElement("Owner")]
            public string Owner { get; set; }

            /// <summary>
            /// 行政区域(必选)
            /// </summary>
            [XmlElement("CivilCode")]
            public string CivilCode { get; set; }

            /// <summary>
            /// 警区(可选)
            /// </summary>
            [XmlElement("Block")]
            public string Block { get; set; }

            /// <summary>
            /// 当为设备时，安装地址(必选)
            /// </summary>
            [XmlElement("Address")]
            public string Address { get; set; }

            /// <summary>
            /// 当为设备时，是否有子设备(必选)，
            /// 1有
            /// 0没有
            /// </summary>
            [XmlIgnore]
            public int? Parental { get; set; }

            [XmlElement("Parental")]
            public string ParentalValue
            {
                get { return Parental.HasValue ? Parental.Value.ToString() : null; }
                set { Parental = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 父设备/区域/系统ID(必选)
            /// </summary>
            [XmlElement("ParentID")]
            public string ParentID { get; set; }

            /// <summary>
            /// 虚拟分组ID
            /// </summary>
            [XmlElement("BusinessGroupID")]
            public string BusinessGroupID { get; set; }

            /// <summary>
            /// 信令安全模式(可选)缺省为0； 
            /// 0：不采用
            /// 2：S/MIME签名方式 
            /// 3：S/MIME加密签名同时采用方式 
            /// 4：数字摘要方式
            /// </summary>
            [XmlIgnore]
            public int? SafetyWay { get; set; }

            [XmlElement("SafetyWay")]
            public string SafetyWayValue
            {
                get { return SafetyWay.HasValue ? SafetyWay.Value.ToString() : null; }
                set { SafetyWay = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 注册方式(必选)缺省为1；
            /// 1:符合IETF FRC 3261标准的认证注册模式；
            /// 2:基于口令的双向认证注册模式；
            /// 3:基于数字证书的双向认证注册模式；
            /// </summary>
            [XmlIgnore]
            public int? RegisterWay { get; set; }

            [XmlElement("RegisterWay")]
            public string RegisterWayValue
            {
                get { return RegisterWay.HasValue ? RegisterWay.Value.ToString() : null; }
                set { RegisterWay = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 证书序列号（有证书的设备必选）
            /// </summary>
            [XmlElement("CertNum")]
            public string CertNum { get; set; }

            /// <summary>
            /// 证书有效标志(有证书的设备必选)，
            /// 0无效
            /// 1有效
            /// </summary>
            [XmlIgnore]
            public int? Certifiable { get; set; }

            [XmlElement("Certifiable")]
            public string CertifiableValue
            {
                get { return Certifiable.HasValue ? Certifiable.Value.ToString() : null; }
                set { Certifiable = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 证书无效原因码(可选)
            /// </summary>
            [XmlIgnore]
            public int? ErrCode { get; set; }

            [XmlElement("ErrCode")]
            public string ErrCodeValue
            {
                get { return ErrCode.HasValue ? ErrCode.Value.ToString() : null; }
                set { ErrCode = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 证书终止有效期(可选)
            /// </summary>
            [XmlElement("EndTime")]
            public string EndTime { get; set; }

            /// <summary>
            /// 保密属性(必选)
            /// 0：不涉密
            /// 1涉密
            /// </summary>
            [XmlIgnore]
            public int? Secrecy { get; set; }

            [XmlElement("Secrecy")]
            public string SecrecyValue
            {
                get { return Secrecy.HasValue ? Secrecy.Value.ToString() : null; }
                set { Secrecy = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 设备/区域/系统IP地址（可选）
            /// </summary>
            [XmlElement("IPAddress")]
            public string IPAddress { get; set; }

            /// <summary>
            /// 设备/区域/系统端口(可选)
            /// </summary>
            [XmlIgnore]
            public ushort? Port { get; set; }

            [XmlElement("Port")]
            public string PortValue
            {
                get { return Port.HasValue ? Port.Value.ToString() : null; }
                set { Port = ushort.TryParse(value, out ushort result) ? result : (ushort?) null; }
            }

            /// <summary>
            /// 设备口令（可选）
            /// </summary>
            [XmlElement("Password")]
            public string Password { get; set; }

            /// <summary>
            /// 设备状态(必选)
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            [XmlElement("Status")]
            public DevStatus Status { get; set; }

            /// <summary>
            /// 经度(可选)
            /// </summary>
            [XmlIgnore]
            public double? Longitude { get; set; }

            [XmlElement("Longitude")]
            public string LongitudeValue
            {
                get => Longitude.HasValue ? Longitude.Value.ToString() : null;
                set => Longitude = double.TryParse(value, out double result) ? result : (double?) null;
            }

            /// <summary>
            /// 纬度(可选)
            /// </summary>
            [XmlIgnore]
            public double? Latitude { get; set; }

            [XmlElement("Latitude")]
            public string LatitudeValue
            {
                get { return Latitude.HasValue ? Latitude.Value.ToString() : null; }
                set { Latitude = double.TryParse(value, out double result) ? result : (double?) null; }
            }

            /// <summary>
            /// 信息项
            /// </summary>
            [XmlElement("Info")]
            public Info InfList { get; set; }

            /// <summary>
            /// 远程设备终结点
            /// </summary>
            public string RemoteEP { get; set; }
        }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public class Info
        {
            /// <summary>
            /// 摄像机类型扩展，标识摄像机类型
            /// 1，球机
            /// 2，半球
            /// 3，固定枪机
            /// 4，遥控枪机
            /// 当目录项为摄像机时可选
            /// </summary>
            [XmlIgnore]
            public int? PTZType { get; set; }

            [XmlElement("PTZType")]
            public string PTZTypeValue
            {
                get { return PTZType.HasValue ? PTZType.Value.ToString() : null; }
                set { PTZType = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 摄像机位置类型扩展
            /// 1，省际检查站
            /// 2，党政机关
            /// 3，车站码头
            /// 4，中心广场
            /// 5，体育场馆
            /// 6，商业中心
            /// 7，宗教场所
            /// 8，校园周边
            /// 9，治安复杂区域
            /// 10，交通干线
            /// 当目录项为摄像机时可选
            /// </summary>
            [XmlIgnore]
            public int? PositionType { get; set; }

            [XmlElement("PositionType")]
            public string PositionTypeValue
            {
                get => PositionType.HasValue ? PositionType.Value.ToString() : null;
                set => PositionType = int.TryParse(value, out int result) ? result : (int?) null;
            }


            /// <summary>
            /// 摄像机按照位置室外、室内属性
            /// 1，室外
            /// 2，室内
            /// 当目录项为摄像机时可选，缺省为1
            /// </summary>
            [XmlIgnore]
            public int? RoomType { get; set; }

            [XmlElement("RoomType")]
            public string RoomTypeValue
            {
                get { return RoomType.HasValue ? RoomType.Value.ToString() : null; }
                set { RoomType = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 摄像机用途属性
            /// 1，治安
            /// 2，交通
            /// 3，重点
            /// 当目录项为摄像机时可选
            /// </summary>
            [XmlIgnore]
            public int? UseType { get; set; }

            [XmlElement("UseType")]
            public string UseTypeValue
            {
                get { return UseType.HasValue ? UseType.Value.ToString() : null; }
                set { UseType = int.TryParse(value, out int result) ? result : (int?) null; }
            }

            /// <summary>
            /// 摄像机补光属性
            /// 1，无补光
            /// 2，红外补光
            /// 3，白光补光
            /// 当目录项为摄像机时可选，缺省为1
            /// </summary>
            [XmlIgnore]
            public int? SupplyLightType { get; set; }

            [XmlElement("SupplyLightType")]
            public string SupplyLightTypeValue
            {
                get => SupplyLightType.HasValue ? SupplyLightType.Value.ToString() : null;
                set => SupplyLightType = int.TryParse(value, out int result) ? result : (int?) null;
            }

            /// <summary>
            /// 摄像机监视方位属性
            /// 1，东
            /// 2，西
            /// 3，南
            /// 4，北
            /// 5，东南
            /// 6，东北
            /// 7，西南
            /// 8，西北
            /// 当目录项为摄像机时且为固定摄像机或设置看守位摄像机时可选
            /// </summary>
            [XmlIgnore]
            public int? DirectionType { get; set; }

            [XmlElement("DirectionType")]
            public string DirectionTypeValue
            {
                get => DirectionType.HasValue ? DirectionType.Value.ToString() : null;
                set => DirectionType = int.TryParse(value, out int result) ? result : (int?) null;
            }

            /// <summary>
            /// 摄像机支持的分辨率，可有多个分辨率值，各个取值间以"/"分隔。
            /// 分辨率取值参见附录F中SDP f字段规定。
            /// 当目录项为摄像机时可选
            /// </summary>
            [XmlElement("Resolution")]
            public string Resolution { get; set; }

            /// <summary>
            /// 虚拟组织所属的业务分组ID，
            /// 业务分组根据特定的业务需求制定，
            /// 一个业务分组包含一组特定的虚拟组织。
            /// </summary>
            [XmlElement("BusinessGroupID")]
            public string BusinessGroupID { get; set; }

            /// <summary>
            /// 下载倍速范围(可选)，各可选参数以"/"分隔
            /// 如设备支持1,2,4倍下载则应写为"1/2/4"
            /// </summary>
            [XmlElement("DownloadSpeed")]
            public string DownloadSpeed { get; set; }

            /// <summary>
            /// 空域编码能力
            /// 0，不支持
            /// 1，1级增强
            /// 2，2级增强
            /// 3，3级增强
            /// (可选)
            /// </summary>
            [XmlIgnore]
            public int? SVCSpaceSupportMode { get; set; }

            [XmlElement("SVCSpaceSupportMode")]
            public string SVCSpaceSupportModeValue
            {
                get => SVCSpaceSupportMode.HasValue ? SVCSpaceSupportMode.Value.ToString() : null;
                set => SVCSpaceSupportMode = int.TryParse(value, out int result) ? result : (int?) null;
            }

            /// <summary>
            /// 时域编码能力
            /// 0，不支持
            /// 1，1级增强
            /// 2，2级增强
            /// 3，3级增强
            /// </summary>
            [XmlIgnore]
            public int? SVCTimeSupportMode { get; set; }

            [XmlElement("SVCTimeSupportMode")]
            public string SVCTimeSupportModeValue
            {
                get => SVCTimeSupportMode.HasValue ? SVCTimeSupportMode.Value.ToString() : null;
                set => SVCTimeSupportMode = int.TryParse(value, out int result) ? result : (int?) null;
            }
        }
    }

    /// <summary>
    /// 目录查询/订阅
    /// </summary>
    [XmlRoot("Query")]
    public class CatalogQuery : XmlHelper<CatalogQuery>
    {
        private static CatalogQuery _instance;

        /// <summary>
        /// 单例模式访问
        /// </summary>
        public static CatalogQuery Instance => _instance ?? (_instance = new CatalogQuery());

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
        /// 系统编码/行政区划码/设备编码/业务分组编码/虚拟组织编码
        /// </summary>
        [XmlElement("DeviceID")]
        public string DeviceID { get; set; }

        /// <summary>
        /// 报警开始时间
        /// </summary>
        [XmlElement("StartAlarmPriority")]
        public string StartAlarmPriority { get; set; }

        /// <summary>
        /// 报警结束时间
        /// </summary>
        [XmlElement("EndAlarmPriority")]
        public string EndAlarmPriority { get; set; }

        /// <summary>
        /// 报警方法
        /// </summary>
        [XmlElement("AlarmMethod")]
        public string AlarmMethod { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [XmlElement("StartTime")]
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [XmlElement("EndTime")]
        public string EndTime { get; set; }
    }
}