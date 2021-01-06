using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using LibCommon.Structs.GB28181;
using LibCommon.Structs.GB28181.XML;
using SIPSorcery.SIP;

namespace LibCommon
{
    public static class GCommon
    {
        public static string BaseStartPath = Environment.CurrentDirectory; //程序启动的目录

        public static string
            BaseStartFullPath = Process.GetCurrentProcess().MainModule.FileName; //程序启动的全路径

        public static string? WorkSpacePath = AppDomain.CurrentDomain.BaseDirectory; //程序运行的目录
        public static string? WorkSpaceFullPath = Environment.GetCommandLineArgs()[0]; //程序运行的全路径
        public static string? CommandLine = Environment.CommandLine; //程序启动命令
        public static string ConfigPath = BaseStartPath + "/Config/";
        public static string TmpPicsPath = BaseStartPath + "/.tmppics/"; //用于截图缓存


        #region 各类事件委托

        /// <summary>
        /// 踢掉掉线的sip设备
        /// </summary>
        /// <param name="guid"></param>
        public delegate void DoKickSipDevice(SipDevice sipDevice);

        /// <summary>
        /// 当sip设备注册时
        /// </summary>
        /// <param name="sipDeviceJson"></param>
        public delegate void RegisterDelegate(string sipDeviceJson);

        /// <summary>
        /// 当sip设备注销时
        /// </summary>
        /// <param name="sipDeviceJson"></param>
        public delegate void UnRegisterDelegate(string sipDeviceJson);

        /// <summary>
        /// 音视频点播完成（结束）
        /// </summary>
        /// <param name="record"></param>
        public delegate void InviteHistroyVideoFinished(RecordInfo.RecItem record);

        /// <summary>
        /// 当收到心跳数据时
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="keepAliveTime"></param>
        /// <param name="lostTimes"></param>
        public delegate void KeepaliveReceived(string deviceId, DateTime keepAliveTime, int lostTimes);

        /// <summary>
        /// 设备就绪通知
        /// </summary>
        /// <param name="sipDevice"></param>
        public delegate void SipDeviceReadyReceived(SipDevice sipDevice);

        /// <summary>
        /// 当收到设备目录时
        /// </summary>
        /// <param name="sipChannel"></param>
        public delegate void CatalogReceived(SipChannel sipChannel);

        /// <summary>
        /// 当设备报警订阅时
        /// </summary>
        /// <param name="sipTransaction"></param>
        public delegate void DeviceAlarmSubscribeDelegate(SIPTransaction sipTransaction);

        /// <summary>
        /// 获取设备状态时
        /// </summary>
        /// <param name="sipDevice"></param>
        /// <param name="deviceStatus"></param>
        public delegate void DeviceStatusReceived(SipDevice sipDevice, DeviceStatus deviceStatus);

        /// <summary>
        /// 收到设备信息时
        /// </summary>
        /// <param name="sipDevice"></param>
        /// <param name="deivceInfo"></param>
        public delegate void DeviceInfoReceived(SipDevice sipDevice, DeviceInfo deivceInfo);

        #endregion


        static GCommon()
        {
            //使用CodePagesEncodingProvider去注册扩展编码,以支持utf-x以外的字符集
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (!Directory.Exists(ConfigPath)) //如果配置文件目录不存在，则创建目录
            {
                Directory.CreateDirectory(ConfigPath);
            }

            if (!Directory.Exists(TmpPicsPath))
            {
                Directory.CreateDirectory(TmpPicsPath);
            }

            //初始化错误代码
            ErrorMessage.Init();
        }
    }
}