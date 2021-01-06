namespace LibCommon.Structs.GB28181.Sys
{
    /// <summary>
    /// 设备状态
    /// </summary>
    public enum DevStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        ON = 0,

        /// <summary>
        /// 故障
        /// </summary>
        OFF = 1,

        /// <summary>
        /// 正常
        /// </summary>
        OK = 2,
    }

    /// <summary>
    /// 设备目录类型
    /// </summary>
    public enum DevCataType
    {
        /// <summary>
        /// 未知的
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// 平台根
        /// </summary>
        ROOT,

        /// <summary>
        /// 省级行政区划
        /// </summary>
        PROVICECATA,

        /// <summary>
        /// 市级行政区划
        /// </summary>
        CITYCATA,

        /// <summary>
        /// 区县级行政区划
        /// </summary>
        AREACATA,

        /// <summary>
        /// 基层接入单位行政区划
        /// </summary>
        BASICUNIT,

        /// <summary>
        /// 系统目录项
        /// </summary>
        SYSTEMCATA,

        /// <summary>
        /// 业务分组目录项
        /// </summary>
        BUSINESSGROUPCATA,

        /// <summary>
        /// 虚拟目录分组项
        /// </summary>
        VIRTUALGROUPCATA,

        /// <summary>
        /// 设备
        /// </summary>
        DEVICE,

        /// <summary>
        /// 其他
        /// </summary>
        OTHER
    }

    public class DevType
    {
        /// <summary>
        /// 获取设备目录类型
        /// </summary>
        /// <param name="devId">编码</param>
        /// <returns></returns>
        public static DevCataType GetCataType(string devId)
        {
            DevCataType DeviceType = DevCataType.UNKNOWN;

            switch (devId.Length)
            {
                case 2:
                    DeviceType = DevCataType.PROVICECATA;
                    break;
                case 4:
                    DeviceType = DevCataType.CITYCATA;
                    break;
                case 6:
                    DeviceType = DevCataType.AREACATA;
                    break;
                case 8:
                    DeviceType = DevCataType.BASICUNIT;
                    break;
                case 20:
                    int extId = int.Parse(devId.Substring(10, 3));
                    if (extId == 200) //ID编码11-13位采用200标识系统ID类型
                    {
                        DeviceType = DevCataType.SYSTEMCATA;
                    }
                    else if (extId == 215) //业务分组标识，编码采用D.1中的20位ID格式，扩展215类型代表业务分组
                    {
                        DeviceType = DevCataType.BUSINESSGROUPCATA;
                    }
                    else if (extId == 216) //虚拟组织标识，编码采用D.1中的20位ID格式，扩展216类型代表虚拟组织
                    {
                        DeviceType = DevCataType.VIRTUALGROUPCATA;
                    }
                    else if (extId == 131 || extId == 132 || extId == 134 || extId == 137) //D.1中摄像机，网络摄像机编码
                    {
                        DeviceType = DevCataType.DEVICE;
                    }
                    else
                    {
                        DeviceType = DevCataType.OTHER; //其他类型
                    }

                    break;
            }

            return DeviceType;
        }
    }
}