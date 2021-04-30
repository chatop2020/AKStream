using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using LibCommon.Structs.GB28181.Sys;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 导出数据到mServer
    /// </summary>
    [XmlRoot("NvrTable")]
    public class NvrExport : XmlHelper<NvrExport>, IDisposable
    {
        private static NvrExport _instance;

        /// <summary>
        /// 单例模式
        /// </summary>
        public static NvrExport Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NvrExport();
                }

                return _instance;
            }
        }

        /// <summary>
        /// 是/否公安平台
        /// </summary>
        [XmlElement("IsGongAn")]
        public int IsGongAn { get; set; }

        /// <summary>
        /// 设备项
        /// </summary>
        [XmlElement("Item")]
        public List<Item> Items { get; set; }

        public void Dispose()
        {
            Items.Clear();
            _instance = null;
        }

        /// <summary>
        /// 设备信息
        /// </summary>
        public class Item
        {
            /// <summary>
            /// 设备Guid
            /// </summary>
            [XmlElement("Guid")]
            public string Guid { get; set; }

            /// <summary>
            /// 设备/区域/系统编码(必选)
            /// </summary>
            [XmlElement("DeviceID")]
            public string DeviceID { get; set; }

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
            /// 当为设备时，安装地址(必选)
            /// </summary>
            [XmlElement("Address")]
            public string Address { get; set; }

            /// <summary>
            /// 当为设备时，是否有子设备(必选)，
            /// 1有
            /// 0没有
            /// </summary>
            [XmlElement("Parental")]
            public string Parental { get; set; }

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
            [XmlElement("SafetyWay")]
            public string SafetyWay { get; set; }

            /// <summary>
            /// 注册方式(必选)缺省为1；
            /// 1:符合IETF FRC 3261标准的认证注册模式；
            /// 2:基于口令的双向认证注册模式；
            /// 3:基于数字证书的双向认证注册模式；
            /// </summary>
            [XmlElement("RegisterWay")]
            public int RegisterWay { get; set; }

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
            [XmlElement("Certifiable")]
            public string Certifiable { get; set; }

            /// <summary>
            /// 证书无效原因码(可选)
            /// </summary>
            [XmlElement("ErrCode")]
            public string ErrCode { get; set; }

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
            [XmlElement("Secrecy")]
            public int Secrecy { get; set; }

            /// <summary>
            /// 设备/区域/系统IP地址（可选）
            /// </summary>
            [XmlElement("IPAddress")]
            public string IPAddress { get; set; }

            /// <summary>
            /// 设备/区域/系统端口(可选)
            /// </summary>
            [XmlElement("Port")]
            public ushort Port { get; set; }

            /// <summary>
            /// 设备口令（可选）
            /// </summary>
            [XmlElement("Password")]
            public string Password { get; set; }

            /// <summary>
            /// 设备状态(必选)
            /// </summary>
            [XmlElement("Status")]
            public DevStatus Status { get; set; }

            /// <summary>
            /// 经度(可选)
            /// </summary>
            [XmlElement("Longitude")]
            public double Longitude { get; set; }

            /// <summary>
            /// 纬度(可选)
            /// </summary>
            [XmlElement("Latitude")]
            public double Latitude { get; set; }

            /// <summary>
            /// 信息项
            /// </summary>
            [XmlElement("Info")]
            public Info InfList { get; set; }

            /// <summary>
            /// 32项扩展信息
            /// </summary>
            [XmlElement("Extend")]
            public Extend ExtendInf { get; set; }
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
            [XmlElement("PTZType")]
            public string PTZType { get; set; }

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
            [XmlElement("PositionType")]
            public string PositionType { get; set; }

            /// <summary>
            /// 摄像机按照位置室外、室内属性
            /// 1，室外
            /// 2，室内
            /// 当目录项为摄像机时可选，缺省为1
            /// </summary>
            [XmlElement("RoomType")]
            public string RoomType { get; set; }

            /// <summary>
            /// 摄像机用途属性
            /// 1，治安
            /// 2，交通
            /// 3，重点
            /// 当目录项为摄像机时可选
            /// </summary>
            [XmlElement("UseType")]
            public string UseType { get; set; }

            /// <summary>
            /// 摄像机补光属性
            /// 1，无补光
            /// 2，红外补光
            /// 3，白光补光
            /// 当目录项为摄像机时可选，缺省为1
            /// </summary>
            [XmlElement("SupplyLightType")]
            public string SupplyLightType { get; set; }

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
            [XmlElement("DirectionType")]
            public string DirectionType { get; set; }
        }

        /// <summary>
        /// 32项扩展属性
        /// </summary>
        public class Extend
        {
            /// <summary>
            /// 地州市级
            /// </summary>
            [XmlElement("CityCode")]
            public string CityCode { get; set; }

            /// <summary>
            /// 县市区级
            /// </summary>
            /// 
            [XmlElement("CountyCode")]
            public string CountyCode { get; set; }

            /// <summary>
            /// 乡镇
            /// </summary> 
            [XmlElement("TownCode")]
            public string TownCode { get; set; }

            /// <summary>
            /// 村社区
            /// </summary> 
            [XmlElement("VillageCode")]
            public string VillageCode { get; set; }

            /// <summary>
            /// 小区
            /// </summary> 
            [XmlElement("CommunityCode")]
            public string CommunityCode { get; set; }

            /// <summary>
            /// 基层
            /// </summary> 
            [XmlElement("GrassRootsCode")]
            public string GrassRootsCode { get; set; }

            /// <summary>
            /// 行业
            /// </summary> 
            [XmlElement("Industry")]
            public string Industry { get; set; }

            /// <summary>
            /// 网络标识
            /// </summary> 
            [XmlElement("NETId")]
            public string NETId { get; set; }

            /// <summary>
            /// 用户
            /// </summary> 
            [XmlElement("User")]
            public string User { get; set; }

            /// <summary>
            /// 单位所在派出所
            /// </summary> 
            [XmlElement("UnitPoliceStation")]
            public string UnitPoliceStation { get; set; }

            /// <summary>
            /// 单位所在警务区
            /// </summary> 
            [XmlElement("UnitPoliceArea")]
            public string UnitPoliceArea { get; set; }

            /// <summary>
            /// 单位类别
            /// </summary> 
            [XmlElement("UnitType")]
            public string UnitType { get; set; }

            /// <summary>
            /// 单位法人
            /// </summary> 
            [XmlElement("UnitJuridical")]
            public string UnitJuridical { get; set; }

            /// <summary>
            /// 单位法人联系电话
            /// </summary> 
            [XmlElement("UnitJuridicalTEL")]
            public string UnitJuridicalTEL { get; set; }

            /// <summary>
            /// 责任人
            /// </summary> 
            [XmlElement("Liable")]
            public string Liable { get; set; }

            /// <summary>
            /// 录入时间
            /// </summary> 
            [XmlElement("EntryTime")]
            public string EntryTime { get; set; }

            /// <summary>
            /// 负责人姓名
            /// </summary> 
            [XmlElement("LiableName")]
            public string LiableName { get; set; }

            /// <summary>
            /// 负责人联系电话
            /// </summary> 
            [XmlElement("LiableTEL")]
            public string LiableTEL { get; set; }

            /// <summary>
            /// 记录更新时间
            /// </summary> 
            [XmlElement("RecordTime")]
            public string RecordTime { get; set; }

            /// <summary>
            /// 建设类型
            /// </summary> 
            [XmlElement("ConstructionType")]
            public string ConstructionType { get; set; }

            /// <summary>
            /// 建设时间
            /// </summary> 
            [XmlElement("ConstructionTime")]
            public string ConstructionTime { get; set; }

            /// <summary>
            /// 维护负责人
            /// </summary> 
            [XmlElement("Maintenance")]
            public string Maintenance { get; set; }

            /// <summary>
            /// 维护负责人手机
            /// </summary> 
            [XmlElement("MaintenanceTEL")]
            public string MaintenanceTEL { get; set; }

            /// <summary>
            /// 备注信息
            /// </summary> 
            [XmlElement("Desription")]
            public string Desription { get; set; }

            /// <summary>
            /// 是否是前端摄像头，用于列表展示,1代表是，0代表其它
            /// </summary> 
            [XmlElement("IsFront")]
            public int IsFront { get; set; }
        }

        #region 方法

        public void Save()
        {
            base.Save(this);
        }

        public void Read()
        {
            _instance = base.Read(this.GetType());
        }

        #endregion
    }
}